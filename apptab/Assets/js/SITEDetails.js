$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    //GetListProjet();

    GetDetails(getUrlParameter("PROSOAID"));
});

let isListeSie = '';
let idpro = '';
let idus = '';

function GetDetails(id) {
    let formData = new FormData();

    let dbase;

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("MappageId", id);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/DetailsMAPP',
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

            $("#proj").val(Datas.data.PROJETNAME);
            $("#user").val(Datas.data.USERNAME);

            idpro = Datas.data.PROJET;
            idus = Datas.data.USER;

            const idU = Datas.data.USER;
            GetSITE(idU);

            isListeSie = Datas.data.SITE;
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function GetSITE() {
    let formData = new FormData();

    formData.append("iProjet", idpro);
    formData.append("iUser", idus);

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/GETALLSITE',
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
            if (Datas.type == "notYet") {
                alert(Datas.msg);

                $(`[data-id="site-list"]`).text("");
                var code1 = ``;
                $.each(Datas.data.etat, function (k, v) {
                    code1 += `
                    <option value="${v.CODE}">${v.LIBELLE}</option>
                `;
                });
                $(`[data-id="site-list"]`).append(code1);

                return;
            }

            $(`[data-id="site-list"]`).text("");

            var code1 = ``;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.CODE}">${v.LIBELLE}</option>
                `;
            });
            $(`[data-id="site-list"]`).append(code1);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    }).done(function (result) {
        if (isListeSie != '') { $("#site").val([...isListeSie]).trigger("change"); }
    });
}

$(`[data-action="AddSITE"]`).click(function () {
    let formData = new FormData();

    let idprosoauP = getUrlParameter("PROSOAID");

    let pr = $("#site").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un site. ");
        return;
    }

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("idMppage", idprosoauP);
    formData.append("listSite", $("#site").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/UpdateSite',
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
            if (Datas.type == "success") {
                alert(Datas.msg);
                window.location = Origin + "/Privilege/SiteList";
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
        },
    });
});

