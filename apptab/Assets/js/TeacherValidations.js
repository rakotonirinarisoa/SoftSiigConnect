let User;
let Origin;

let ListCodeJournal;
let ListCompteG;

var content;
let validate;

let ListResult;
let ListResultAnomalie;
let contentAnomalies;
var rdv
let contentpaie;
let ListResultpaie;
let reglementresult;

let listEtat;
let etaCode;
$(document).ready(() => {

    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    
    GetAllProjectUser();
});

$(document).on("change", "[code-project]", () => {
    GetListCodeJournal();
    ChargeLoad();
});
function GetAllProjectUser() {

    let formData = new FormData();
    let codeproject = $("#Fproject").val();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("codeproject", codeproject);
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
            console.log(reglementresult);
            let listproject = ``;
            if (reglementresult.length) {
                $.each(reglementresult, function (k, v) {
                    listproject += `<option value="${v.ID}">${v.PROJET}</option>`;
                })
            } else {
                listproject += `<option value="${reglementresult.ID}" selected>${reglementresult.PROJET}</option>`;
            }

            $("#Fproject").html(listproject);
            GetListCodeJournal();
            ChargeLoad();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}
function ChargeLoad() {
    let formData = new FormData();

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("ChoixBase", baseName);
    if (baseName == 2) {

        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetElementAvaliderLoad',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                var Datas = JSON.parse(result);
                console.log(Datas.data);

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
                    //window.location = window.location.origin;
                    ListResult = Datas.data
                    console.log(ListResult);
                    $.each(ListResult, function (k, v) {
                        content += `
                    <tr compteG-id="${v.IDREGLEMENT}">
                        <td>
                            <input type="checkbox" name = "checkprod" compteg-ischecked onchange = "checkdel()"/>
                        </td><td>${v.IDREGLEMENT}</td>
                        <td>${v.dateOrdre}</td>
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
                        <td></td>
                        <td class="elerfr" style="font-weight: bold; text-align:center" ><div onclick="Refuser('${v.IDREGLEMENT}')"><i class="fa fa-times fa-lg text-dark"</i></div></td>
                    </tr>`

                    });
                    $('.afb160').empty();
                    $('.afb160').html(content);
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
            url: Origin + '/Home/GetElementAvalider',
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
                    //window.location = window.location.origin;
                    ListResult = Datas.data
                    content = ``;
                    $.each(ListResult, function (k, v) {
                        content += `
                    <tr compteG-id="${v.No}">
                        <td>
                            <input type="checkbox" name = "checkprod" compteg-ischecked onchange = "checkdel()"/>
                        </td><td>${v.No}</td>
                        <td>${v.Date}</td>
                        <td>${v.NoPiece}</td>
                        <td>${v.Compte}</td>
                        <td>${v.Libelle}</td>
                        <td>${v.Montant}</td>
                        <td>${v.MontantDevise}</td>
                        <td>${v.Mon}</td>
                        <td>${v.Rang}</td>
                        <td>${v.Poste}</td>
                        <td>${v.FinancementCategorie}</td>
                        <td>${v.Commune}</td>
                        <td>${v.Plan6}</td>
                        <td>${v.Journal}</td>
                        <td>${v.Marche}</td>
                        <td>${v.Status}</td>
                    </tr>`

                    });
                    $('.afb160').empty();
                    $('.afb160').html(content);

                }


            },
            error: function () {
                alert("Problème de connexion. ");
            }
        });

        $('.afb160').empty()
    }

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
            let code = ``;
            let codeAuxi = ``;
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

