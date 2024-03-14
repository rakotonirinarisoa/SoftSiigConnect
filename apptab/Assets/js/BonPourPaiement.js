﻿var table = undefined;
let arr = [];

function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

function GetEtat() {
    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetEtat',
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
            listEtat = Datas.data
            if (Datas.type == "error") {
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                return;
            }
            etaCode = `<option value = "Tous" > Tous</option> `;
            $.each(listEtat, function (k, v) {
                etaCode += `
                    <option value="${v}">${v}</option>
                `;
            });
            $(`[ETAT-list]`).html('');
            $(`[ETAT-list]`).append(etaCode);

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function GetTypeP() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetTypeP',
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
            baseName = Datas;
            if (baseName == 1) {
                $(`[code_Type]`).val('');
                $(`[code_Type]`).val('BR');

            } else {
                $(`[code_Type]`).val('');
                $(`[code_Type]`).val('COMPTA');
            }

            if (Datas.type == "error") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
};
function GetListCodeJournal() {
    let formData = new FormData();

    let codeproject = $("#Fproject").val();

    formData.append("codeproject", codeproject);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCODEJournal',
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
                return;
            }

            let code = ``;
            ListCodeJournal = Datas.data;

            $.each(ListCodeJournal, function (k, v) {
                code += `
                    <option value="${v.CODE}">${v.CODE}</option>
                `;
            });

            $(`[codej-list]`).append(code);
            $(`[codej-libelle]`).val(ListCodeJournal[0].LIBELLE);
            GetEtat();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    }).done(function (res) {
        GetListCompG();
    });
}

