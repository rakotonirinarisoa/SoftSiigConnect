let User;
let Origin;
let compteur = 1;
let table;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);

    //GetListProjet();
    //GetUsers(undefined);
    
    IsProjet();

    GetListLOAD();//Partie ORDSEC
    GetListLOADOTHER();//Partie ORDSEC

    $(`[data-widget="pushmenu"]`).on('click', () => {
        $(`[data-action="SaveV"]`).toggleClass('custom-fixed-btn');
    });
});

function IsProjet() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetIsProjet',
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

            $("#proj").val(`${Datas.data}`);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

//function GetUsers(id) {
//    let formData = new FormData();

//    formData.append("suser.LOGIN", User.LOGIN);
//    formData.append("suser.PWD", User.PWD);
//    formData.append("suser.ROLE", User.ROLE);

//    if (!id) {
//        formData.append("suser.IDPROJET", User.IDPROJET);
//    } else {
//        formData.append("suser.IDPROJET", id);
//    }

//    $.ajax({
//        type: "POST",
//        url: Origin + '/Traitement/DetailsInfoPro',
//        data: formData,
//        cache: false,
//        contentType: false,
//        processData: false,
//        success: function (result) {
//            var Datas = JSON.parse(result);

//            if (Datas.type == "error") {
//                alert(Datas.msg);
//                return;
//            }
//            if (Datas.type == "login") {
//                alert(Datas.msg);
//                window.location = window.location.origin;
//                return;
//            }

//            $("#proj").val(`${Datas.data.PROJ}`);
//        },
//        error: function () {
//            alert("Problème de connexion. ");
//        }
//    });
//}

//let urlOrigin = "http://softwell.cloud/OPAVI";
//function GetListProjet() {
//    let formData = new FormData();

//    formData.append("suser.LOGIN", User.LOGIN);
//    formData.append("suser.PWD", User.PWD);
//    formData.append("suser.ROLE", User.ROLE);
//    formData.append("suser.IDPROJET", User.IDPROJET);

//    $.ajax({
//        type: "POST",
//        url: Origin + '/Traitement/GetAllPROJET',
//        data: formData,
//        cache: false,
//        contentType: false,
//        processData: false,
//        success: function (result) {
//            var Datas = JSON.parse(result);
//            console.log(Datas);

//            if (Datas.type == "error") {
//                alert(Datas.msg);
//                return;
//            }
//            if (Datas.type == "login") {
//                alert(Datas.msg);
//                window.location = window.location.origin;
//                return;
//            }

//            $(`[data-id="proj-list"]`).text("");
//            var code = ``;
//            $.each(Datas.data, function (k, v) {
//                code += `
//                    <option value="${v.ID}">${v.PROJET}</option>
//                `;
//            });
//            $(`[data-id="proj-list"]`).append(code);

//        },
//        error: function (e) {
//            console.log(e);
//            alert("Problème de connexion. ");
//        }
//    })
//}

//$('#proj').on('change', () => {
//    const id = $('#proj').val();

//    GetUsers(id);
//});


