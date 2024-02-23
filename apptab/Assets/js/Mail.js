let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;
    $(`[data-id="username"]`).text(User.LOGIN);
    GetListProjet();
});

//let urlOrigin = Origin;
//let urlOrigin = "http://softwell.cloud/OPAVI";
function GetUsers() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());
    
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/DetailsMail',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);

            if (Datas.type == "error") {
                alert(Datas.msg);
                $("#ParaT").val("");
                $("#ParaV").val("");
                $("#ParaPi").val("");
                $("#ParaPe").val("");
                $("#ParaPv").val("");
                $("#ParaPp").val("");
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            $("#ParaT").val(Datas.data.MAILTE);
            $("#ParaV").val(Datas.data.MAILTV);
            $("#ParaPi").val(Datas.data.MAILPI);
            $("#ParaPe").val(Datas.data.MAILPE);
            $("#ParaPv").val(Datas.data.MAILPV);
            $("#ParaPp").val(Datas.data.MAILPP);
            //$("#ParaPb").val(Datas.data.MAILPB);

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
    let ParaT = $("#ParaT").val();
    let ParaV = $("#ParaV").val();
    let ParaPi = $("#ParaPi").val();
    let ParaPe = $("#ParaPe").val();
    let ParaPv = $("#ParaPv").val();
    let ParaPp = $("#ParaPp").val();
    //let ParaPb = $("#ParaPb").val();
    if (!ParaT || !ParaV || !ParaPi || !ParaPe || !ParaPv || !ParaPp /*|| !ParaPb*/) {
        alert("Veuillez renseigner les mails. ");
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

    formData.append("param.MAILTE", $(`#ParaT`).val());
    formData.append("param.MAILTV", $(`#ParaV`).val());
    formData.append("param.MAILPI", $(`#ParaPi`).val());
    formData.append("param.MAILPE", $(`#ParaPe`).val());
    formData.append("param.MAILPV", $(`#ParaPv`).val());
    formData.append("param.MAILPP", $(`#ParaPp`).val());
    //formData.append("param.MAILPB", $(`#ParaPb`).val());

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/SuperAdmin/UpdateMail',
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