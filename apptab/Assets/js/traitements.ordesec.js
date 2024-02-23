let User;
let Origin;
let compteur = 1;
let table = undefined;

function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

function GetListLOAD() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    //if (!id) {
    //    formData.append("suser.IDPROJET", User.IDPROJET);
    //} else {
    //    formData.append("suser.IDPROJET", id);
    //}

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIGLOAD',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
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

            if (Datas.type == "success") {
                listResult = Datas.data
                contentpaie = ``;

                const data = [];

                console.log(listResult);

                $.each(listResult, function (_, v) {
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
                        montant: formatCurrency(String(v.MONT)),
                        dateDEF: formatDate(v.DATEDEF),
                        dateTEF: formatDate(v.DATETEF),
                        dateBE: formatDate(v.DATEBE),
                        utilisateur: v.LIEN,
                        dateGeneration: formatDate(v.DATECREATION),
                        imputation: '',
                        piecesJustificatives: '',
                        document: '',
                        rejeter: ''
                    });
                });

                if (table !== undefined) {
                    table.destroy();
                }

                table = $('#TBD_PROJET').DataTable({
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
                        { data: 'utilisateur' },
                        { data: 'dateGeneration' },
                        {
                            data: 'imputation',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalD('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-danger"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'piecesJustificatives',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalF('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-danger"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'document',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalLIAS('${row.id}')">
                                        <i class="fa fa-tags fa-lg text-danger"></i>
                                    </div>
                                `;
                            }
                        },
                        {
                            data: 'rejeter',
                            render: function (_, _, row, _) {
                                return `
                                    <div onclick="modalREJET('${row.id}')">
                                        <i class="fa fa-times fa-lg text-dark"></i>
                                    </div>
                                `;
                            }
                        }
                    ],
                    createdRow: function (row, _, _) {
                        $(row).addClass('select-text');
                    },
                    columnDefs: [
                        {
                            targets: [-4, -3, -2, -1],
                            className: 'elerfr'
                        }
                    ],
                    colReorder: {
                        enable: true,
                        fixedColumnsLeft: 1
                    },
                    deferRender: true,
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
}

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
    $('.afb160Paie').html('');
    $('.traitementORDSEC').html('');
    $('.traitementORDSECOTHER').html('');

    GetListLOAD();
});

$('[data-action="GenereSIIG"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");
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
        url: Origin + '/Traitement/GenerationSIIG',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            console.log(Datas);

            if (Datas.type == "error") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
            if (Datas.type == "success") {
                listResult = Datas.data
                contentpaie = ``;
                $.each(listResult, function (k, v) {
                    contentpaie += `
                    <tr compteG-id="${v.No}" class="select-text">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked  onchange = "checkdel()"/>
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.SOA}</td>
                        <td style="font-weight: bold; text-align:center">${v.PROJET}</td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.LIEN}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATECREATION)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalREJET('${v.No}')"><i class="fa fa-times fa-lg text-dark"></i></div>
                        </td>
                    </tr>`
                });

                $('.traitementORDSEC').empty();
                $('.traitementORDSEC').html(contentpaie);
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

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetCheckedEcritureF',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
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
        url: Origin + '/Traitement/GetCheckedEcritureORDSEC',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
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
