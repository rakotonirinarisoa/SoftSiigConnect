var table = undefined;

let list = [];

const NUMBER_OF_ROWS = 5;

function parseList(array) {
    const result = [];

    let rowNumber = 0;

    for (let i = 0; i < array.length; i += 1) {
        rowNumber = 0;

        for (let j = 0; j < array[i].TraitementsEngagementsDetails.length; j += 1) {
            let total = 0;

            for (let k = 0; k < NUMBER_OF_ROWS; k += 1) {
                let etape = '';
                let beneficiaire = '';
                let dateTraitement = '';
                let montant = '';
                let agent = '';
                let dureeTraitement = '';

                switch (k) {
                    case 0:
                        etape = 'Transfert et Validation RAF';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].TRANSFERTRAFAGENT;
                        dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTTRANSFERTRAF;

                        total += Number(dureeTraitement);

                        break;
                    case 1:
                        etape = 'Validation ORDESEC';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].VALORDSECAGENT;
                        dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTVALORDSEC;

                        total += Number(dureeTraitement);

                        break;
                    case 2:
                        etape = 'Transféré SIIGFP';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATESENDSIIG === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATESENDSIIG);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].SENDSIIGAGENT;
                        dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTSENDSIIG;

                        total += Number(dureeTraitement);

                        break;
                    case 3:
                        etape = 'Intégré SIIGFP';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATESIIGFP === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATESIIGFP);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].SIIGFPAGENT;
                        dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTSIIGFP;
                        total += Number(dureeTraitement);

                        break;
                    default:
                        break;
                }

                result.push({
                    rowNumber,
                    soa: array[i].SOA,
                    num: array[i].TraitementsEngagementsDetails[j].NUM_ENGAGEMENT,
                    etape,
                    beneficiaire,
                    montant,
                    agent,
                    dateTraitement,
                    dureeTraitement
                });

                rowNumber += 1;
            }

            result[(i + j + 1) * NUMBER_OF_ROWS - 1].dureeTraitement = total;
        }
    }

    list = result;
}

function setDataTable() {
    if (table !== undefined) {
        table.destroy();
    }

    table = $('#dashboard').DataTable({
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
        info: false,
        colReorder: false,
        rowsGroup: [0, 1],
        order: [['desc']],
        createdRow: function (row, data, _) {
            if (data.rowNumber !== 0) {
                $('td:eq(0)', row).addClass('delete-td');
                $('td:eq(1)', row).addClass('delete-td');
            }

            if (data.rowNumber % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
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

$('[data-action="GenereLISTE"]').click(async function () {
    const dd = $("#dateD").val();
    const df = $("#dateF").val();

    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer la liste.");

        return;
    }

    const pr = $("#proj").val();

    if (!pr) {
        alert("Veuillez sélectionner au moins un projet.");

        return;
    }

    const formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("listProjet", $("#proj").val());

    $.ajax({
        type: 'POST',
        url: Origin + `/BordTraitement/GenereDelaisTraitementEngagements`,
        contentType: false,
        data: formData,
        cache: false,
        processData: false,
        beforeSend: function () {
            loader.removeClass('display-none');
        },
        complete: function () {
            loader.addClass('display-none');
        },
        success: function (result) {
            const res = JSON.parse(result);

            const { type, msg } = res;

            if (type === 'error' || type === 'PEtat' || type === 'Prese') {
                alert(msg);

                return;
            }

            const { data } = res;

            parseList(data);

            setDataTable();
        },
        Error: function (_, e) {
            alert(e);
        }
    });
});

$(document).ready(async () => {
    User = JSON.parse(sessionStorage.getItem("user"));

    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;

    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    $(`[data-widget="pushmenu"]`).on('click', () => {
        $(`[data-action="SaveV"]`).toggleClass('custom-fixed-btn');
    });

    GetListProjet();
});

$('#export-excel-btn').on('click', () => {
    $(`td.delete-td`).remove();

    tableToExcel('dashboard', 'DELAIS DE TRAITEMENT ENGAGEMENTS', setDataTable);
});

$('#proj').on('change', () => {
    emptyTable();
});

function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

function GetListProjet() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/BordTraitement/GetAllPROJET',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function () {
            loader.removeClass('display-none');
        },
        complete: function () {
            loader.addClass('display-none');
        },
        success: function (result) {
            var Datas = JSON.parse(result);

            if (Datas.type == "error") {
                alert(Datas.msg);

                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);

                window.location = window.location.origin;

                return;
            }

            $(`[data-id="proj-list"]`).text("");
            var code = ``;
            $.each(Datas.data.List, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
            });

            $(`[data-id="proj-list"]`).append(code);

            $("#proj").val([...Datas.data.PROJET]).trigger('change');
        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

$('.Checkall').change(function () {
    if ($('.Checkall').prop("checked") == true) {

        $('[compteg-ischecked]').prop("checked", true);
    } else {
        $('[compteg-ischecked]').prop("checked", false);
    }
});

function emptyTable() {
    const data = [];

    if (table !== undefined) {
        table.destroy();
    }

    table = $('#dashboard').DataTable({
        data,
        paging: false,
        ordering: false,
        info: false,
        colReorder: false
    });
}