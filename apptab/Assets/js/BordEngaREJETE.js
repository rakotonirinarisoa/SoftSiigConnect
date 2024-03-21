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

    table = $('#TBD_PROJET_ORDSEC').DataTable({
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

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("listProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/BordTraitement/GenereREJETE',
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
                        ref: v.REF,
                        benef: v.BENEF,
                        MONTENGAGEMENT: formatCurrency(String(v.MONTENGAGEMENT).replace(",", ".")),

                        AGENTREJETE: v.AGENTREJETE,
                        DATEREJETE: formatDate(v.DATEREJETE),
                        MOTIF: v.MOTIF,
                        COMMENTAIRE: v.COMMENTAIRE.replace('\r\n', '<br/>'),
                        //imputation: '',
                        //piecesJustificatives: '',
                        //document: '',
                        /*rejeter: '',*/
                        /*isLATE: v.isLATE*/
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
                        { data: 'soa' },
                        { data: 'projet' },
                        { data: 'ref' },
                        { data: 'benef' },
                        { data: 'MONTENGAGEMENT' },
                        { data: 'AGENTREJETE' },
                        { data: 'DATEREJETE' },
                        { data: 'MOTIF' },
                        { data: 'COMMENTAIRE' },
                        //{
                        //    data: 'imputation',
                        //    render: function (_, _, row, _) {
                        //        return `
                        //            <div onclick="modalD('${row.id}','${row.projet}')">
                        //                <i class="fa fa-tags fa-lg text-danger elerfr"></i>
                        //            </div>
                        //        `;
                        //    }
                        //},
                        //{
                        //    data: 'piecesJustificatives',
                        //    render: function (_, _, row, _) {
                        //        return `
                        //            <div onclick="modalF('${row.id}','${row.projet}')">
                        //                <i class="fa fa-tags fa-lg text-success elerfr"></i>
                        //            </div>
                        //        `;
                        //    }
                        //},
                        //{
                        //    data: 'document',
                        //    render: function (_, _, row, _) {
                        //        return `
                        //            <div onclick="modalLIAS('${row.id}','${row.projet}')">
                        //                <i class="fa fa-tags fa-lg text-info elerfr"></i>
                        //            </div>
                        //        `;
                        //    }
                        //}
                    ],
                    createdRow: function (row, data, _) {
                        $(row).attr('compteG-id', data.id);
                        $(row).addClass('select-text');

                        //if (data.isLATE) {
                        //    $(row).attr('style', "background-color: #FF7F7F !important;");
                        //}
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
                    if (i == 0) {
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