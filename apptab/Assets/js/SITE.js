$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    GetListSociete();
    GetListProjet();
});

function GetListSociete() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/FillTableSITE',
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

            $(`[data-id="ubody"]`).text("");

            var code = ``;
            if (Datas.data != null) {
                $.each(Datas.data, function (k, v) {
                    code += `
                    <tr data-PROJETId="${v.ID}" class="text-nowrap last-hover">
                        <td>${v.PROJET}</td>
                        <td>${v.USER}</td>
                        <td>${v.SITES}</td>
                        
                        <td class="elerfr">
                            <div onclick="DetailPROSOA('${v.ID}')"><i class="fa fa-pen-alt text-warning"></i></div>
                        </td>
                        <td class="elerfr">
                            <div onclick="deletePROSOA('${v.ID}')"><i class="fa fa-trash text-danger"></i></div>
                        </td>
                    </tr>
                `;
                });
            }

            $(`[data-id="ubody"]`).append(code);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function DetailPROSOA(id) {
    window.location = Origin + "/Privilege/SuperAdminDetailFPROSOA?PROSOAID=" + id;
}

function deletePROSOA(id) {
    //alert("eto");
    if (!confirm("Etes-vous sûr de vouloir supprimer le privilège SITE de l'utilisateur ?")) return;
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("SITE", id);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/DeleteSITE',
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
            alert(Datas.msg);
            $(`[data-projetid="${id}"]`).remove();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function GetListProjet() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/GetAllPROJET',
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
            var code = `<option value=""></option>`;
            $.each(Datas.data.List, function (k, v) {
                code += `
                    <option value="${v.ID}">${v.PROJET}</option>
                `;
            });
            $(`[data-id="proj-list"]`).append(code);

        },
        error: function (e) {
            alert("Problème de connexion. ");
        }
    });
}

$('#proj').on('change', () => {
    const id = $('#proj').val();
    GetUsers(id);
});

function GetUsers() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/GETALLUSER',
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

                $(`[data-id="user-list"]`).text("");
                var code1 = `<option value=""></option>`;
                $.each(Datas.data.etat, function (k, v) {
                    code1 += `
                    <option value="${v.ID}">${v.LOGIN}</option>
                `;
                });
                $(`[data-id="user-list"]`).append(code1);

                return;
            }

            $(`[data-id="user-list"]`).text("");

            var code1 = `<option value=""></option>`;
            $.each(Datas.data.etat, function (k, v) {
                code1 += `
                    <option value="${v.ID}">${v.LOGIN}</option>
                `;
            });
            $(`[data-id="user-list"]`).append(code1);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$('#user').on('change', () => {
    const id = $('#user').val();
    GetSITE(id);
});
function GetSITE() {
    let formData = new FormData();

    formData.append("iProjet", $("#proj").val());
    formData.append("iUser", $("#user").val());

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

            let issite = Datas.data.site;

            $("#site").val([...issite]).trigger('change');
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$(`[data-action="AddSITE"]`).click(function () {
    let formData = new FormData();

    let socP = $("#proj").val();
    if (!socP) {
        alert("Veuillez renseigner le projet. ");
        return;
    }

    let soc = $("#user").val();
    if (!soc) {
        alert("Veuillez renseigner l'utilisateur. ");
        return;
    }

    let pr = $("#site").val();
    if (!pr) {
        alert("Veuillez sélectionner au moins un site. ");
        return;
    }

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("site.IDPROJET", $(`#proj`).val());
    formData.append("site.IDUSER", $(`#user`).val());
    formData.append("listSite", $("#site").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/AddSite',
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

