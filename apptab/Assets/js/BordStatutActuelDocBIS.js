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
                                    //doc.defaultStyle.margin = [12, 12, 12, 12];
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