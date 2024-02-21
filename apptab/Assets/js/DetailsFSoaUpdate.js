﻿let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;
    //$("#base-container").hide();

    GetFSOA();
});
//let urlOrigin = Origin;
//let urlOrigin = "http://softwell.cloud/OPAVI";

function GetFSOA() {
    let formData = new FormData();

    let dbase;

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("SOAID", getUrlParameter("SOAID"));

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/DetailsFSOA',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            console.log(Datas);

            $('#Soa').val(Datas.data);
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
    }).done(function(result){
        var Datas = JSON.parse(result);
    });
}

$(`[data-action="UpdateFSOAJS"]`).click(function () {
    let formData = new FormData();
    let auth = "0";

    if ($(`[data-id="auth-list"]`).val() != null) {
        auth = $(`[data-id="auth-list"]`).val();
    }

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("SOAID", getUrlParameter("SOAID"));
    formData.append("SOAID_2", $('#Soa').val());

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/UpdatFSOA',
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
            if (Datas.type == "success") {
                alert(Datas.msg);
                window.location = Origin + "/SuperAdmin/SOAList";
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