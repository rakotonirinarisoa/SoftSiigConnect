﻿$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    GetListUser();
    GetListPeriode();
    GetListType();
    GetListLien();

    $(`[data-id="lien-list"]`).change(function (k, v) {
        let nom = $(`#Lien`).val();
        $(`#LienTRUE`).val(nom);
    });
});

function GetListUser() {
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

            $(`[data-id="societe-list"]`).text("");
            var code = ``;
            $.each(Datas.data.List, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
            });
            $(`[data-id="societe-list"]`).append(code);

        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

function GetListPeriode() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/GetAllPeriode',
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

            $(`[data-id="periode-list"]`).text("");
            var code = ``;
            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.PERIODE}">${v.PERIODE}</option>
                `;
            });
            $(`[data-id="periode-list"]`).append(code);

        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

function GetListType() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/GetAllType',
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

            $(`[data-id="type-list"]`).text("");
            var code = ``;
            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.TYPE}">${v.TYPE}</option>
                `;
            });
            $(`[data-id="type-list"]`).append(code);

        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

function GetListLien() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/GetAllLien',
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

            $(`[data-id="lien-list"]`).text("");
            var code = ``;
            $.each(Datas.data, function (k, v) {
                code += `
                    <option value="${v.LIEN}">${v.TITLE}</option>
                `;
            });
            $(`[data-id="lien-list"]`).append(code);

        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    })
}

$(`[data-action="AddnewUser"]`).click(function () {
    let formData = new FormData();

    let Title = $(`#Title`).val();
    let Annee = $(`#Annee`).val();
    let Periode = $(`#Periode`).val();
    let Type = $(`#Type`).val();
    let Lien = $(`#LienTRUE`).val();
    let TITLEDOCS = $(`#Lien`).val();
    if (!Title || !Annee || !Periode || !Type || !Lien || !TITLEDOCS) {
        alert("Veuillez renseigner les informations afin d'enregistrer le document. ");
        return;
    }

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("Title", Title);
    formData.append("Annee", Annee);
    formData.append("Periode", Periode);
    formData.append("Type", Type);
    formData.append("Lien", Lien);
    formData.append("TITLEDOCS", TITLEDOCS);

    formData.append("IDPROJET", $(`#IDProjet`).val());

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/Create',
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
                window.location = Origin + "/RSF/Index";
                /*window.history.back();*/
                /*location.replace(document.referrer);*/
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
            }
        },
    });
});
