let dataTable;

let list = [];

const NUMBER_OF_ROWS = 5;

function parseList(array) {
    const result = [];

    let stepNumber = 0;
    let rowNumber = 0;

    let previousSOA = '';

    for (let i = 0; i < array.length; i += 1) {
        if (array[i].SOA === previousSOA) {
            stepNumber += 1;
        } else {
            stepNumber = 0;
        }

        rowNumber = 0;

        for (let j = 0; j < array[i].TraitementsEngagementsDetails.length; j += 1) {
            for (let k = 0; k < NUMBER_OF_ROWS; k += 1) {
                let etape = '';
                let dateTraitement = ''; 
                let montant = formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                let agent = '';

                switch (k) {
                    case 0:
                        etape = 'Transfert et Validation RAF';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF);

                        break;
                    case 1:
                        etape = 'Validation ORDESEC';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC);

                        break;
                    case 2:
                        etape = 'Transféré SIIGFP';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATESENDSIIG === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATESENDSIIG);

                        break;
                    case 3:
                        etape = 'Intégré SIIGFP';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATENGAGEMENT === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATENGAGEMENT);

                        break;
                    default:
                        etape = '';
                        dateTraitement = '';
                        montant = '';

                        break;
                }

                result.push({
                    stepNumber,
                    rowNumber,
                    soa: array[i].SOA,
                    num: array[i].NUM_ENGAGEMENT,
                    etape,
                    beneficiaire: array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE,
                    montant,
                    agent,
                    dateTraitement,
                    dureeTraitement: ''
                });

                rowNumber += 1;
            }
        }

        previousSOA = array[i].SOA;
    }

    list = result;
}

function setDataTable() {
    if (dataTable !== undefined) {
        dataTable.destroy();
    }

    dataTable = $('#dashboard').DataTable({
        data: list,
        columns: [
            {
                data: 'soa'
            },
            {
                data: 'num'
            },
            {
                data: 'etape'
            },
            {
                data: 'beneficiaire'
            },
            {
                data: 'montant'
            },
            {
                data: 'agent'
            },
            {
                data: 'dateTraitement'
            },
            {
                data: 'dureeTraitement'
            }
        ],
        lengthChange: false,
        paging: false,
        ordering: false,
        colReorder: false,
        rowsGroup: [0, 1],
        order: [['desc']],
        createdRow: function (row, data, _) {
            if (data.stepNumber !== 0 || data.rowNumber !== 0) {
                $('td:eq(0)', row).addClass('delete-td');
                $('td:eq(1)', row).addClass('delete-td');
            }

            if (data.stepNumber !== 0 && data.rowNumber === 0) {
                $('td:eq(1)', row).removeClass('delete-td');
            }

            if (data.rowNumber === NUMBER_OF_ROWS - 1) {
                $('td:eq(3)', row).attr('colspan', 4).css({ 'text-align': 'center' });
                $('td:eq(3)', row).text('Durée totale');

                $('td:eq(4)', row).text(data.dureeTraitement);

                $('td:eq(5)', row).text('').css({ 'display': 'none' });
                $('td:eq(6)', row).text('').css({ 'display': 'none' });
                $('td:eq(7)', row).text('').css({ 'display': 'none' });
            }
        }
    });
}

async function getTraitementEngagements() {
    const payload = {
        LOGIN: User.LOGIN,
        PWD: User.PWD
    }

    $.ajax({
        type: 'POST',
        url: Origin + `/BordTraitement/TraitementEngagements`,
        contentType: 'application/json',
        datatype: 'json',
        data: JSON.stringify({ ...payload }),
        beforeSend: function () {
            loader.removeClass('display-none');
        },
        complete: function () {
            loader.addClass('display-none');
        },
        success: function (result) {
            const { data } = JSON.parse(result);

            parseList(data);

            setDataTable();
        },
        Error: function (_, e) {
            alert(e);
        }
    });
}

$(document).ready(async () => {
    User = JSON.parse(sessionStorage.getItem("user"));

    if (User === undefined) {
        window.location = User.origin;
    }

    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    await getTraitementEngagements();

    setDataTable();
});

$('#export-excel-btn').on('click', () => {
    $(`td.delete-td`).remove();

    tableToExcel('dashboard', 'e2fkj', setDataTable);
});
