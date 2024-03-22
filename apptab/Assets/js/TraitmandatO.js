﻿var table = undefined;

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
                    buttons: ['colvis',
                        {
                            text: '<i class="fa fa-file-pdf"> Exporter en PDF</i>',
                            className: 'custombutton-collection-pdf',
                            customize: function (doc) {
                                // Personnalisation du document PDF ici
                                // Par exemple, tu peux définir les largeurs des colonnes comme suit :
                                doc.content[1].table.widths =
                                    Array(doc.content[1].table.body[0].length + 1).join('*').split('');

                                // Ajouter des couleurs de fond aux cellules, etc.
                                let body = doc.content[1].table.body;
                                body.forEach(function (row, index) {
                                    // Ignorer l'en-tête du tableau
                                    if (index === 0) return;

                                    // Appliquer la couleur de fond à la première cellule de chaque ligne
                                    row[0].fillColor = '#f0f0f0';
                                });
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

                $('#TBD_PROJET_OTHER tfoot th').each(function (i) {
                    if (i == 0 || i >= 13) {
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
                    buttons: ['colvis'],
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

                $('#TBD_PROJET_OTHER tfoot th').each(function (i) {
                    if (i == 0 || i >= 13) {
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
            enable: true,
            fixedColumnsLeft: 1
        },
        deferRender: true,
        dom: 'Bfrtip',
        buttons: ['colvis'],
    });
}