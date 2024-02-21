﻿let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetUsers();
});

//let urlOrigin = "http://softwell.cloud/OPAVI";

function GetUsers() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    /*formData.append("UserId", getUrlParameter("UserId"));*/

    $.ajax({
        type: "POST",
        url: Origin + '/User/Param',
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

            $("#MDPA").val(Datas.data.PWD);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$(`[data-action="UpdateUser"]`).click(function () {
    let newpwd = $(`#MDPN`).val();
    let newpwdConf = $(`#MDPC`).val();
    if(newpwd != newpwdConf){
        alert("Les mots de passe ne correspondent pas. ");
        return;
    }

    let MDPA = $(`#MDPA`).val(); 
    if (!MDPA) {
        alert("Veuillez renseigner l'ancien mot de passe. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("user.PWD", $(`#MDPN`).val());
    formData.append("user.LOGIN", $(`#MDPA`).val());
    
    $.ajax({
        type: "POST",
        url: Origin + '/User/UpdateMDP',
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
            if (Datas.type == "success") {
                alert(Datas.msg);
                sessionStorage.setItem("user", null);
                window.location = window.location.origin;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
            }
        },
    });
});
