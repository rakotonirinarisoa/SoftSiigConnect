﻿$(document).ready(() => {
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

$('#projMANDAT').on('change', () => {
    emptyTableTRM();
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
            
            $(`[data-id="proj-listMANDAT"]`).text("");

            var code = ``;

            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
            });
            
            $(`[data-id="proj-listMANDAT"]`).append(code);

            emptyTableTRM();
        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

//GENERER//
$('[data-action="GenereR"]').click(async function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");
        return;
    }

    let pr = $("#projMANDAT").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un projet. ");
        return;
    }

    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    formData.append("iProjet", $("#projMANDAT").val());

    $.ajax({
        type: "POST",
        async: true,
        url: Origin + '/Traitement/Generation',
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
            if (Datas.type == "PEtat") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "Prese") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
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

                table = $('#TBD_PROJET_MANDAT').DataTable({
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
                        enable: true,
                        fixedColumnsLeft: 1
                    },
                    deferRender: true,
                    dom: 'Bfrtip',
                    buttons: ['colvis'],
                    initComplete: function () {
                        $(`thead td[data-column-index="${0}"]`).removeClass('sorting_asc').removeClass('sorting_desc');
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
        alert("Veuillez sélectionner au moins un mandat afin de l'enregistrer et l'envoyer pour validation. ");
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

    formData.append("iProjet", $("#projMANDAT").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetCheckedEcritureF',
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

function emptyTableTRM() {
    const data = [];

    if (table !== undefined) {
        table.destroy();
    }

    table = $('#TBD_PROJET_MANDAT').DataTable({
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
            enable: true,
            fixedColumnsLeft: 1
        },
        deferRender: true,
        dom: 'Bfrtip',
        buttons: ['colvis'],
        initComplete: function () {
            $(`thead td[data-column-index="${0}"]`).removeClass('sorting_asc').removeClass('sorting_desc');
        }
    });
}

$('#btn-export-excel').on('click', () => {
    exportTableToExcel('TBD_PROJET_MANDAT');
});
