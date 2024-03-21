$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetListProjet();
});

function GetUsers() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/Parametre/DetailsCorrEtat',
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
                $("#defC").val("");
                $("#tefC").val("");
                $("#beC").val("");
                $("#defCA").val("");
                $("#tefCA").val("");
                $("#beCA").val("");
                $("#stat1").val("");
                $("#stat2").val("");
                $("#stat3").val("");
                $("#stata1").val("");
                $("#stata2").val("");
                $("#stata3").val("");
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }

            if (Datas.data.crpto.IDPROJET != 0)
                $("#proj").val(`${Datas.data.crpto.IDPROJET}`);
            else
                $("#proj").val("");

            $(`[data-id="stat1-list"]`).text("");
            $(`[data-id="stat2-list"]`).text("");
            $(`[data-id="stat3-list"]`).text("");
            var code1 = ``;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.NUM}">${v.NOM}</option>
                `;
            });
            $(`[data-id="stat1-list"]`).append(code1);
            $(`[data-id="stat2-list"]`).append(code1);
            $(`[data-id="stat3-list"]`).append(code1);

            $(`[data-id="stata1-list"]`).text("");
            $(`[data-id="stata2-list"]`).text("");
            $(`[data-id="stata3-list"]`).text("");
            var code2 = ``;
            $.each(Datas.data.etatAvance, function (k, v) {
                code2 += `
                    <option value="${v.NUM}">${v.NOM}</option>
                `;
            });
            $(`[data-id="stata1-list"]`).append(code2);
            $(`[data-id="stata2-list"]`).append(code2);
            $(`[data-id="stata3-list"]`).append(code2);

            $("#defC").val(Datas.data.crpto.DEF);
            $("#tefC").val(Datas.data.crpto.TEF);
            $("#beC").val(Datas.data.crpto.BE);
            $("#defCA").val(Datas.data.crpto.DEFA);
            $("#tefCA").val(Datas.data.crpto.TEFA);
            $("#beCA").val(Datas.data.crpto.BEA);

            $("#stat1").val(Datas.data.crpto.DEF);
            $("#stat2").val(Datas.data.crpto.TEF);
            $("#stat3").val(Datas.data.crpto.BE);
            $("#stata1").val(Datas.data.crpto.DEFA);
            $("#stata2").val(Datas.data.crpto.TEFA);
            $("#stata3").val(Datas.data.crpto.BEA);
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

$('#stat1').on('change', () => {
    const id = $('#stat1').val();
    $("#defC").val(id);
});
$('#stat2').on('change', () => {
    const id = $('#stat2').val();
    $("#tefC").val(id);
});
$('#stat3').on('change', () => {
    const id = $('#stat3').val();
    $("#beC").val(id);
});

$('#stata1').on('change', () => {
    const id = $('#stata1').val();
    $("#defCA").val(id);
});
$('#stata2').on('change', () => {
    const id = $('#stata2').val();
    $("#tefCA").val(id);
});
$('#stata3').on('change', () => {
    const id = $('#stata3').val();
    $("#beCA").val(id);
});

$(`[data-action="UpdateUser"]`).click(function () {
    let defC = $("#defC").val();
    let tefC = $("#tefC").val();
    let beC = $("#beC").val();
    let defCA = $("#defCA").val();
    let tefCA = $("#tefCA").val();
    let beCA = $("#beCA").val();
    if (!defC || !tefC || !beC || !defCA || !tefCA || !beCA) {
        alert("Veuillez renseigner les informations sur la correspondance des états. ");
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

    formData.append("param.DEF", $(`#defC`).val());
    formData.append("param.TEF", $(`#tefC`).val());
    formData.append("param.BE", $(`#beC`).val());
    formData.append("param.DEFA", $(`#defCA`).val());
    formData.append("param.TEFA", $(`#tefCA`).val());
    formData.append("param.BEA", $(`#beCA`).val());

    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Parametre/UpdateCorrEtat',
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
