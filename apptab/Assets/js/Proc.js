﻿let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetListProjet();
});

//let urlOrigin = "http://softwell.cloud/OPAVI";
function GetUsers() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/Parametre/DetailsProc',
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
                $("#Para").val("");
                $("#Code").val("");
                $("#ParaD").val("");
                $("#CodeD").val("");
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            $("#Para").val(Datas.data.PROCEDURE);
            $("#Code").val(Datas.data.CODE);
            $("#ParaD").val(Datas.data.PROCEDUREDEG);
            $("#CodeD").val(Datas.data.CODEDEG);

            if (Datas.data.IDPROJET != 0)
                $("#proj").val(`${Datas.data.IDPROJET}`);
            else
                $("#proj").val("");
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$('#proj').on('change', () => {
    const id = $('#proj').val();
    GetUsers(id);
});

$(`[data-action="UpdateUser"]`).click(function () {
    let user = $("#Para").val();
    let code = $("#Code").val();
    let userD = $("#ParaD").val();
    let codeD = $("#CodeD").val();
    if (!user || !code || !userD || !codeD) {
        alert("Veuillez renseigner l'information sur le procédure. ");
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
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("param.PROCEDURE", $(`#Para`).val());
    formData.append("param.CODE", $(`#Code`).val());
    formData.append("param.PROCEDUREDEG", $(`#ParaD`).val());
    formData.append("param.CODEDEG", $(`#CodeD`).val());

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Parametre/UpdateProc',
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
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
            }
        },
    });
});

function GetListProjet() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Parametre/GetAllPROJET',
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
            //let i = 0;
            let pr = ``;
            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
                //pr = v.PROJET;
                //i++;
            });

            $(`[data-id="proj-list"]`).append(code);

            GetUsers();
        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}
