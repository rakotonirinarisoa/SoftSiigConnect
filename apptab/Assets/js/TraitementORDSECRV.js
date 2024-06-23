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
    GetListLOAD();
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
        url: Origin + '/TraitementRevers/GetAllPROJET',
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

            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
            });

            $(`[data-id="proj-list"]`).append(code);

            GetListLOAD();
        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

function GetListLOAD() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/TraitementRevers/GenerationSIIGLOAD',
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
                        id: v.No,
                        soa: v.SOA,
                        projet: v.PROJET,
                        site: v.SITE,
                        ref: v.REF,
                        npiece: v.NPIECE,
                        objet: v.OBJ,
                        titulaire: v.TITUL,
                        dateMandat: formatDate(v.DATE),
                        compte: v.COMPTE,
                        pcop: v.PCOP,
                        montant: formatCurrency(String(v.MONT).replace(",", ".")),
                        dateDEF: formatDate(v.DATEDEF),
                        dateTEF: formatDate(v.DATETEF),
                        dateBE: formatDate(v.DATEBE),
                        utilisateur: v.LIEN,
                        dateGeneration: formatDate(v.DATECREATION),
                        imputation: '',
                        piecesJustificatives: '',
                        document: '',
                        rejeter: '',
                        isLATE: v.isLATE
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET_ORDSEC').DataTable({
                    data,
                    columns: [
                        {
                            data: 'id',
                            render: function (data, _, _, _) {
                                return `
                                    <input type="checkbox" name="checkprod" compteg-ischecked class="chk" onchange="checkdel('${data}')" />
                                `;
                            },
                            orderable: false
                        },
                        {
                            data: 'rejeter',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalREJET('${row.id}')">
                                        <i class="fa fa-times fa-lg text-dark elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        { data: 'soa' },
                        { data: 'projet' },
                        { data: 'site' },
                        { data: 'ref' },
                        { data: 'npiece' },
                        { data: 'objet' },
                        { data: 'titulaire' },
                        { data: 'dateMandat' },
                        { data: 'compte' },
                        { data: 'pcop' },
                        { data: 'montant' },
                        { data: 'dateDEF' },
                        { data: 'dateTEF' },
                        { data: 'dateBE' },
                        { data: 'utilisateur' },
                        { data: 'dateGeneration' },
                        {
                            data: 'imputation',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalD('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-danger elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'piecesJustificatives',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalF('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-success elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'document',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalLIAS('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-info elerfr"></i>
                                    </div>
                                `;
                            }
                        }
                    ],
                    createdRow: function (row, data, _) {
                        $(row).attr('compteG-id', data.id);
                        $(row).addClass('select-text');
                        if (data.isLATE) {
                            //$(row).addClass("demoRayure");
                            $(row).children('td').eq(0).addClass("demoRayure");
                        }
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
                    pageLength: 25,
                    buttons: ['colvis'],
                    caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
                    buttons: ['colvis',
                        {
                            extend: 'pdfHtml5',
                            title: 'REVERSEMENTS A VALIDER',
                            messageTop: 'Liste des reversements en attente validation',
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-pdf',
                            exportOptions: {
                                columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17]
                            },
                            customize: function (doc) {
                                doc.defaultStyle.alignment = 'left';
                                //doc.defaultStyle.margin = [12, 12, 12, 12];
                            },
                            download: 'open'
                        },
                        {
                            extend: 'excelHtml5',
                            title: 'REVERSEMENTS A VALIDER',
                            messageTop: 'Liste des reversements en attente validation',
                            text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-excel',
                            exportOptions: {
                                columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17],
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data === 'undefined') {
                                            return;
                                        }
                                        if (data == null) {
                                            return data;
                                        }
                                        if (column === 12) {
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

                $('#TBD_PROJET_ORDSEC tfoot th').each(function (i) {
                    if (i == 0 || i == 1 || i >= 18) {
                        $(this).addClass("NOTVISIBLE");
                    }
                });
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$('[data-action="GenereSIIG"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les reversements. ");
        return;
    }

    let pr = $("#proj").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un projet. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/TraitementRevers/GenerationSIIG',
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
                        id: v.No,
                        soa: v.SOA,
                        projet: v.PROJET,
                        site: v.SITE,
                        ref: v.REF,
                        npiece: v.NPIECE,
                        objet: v.OBJ,
                        titulaire: v.TITUL,
                        dateMandat: formatDate(v.DATE),
                        compte: v.COMPTE,
                        pcop: v.PCOP,
                        montant: formatCurrency(String(v.MONT).replace(",", ".")),
                        dateDEF: formatDate(v.DATEDEF),
                        dateTEF: formatDate(v.DATETEF),
                        dateBE: formatDate(v.DATEBE),
                        utilisateur: v.LIEN,
                        dateGeneration: formatDate(v.DATECREATION),
                        imputation: '',
                        piecesJustificatives: '',
                        document: '',
                        rejeter: '',
                        isLATE: v.isLATE
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET_ORDSEC').DataTable({
                    data,
                    columns: [
                        {
                            data: 'id',
                            render: function (data, _, _, _) {
                                return `
                                    <input type="checkbox" name="checkprod" compteg-ischecked class="chk" onchange="checkdel('${data}')" />
                                `;
                            },
                            orderable: false
                        },
                        {
                            data: 'rejeter',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalREJET('${row.id}')">
                                        <i class="fa fa-times fa-lg text-dark elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        { data: 'soa' },
                        { data: 'projet' },
                        { data: 'site' },
                        { data: 'ref' },
                        { data: 'npiece' },
                        { data: 'objet' },
                        { data: 'titulaire' },
                        { data: 'dateMandat' },
                        { data: 'compte' },
                        { data: 'pcop' },
                        { data: 'montant' },
                        { data: 'dateDEF' },
                        { data: 'dateTEF' },
                        { data: 'dateBE' },
                        { data: 'utilisateur' },
                        { data: 'dateGeneration' },
                        {
                            data: 'imputation',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalD('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-danger elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'piecesJustificatives',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalF('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-success elerfr"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'document',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalLIAS('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-info elerfr"></i>
                                    </div>
                                `;
                            }
                        }
                    ],
                    createdRow: function (row, data, _) {
                        $(row).attr('compteG-id', data.id);
                        $(row).addClass('select-text');
                        if (data.isLATE) {
                            //$(row).addClass("demoRayure");
                            $(row).children('td').eq(0).addClass("demoRayure");
                        }
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
                            title: 'REVERSEMENTS A VALIDER',
                            messageTop: 'Liste des reversements en attente validation',
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-pdf',
                            exportOptions: {
                                columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17]
                            },
                            customize: function (doc) {
                                doc.defaultStyle.alignment = 'left';
                                //doc.defaultStyle.margin = [12, 12, 12, 12];
                            },
                            download: 'open'
                        },
                        {
                            extend: 'excelHtml5',
                            title: 'REVERSEMENTS A VALIDER',
                            messageTop: 'Liste des reversements en attente validation',
                            text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-excel',
                            exportOptions: {
                                columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17],
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data === 'undefined') {
                                            return;
                                        }
                                        if (data == null) {
                                            return data;
                                        }
                                        if (column === 12) {
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

                $('#TBD_PROJET_ORDSEC tfoot th').each(function (i) {
                    if (i == 0 || i == 1 || i >= 18) {
                        $(this).addClass("NOTVISIBLE");
                    }
                });
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});

$('[data-action="SaveV"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");

    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    if (list.length == 0) {
        alert("Veuillez sélectionner au moins un reversement afin de l'enregistrer et l'envoyer pour validation. ");
        return;
    }

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("listCompte", list);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/TraitementRevers/GetCheckedEcritureF',
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
            alert(Datas.msg);
            $.each(CheckList, (k, v) => {
                list.push($(v).remove());
            });
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});

$('.Checkall').change(function () {

    if ($('.Checkall').prop("checked") == true) {

        $('[compteg-ischecked]').prop("checked", true);
    } else {
        $('[compteg-ischecked]').prop("checked", false);
    }

});

$('[data-action="SaveSIIG"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("listCompte", list);

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/TraitementRevers/GetCheckedEcritureORDSEC',
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
            alert(Datas.msg);
            $.each(CheckList, (k, v) => {
                list.push($(v).remove());
            });
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
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
    });
}