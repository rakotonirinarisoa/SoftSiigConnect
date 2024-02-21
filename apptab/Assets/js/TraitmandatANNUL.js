$(document).ready(() => {
    $(`[data-widget="pushmenu"]`).on('click', () => {
        $(`[data-action="SaveSIIG"]`).toggleClass('custom-fixed-btn');
    });
});

let clickedANN;

function modalREJET(id) {

    clickedANN = id;

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("IdF", clickedANN);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetIsMotif',
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

            $(`[data-id="MOTIF-list"]`).text("");

            var code = ``;
            ListResult = Datas.data
            $.each(ListResult, function (k, v) {
                code += `
                    <option value="${v.REF}" id="${v.REF}">${v.REF}</option>
                `;
            });

            $(`[data-id="MOTIF-list"]`).append(code);
        },
        error: function () {
            alert("Probl�me de connexion. ");
        }
    });

    $("#Commentaire").val("");

    $('#annuler-modal').modal('toggle');
}

$(`[data-action="ANNULMANDAT"]`).click(function () {
    let user = $("#Motif").val();
    if (!user) {
        alert("Veuillez renseigner le motf du rejet avant l'annulation du mandat. ");
        return;
    }

    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("IdF", clickedANN);
    formData.append("Comm", $(`#Commentaire`).val());
    formData.append("Motif", user);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/AnnulationMandat',
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

                $(`[compteG-id="${clickedANN}"]`).remove();
                
                $("#annuler-modal").modal("toggle");
                
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
            }
        },
    });
});