//GENERER//
$('[data-action="GenereR"]').click(async function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");
        return;
    }

    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    $.ajax({
        type: "POST",
        async : true,
        url: Origin + '/Traitement/Generation',
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
            if (Datas.type == "PEtat") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "Prese") {
                alert(Datas.msg);
                return;
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
            if (Datas.type == "success") {
                //window.location = window.location.origin;
                ListResult = Datas.data
                contentpaie = ``;
                $.each(ListResult, function (k, v) {
                    contentpaie += `
                    <tr compteG-id="${v.No}" class="select-text">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked class="chk" onchange = "checkdel('${v.No}')" />
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                    </tr>
                    `
                });

                $('.afb160Paie').empty();
                $('.afb160Paie').html(contentpaie);

                //new DataTable(`#TBD_PROJET`, {
                //    //dom: 'Bfrtip',
                //    //buttons: ['colvis'],
                //    //colReorder: false,
                    
                //    responsive: true,
                //    retrieve: true,
                //    paging: true,
                //    search: true
                //    //destroy:true
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});

function checkdel(id) {
    $('.Checkall').prop("checked", false);
}

//SIIGLOAD//
function GetListLOAD() {
    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIGLOAD',
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
            if (Datas.type == "success") {
                //window.location = window.location.origin;
                ListResult = Datas.data
                contentpaie = ``;
                $.each(ListResult, function (k, v) {
                    contentpaie += `
                   <tr compteG-id="${v.No}" class="select-text caret">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked  onchange = "checkdel()"/>
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.LIEN}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATECREATION)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalREJET('${v.No}')"><i class="fa fa-times fa-lg text-dark"></i></div>
                        </td>
                    </tr>`
                });

                $('.traitementORDSEC').empty();
                $('.traitementORDSEC').html(contentpaie);

                //new DataTable(`#TBD_PROJET`, {
                //    //dom: 'Bfrtip',
                //    //buttons: ['colvis'],
                //    //colReorder: false,

                //    responsive: true,
                //    retrieve: true,
                //    paging: true,
                //    search: true
                //    //destroy:true
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

//GetListLOADOTHER//
function GetListLOADOTHER() {
    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIGLOADOTHER',
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
            if (Datas.type == "success") {
                //window.location = window.location.origin;
                ListResult = Datas.data
                contentpaie = ``;
                $.each(ListResult, function (k, v) {
                    contentpaie += `
                   <tr compteG-id="${v.No}" class="select-text caret">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked  onchange = "checkdel()"/>
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                    </tr>`
                });

                $('.traitementORDSECOTHER').empty();
                $('.traitementORDSECOTHER').html(contentpaie);

                //new DataTable(`#TBD_PROJET`, {
                //    //dom: 'Bfrtip',
                //    //buttons: ['colvis'],
                //    //colReorder: false,

                //    responsive: true,
                //    retrieve: true,
                //    paging: true,
                //    search: true
                //    //destroy:true
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

//GENERER SIIGOTHER//
$('[data-action="GenereSIIGOTHER"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");
        return;
    }

    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIGOTHER',
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
            if (Datas.type == "success") {
                //window.location = window.location.origin;
                ListResult = Datas.data
                contentpaie = ``;
                $.each(ListResult, function (k, v) {
                    contentpaie += `
                   <tr compteG-id="${v.No}" class="select-text caret">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked  onchange = "checkdel()"/>
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                    </tr>`
                });

                $('.traitementORDSECOTHER').empty();
                $('.traitementORDSECOTHER').html(contentpaie);

                //new DataTable(`#TBD_PROJET`, {
                //    //dom: 'Bfrtip',
                //    //buttons: ['colvis'],
                //    //colReorder: false,

                //    responsive: true,
                //    retrieve: true,
                //    paging: true,
                //    search: true
                //    //destroy:true
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});

//GENERER SIIG//
$('[data-action="GenereSIIG"]').click(function () {
    let dd = $("#dateD").val();
    let df = $("#dateF").val();
    if (!dd || !df) {
        alert("Veuillez renseigner les dates afin de générer les mandats. ");
        return;
    }

    let formData = new FormData();
    //alert(baseName);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GenerationSIIG',
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
            if (Datas.type == "success") {
                //window.location = window.location.origin;
                ListResult = Datas.data
                contentpaie = ``;
                $.each(ListResult, function (k, v) {
                    contentpaie += `
                    <tr compteG-id="${v.No}" class="select-text">
                        <td style="font-weight: bold; text-align:center">
                            <input type="checkbox" name = "checkprod" compteg-ischecked  onchange = "checkdel()"/>
                        </td>
                        <td style="font-weight: bold; text-align:center">${v.REF}</td>
                        <td style="font-weight: bold; text-align:center">${v.OBJ}</td>
                        <td style="font-weight: bold; text-align:center">${v.TITUL}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.COMPTE}</td>
                        <td style="font-weight: bold; text-align:center">${v.PCOP}</td>
                        <td style="font-weight: bold; text-align:center">${formatCurrency(String(v.MONT).replace(",", "."))}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEDEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATETEF)}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATEBE)}</td>
                        <td style="font-weight: bold; text-align:center">${v.LIEN}</td>
                        <td style="font-weight: bold; text-align:center">${formatDate(v.DATECREATION)}</td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalD('${v.No}')"><i class="fa fa-tags fa-lg text-danger"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalF('${v.No}')"><i class="fa fa-tags fa-lg text-success"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalLIAS('${v.No}')"><i class="fa fa-tags fa-lg text-info"></i></div>
                        </td>
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="modalREJET('${v.No}')"><i class="fa fa-times fa-lg text-dark"></i></div>
                        </td>
                    </tr>`
                });

                $('.traitementORDSEC').empty();
                $('.traitementORDSEC').html(contentpaie);

                //new DataTable(`#TBD_PROJET`, {
                //    //dom: 'Bfrtip',
                //    //buttons: ['colvis'],
                //    //colReorder: false,

                //    responsive: true,
                //    retrieve: true,
                //    paging: true,
                //    search: true
                //    //destroy:true
                //});
            }
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});

$('[data-action="SaveV"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    if (list.length == 0) {
        alert("Veuillez sélectionner au moins un mandat afin de l'enregistrer et l'envoyer pour validation. ");
        return;
    }
    
    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("listCompte", list);

    formData.append("DateDebut", $('#dateD').val());
    formData.append("DateFin", $('#dateF').val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetCheckedEcritureF',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            alert(Datas.msg);
            $.each(CheckList, (k, v) => {
                list.push($(v).remove());
            });
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});
$('.Checkall').change(function () {

    if ($('.Checkall').prop("checked") == true) {

        $('[compteg-ischecked]').prop("checked", true);
    } else {
        $('[compteg-ischecked]').prop("checked", false);
    }

});


$('[data-action="SaveSIIG"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    let formData = new FormData();
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("listCompte", list);

    //formData.append("DateDebut", $('#dateD').val());
    //formData.append("DateFin", $('#dateF').val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/GetCheckedEcritureORDSEC',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            alert(Datas.msg);
            $.each(CheckList, (k, v) => {
                list.push($(v).remove());
            });
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
});
