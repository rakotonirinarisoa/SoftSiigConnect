let ContentAnomalie;
let Projet;
$(document).ready(() => {

    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    $(`[tab="autre"]`).hide();
    GetAllProjectUser();
    
    //GetListCodeJournal();
    //GetListCompG();
});
function GetAnomalie() {
    let formData = new FormData();

    let codeproject = $("#Fproject").val();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.ID", User.ID);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("codeproject", codeproject);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetAnomalieTomOP',
        //url: Origin + '/Home/GetAnomalieBack',
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
            //alert(Datas.data)
            if (Datas.type == "error") {
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
           
            ListResultBr = Datas.datas;
            console.log(ListResultBr);
            content = ``;
           
            $.each(ListResultBr, function (k, v) {
                ContentAnomalie += `<tr compteG-id="${v.ID}">`

                        if (!v.JOURNAL) ContentAnomalie += `<td class ="AnomalieClass">${v.JOURNAL}</td>`;
                        else ContentAnomalie += ` <td>${v.JOURNAL}</td>`;
                        if (!v.LIBELLE) ContentAnomalie += `<td class ="AnomalieClass">${v.LIBELLE}</td>`;
                        else ContentAnomalie += ` <td>${v.LIBELLE}</td>`;
                        if (!v.COMPTEG) ContentAnomalie += `<td class ="AnomalieClass">${v.COMPTEG}</td>`;
                        else ContentAnomalie += ` <td>${v.COMPTEG}</td>`;
                        if (v.RIB == null) ContentAnomalie += `<td class ="AnomalieClass">${v.RIB}</td>`;
                        else ContentAnomalie += ` <td>${v.RIB}</td>`;
                        if (v.AGENCE == null) ContentAnomalie += `<td class ="AnomalieClass">${v.AGENCE}</td>`;
                        else ContentAnomalie += ` <td>${v.AGENCE}</td>`;
                        if (v.GUICHET == null) ContentAnomalie += `<td class ="AnomalieClass">${v.GUICHET}</td>`;
                        else ContentAnomalie += ` <td>${v.GUICHET}</td>`;
                        if (v.TYPE == null) ContentAnomalie += `<td class ="AnomalieClass">${v.TYPE}</td>`;
                        else ContentAnomalie += ` <td>${v.TYPE}</td>`;
                        if (v.IDPROJECT == null) ContentAnomalie += `<td class ="AnomalieClass">${v.IDPROJECT}</td>`;
                                else ContentAnomalie += ` <td>${v.IDPROJECT}</td>`;
                ContentAnomalie += `</tr>`;
            });
          
            console.log(ContentAnomalie);
            $('#historiques').html(ContentAnomalie);
            //window.location = '/Home/GetFile?file=' + Datas.data;

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

$('[data-action="GetElementChecked"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });
    let codeproject = $("#Fproject").val();

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("listCompte", list);
    formData.append("baseName", baseName);
    formData.append("journal", $('#commercial').val());
    formData.append("etat", "");
    formData.append("devise", false);
    formData.append("codeproject", codeproject);

    let listid = list.splice(',');
    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetCancel',
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
            //alert(Datas.data)
            if (Datas.type == "error") {
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
            ListResult = Datas.data;
            content = ``;
            GetHistoriques()
            //window.location = '/Home/GetFile?file=' + Datas.data;

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});
$(`[tab="autre"]`).hide();

function GetAllProjectUser() {

    let formData = new FormData();
    let codeproject = $("#Fproject").val();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);
    formData.append("codeproject", codeproject);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetAllProjectUser',
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

            reglementresult = ``;

            reglementresult = Datas.data;

            let listproject = ``;

            if (reglementresult.length) {
                $.each(reglementresult, function (k, v) {
                    listproject += `<option value="${v.ID}">${v.PROJET}</option>`;
                })
            } else {
                listproject += `<option value="${reglementresult.ID}" selected>${reglementresult.PROJET}</option>`;
            }

            $("#Fproject").html(listproject);
            GetTypeP();
            GetAnomalie();
            //GetListCodeJournal();
           // LoadValidate();
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}
function GetTypeP() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDSOCIETE", User.IDSOCIETE);

    let codeproject = $("#Fproject").val();
    formData.append("codeproject", codeproject);

    $.ajax({
        type: "POST",
        url: Origin + '/Home/GetTypeP',
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
            baseName = Datas;
            if (baseName == 1) {
                $(`[code_Type]`).val('');
                $(`[code_Type]`).val('BR');

            } else {
                $(`[code_Type]`).val('');
                $(`[code_Type]`).val('COMPTA');
            }

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
    });
};