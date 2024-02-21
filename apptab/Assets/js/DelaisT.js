let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;
    $(`[data-id="username"]`).text(User.LOGIN);
    GetUsers();
});

//let urlOrigin = Origin;
//let urlOrigin = "http://softwell.cloud/OPAVI";
function GetUsers() {
    let formData = new FormData();
    
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/DetailsDelais',
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
            
            $("#ParaV").val(Datas.data.DELTV);
            $("#ParaS").val(Datas.data.DELSIIGFP);
            $("#ParaPe").val(Datas.data.DELPE);
            $("#ParaPv").val(Datas.data.DELPV);
            $("#ParaPp").val(Datas.data.DELPP);
            $("#ParaPb").val(Datas.data.DELPB);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$(`[data-action="UpdateUser"]`).click(function () {
    let ParaV = $("#ParaV").val();
    let ParaS = $("#ParaS").val();
    let ParaPe = $("#ParaPe").val();
    let ParaPv = $("#ParaPv").val();
    let ParaPp = $("#ParaPp").val();
    let ParaPb = $("#ParaPb").val();
    if (!ParaV || !ParaS || !ParaPe || !ParaPv || !ParaPp || !ParaPb) {
        alert("Veuillez renseigner les délais de traitement. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("param.DELTV", $(`#ParaV`).val());
    formData.append("param.DELSIIGFP", $(`#ParaS`).val());
    formData.append("param.DELPE", $(`#ParaPe`).val());
    formData.append("param.DELPV", $(`#ParaPv`).val());
    formData.append("param.DELPP", $(`#ParaPp`).val());
    formData.append("param.DELPB", $(`#ParaPb`).val());

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/UpdateDelais',
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
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
            }
        },
    });
});