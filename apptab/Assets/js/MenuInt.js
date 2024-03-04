$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;
    $(`[data-id="username"]`).text(User.LOGIN);
    GetUsers();
});

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
            
            $("#ParaV0").val(Datas.data.MTNON);
            $("#ParaV").val(Datas.data.MT0);
            $("#ParaS").val(Datas.data.MT1);
            $("#ParaSiig").val(Datas.data.MT2);
            $("#ParaPe").val(Datas.data.MP1);
            $("#ParaPv").val(Datas.data.MP2);
            $("#ParaPp").val(Datas.data.MP3);
            $("#ParaPb").val(Datas.data.MP4);

            $("#Md0").val(Datas.data.MD0);
            $("#Md1").val(Datas.data.MD1);
            $("#Md2").val(Datas.data.MD2);

            $("#Mop0").val(Datas.data.MOP0);
            $("#Mop1").val(Datas.data.MOP1);
            $("#Mop2").val(Datas.data.MOP2);
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
    let ParaSiig = $("#ParaSiig").val();
    let ParaPe = $("#ParaPe").val();
    let ParaPv = $("#ParaPv").val();
    let ParaPp = $("#ParaPp").val();
    let ParaPb = $("#ParaPb").val();

    let Md0 = $("#Md0").val();
    let Md1 = $("#Md1").val();
    let Md2 = $("#Md2").val();

    let Mop0 = $("#Mop0").val();
    let Mop1 = $("#Mop1").val();
    let Mop2 = $("#Mop2").val();

    if (!ParaV0 || !ParaV || !ParaS || !ParaSiig || !ParaPe || !ParaPv || !ParaPp || !ParaPb || !Md0 || !Md1 || !Md2 || !Mop0 || !Mop1 || !Mop2) {
        alert("Veuillez renseigner les intitulés des menus. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("param.MTNON", $(`#ParaV0`).val());
    formData.append("param.MT0", $(`#ParaV`).val());
    formData.append("param.MT1", $(`#ParaS`).val());
    formData.append("param.MT2", $(`#ParaSiig`).val());
    formData.append("param.MP1", $(`#ParaPe`).val());
    formData.append("param.MP2", $(`#ParaPv`).val());
    formData.append("param.MP3", $(`#ParaPp`).val());
    formData.append("param.MP4", $(`#ParaPb`).val());

    formData.append("param.MD0", $(`#Md0`).val());
    formData.append("param.MD1", $(`#Md1`).val());
    formData.append("param.MD2", $(`#Md2`).val());

    formData.append("param.MOP0", $(`#Mop0`).val());
    formData.append("param.MOP1", $(`#Mop1`).val());
    formData.append("param.MOP2", $(`#Mop2`).val());

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/UpdateMenu',
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