function FillAUXI() {
    var list = ListCompteG.filter(x => x.COGE == $(`[compG-list]`).val()).pop();
    console.log(list);
    let code = `<option value="Tous"> Tous</option> `;
    $.each(list.AUXI, function (k, v) {
        code += `
                    <option value="${v}">${v}</option>
                `;
    });

    $(`[auxi-list]`).html('');
    $(`[auxi-list]`).html(code);
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
function FillCompteName() {
    var nom = $(`[compG-list]`).val() + " " + $(`[auxi-list]`).val();
    $(`[compte-name`).val(nom);
}

$(document).on("change", "[compG-list]", () => {
    FillAUXI();
    FillCompteName();
});

$(document).on("change", "[auxi-list]", () => {
    FillCompteName();
});

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

                console.log($(this));
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
function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

$('[data-action="ChargerJs"]').click(function () {
    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("ChoixBase", baseName);

    let datein = $('#Pdu').val();
    let dateout = $('#Pau').val();
    let journal = $('#commercial').val();
    let comptaG = $('#comptaG').val();
    let auxi = $('#auxi').val();
    let dateP = $('#Pay').val();
    let etat = $('#etat').val();
    if (baseName == 2) {
       // compta
        formData.append("datein", !datein ? '' : datein );
        formData.append("dateout", !dateout ? '' : dateout);
        formData.append("journal", !journal ? '' : journal);
        formData.append("comptaG", !comptaG ? '' : comptaG)
        formData.append("auxi",!auxi ? '' : auxi );
        formData.append("auxi1", !auxi ? '' : auxi);
        formData.append("dateP", !dateP ? '' : dateP);
        formData.append("devise", false);
        formData.append("etat", !etat ? '' : etat);
        
        $.ajax({
            type: "POST",
            url: Origin + '/Home/GetElementAvalider',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                var Datas = JSON.parse(result);
                console.log(Datas.data);

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
                    //window.location = window.location.origin;
                    ListResult = Datas.data
                    $.each(ListResult, function (k, v) {
                        content += `
                    <tr compteG-id="${v.IDREGLEMENT}">
                        <td>
                            <input type="checkbox" name = "checkprod" compteg-ischecked onchange = "checkdel()"/>
                        </td><td>${v.IDREGLEMENT}</td>
                        <td>${v.dateOrdre}</td>
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
                        <td></td>
                        <td class="elerfr" style="font-weight: bold; text-align:center" ><div onclick="Refuser('${v.IDREGLEMENT}')"><i class="fa fa-times fa-lg text-dark"</i></div></td>
                    </tr>`

                    });
                    $('.afb160').empty();
                    $('.afb160').html(content);
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
            url: Origin + '/Home/GetElementAvalider',
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
                    //window.location = window.location.origin;
                    ListResult = Datas.data
                    content = ``;
                    $.each(ListResult, function (k, v) {
                        content += `
                    <tr compteG-id="${v.IDREGLEMENT}">
                        <td>
                            <input type="checkbox" name = "checkprod" compteg-ischecked onchange = "checkdel()"/>
                        </td><td>${v.IDREGLEMENT}</td>
                        <td>${v.dateOrdre}</td>
                        <td>${v.NoPiece}</td>
                        <td>${v.Compte}</td>
                        <td>${v.Libelle}</td>
                        <td>${v.Montant}</td>
                        <td>${v.MontantDevise}</td>
                        <td>${v.Mon}</td>
                        <td>${v.Rang}</td>
                        <td>${v.Poste}</td>
                        <td>${v.FinancementCategorie}</td>
                        <td>${v.Commune}</td>
                        <td>${v.Plan6}</td>
                        <td>${v.Journal}</td>
                        <td>${v.Marche}</td>
                        <td>${v.Status}</td>
                    </tr>`

                    });
                    $('.afb160').empty();
                    $('.afb160').html(content);

                }


            },
            error: function () {
                alert("Problème de connexion. ");
            }
        });

        $('.afb160').empty()
    }

});
function Refuser(id) {
    $('#F-modal').modal('toggle');
    $('#F-modal').attr("data-id", id);
    modalREJET(id);
    
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

            $(`[data-id="MOTIF-list"]`).text("");

            var code = ``;
            ListResult = Datas.data
            $.each(ListResult, function (k, v) {
                code += `
                    <option value="${v.REF}" id="${k}">${v.REF}</option>
                `;
            });
            
            $(`[data-id="MOTIF-list"]`).append(code);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

    $('#F-modal').modal('toggle');
}
function AcceptRefuser() {
    const id = $('#F-modal').attr("data-id"); 
    let motif = $("#Motif").val();
    let commentaire = $("#Commentaire").val();

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("baseName", baseName);
    formData.append("id", id);
    formData.append("motif", motif);
    formData.append("commentaire", commentaire);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/CancelEcriture',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            console.log(Datas);

            ListResultAnomalie = "";
            contentAnomalies = ``;
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
            }
            $(`[compteG-id="${id}"]`).remove();
            $('#F-modal').modal('toggle');
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });

}
$('[data-action="GetElementChecked"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    let formData = new FormData();
    console.log(list);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("listCompte", list);
    formData.append("baseName", baseName);
    formData.append("journal", $('#commercial').val());
    formData.append("devise", false);

    let datein = $('#Pdu').val();
    let dateout = $('#Pau').val();
    let journal = $('#commercial').val();
    let comptaG = $('#comptaG').val();
    let auxi = $('#auxi').val();
    let dateP = $('#Pay').val();
    let etat = $('#etat').val();

    let listid = list.splice(',');

    formData.append("datein", datein);
    formData.append("dateout",dateout);
    formData.append("journal",journal);
    formData.append("comptaG", comptaG)
    formData.append("auxi",auxi);
    formData.append("auxi1",auxi);
    formData.append("dateP", dateP);
    formData.append("devise", false);
    formData.append("etat", etat);
    formData.append("listCompte", listid);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetElementValiderF',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            reglementresult = ``;
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
            console.log(Datas);

            ListResultAnomalie = "";
            contentAnomalies = ``;
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
                //window.location = window.location.origin;
                ListResultAnomalie = Datas.data;
                $.each(ListResultAnomalie, function (k, v) {
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


//$(`[tab="autre"]`).hide();
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
        $('.afb160').empty();
        $('#afb').empty();
        //GetListCodeJournal();
    }

});

let urlOrigin = Origin;
//let urlOrigin = "http://softwell.cloud/OPAVI";
