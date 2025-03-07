$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetListUser();
});

function GetListUser() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/FillTable',
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
            $.each(Datas.data, function (k, v) {
                code += `
                    <tr data-userId="${v.ID}" class="text-nowrap last-hover">
                        <td>${v.SOA}</td>
                        <td>${v.PROJET}</td>
                        <td>${v.TITRE}</td>
                        <td>${v.ANNEE}</td>
                        <td>${v.MOIS}</td>
                        <td>${v.PERIODE}</td>
                        <td>${v.TYPE}</td>
                        <td style="font-weight: bold; text-align:center"><a href="${v.LIEN}" target="_blank">${v.LIEN}</a></td>
                        <td class="elerfr">
                            <div onclick="DetailUpdate('${v.ID}')"><i class="fa fa-pen-alt text-warning"></i></div>
                        </td>
                        <td class="elerfr">
                            <div onclick="deleteMAPP('${v.ID}')"><i class="fa fa-trash text-danger"></i></div>
                        </td>
                    </tr>
                `;
            });

            $(`[data-id="ubody"]`).append(code);

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function deleteMAPP(id) {
    if (!confirm("Etes-vous sûr de vouloir supprimer le document ?")) return;
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("MAPPId", id);

    $.ajax({
        type: "POST",
        url: Origin + '/RSF/Delete',
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

            alert(Datas.msg);
            $(`[data-userId="${id}"]`).remove();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function DetailUpdate(id) {
    window.location = Origin + "/RSF/Details?UserId=" + id;
}
