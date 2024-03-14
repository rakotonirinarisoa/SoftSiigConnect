$(document).ready(() => {
    $(`[data-widget="pushmenu"]`).on('click', () => {
        $(`[data-action="SaveSIIG"]`).toggleClass('custom-fixed-btn');
    });
});

$(`[data-action="ANNULMANDAT"]`).click(function () {
    let user = $("#Motif").val();
    if (!user) {
        alert("Veuillez renseigner le motif du rejet avant l'annulation du mandat. ");
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
    formData.append("iProjet", $("#proj").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/AnnulationMandat',
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
            }
            if (Datas.type == "success") {
                alert(Datas.msg);

                $(`[compteG-id="${clickedANN}"]`).remove();

                $("#annuler-modal").modal("toggle");

                return;
            }
        },
    });
});
