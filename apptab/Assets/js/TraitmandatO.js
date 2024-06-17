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

    //$("#TBD_PROJET_OTHER tfoot th").each(function () {
    //    var title = $(this).text();
    //    $(this).html('<input type="text" placeholder="Search ' + title + '" />');
    //});
});

function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

$('#proj').on('change', () => {
    GetListLOADOTHER();
});

function GetListProjet() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetAllPROJET',
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
            
            GetListLOADOTHER();
        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

function GetListLOADOTHER() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIGLOADOTHER',
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
                listResult = Datas.data;

                const data = [];

                $.each(listResult, function (k, v) {
                    data.push({
                        id: v.No,
                        soa: v.SOA,
                        projet: v.PROJET,
                        ref: v.REF,
                        objet: v.OBJ,
                        titulaire: v.TITUL,
                        dateMandat: formatDate(v.DATE),
                        compte: v.COMPTE,
                        pcop: v.PCOP,
                        montant: formatCurrency(String(v.MONT).replace(",", ".")),
                        dateDEF: formatDate(v.DATEDEF),
                        dateTEF: formatDate(v.DATETEF),
                        dateBE: formatDate(v.DATEBE),
                        imputation: '',
                        piecesJustificatives: '',
                        document: ''
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET_OTHER').DataTable({
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
                        { data: 'soa' },
                        { data: 'projet' },
                        { data: 'ref' },
                        { data: 'objet' },
                        { data: 'titulaire' },
                        { data: 'dateMandat' },
                        { data: 'compte' },
                        { data: 'pcop' },
                        { data: 'montant' },
                        { data: 'dateDEF' },
                        { data: 'dateTEF' },
                        { data: 'dateBE' },
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
                            targets: [-3, -2, -1]
                        }
                    ],
                    colReorder: {
                        enable: false,
                        fixedColumnsLeft: 1
                    },
                    deferRender: true,
                    dom: 'Bfrtip',
                    pageLength: 25,
                    caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
                    buttons: ['colvis',
                        {
                            extend: 'pdfHtml5',
                            title: 'DEPENSES INITIEES',
                            messageTop: 'La liste des dépenses initiées dans TOM²PRO',
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-pdf',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
                            },
                            customize: function (doc) {
                                doc.defaultStyle.alignment = 'left';
                                //doc.defaultStyle.margin = [12, 12, 12, 12];
                            },
                            download: 'open'
                        },
                        {
                            extend: 'excelHtml5',
                            title: 'DEPENSES INITIEES',
                            messageTop: 'La liste des dépenses initiées dans TOM²PRO',
                            text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-excel',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data === 'undefined') {
                                            return;
                                        }
                                        if (data == null) {
                                            return data;
                                        }
                                        if (column === 9) {                   
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

                /*var footer = $("#TBD_PROJET_OTHER tfoot tr");*/
                $('#TBD_PROJET_OTHER tfoot th').each(function (i) {
                    if (i == 0 || i >= 13) {
                        /*$("#TBD_PROJET_OTHER thead").append(footer);*/
                        $(this).addClass("NOTVISIBLE");
                    }
                });

                //// Apply the search
                //$("#TBD_PROJET_OTHER thead").on("keyup", "input", function () {
                //    table.column($(this).parent().index())
                //        .search(this.value)
                //        .draw();
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$('[data-action="GenereSIIGOTHER"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");

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
        url: Origin + '/Traitement/GenerationSIIGOTHER',
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
                listResult = Datas.data;

                const data = [];

                $.each(listResult, function (k, v) {
                    data.push({
                        id: v.No,
                        soa: v.SOA,
                        projet: v.PROJET,
                        ref: v.REF,
                        objet: v.OBJ,
                        titulaire: v.TITUL,
                        dateMandat: formatDate(v.DATE),
                        compte: v.COMPTE,
                        pcop: v.PCOP,
                        montant: formatCurrency(String(v.MONT).replace(",", ".")),
                        dateDEF: formatDate(v.DATEDEF),
                        dateTEF: formatDate(v.DATETEF),
                        dateBE: formatDate(v.DATEBE),
                        imputation: '',
                        piecesJustificatives: '',
                        document: ''
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET_OTHER').DataTable({
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
                        { data: 'soa' },
                        { data: 'projet' },
                        { data: 'ref' },
                        { data: 'objet' },
                        { data: 'titulaire' },
                        { data: 'dateMandat' },
                        { data: 'compte' },
                        { data: 'pcop' },
                        { data: 'montant' },
                        { data: 'dateDEF' },
                        { data: 'dateTEF' },
                        { data: 'dateBE' },
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
                            targets: [-3, -2, -1]
                        }
                    ],
                    colReorder: {
                        enable: false,
                        fixedColumnsLeft: 1
                    },
                    deferRender: true,
                    dom: 'Bfrtip',
                    pageLength: 25,
                    caption: 'SOFT EXPENDITURES TRACKERS ' + new Date().toLocaleDateString(),
                    buttons: ['colvis',
                        {
                            extend: 'pdfHtml5',
                            title: 'DEPENSES INITIEES',
                            messageTop: 'La liste des dépenses initiées dans TOM²PRO',
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-pdf',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
                            },
                            customize: function (doc) {
                                doc.defaultStyle.alignment = 'left';
                                //doc.defaultStyle.margin = [12, 12, 12, 12];
                            },
                            download: 'open'
                        },
                        {
                            extend: 'excelHtml5',
                            title: 'DEPENSES INITIEES',
                            messageTop: 'La liste des dépenses initiées dans TOM²PRO',
                            text: '<i class="fa fa-file-excel"> Exporter en Excel</i>',
                            orientation: 'landscape',
                            pageSize: 'A4',
                            charset: "utf-8",
                            bom: true,
                            className: 'custombutton-collection-excel',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data === 'undefined') {
                                            return;
                                        }
                                        if (data == null) {
                                            return data;
                                        }
                                        if (column === 9) {
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
                            }
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

                //var footer = $("#TBD_PROJET_OTHER tfoot tr");
                $('#TBD_PROJET_OTHER tfoot th').each(function (i) {
                    if (i == 0 || i >= 13) {
                        //$("#TBD_PROJET_OTHER thead").append(footer);
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

    table = $('#TBD_PROJET_OTHER').DataTable({
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