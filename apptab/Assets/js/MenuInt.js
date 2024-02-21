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
        url: Origin + '/SuperAdmin/DetailsMenu',
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
            
            $("#ParaV0").val(Datas.data.MT0);
            $("#ParaV").val(Datas.data.MT1);
            $("#ParaS").val(Datas.data.MT2);
            $("#ParaPe").val(Datas.data.MP1);
            $("#ParaPv").val(Datas.data.MP2);
            $("#ParaPp").val(Datas.data.MP3);
            $("#ParaPb").val(Datas.data.MP4);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$(`[data-action="UpdateUser"]`).click(function () {
    let ParaV0 = $("#ParaV0").val();
    let ParaV = $("#ParaV").val();
    let ParaS = $("#ParaS").val();
    let ParaPe = $("#ParaPe").val();
    let ParaPv = $("#ParaPv").val();
    let ParaPp = $("#ParaPp").val();
    let ParaPb = $("#ParaPb").val();
    if (!ParaV || !ParaS || !ParaPe || !ParaPv || !ParaPp || !ParaPb) {
        alert("Veuillez renseigner les intitulés des menus. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("param.MT0", $(`#ParaV0`).val());
    formData.append("param.MT1", $(`#ParaV`).val());
    formData.append("param.MT2", $(`#ParaS`).val());
    formData.append("param.MP1", $(`#ParaPe`).val());
    formData.append("param.MP2", $(`#ParaPv`).val());
    formData.append("param.MP3", $(`#ParaPp`).val());
    formData.append("param.MP4", $(`#ParaPb`).val());

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/UpdateMenu',
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