function chargeLoad() {
    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("ChoixBase", baseName);

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    if (baseName == 2) {
        //compta

        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetAcceptecritureLoad',
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
                    return;
                }
                if (Datas.type == "success") {
                    content = ``;
                    listResult = Datas.data;

                    const data = [];
                    arr = data;
                    $.each(listResult, function (_, v) {
                        data.push({
                            checkbox: '',
                            id: isNullOrUndefined(v.IDREGLEMENT) ? '' : v.IDREGLEMENT,
                            dateOrdre: isNullOrUndefined(v.dateOrdre) ? '' : v.dateOrdre,
                            noPiece: isNullOrUndefined(v.NoPiece) ? '' : v.NoPiece,
                            compte: isNullOrUndefined(v.Compte) ? '' : v.Compte,
                            libelle: isNullOrUndefined(v.Libelle) ? '' : v.Libelle,
                            debit: isNullOrUndefined(v.Debit) ? '' : formatCurrency(String(v.Debit).replace(",", ".")),
                            credit: isNullOrUndefined(v.Credit) ? '' : formatCurrency(String(v.Credit).replace(",", ".")),
                            montant: isNullOrUndefined(v.Montant) ? '' : formatCurrency(String(v.Montant)).replace(",", "."),
                            montantDevise: isNullOrUndefined(v.MontantDevise) ? '' : formatCurrency(String(v.MontantDevise).replace(",", ".")),
                            mon: isNullOrUndefined(v.Mon) ? '' : v.Mon,
                            rang: isNullOrUndefined(v.Rang) ? '' : v.Rang,
                            financementCategorie: isNullOrUndefined(v.FinancementCategorie) ? '' : v.FinancementCategorie,
                            commune: isNullOrUndefined(v.Commune) ? '' : v.Commune,
                            plan: isNullOrUndefined(v.Plan6) ? '' : v.Plan6,
                            journal: isNullOrUndefined(v.Journal) ? '' : v.Journal,
                            marche: isNullOrUndefined(v.Marche) ? '' : v.Marche,
                            rejeter: '',
                            isLATE: v.IsLATE,
                            estAvance: v.AVANCE ? 'Avance' : 'Paiement',
                            numeroliquidations: v.NUMEROLIQUIDATION 
                        });
                    });

                    if (table !== undefined) {
                        table.destroy();
                    }

                    table = $('#TDB_OPA').DataTable({
                        data,
                        columns: [
                            {
                                data: 'checkbox',
                                render: function () {
                                    return `
                                        <input type="checkbox" name="checkprod" compteg-ischecked onchange="checkdel()" />
                                    `;
                                },
                                orderable: false
                            },
                            { data: 'id' },
                            { data: 'dateOrdre' },
                            { data: 'noPiece' },
                            { data: 'compte' },
                            { data: 'libelle' },
                            { data: 'debit' },
                            { data: 'credit' },
                            { data: 'montant' },
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' },
                            { data: 'numeroliquidations' },
                            { data: 'estAvance' },
                            {
                                data: 'rejeter',
                                render: function (_, _, row, _) {
                                    return `
                                        <div onclick="Refuser('${row.id}')">
                                            <i class="fa fa-times fa-lg text-dark"></i>
                                        </div>
                                    `;
                                }
                            }
                        ],
                        createdRow: function (row, data, _) {
                            $(row).attr('compteG-id', data.id);

                            $(row).addClass('select-text');
                            if (data.isLATE) {
                                $(row).attr('style', "background-color: #FF7F7F !important;");
                            }
                        },
                        columnDefs: [
                            {
                                targets: [-1],
                                className: 'elerfr'
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

    } else {
        //BR
        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetAcceptecritureLoad',
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
                    return;
                }
                if (Datas.type == "success") {
                    listResult = Datas.data;

                    const data = [];
                    arr = data;
                    $.each(listResult, function (_, v) {
                        data.push({
                            checkbox: '',
                            id: isNullOrUndefined(v.IDREGLEMENT) ? '' : v.IDREGLEMENT,
                            dateOrdre: isNullOrUndefined(v.dateOrdre) ? '' : v.dateOrdre,
                            noPiece: isNullOrUndefined(v.NoPiece) ? '' : v.NoPiece,
                            compte: isNullOrUndefined(v.Compte) ? '' : v.Compte,
                            libelle: isNullOrUndefined(v.Libelle) ? '' : v.Libelle,
                            debit: isNullOrUndefined(v.Debit) ? '' : formatCurrency(String(v.Debit).replace(",", ".")),
                            credit: isNullOrUndefined(v.Credit) ? '' : formatCurrency(String(v.Credit).replace(",", ".")),
                            montant: isNullOrUndefined(v.Montant) ? '' : formatCurrency(String(v.Montant)).replace(",", "."),
                            montantDevise: isNullOrUndefined(v.MontantDevise) ? '' : formatCurrency(String(v.MontantDevise).replace(",", ".")),
                            mon: isNullOrUndefined(v.Mon) ? '' : v.Mon,
                            rang: isNullOrUndefined(v.Rang) ? '' : v.Rang,
                            financementCategorie: isNullOrUndefined(v.FinancementCategorie) ? '' : v.FinancementCategorie,
                            commune: isNullOrUndefined(v.Commune) ? '' : v.Commune,
                            plan: isNullOrUndefined(v.Plan6) ? '' : v.Plan6,
                            journal: isNullOrUndefined(v.Journal) ? '' : v.Journal,
                            marche: isNullOrUndefined(v.Marche) ? '' : v.Marche,
                            rejeter: '',
                            isLATE: v.IsLATE,
                            type: v.AVANCE ? 'Avance':'Paiement',
                            estAvance: v.AVANCE,
                            numeroliquidations: v.NUMEROLIQUIDATION
                        });
                    });

                    if (table !== undefined) {
                        table.destroy();
                    }

                    table = $('#TDB_OPA').DataTable({
                        data,
                        columns: [
                            {
                                data: 'checkbox',
                                render: function () {
                                    return `
                                        <input type="checkbox" name="checkprod" compteg-ischecked onchange="checkdel()" />
                                    `;
                                },
                                orderable: false
                            },
                            { data: 'id' },
                            { data: 'dateOrdre' },
                            { data: 'noPiece' },
                            { data: 'compte' },
                            { data: 'libelle' },
                            { data: 'debit' },
                            { data: 'credit' },
                            { data: 'montant' },
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' },
                            { data: 'numeroliquidations' },
                            { data: 'type' },
                            {
                                data: 'rejeter',
                                render: function (_, _, row, _) {
                                    return `
                                        <div onclick="Refuser('${row.id}')">
                                            <i class="fa fa-times fa-lg text-dark"></i>
                                        </div>
                                    `;
                                }
                            }
                        ],
                        createdRow: function (row, data, _) {
                            $(row).attr('compteG-id', data.id);
                        },
                        columnDefs: [
                            {
                                targets: [-1],
                                className: 'elerfr'
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

    }
}

function GetAllProjectUser() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetAllProjectUser',
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
            reglementresult = ``;
            reglementresult = Datas.data;

            let listproject = ``;
            
            if (reglementresult.length) {
                $.each(reglementresult, function (_, v) {
                    listproject += `<option value="${v.ID}">${v.PROJET}</option>`;
                })
            } else {
                listproject += `<option value="${reglementresult.ID}" selected>${reglementresult.PROJET}</option>`;
            }

            $("#Fproject").html(listproject);
            GetTypeP();
            GetListCodeJournal();
            chargeLoad();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function FillAUXI() {
    var list = ListCompteG.filter(x => x.COGE == $(`[compG-list]`).val()).pop();

    let code = `<option value="Tous"> Tous</option> `;
    $.each(list.AUXI, function (_, v) {
        code += `
            <option value="${v}">${v}</option>
        `;
    });

    $(`[auxi-list]`).html('');
    $(`[auxi-list]`).html(code);
}

function FillCompteName() {
    var nom = $(`[compG-list]`).val() + " " + $(`[auxi-list]`).val();
    $(`[compte-name`).val(nom);
}

function GetListCompG() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    formData.append("baseName", baseName);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCompteG',
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
                return;
            }
            let code = ``;
            let codeAuxi = ``;
            ListCompteG = Datas.data;

            $.each(ListCompteG, function (_, v) {
                code += `
                    <option value="${v.COGE}">${v.COGE}</option>
                `;
            });
            $(`[compG-list]`).html('');
            $(`[compG-list]`).append(code);

            FillAUXI();
            FillCompteName();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function modalREJET(id) {

    clickedANN = id;

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("IdF", clickedANN);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetIsMotif',
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
                return;
            }

            $(`[data-id="MOTIF-list"]`).text("");

            var code = ``;
            listResult = Datas.data;

            $.each(listResult, function (_, v) {
                code += `
                    <option value="${v.REF}" id="${v.ID}">${v.REF}</option>
                `;
            });

            $(`[data-id="MOTIF-list"]`).append(code);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

    $('#annuler-modal').modal('toggle');
}

function Refuser(id) {
    $('#F-modal').modal('toggle');
    $('#F-modal').attr("data-id", id);
    modalREJET(id);
}

function AcceptRefuser() {
    const id = $('#F-modal').attr("data-id");
    let motif = $("#Motif").val();
    let commentaire = $("#Commentaire").val();

  

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    formData.append("baseName", baseName);
    formData.append("id", id);
    formData.append("motif", motif);
    formData.append("commentaire", commentaire);

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/CancelEcriture',
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

            listResultAnomalie = "";
            contentAnomalies = ``;
            if (Datas.type == "error") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "success") {
                $(`[compteG-id="${id}"]`).remove();
                $('#F-modal').modal('toggle');
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                return;
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

}
//==============================================================================================Ready===================================================================================

$(document).ready(() => {

    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetAllProjectUser();

});

$(document).on("change", "[code-project]", () => {
    $('#commercial').html('');
    GetTypeP();
    GetListCodeJournal();
    chargeLoad();
});

$(document).on("change", "[compG-list]", () => {
    FillAUXI();
    FillCompteName();
});

$(document).on("change", "[auxi-list]", () => {
    FillCompteName();
});

$(document).on("change", "[codej-list]", () => {
    var code = ListCodeJournal.filter(function (e) { return e.CODE == $(`[codej-list]`).val(); })[0];
    //$(`[codej-libelle]`).val(code.LIBELLE);
});

$(document).on("click", "[data-target]", function () {
    let me = $(this).closest("[data-target]");
    if ($(me).attr("data-type") == "switch_tab") {
        let target = $(`#${$(me).attr("data-target")}`);


        $(`[data-type="switch_tab"]`).each(function (i) {
            if ($(this).hasClass('active')) {

                $(this).removeClass('active');
                $(`#${$(this).attr("data-target")}`).hide();
            }
        });
        $(me).addClass("active");
        $(target).show();
    }
});

$('.Checkall').change(function () {

    if ($('.Checkall').prop("checked") == true) {

        $('[compteg-ischecked]').prop("checked", true);
    } else {
        $('[compteg-ischecked]').prop("checked", false);
    }

});
//==============================================================================================ChargeJs===================================================================================

$('[data-action="ChargerJs"]').click(function () {
    let formData = new FormData();
    let codeproject = $("#Fproject").val();

    formData.append("codeproject", codeproject);

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("ChoixBase", baseName);
    if (baseName == 2) {
        //compta
        formData.append("datein", $('#Pdu').val());
        formData.append("dateout", $('#Pau').val());
        formData.append("journal", $('#commercial').val());
        formData.append("comptaG", $('#comptaG').val());
        formData.append("auxi", $('#auxi').val());
        formData.append("auxi1", $('#auxi').val());
        formData.append("dateP", $('#Pay').val());
        formData.append("devise", false);
        formData.append("etat", $('#etat').val());

        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetAcceptecriture',
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
                $('.afb160').html('');
                if (Datas.type == "error") {
                    alert(Datas.msg);
                    return;
                }
                if (Datas.type == "login") {
                    alert(Datas.msg);
                    return;
                }
                if (Datas.type == "success") {
                    listResult = Datas.data;

                    const data = [];
                    arr = data;
                    $.each(listResult, function (k, v) {
                        data.push({
                            checkbox: '',
                            id: isNullOrUndefined(v.IDREGLEMENT) ? '' : v.IDREGLEMENT,
                            dateOrdre: isNullOrUndefined(v.dateOrdre) ? '' : v.dateOrdre,
                            noPiece: isNullOrUndefined(v.NoPiece) ? '' : v.NoPiece,
                            compte: isNullOrUndefined(v.Compte) ? '' : v.Compte,
                            libelle: isNullOrUndefined(v.Libelle) ? '' : v.Libelle,
                            debit: isNullOrUndefined(v.Debit) ? '' : formatCurrency(String(v.Debit).replace(",", ".")),
                            credit: isNullOrUndefined(v.Credit) ? '' : formatCurrency(String(v.Credit).replace(",", ".")),
                            montant: isNullOrUndefined(v.Montant) ? '' : formatCurrency(String(v.Montant)).replace(",", "."),
                            montantDevise: isNullOrUndefined(v.MontantDevise) ? '' : formatCurrency(String(v.MontantDevise).replace(",", ".")),
                            mon: isNullOrUndefined(v.Mon) ? '' : v.Mon,
                            rang: isNullOrUndefined(v.Rang) ? '' : v.Rang,
                            financementCategorie: isNullOrUndefined(v.FinancementCategorie) ? '' : v.FinancementCategorie,
                            commune: isNullOrUndefined(v.Commune) ? '' : v.Commune,
                            plan: isNullOrUndefined(v.Plan6) ? '' : v.Plan6,
                            journal: isNullOrUndefined(v.Journal) ? '' : v.Journal,
                            marche: isNullOrUndefined(v.Marche) ? '' : v.Marche,
                            rejeter: '',
                            isLATE: v.IsLATE ,
                            estAvance : v.AVANCE,
                            numeroliquidations: v.NUMEROLIQUIDATION,
                            type: v.AVANCE ? 'Avance' : 'Paiement'
                        });
                    });

                    if (table !== undefined) {
                        table.destroy();
                    }

                    table = $('#TDB_OPA').DataTable({
                        data,
                        columns: [
                            {
                                data: 'checkbox',
                                render: function () {
                                    return `
                                        <input type="checkbox" name="checkprod" compteg-ischecked onchange="checkdel()" />
                                    `;
                                },
                                orderable: false
                            },
                            { data: 'id' },
                            { data: 'dateOrdre' },
                            { data: 'noPiece' },
                            { data: 'compte' },
                            { data: 'libelle' },
                            { data: 'debit' },
                            { data: 'credit' },
                            { data: 'montant' },
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' },
                            {
                                data: 'numeroliquidations',
                                render: function (data, _, row, _) {
                                    return `
                                        <div onclick="showLiquidationModal('${row.id}', '${row.numeroliquidations}', '${row.estAvance}')">
                                            ${data}
                                        </div>
                                    `;
                                }
                            },
                            { data: 'type' },
                            {
                                data: 'rejeter',
                                render: function (_, _, row, _) {
                                    return `
                                        <div onclick="Refuser('${row.id}')">
                                            <i class="fa fa-times fa-lg text-dark"></i>
                                        </div>
                                    `;
                                }
                            }
                        ],
                        createdRow: function (row, data, _) {
                            $(row).attr('compteG-id', data.id);

                            $(row).addClass('select-text');
                            if (data.isLATE) {
                                $(row).attr('style', "background-color: #FF7F7F !important;");
                            }
                        },

                        columnDefs: [
                            {
                                targets: [-1],
                                className: 'elerfr'
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

    } else {
        //BR
        formData.append("datein", $('#Pdu').val());
        formData.append("dateout", $('#Pau').val());
        formData.append("journal", $('#commercial').val());
        formData.append("comptaG", $('#comptaG').val());
        formData.append("auxi", $('#auxi').val());
        formData.append("auxi1", $('#auxi').val());
        formData.append("dateP", $('#Pay').val());
        formData.append("etat", $('#etat').val());
        formData.append("devise", false);


        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetAcceptecriture',
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
                    return;
                }
                if (Datas.type == "success") {
                    listResult = Datas.data
                    content = ``;
                    const data = [];
                    arr = data;
                    $.each(listResult, function (_, v) {
                        data.push({
                            checkbox: '',
                            id: isNullOrUndefined(v.IDREGLEMENT) ? '' : v.IDREGLEMENT,
                            dateOrdre: isNullOrUndefined(v.dateOrdre) ? '' : v.dateOrdre,
                            noPiece: isNullOrUndefined(v.NoPiece) ? '' : v.NoPiece,
                            compte: isNullOrUndefined(v.Compte) ? '' : v.Compte,
                            libelle: isNullOrUndefined(v.Libelle) ? '' : v.Libelle,
                            debit: isNullOrUndefined(v.Debit) ? '' : formatCurrency(String(v.Debit).replace(",", ".")),
                            credit: isNullOrUndefined(v.Credit) ? '' : formatCurrency(String(v.Credit).replace(",", ".")),
                            montant: isNullOrUndefined(v.Montant) ? '' : formatCurrency(String(v.Montant)).replace(",", "."),
                            montantDevise: isNullOrUndefined(v.MontantDevise) ? '' : formatCurrency(String(v.MontantDevise).replace(",", ".")),
                            mon: isNullOrUndefined(v.Mon) ? '' : v.Mon,
                            rang: isNullOrUndefined(v.Rang) ? '' : v.Rang,
                            financementCategorie: isNullOrUndefined(v.FinancementCategorie) ? '' : v.FinancementCategorie,
                            commune: isNullOrUndefined(v.Commune) ? '' : v.Commune,
                            plan: isNullOrUndefined(v.Plan6) ? '' : v.Plan6,
                            journal: isNullOrUndefined(v.Journal) ? '' : v.Journal,
                            marche: isNullOrUndefined(v.Marche) ? '' : v.Marche,
                            rejeter: '',
                            isLATE: v.IsLATE,
                            estAvance: v.AVANCE,
                            numeroliquidations: v.NUMEROLIQUIDATION,
                            type: v.AVANCE ? 'Avance' : 'Paiement'
                        });
                    });

                    if (table !== undefined) {
                        table.destroy();
                    }

                    table = $('#TDB_OPA').DataTable({
                        data,
                        columns: [
                            {
                                data: 'checkbox',
                                render: function () {
                                    return `
                                        <input type="checkbox" name="checkprod" compteg-ischecked onchange="checkdel()" />
                                    `;
                                },
                                orderable: false
                            },
                            { data: 'id' },
                            { data: 'dateOrdre' },
                            { data: 'noPiece' },
                            { data: 'compte' },
                            { data: 'libelle' },
                            { data: 'debit' },
                            { data: 'credit' },
                            { data: 'montant' },
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' },
                            { data: 'numeroliquidations' },
                            { data: 'type' },
                            {
                                data: 'rejeter',
                                render: function (_, _, row, _) {
                                    return `
                                        <div onclick="Refuser('${row.id}')">
                                            <i class="fa fa-times fa-lg text-dark"></i>
                                        </div>
                                    `;
                                }
                            }
                        ],
                        createdRow: function (row, data, _) {
                            $(row).attr('compteG-id', data.id);

                            $(row).addClass('select-text');
                            if (data.isLATE) {
                                $(row).attr('style', "background-color: #FF7F7F !important;");
                            }
                        },
                        columnDefs: [
                            {
                                targets: [-1],
                                className: 'elerfr'
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
    }
});
//==============================================================================================Checked===================================================================================

$('[data-action="GetElementChecked"]').click(function () {
    let checkList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    //$.each(checkList, (k, v) => {
    //    list.push($(v).attr("compteG-id"));
    //});
    for (let i = 0; i < checkList.length; i += 1) {
        const id = $(checkList[i]).attr("compteG-id");

        const item = arr.find(item => item.id === id);

        list.push({
            id,
            estAvance: item.estAvance
        });

        console.log(item);
    }

    let formData = new FormData();
    //let listid = list.splice(',');

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);


    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    let datein = $('#Pdu').val();
    let dateout = $('#Pau').val();
    let journal = $('#commercial').val();
    let comptaG = $('#comptaG').val();
    let auxi = $('#auxi').val();
    let dateP = $('#Pay').val();
    let etat = $('#etat').val();

    formData.append("datein", datein);
    formData.append("dateout", dateout);
    formData.append("journal", journal);
    formData.append("comptaG", comptaG)
    formData.append("auxi", auxi);
    formData.append("auxi1", auxi);
    formData.append("dateP", dateP);
    formData.append("devise", false);
    formData.append("etat", etat);
    //formData.append("listCompte", listid);

    formData.append("listCompte", JSON.stringify(list));
    formData.append("baseName", baseName);

    //ValidationsEcrituresF
    //tokony io no antsoina
    $.ajax({
        type: "POST",
        url: Origin + '/Home/ValidationsEcrituresF',
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
            reglementresult = ``;
            reglementresult = Datas.data;

            for (let i = 0; i < checkList.length; i += 1) {
                table.row($(checkList[i])).remove().draw();
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

});

var baseName = "2";

$(`[name="options"]`).on("change", (k, v) => {

    var baseId = $(k.target).attr("data-id");
    baseName = baseId;
    if (baseId == "1") {
        $(`[tab="paie"]`).show();
        $(`[tab="autre"]`).hide();
        //GetListCodeJournal();
    } else {
        $(`[tab="autre"]`).show();
        $(`[tab="paie"]`).hide();
        $('#afb').empty();
        //GetListCodeJournal();
    }
});
