var table = undefined;

let list = [];

//const NUMBER_OF_ROWS = 5;
const NUMBER_OF_ROWS = 2;

function setSum(array, startIndex, endIndex) {
    let total = 0;

    for (let i = startIndex; i <= endIndex; i += 1) {
        total += Number(array[i].dureeTraitement);
    }

    array[endIndex + 1].dureeTraitement = total;
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

function parseList(array) {
    const result = [];

    let rowNumber = 0;

    for (let i = 0; i < array.length; i += 1) {
        rowNumber = 0;

        for (let j = 0; j < array[i].TraitementsEngagementsDetails.length; j += 1) {
            for (let k = 0; k < NUMBER_OF_ROWS; k += 1) {
                let etape = '';
                let beneficiaire = '';
                let dateTraitement = '';
                let montant = '';
                let agent = '';
                let dureeTraitement = '';

                switch (k) {
                    case 0:
                        etape = 'Tris et Validation';
                        dateTraitement = array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATETRANSFERTRAF);
                        beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                        montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                        agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].TRANSFERTRAFAGENT;
                        dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTTRANSFERTRAF;

                        break;
                    //case 1:
                    //    etape = 'Validation';
                    //    dateTraitement = array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATEVALORDSEC);
                    //    beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                    //    montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                    //    agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].VALORDSECAGENT;
                    //    dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTVALORDSEC;

                    //    break;
                    //case 2:
                    //    etape = 'Transféré SIIGFP';
                    //    dateTraitement = array[i].TraitementsEngagementsDetails[j].DATESENDSIIG === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATESENDSIIG);
                    //    beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                    //    montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                    //    agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].SENDSIIGAGENT;
                    //    dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTSENDSIIG;

                    //    break;
                    //case 3:
                    //    etape = 'Intégré SIIGFP';
                    //    dateTraitement = array[i].TraitementsEngagementsDetails[j].DATESIIGFP === undefined ? '' : formatDate(array[i].TraitementsEngagementsDetails[j].DATESIIGFP);
                    //    beneficiaire = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].BENEFICIAIRE;
                    //    montant = dateTraitement === '' ? '' : formatCurrency(String(array[i].TraitementsEngagementsDetails[j].MONTENGAGEMENT).replace(',', '.'));
                    //    agent = dateTraitement === '' ? '' : array[i].TraitementsEngagementsDetails[j].SIIGFPAGENT;
                    //    dureeTraitement = array[i].TraitementsEngagementsDetails[j].DUREETRAITEMENTSIIGFP;

                    //    break;
                    default:
                        break;
                }

                result.push({
                    rowNumber,
                    soa: array[i].SOA,
                    projet: array[i].TraitementsEngagementsDetails[j].PROJET,
                    site: array[i].TraitementsEngagementsDetails[j].SITE,
                    type: array[i].TraitementsEngagementsDetails[j].TYPE,
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
        }
    }

    calculateDuration(result);

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
                data: 'projet'
            },
            {
                data: 'site'
            },
            {
                data: 'type'
            },
            {
                data: 'num',
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
        rowsGroup: [0, 1, 2, 3, 4],
        order: [['desc']],
        createdRow: function (row, data, _) {
            if (data.rowNumber % NUMBER_OF_ROWS === NUMBER_OF_ROWS - 1) {
                $('td:eq(6)', row).attr('colspan', 4).css({ 'text-align': 'center' });
                $('td:eq(6)', row).text('Durée totale');

                $('td:eq(7)', row).text(data.dureeTraitement);

                $('td:eq(8)', row).text('').css({ 'display': 'none' });
                $('td:eq(9)', row).text('').css({ 'display': 'none' });
                $('td:eq(10)', row).text('').css({ 'display': 'none' });
            }
        },
        deferRender: true,
        //dom: 'Bfrtip',
        //buttons: ['colvis'],
        //pageLength: 25,
        caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
        //buttons: ['colvis',
        //    {
        //        extend: 'pdfHtml5',
        //        title: 'DELAIS DE TRAITEMENT DES DEPENSES A PAYER et AVANCES',
        //        messageTop: 'Suvi des délais de traitement des dépenses à payer et avances',
        //        text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
        //        orientation: 'landscape',
        //        pageSize: 'A4',
        //        charset: "utf-8",
        //        bom: true,
        //        className: 'custombutton-collection-pdf',
        //        exportOptions: {
        //            columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
        //        },
        //        customize: function (doc) {
        //            doc.defaultStyle.alignment = 'left';
        //            //doc.defaultStyle.margin = [12, 12, 12, 12];
        //        },
        //        download: 'open'
        //    },
        //    {
        //        extend: 'excelHtml5',
        //        title: 'DELAIS DE TRAITEMENT DES DEPENSES A PAYER et AVANCES',
        //        messageTop: 'Suvi des délais de traitement des dépenses à payer et avances',
        //        text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
        //        orientation: 'landscape',
        //        pageSize: 'A4',
        //        charset: "utf-8",
        //        bom: true,
        //        className: 'custombutton-collection-excel',
        //        exportOptions: {
        //            columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
        //            format: {
        //                body: function (data, row, column, node) {
        //                    if (typeof data === 'undefined') {
        //                        return;
        //                    }
        //                    if (data == null) {
        //                        return data;
        //                    }
        //                    if (column === 7) {
        //                        var arr = data.split(',');
        //                        if (arr.length == 1) { return data; }

        //                        arr[0] = arr[0].toString().replace(/[\.]/g, "");
        //                        if (arr[0] > '' || arr[1] > '') {
        //                            data = arr[0] + '.' + arr[1];
        //                        } else {
        //                            return '';
        //                        }
        //                        return data.toString().replace(/[^\d.-]/g, "");
        //                    }
        //                    return data;
        //                }
        //            }
        //        },
        //    }
        //],
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

    let site = $("#site").val();
    if (!site) {
        alert("Veuillez sélectionner au moins un site. ");
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
    formData.append("listSite", $("#site").val());

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
    $('td').filter(function () {
        return $(this).css('display') === 'none';
    }).remove();

    tableToExcel('dashboard', 'DELAIS DE TRAITEMENT DEPENSES A PAYER ET AVANCES', setDataTable);
});

$('#proj').on('change', () => {
    emptyTable();

    $(`[data-id="site-list"]`).text("");
    var code1 = ``;
    $(`[data-id="site-list"]`).append(code1);

    const id = $('#proj').val();
    GetSITE();
});

function GetSITE() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/BordTraitement/GETALLSITE',
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
            if (Datas.type == "notYet") {
                alert(Datas.msg);

                $(`[data-id="site-list"]`).text("");
                var code1 = ``;
                $(`[data-id="site-list"]`).append(code1);

                return;
            }

            $(`[data-id="site-list"]`).text("");

            var code1 = ``;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v}">${v}</option>
                `;
            });
            $(`[data-id="site-list"]`).append(code1);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

//Fonction handleSelectAll
var issite2 = [];
var isHandlingSelectAll = false;

function handleSelectAll() {
    try {

        if (isHandlingSelectAll) {
            return;
        }

        isHandlingSelectAll = true;

        var selectedValues = $("#site").val() || [];
        var allOptionSelected = selectedValues.includes('All');

        if (allOptionSelected) {
            issite2 = $("#site option").not('[value="All"]').map(function () {
                return $(this).val();
            }).get();

            if (issite2.length > 0) {
                $("#site").val(issite2).trigger('change');
                //$("#site").select2();
            }
        } else {
            var siteSansAll = selectedValues.filter(function (value) {
                return value !== 'All';
            })

            if (siteSansAll.length > 0) {
                $("#site").val([...siteSansAll]).trigger('change');
                //$("#site").select2();
            }
        }

        isHandlingSelectAll = false;
    } catch (error) {

    } finally {

    }
}

//Ajoutez l'événement "change" au dropdown du site//
$("#site").on('change', handleSelectAll);

$('#site').on('input', () => {
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
        //dom: 'Bfrtip',
        //buttons: ['colvis'],
        //pageLength: 25,
    });
}