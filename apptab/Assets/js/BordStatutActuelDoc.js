var table = undefined;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    GetListProjet();

    $(`[data-widget="pushmenu"]`).on('click', () => {
        $(`[data-action="SaveV"]`).toggleClass('custom-fixed-btn');
    });
});

$('#proj').on('change', () => {
    emptyTable();

    $(`[data-id="site-list"]`).text("");
    var code1 = ``;
    $(`[data-id="site-list"]`).append(code1);

    GetSITE();
});

function GetSITE() {
    let pr = $("#proj").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un projet. ");
        return;
    }

    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/EtatGED/GETALLSITE',
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

                $(`[data-id="site-list"]`).text("");
                var code1 = ``;
                $(`[data-id="site-list"]`).append(code1);

                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            $(`[data-id="site-list"]`).text("");
            var code1 = ``;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.Id}">${v.Code}</option>
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

    $(`[data-id="typeDoc-list"]`).text("");
    var code1 = ``;
    $(`[data-id="typeDoc-list"]`).append(code1);

    GetTypeDocs();
});

function GetTypeDocs() {
    let pr = $("#proj").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un projet. ");
        return;
    }

    let selectedValues = $("#site").val() || [];
    let allOptionSelected = selectedValues.includes('All');

    if (allOptionSelected) {
        selectedValues = $("#site option").not('[value="All"]').map(function () {
            return $(this).val();
        }).get();
    }

    let formData = new FormData();

    formData.append("iProjet", pr);
    formData.append("iSite", selectedValues);

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/EtatGED/GETALLTYPEDOCS',
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

                $(`[data-id="typeDoc-list"]`).text("");
                var code1 = ``;
                $(`[data-id="typeDoc-list"]`).append(code1);

                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            $(`[data-id="typeDoc-list"]`).text("");
            var code1 = `<option value="">Tous</option>`;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.Id}">${v.Title}</option>
                `;
            });
            $(`[data-id="typeDoc-list"]`).append(code1);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

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

    table = $('#TBD_PROJET_ORDSEC').DataTable({
        data,
        colReorder: {
            enable: false,
            fixedColumnsLeft: 1
        },
        deferRender: true,
        dom: 'Bfrtip',
        buttons: ['colvis'],
        pageLength: 25,
    });
}

$('[data-action="GenereLISTE"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer la liste. ");
        return;
    }

    let pr = $("#proj").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un projet. ");
        return;
    }

    let site = $("#site").val();
    if (!site) {
        alert("Veuillez sélectionner au moins un site. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("listProjet", $("#proj").val());
    formData.append("listSite", $("#site").val());
    formData.append("TypeDoc", $("#typeDoc").val());

    $.ajax({
        type: "POST",
        url: Origin + '/EtatGED/GenereLISTERFR',
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
                emptyTable();
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
            if (Datas.type == "PEtat") {
                alert(Datas.msg);
                emptyTable();
                return;
            }
            if (Datas.type == "Prese") {
                alert(Datas.msg);
                emptyTable();
                return;
            }
            if (Datas.type == "success") {
                listResult = Datas.data

                const data = [];

                $.each(listResult, function (_, v) {
                    data.push({
                        REFERENCE: v.REFERENCE,
                        DOCUMENT: v.DOCUMENT,
                        FOURNISSEUR: v.FOURNISSEUR,
                        MONTANT: v.MONTANT,
                        TYPE: v.TYPE,
                        STEPNOW: v.STEPNOW,
                        STEPNEXT: v.STEPNEXT,
                        VALIDATEURNEXT: v.VALIDATEURNEXT,
                        DUREENEXT: v.DUREENEXT,
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET_ORDSEC').DataTable({
                    data,
                    columns: [
                        { data: 'REFERENCE' },
                        { data: 'DOCUMENT' },
                        { data: 'FOURNISSEUR' },
                        { data: 'MONTANT' },
                        { data: 'TYPE' },
                        { data: 'STEPNOW' },
                        { data: 'STEPNEXT' },
                        { data: 'VALIDATEURNEXT' },
                        { data: 'DUREENEXT' },
                    ],
                    createdRow: function (row, data, _) {
                        $(row).attr('compteG-id', data.id);
                        $(row).addClass('select-text');
                    },
                    columnDefs: [
                        {
                            targets: [-4, -3, -2, -1]
                        }
                    ],
                    colReorder: {
                        enable: false,
                        fixedColumnsLeft: 1
                    },
                    deferRender: true,
                    dom: 'Bfrtip',
                    buttons: ['colvis'],
                    pageLength: 25,
                    caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
                    buttons: ['colvis',
                        {
                            extend: 'pdfHtml5',
                            title: 'SITUATION DES ETAPES PAR TYPE DE DOCUMENT',
                            messageTop: 'Liste situation des étapes par type de document',
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-pdf',
                            //exportOptions: {
                            //    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8],
                            //},
                            customize: function (doc) {
                                doc.defaultStyle.alignment = 'left';
                                //doc.defaultStyle.margin = [12, 12, 12, 12];
                            },
                            download: 'open'
                        },
                        {
                            extend: 'excelHtml5',
                            title: 'SITUATION DES ETAPES PAR TYPE DE DOCUMENT',
                            messageTop: 'Liste situation des étapes par type de document',
                            text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-excel',
                            exportOptions: {
                                //columns: [0, 1, 2, 3, 4, 5, 6, 7, 8],
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data === 'undefined') {
                                            return;
                                        }
                                        if (data == null) {
                                            return data;
                                        }
                                        if (column === 3) {
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
                    initComplete: function () {
                        $(`thead td[data-column-index="${0}"]`).removeClass('sorting_asc').removeClass('sorting_desc');

                        count = 0;
                        this.api().columns().every(function () {
                            var title = this.header();
                            //replace spaces with dashes
                            title = $(title).html().replace(/[\W]/g, '-');
                            var column = this;
                            var select = $('<select id="' + title + '" class="select2" ></select>')
                                .appendTo($(column.footer()).empty())
                                .on('change', function () {
                                    //Get the "text" property from each selected data 
                                    //regex escape the value and store in array
                                    var data = $.map($(this).select2('data'), function (value, key) {
                                        return value.text ? '^' + $.fn.dataTable.util.escapeRegex(value.text) + '$' : null;
                                    });

                                    //if no data selected use ""
                                    if (data.length === 0) {
                                        data = [""];
                                    }

                                    //join array into string with regex or (|)
                                    var val = data.join('|');

                                    //search for the option(s) selected
                                    column
                                        .search(val ? val : '', true, false)
                                        .draw();
                                });

                            column.data().unique().sort().each(function (d, j) {
                                select.append('<option value="' + d + '">' + d + '</option>');
                            });

                            //use column title as selector and placeholder
                            $('#' + title).select2({
                                multiple: true,
                                closeOnSelect: false

                            });

                            //initially clear select otherwise first option is selected
                            $('.select2').val(null).trigger('change');
                        });
                    }
                });

                //$('#TBD_PROJET_ORDSEC tfoot th').each(function (i) {
                //    if (i == 0) {
                //        $(this).addClass("NOTVISIBLE");
                //    }
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});