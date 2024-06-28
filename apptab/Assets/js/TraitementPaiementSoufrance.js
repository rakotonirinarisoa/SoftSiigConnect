var table = undefined;

let list = [];

//const NUMBER_OF_ROWS = 5;
const NUMBER_OF_ROWS = 4;

function parseList(array) {
    const result = [];

    let rowNumber = 0;
    
    for (let i = 0; i < array.length; i += 1) {
        rowNumber = 0;

        for (let j = 0; j < array[i].TraitementPaiementDetails.length; j += 1) {
            for (let k = 0; k < NUMBER_OF_ROWS; k += 1) {
                let etape = '';
                let beneficiaire = '';
                let dateTraitement = '';
                let montant = '';
                let agent = '';
                let dureeTraitement = '';
                let dureePrevu = '';
                let depassement = '';

                switch (k) {
                    case 0:
                        etape = 'Transfert et Validation';
                        dateTraitement = array[i].TraitementPaiementDetails[j].DATETRANSFERTRAF === undefined ? '' : formatDate(array[i].TraitementPaiementDetails[j].DATETRANSFERTRAF);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementPaiementDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].TRANSFERTRAFAGENT;
                        dureeTraitement = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTTRANSFERTOP;
                        dureePrevu = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTPREVUEOP;
                        depassement = array[i].TraitementPaiementDetails[j].DEPASSEMENTOP;

                        break;
                    case 1:
                        //etape = 'Envoi pour validation';
                        etape = 'Validation';
                        dateTraitement = array[i].TraitementPaiementDetails[j].DATEVALORDSEC === undefined ? '' : formatDate(array[i].TraitementPaiementDetails[j].DATEVALORDSEC);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementPaiementDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].VALORDSECAGENT;
                        dureeTraitement = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTTRANSFERTAC;
                        dureePrevu = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTPREVUEAC;
                        depassement = array[i].TraitementPaiementDetails[j].DEPASSEMENTAC;

                        break;
                    //case 2:
                    //    etape = 'Validation';
                    //    dateTraitement = array[i].TraitementPaiementDetails[j].DATESENDSIIG === undefined ? '' : formatDate(array[i].TraitementPaiementDetails[j].DATESENDSIIG);
                    //    beneficiaire = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].BENEFICIAIRE;
                    //    montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementPaiementDetails[j].MONTENGAGEMENT).replace(',', '.'));
                    //    agent = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].SENDSIIGAGENT;
                    //    dureeTraitement = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTTRANSFERTBK;
                    //    dureePrevu = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTPREVUEBK;
                    //    depassement = array[i].TraitementPaiementDetails[j].DEPASSEMENTBK;

                    //    break;
                    //case 3:
                    case 2:
                        etape = 'Envoi Fichier BANQUE';
                        dateTraitement = array[i].TraitementPaiementDetails[j].DATESIIGFP === undefined ? '' : formatDate(array[i].TraitementPaiementDetails[j].DATESIIGFP);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementPaiementDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementPaiementDetails[j].SIIGFPAGENT;
                        dureeTraitement = '';
                        dureePrevu = array[i].TraitementPaiementDetails[j].DUREETRAITEMENTPREVUEBK;
                        depassement = '';

                        break;
                    default:
                        break;
                }

                result.push({
                    rowNumber,
                    soa: array[i].SOA,
                    projet: array[i].TraitementPaiementDetails[j].PROJET,
                    site:array[i].TraitementPaiementDetails[j].SITE,
                    type: array[i].TraitementPaiementDetails[j].AVANCE ? 'Avance' : 'Paiement',
                    num: array[i].TraitementPaiementDetails[j].NUM_ENGAGEMENT,
                    etape,
                    beneficiaire,
                    montant,
                    agent,
                    dateTraitement,
                    dureeTraitement,
                    dureePrevu,
                    depassement
                });

                rowNumber += 1;
            }
        }
    }

    calculateDuration(result);

    list = result;
}

