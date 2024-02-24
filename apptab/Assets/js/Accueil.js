let User;
let Origin;

let ListCodeJournal;
let ListCompteG;

var content;
let validate;

let listResult;
let listResultAnomalie;
let contentAnomalies;
var rdv
let contentpaie;
let listResultpaie;
let reglementresult;

let listEtat;
let etaCode;

var baseName;

let table = undefined;

function checkdel(id) {
    $('.Checkall').prop("checked", false);
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
        success: function (result) {
            var Datas = JSON.parse(result);
            baseName = Datas;

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
        success: function (result) {
            var Datas = JSON.parse(result);
            listEtat = Datas.data
            if (Datas.type == "error") {
                return;
            }
            if (Datas.type == "login") {
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

function GetListCompG() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    //formData.append("baseName", id);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCompteG',
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

            let code = ``;

            ListCompteG = Datas.data;

            $.each(ListCompteG, function (k, v) {
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

function GetListCodeJournal() {
    let formData = new FormData();

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);
    //formData.append("baseName", id);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCODEJournal',
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
                //window.location = window.location.origin;
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
        success: function (result) {
            var Datas = JSON.parse(result);
            reglementresult = ``;
            reglementresult = Datas.data;
            
            let listproject = ``;

            if (reglementresult.length) {
                $.each(reglementresult, function (k, v) {
                    listproject += `<option value="${v.ID}">${v.PROJET}</option>`;
                })
            } else {
                listproject += `<option value="${reglementresult.ID}" selected>${reglementresult.PROJET}</option>`;
            }

            $("#Fproject").html(listproject);
            GetTypeP();
            GetListCodeJournal();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function FillAUXI() {
    var list = ListCompteG.filter(x => x.COGE == $(`[compG-list]`).val()).pop();

    let code = `<option value="Tous"> Tous</option> `;
    $.each(list.AUXI, function (k, v) {
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

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    //$(`[tab="autre"]`).hide();

    /*console.log($(`[tab="autre"]`).hide());*/
    GetAllProjectUser();
    //GetTypeP();
    //GetUR();
    //GetListCodeJournal();
    //GetListCompG();
});

$(document).on("change", "[code-project]", () => {
    GetListCodeJournal();
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
    $(`[codej-libelle]`).val(code.LIBELLE);
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

$(`[data-action="CreateTxt"]`).click(function () {
    getelementTXT(0);
})

$(`[data-action="CreateTxtCrypter"]`).click(function () {
    getelementTXT(1);
})

$(`[data-action="CreateTxtSend"]`).click(function () {
    getelementTXT(2);
})

$(`[data-action="CreateTxtFTPCrypter"]`).click(function () {
    getelementTXT(3);
})

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
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    formData.append("ChoixBase", baseName);

    if (baseName == 2) {
        formData.append("datein", $('#Pdu').val());
        formData.append("dateout", $('#Pau').val());
        formData.append("journal", $('#commercial').val());
        formData.append("comptaG", $('#comptaG').val());
        formData.append("auxi", $('#auxi').val());
        formData.append("auxi1", $('#auxi').val());
        formData.append("dateP", $('#Pay').val());

        if ($('#ChkDevise').prop("checked") == true) {
            formData.append("devise", true);
        } else {
            formData.append("devise", false);
        }

        $.ajax({
            type: "POST",
            url: Origin + '/Home/getelementjs',
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
                    listResult = Datas.data;
                    console.log(listResult)

                    const data = [];

                    $.each(listResult, function (_, v) {
                       
                        data.push({
                            checkbox: '',
                            id: v.No,
                            dateOrdre: v.DateOrdre === null ? 'NULL' : v.DateOrdre,
                            noPiece: v.NoPiece === null ? 'NULL' : v.NoPiece,
                            compte: v.Compte === null ? 'NULL' : v.Compte,
                            libelle: v.Libelle === null ? 'NULL' : v.Libelle,
                            debit: v.Debit === null ? 'NULL' : v.Debit,
                            credit: v.Credit === null ? 'NULL' : v.Credit,
                            montantDevise: v.MontantDevise === null ? 'NULL' : v.MontantDevise,
                            mon: v.Mon === null ? 'NULL' : v.Mon,
                            rang: v.Rang === null ? 'NULL' : v.Rang,
                            financementCategorie: v.FinancementCategorie === null ? 'NULL' : v.FinancementCategorie,
                            commune: v.Commune ===null ? 'NULL' :v.Commune,
                            plan: v.Plan6 === null ? 'NULL' : v.Plan6,
                            journal: v.Journal === null ? 'NULL' : v.Journal,
                            marche: v.Marche === null ? 'NULL' : v.Marche,
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
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' }
                        ],
                        colReorder: {
                            enable: true,
                            fixedColumnsLeft: 1
                        },
                        deferRender: true,
                        createdRow: function (row, data, _) {
                            $(row).attr('compteG-id', data.id);
                            $(row).addClass('select-text');
                        },
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

        let codeproject = $("#Fproject").val();
        formData.append("codeproject", codeproject);

        if ($('#ChkDevise').prop("checked") == true) {
            formData.append("devise", true);
        } else {
            formData.append("devise", false);
        }

        $.ajax({
            type: "POST",
            url: Origin + '/Home/getelementjsBR',
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
                    content = ``;

                    const data = [];

                    $.each(listResult, function (_, v) {
                        data.push({
                            checkbox: '',
                            id: v.No,
                            date: v.Date,
                            noPiece: v.NoPiece,
                            compte: v.Compte,
                            libelle: v.Libelle,
                            montant: v.Montant,
                            montantDevise: v.MontantDevise,
                            mon: v.Mon,
                            rang: v.Rang,
                            poste: v.Poste,
                            financementCategorie: v.FinancementCategorie,
                            commune: v.Commune,
                            plan: v.Plan6,
                            journal: v.Journal,
                            marche: v.Marche,
                            status: v.Status === undefined ? '' : v.Status
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
                            { data: 'date' },
                            { data: 'noPiece' },
                            { data: 'compte' },
                            { data: 'libelle' },
                            { data: 'montant' },
                            { data: 'montantDevise' },
                            { data: 'mon' },
                            { data: 'rang' },
                            { data: 'poste' },
                            { data: 'financementCategorie' },
                            { data: 'commune' },
                            { data: 'plan' },
                            { data: 'journal' },
                            { data: 'marche' },
                            { data: 'status' }
                        ],
                        colReorder: {
                            enable: true,
                            fixedColumnsLeft: 1
                        },
                        deferRender: true,
                        createdRow: function (row, _, _) {
                            $(row).attr('compteG-id', row.id);
                            $(row).addClass('select-text');
                        },
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

        $('.afb160').empty()
    }
});

$('[data-action="GetElementChecked"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    let codeproject = $("#Fproject").val();

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("listCompte", list);
    formData.append("codeproject", codeproject);
    //formData.append("baseName", baseName);
    formData.append("journal", $('#commercial').val());
    formData.append("devise", false);
    let listid = list.splice(',');
    formData.append("datein", $('#Pdu').val());
    formData.append("dateout", $('#Pau').val());
    formData.append("comptaG", $('#comptaG').val());
    formData.append("auxi", $('#auxi').val());
    formData.append("auxi1", $('#auxi').val());
    formData.append("dateP", $('#Pay').val());
    formData.append("etat", $('#etat').val());
    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCheckedCompte',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            $.each(listid, (k, v) => {
                $(`[compteG-id="${v}"]`).remove();
            });
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

});

$('[data-action="GetAnomalieListes"]').click(function () {

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    formData.append("baseName", baseName);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetAnomalieBack',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);

            listResultAnomalie = "";
            contentAnomalies = ``;

            if (Datas.type == "error") {
                alert(Datas.msg);

                return;
            }

            if (Datas.type == "login") {
                alert(Datas.msg);

                return;
            }

            if (Datas.type == "success") {
                listResultAnomalie = Datas.data;

                $.each(listResultAnomalie, function (_, v) {
                    contentAnomalies += `<tr compteG-id="${v.No}">
                        <td>
                            <input type="checkbox" name = "checkprod" compteg-ischecked/>
                        </td><td>${v.No}</td>
                        <td>${v.DateOrdre}</td>
                        <td>${v.NoPiece}</td>
                        <td>${v.Compte}</td>
                        <td>${v.Libelle}</td>
                        <td>${v.Debit}</td>
                        <td>${v.Credit}</td>
                        <td>${v.MontantDevise}</td>
                        <td>${v.Mon}</td>
                        <td>${v.Rang}</td>
                        <td>${v.FinancementCategorie}</td>
                        <td>${v.Commune}</td>
                        <td>${v.Plan6}</td>
                        <td>${v.Journal}</td>
                        <td>${v.Marche}</td>
                    </tr>`

                });

                $('.anomalieslist').html(contentAnomalies);
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
})

function getelementTXT(a) {

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.ID", User.ID);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);
    formData.append("baseName", baseName);
    formData.append("codeJ", $('#commercial').val());
    formData.append("devise", false);
    formData.append("intbasetype", a);
    $.ajax({
        type: "POST",
        url: Origin + '/Home/CreateZipFile',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            alert(Datas.data)
            if (Datas.type == "error") {
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            window.location = '/Home/GetFile?file=' + Datas.data;

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

//$(`[tab="autre"]`).hide();
var baseName = "2";

$(`[name="options"]`).on("change", (k, v) => {
    var baseId = $(k.target).attr("data-id");
    if (baseId == 0) {
        baseName = "2"
    } else {
        baseName = baseId;
    }

    $(`[tab="autre"]`).show();
    $('.afb160').empty();
    $('#afb').empty();

    GetListCodeJournal(baseName);
});
