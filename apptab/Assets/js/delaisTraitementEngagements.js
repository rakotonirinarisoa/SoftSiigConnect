let dataTable;

let data;

const NUMBER_OF_ROWS = 5;

function convertData(result) {
    const res = [];

    for (let i = 0; i < result.length; i += 1) {
        for (let j = 0; j < NUMBER_OF_ROWS; j += 1) {
            switch (j) {
                case 0:
                    res.push({
                        soa: result[i].SOA,
                        num: result[i].NUM_ENGAGEMENT,
                        etape: 'Transfert et Validation RAF',
                        beneficiaire: result[i].BENEFICIAIRE,
                        montant: result[i].MONTENGAGEMENT,
                        agent: '',
                        dateTraitement: result[i].DATETRANSFERTRAF === undefined ? '' : result[i].DATETRANSFERTRAF,
                        dureeTraitement: ''
                    });

                    break;
                case 1:
                    res.push({
                        soa: result[i].SOA,
                        num: result[i].NUM_ENGAGEMENT,
                        etape: 'Validation ORDESEC',
                        beneficiaire: result[i].BENEFICIAIRE,
                        montant: result[i].MONTENGAGEMENT,
                        agent: '',
                        dateTraitement: result[i].DATEVALORDSEC === undefined ? '' : result[i].DATEVALORDSEC,
                        dureeTraitement: ''
                    });

                    break;
                case 2:
                    res.push({
                        soa: result[i].SOA,
                        num: result[i].NUM_ENGAGEMENT,
                        etape: 'Transféré SIIGFP',
                        beneficiaire: result[i].BENEFICIAIRE,
                        montant: result[i].MONTENGAGEMENT,
                        agent: '',
                        dateTraitement: result[i].DATESENDSIIG === undefined ? '' : result[i].DATESENDSIIG,
                        dureeTraitement: ''
                    });

                    break;
                case 3:
                    res.push({
                        soa: result[i].SOA,
                        num: result[i].NUM_ENGAGEMENT,
                        etape: 'Intégré SIIGFP',
                        beneficiaire: result[i].BENEFICIAIRE,
                        montant: result[i].MONTENGAGEMENT,
                        agent: '',
                        dateTraitement: result[i].DATENGAGEMENT === undefined ? '' : result[i].DATENGAGEMENT,
                        dureeTraitement: ''
                    });

                    break;
                default:
                    res.push({
                        soa: result[i].SOA,
                        num: result[i].NUM_ENGAGEMENT,
                        etape: '',
                        beneficiaire: result[i].BENEFICIAIRE,
                        montant: result[i].MONTENGAGEMENT === undefined ? '' : result[i].MONTENGAGEMENT,
                        agent: '',
                        dateTraitement: '',
                        dureeTraitement: ''
                    });
            }
        }
    }

    console.log(res);
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
        success: function (res) {
            const { data } = JSON.parse(res);

            convertData(data);
        },
        Error: function (_, e) {
            alert(e);
        }
    });
}

function setDataTable() {
    data = [
        { id: 0, soa: 'PROJET 1', num: 'M0001', etape: 'Transfert et Validation RAF', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 1, soa: 'PROJET 1', num: 'M0001', etape: 'Transféré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 2, soa: 'PROJET 1', num: 'M0001', etape: 'Intégré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 3, soa: 'PROJET 1', num: 'M0001', etape: '12', beneficiaire: '', montant: 'Durée totale', agent: '', dateTraitement: '', dureeTraitement: '0' },

        { id: 4, soa: 'PROJET 2', num: 'M0002', etape: 'Transfert et Validation RAF', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 5, soa: 'PROJET 2', num: 'M0002', etape: 'Transféré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 6, soa: 'PROJET 2', num: 'M0002', etape: 'Intégré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 7, soa: 'PROJET 2', num: 'M0002', etape: '12', beneficiaire: '', etape: '', montant: 'Durée totale', agent: '', dateTraitement: '', dureeTraitement: '' },

        { id: 8, soa: 'PROJET 3', num: 'M0003', etape: 'Transfert et Validation RAF', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 9, soa: 'PROJET 3', num: 'M0003', etape: 'Transféré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 10, soa: 'PROJET 3', num: 'M0003', etape: 'Intégré SIIGFP', beneficiaire: '', montant: '', agent: '', dateTraitement: '', dureeTraitement: '' },
        { id: 11, soa: 'PROJET 3', num: 'M0003', etape: '12', beneficiaire: '', montant: 'Durée totale', agent: '', dateTraitement: '', dureeTraitement: '0' }
    ];

    if (dataTable !== undefined) {
        dataTable.destroy();
    }

    dataTable = $('#dashboard').DataTable({
        data,
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
            if (data.id % NUMBER_OF_ROWS !== 0) {
                $('td:eq(0)', row).addClass('delete-td');
                $('td:eq(1)', row).addClass('delete-td');
            }

            if (data.id % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
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
