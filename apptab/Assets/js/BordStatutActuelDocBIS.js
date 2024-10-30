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
                var code1 = `<option value="All">Select All</option>`;
                $(`[data-id="site-list"]`).append(code1);

                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            // Désactivé-ko lo le evenement onchange fa lasa boucle//
            $("#site").off('change').on('change', handleSelectAll);

            $(`[data-id="site-list"]`).text("");
            var code1 = `<option value="All">Select All</option>`;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.Id}">${v.Code}</option>
                `;
            });
            $(`[data-id="site-list"]`).append(code1);

            $("#site").val([]).trigger('change');
            $("#site").select2();
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

$('#site').on('change', () => {
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

    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());
    formData.append("iSite", $("#site").val());

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
            var code1 = ``;
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
        url: Origin + '/EtatGED/GenereLISTEBIS',
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

                if (listResult.list.length != 0) {
                    var RFRcontent = ``;
                    RFRcontent += `
                <table class="table table-hover table-striped table-bordered" display responsive nowrap" width="100%" id="TBD_PROJET_ORDSEC">
                    <thead style="position:sticky">
                        <tr class="thead-accueil2" style="white-space: nowrap;">
                            <th colspan="4"></th>
                `;

                    //<th colspan=""></th> number of columns
                    let nombreEtape = listResult.nombreEtape
                    RFRcontent += `
                            <th colspan="${nombreEtape}" style="text-align:center">Etapes</th>
                `;

                    RFRcontent += `
                            <th colspan="2"></th>
                        </tr>
                        <tr class="thead-accueil1" style="white-space: nowrap;">
                            <td style="font-weight:bold; text-align:center">Référence</td>
                            <td style="font-weight:bold; text-align:center">Document</td>
                            <td style="font-weight:bold; text-align:center">Fournisseur</td>
                            <td style="font-weight:bold; text-align:center">Montant</td>
                `;

                    //each()<td>
                    $.each(listResult.listEtape, function (k, v) {
                        RFRcontent += `
                            <td style="font-weight: bold; text-align: center">${v}</td>
                    `;
                    });

                    RFRcontent += `
                            <td style="font-weight: bold; text-align: center">Archive</td>
                            <td style="font-weight: bold; text-align: center">Rattachement TOMATE</td>
                        </tr>
                    </thead>
                    <tbody class="traitementORDSEC"></tbody>
                    <tfoot style="opacity:50%">
                        <tr>
                            <th>Référence</th>
                            <th>Document</th>
                            <th>Fournisseur</th>
                            <th>Montant</th>
                `;

                    //each()<th>
                    $.each(listResult.listEtape, function (k, v) {
                        RFRcontent += `
                            <th>${v}</th>
                    `;
                    });

                    RFRcontent += `
                            <th>Archive</th>
                            <th>Rattachement TOMATE</th>
                        </tr>
                    </tfoot>
                </table>
                `;

                    $('#RFRTable').html(RFRcontent);

                    const data = [];
                    let tab = [];

                    $.each(listResult.list, function (_, v) {

                        const foo = new Map();
                        tab = [];

                        //for (let i = 0; i < v.DATESTEP.length; i += 1) {
                        for (let i = 0; i < nombreEtape; i += 1) {
                            tab.push({
                                data: `Etape ${i + 1}`
                            });

                            foo.set(tab[i].data, v.DATESTEP[i]);
                        }

                        const tmp = Object.fromEntries(foo.entries())

                        data.push({
                            REFERENCE: v.REFERENCE,
                            DOCUMENT: v.DOCUMENT,
                            FOURNISSEUR: v.FOURNISSEUR,
                            MONTANT: v.MONTANT,
                            ARCHIVEDATE: v.ARCHIVEDATE,
                            RATTACHTOM: v.RATTACHTOM,
                            ...tmp,

                        });
                    });

                    console.log(data);
                    console.log(tab);
                    console.log(...tab);

                    //console.log(data);

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

                            ...tab,

                            { data: 'ARCHIVEDATE' },
                            { data: 'RATTACHTOM' },
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
                                title: 'ETAT D\'AVANCEMENT PAR TYPE DE DOCUMENT',
                                messageTop: 'Liste des états d\'avancement par type de document',
                                text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                                orientation: 'landscape',
                                pageSize: 'A4',
                                charset: "utf-8",
                                bom: true,
                                className: 'custombutton-collection-pdf',
                                //exportOptions: {
                                //    columns: [0, 1, 2, 3, 4, 5, 6, 7],
                                //},
                                customize: function (doc) {
                                    doc.defaultStyle.alignment = 'left';
                                    doc.content.splice(1, 0, {
                                        margin: [0, -50, 0, 0],
                                        alignment: 'left',
                                        image:
                                            'data:image/png;base64,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'
                                    });
                                },
                                download: 'open'
                            },
                            {
                                extend: 'excelHtml5',
                                title: 'ETAT D\'AVANCEMENT PAR TYPE DE DOCUMENT',
                                messageTop: 'Liste des états d\'avancement par type de document',
                                text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                                orientation: 'landscape',
                                pageSize: 'A4',
                                charset: "utf-8",
                                bom: true,
                                className: 'custombutton-collection-excel',
                                exportOptions: {
                                    //columns: [0, 1, 2, 3, 4, 5, 6, 7],
                                    format: {
                                        body: function (data, row, column, node) {
                                            if (typeof data === 'undefined') {
                                                return;
                                            }
                                            if (data == null) {
                                                return data;
                                            }
                                            //if (column === 3) {
                                            //    var arr = data.split(',');
                                            //    if (arr.length == 1) { return data; }

                                            //    arr[0] = arr[0].toString().replace(/[\.]/g, "");
                                            //    if (arr[0] > '' || arr[1] > '') {
                                            //        data = arr[0] + '.' + arr[1];
                                            //    } else {
                                            //        return '';
                                            //    }
                                            //    return data.toString().replace(/[^\d.-]/g, "");
                                            //}
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

                    //console.log(table);

                    //$('#TBD_PROJET_ORDSEC tfoot th').each(function (i) {
                    //    if (i == 0) {
                    //        $(this).addClass("NOTVISIBLE");
                    //    }
                    //});
                }
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});