function setSum(array, startIndex, endIndex) {
    let total = 0;
    let totalPREVU = 0;
    let totalDEPASS = 0;

    for (let i = startIndex; i <= endIndex; i += 1) {
        total += Number(array[i].dureeTraitement);
        totalPREVU += Number(array[i].dureePrevu);
        totalDEPASS += Number(array[i].depassement);
    }

    array[endIndex + 1].dureeTraitement = total;
    array[endIndex + 1].dureePrevu = totalPREVU;
    array[endIndex + 1].depassement = totalDEPASS;
}
function calculateDuration(array) {
    let pointer = 0;

    for (let i = 0; i < array.length; i += 1) {
        if (i % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
            setSum(array, pointer, i - 1);

            pointer = i + 1;
        }
    }
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
                data: 'projet'
            },
            {
                data: 'site'
            },
            {
                data: 'type'
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
            },
            {
                data: 'dureePrevu'
            },
            {
                data: 'depassement'
            },
        ],
        lengthChange: false,
        paging: false,
        ordering: false,
        info: false,
        colReorder: false,
        rowsGroup: [0, 1, 2, 3, 4],
        order: [['desc']],
        //createdRow: function (row, data, _) {
        //    if (data.rowNumber !== 0) {
        //        $('td:eq(0)', row).addClass('delete-td');
        //        $('td:eq(1)', row).addClass('delete-td');
        //    }

        //    if (data.rowNumber % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
        //        $('td:eq(6)', row).attr('colspan', 4).css({ 'text-align': 'center' });
        //        $('td:eq(6)', row).text('Durée totale');

        //        /*$('td:eq(4)', row).text(data.dateTraitement);*/
        //        $('td:eq(7)', row).text(data.dureeTraitement);
        //        $('td:eq(8)', row).text(data.dureePrevu);
        //        $('td:eq(9)', row).text(data.depassement);

        //        $('td:eq(10)', row).text('').css({ 'display': 'none' });
        //        $('td:eq(11)', row).text('').css({ 'display': 'none' });
        //        $('td:eq(12)', row).text('').css({ 'display': 'none' });
        //    }
        //}
        createdRow: function (row, data, _) {
            if (data.rowNumber % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
                $('td:eq(6)', row).attr('colspan', 4).css({ 'text-align': 'center' });
                $('td:eq(6)', row).text('Durée totale');

                $('td:eq(7)', row).text(data.dureeTraitement);
                $('td:eq(8)', row).text(data.dureePrevu);
                $('td:eq(9)', row).text(data.depassement);

                $('td:eq(10)', row).text('').css({ 'display': 'none' });
                $('td:eq(11)', row).text('').css({ 'display': 'none' });
                $('td:eq(12)', row).text('').css({ 'display': 'none' });
            }
        },
        deferRender: true,
        dom: 'Bfrtip',
        buttons: ['colvis'],
        caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
        buttons: ['colvis',
            {
                extend: 'pdfHtml5',
                title: 'TRAITEMENTS EN SOUFFRANCE (PAR RAPPORT AU DELAI MOYEN)',
                messageTop: 'Liste des paiements en souffrance (par rapport au délai moyen)',
                text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                orientation: 'landscape',
                pageSize: 'A4',
                charset: "utf-8",
                bom: true,
                className: 'custombutton-collection-pdf',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                    //grouped_array_index: [1] //note the brackets, i think this is so you can group by multiple columns.
                },
                customize: function (doc) {
                    doc.defaultStyle.alignment = 'left';
                    //doc.defaultStyle.margin = [12, 12, 12, 12];
                },
                download: 'open'
            },
            {
                extend: 'excelHtml5',
                title: 'TRAITEMENTS EN SOUFFRANCE (PAR RAPPORT AU DELAI MOYEN)',
                messageTop: 'Liste des paiements en souffrance (par rapport au délai moyen)',
                text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                orientation: 'landscape',
                pageSize: 'A4',
                charset: "utf-8",
                bom: true,
                className: 'custombutton-collection-excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                    format: {
                        body: function (data, row, column, node) {
                            if (typeof data === 'undefined') {
                                return;
                            }
                            if (data == null) {
                                return data;
                            }
                            if (column === 7) {
                                var arr = data.split(',');
                                if (arr.length == 1) { return data; }

                                arr[0] = arr[0].toString().replace(/[\.]/g, "");
                                if (arr[0] > '' || arr[1] > '') {
                                    data = arr[0] + '.' + arr[1];
                                } else {
                                    return '';
                                }
                                return data.toString().replace(/[^\d.-]/g, "");
                            }
                            return data;
                        }
                    }
                },
            }
        ],
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
        url: Origin + `/BordTraitement/GenereTraitementPaiementSouffrance`,
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

//$('#export-excel-btn').on('click', () => {
//    $(`td.delete-td`).remove();

//    tableToExcel('dashboard', 'DELAIS DE TRAITEMENT ENGAGEMENTS', setDataTable);
//});
$('#export-excel-btn').on('click', () => {
    $('td').filter(function () {
        return $(this).css('display') === 'none';
    }).remove();

    tableToExcel('dashboard', 'TRAITEMENT EN SOUFFRANCE', setDataTable);
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
        colReorder: false,

        deferRender: true,
        dom: 'Bfrtip',
        buttons: ['colvis'],
    });
}