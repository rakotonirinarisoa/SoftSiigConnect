let contentpaie;
var table = undefined;
let arr = [];
let idtype = 0;
const pass = $('#user-password');
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
function checkdel(id) {
    $('.Checkall').prop("checked", false);
}
function GetHistoriques() {
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
        url: Origin + '/Home/GetHistoriques',
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
            const data = [];
            ListResult = Datas.data
            ListResultBr = Datas.databr
            content = ``;
            $.each(ListResult, function (k, v) {
                data.push({
                    checkbox: '',
                    dateREG: isNullOrUndefined(v.DATEAFB) ? '' : v.DATEAFB,
                    id: isNullOrUndefined(v.NUMENREG) ? '' : v.NUMENREG,
                    dateFile: isNullOrUndefined(v.DATEAFB) ? '' : v.DATEAFB,
                    site: isNullOrUndefined(v.SITE) ? '' : v.SITE,
                    Date: isNullOrUndefined(v.DATE) ? '' : v.DATE,
                    Guichet: isNullOrUndefined(v.GUICHET) ? '' : formatCurrency(String(v.GUICHET).replace(",", ".")),
                    Journal: isNullOrUndefined(v.CODE_J) ? '' : formatCurrency(String(v.CODE_J).replace(",", ".")),
                    NomFichier: isNullOrUndefined(v.LIBELLE) ? '' : v.LIBELLE,
                    Banque: isNullOrUndefined(v.BANQUE) ? '' : v.BANQUE,
                    Montant: isNullOrUndefined(v.MONTANT) ? '' : formatCurrency(v.MONTANT),
                    RIB: isNullOrUndefined(v.RIB) ? '' : v.RIB,
                    login: isNullOrUndefined(v.LOGIN) ? '' : v.LOGIN,
                    Notifications: isNullOrUndefined(v.NOTIFICATION) ? '' : v.NOTIFICATION,
                })
            });
            $.each(ListResultBr, function (k, v) {
                data.push({
                    checkbox: '',
                    dateREG: isNullOrUndefined(v.DATEAFB) ? '' : v.DATEAFB,
                    id: isNullOrUndefined(v.NUMENREG) ? '' : v.NUMENREG,
                    dateFile: isNullOrUndefined(v.DATEAFB) ? '' : v.DATEAFB,
                    site: isNullOrUndefined(v.SITE) ? '' : v.SITE,
                    Date: isNullOrUndefined(v.DATE) ? '' : v.DATE,
                    Guichet: isNullOrUndefined(v.GUICHET) ? '' : formatCurrency(String(v.GUICHET).replace(",", ".")),
                    Journal: isNullOrUndefined(v.CODE_J) ? '' : formatCurrency(String(v.CODE_J).replace(",", ".")),
                    NomFichier: isNullOrUndefined(v.LIBELLE) ? '' : v.LIBELLE,
                    Banque: isNullOrUndefined(v.BANQUE) ? '' : v.BANQUE,
                    Montant: isNullOrUndefined(v.MONTANT) ? '' : formatCurrency(v.MONTANT),
                    RIB: isNullOrUndefined(v.RIB) ? '' : v.RIB,
                    login: isNullOrUndefined(v.LOGIN) ? '' : v.LOGIN,
                    Notifications: isNullOrUndefined(v.NOTIFICATION) ? '' : v.NOTIFICATION,
                })
            });
           
            if (table !== undefined) {
                table.destroy();
            }
            table = $('#idTable').DataTable({
                data,
                columns: [
                    {
                        data: 'checkbox',
                        render: function () {
                            return `
                                        <input type="checkbox" name="checkprod" compteg-ischecked onchange="checkdel()" />
                                    `;
                        },
                        orderable: false
                    },
                    { data: 'dateREG' },
                    { data: 'id' },
                    { data: 'dateFile' },
                    { data: 'site' },
                    { data: 'Journal' },
                    { data: 'NomFichier' },
                    { data: 'Banque' },
                    { data: 'Montant' },
                    { data: 'RIB' },
                    { data: 'login' },
                    //{
                    //    data: 'Notifications',
                    //    render: function (data, _, row, _) {
                    //        return `
                    //                    <div onclick="showLiquidationModal('${codeproject}', '${row.numeroliquidations}', '${row.estAvance}')" style="color: #007bff; text-decoration: underline; cursor: pointer;">
                    //                        ${data}
                    //                    </div>
                    //                `;
                    //    }
                    //},
                ],
                createdRow: function (row, data, _) {
                    $(row).attr('compteG-id', data.id);
                    $(row).addClass('select-text');
                    if (data.isLATE) {
                        //$(row).addClass("demoRayure");
                        $(row).children('td').eq(0).addClass("demoRayure");
                    } if (data.AUTREOP) {
                        $(row).children('td').eq(0).addClass("AUTREOPCSS");
                    }
                    if (data.estAvance) {
                        $(row).children('td').eq(0).addClass("AVANCECSS");
                    }
                  
                },
                columnDefs: [
                    {
                        targets: [-1],
                        className: 'elerfr'
                    }
                ],
                colReorder: {
                    enable: false,
                    fixedColumnsLeft: 1
                },
                deferRender: true,
                pageLength: 25,
                dom: 'Bfrtip',
                buttons: ['colvis'],

                initComplete: function () {
                    $(`thead td[data-column-index="${0}"]`).removeClass('sorting_asc').removeClass('sorting_desc');
                    count = 0;
                    this.api().columns().every(function () {
                        var title = this.header();
                        //replace spaces with dashes
                        title = $(title).html().replace(/[\W]/g, '-');
                        var column = this;
                        var select = $('<select id="' + title + '" class="select2" ></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                //Get the "text" property from each selected data 
                                //regex escape the value and store in array
                                var data = $.map($(this).select2('data'), function (value, key) {
                                    return value.text ? '^' + $.fn.dataTable.util.escapeRegex(value.text) + '$' : null;
                                });

                                //if no data selected use ""
                                if (data.length === 0) {
                                    data = [""];
                                }

                                //join array into string with regex or (|)
                                var val = data.join('|');

                                //search for the option(s) selected
                                column
                                    .search(val ? val : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });

                        //use column title as selector and placeholder
                        $('#' + title).select2({
                            multiple: true,
                            closeOnSelect: false

                        });

                        //initially clear select otherwise first option is selected
                        $('.select2').val(null).trigger('change');
                    });
                }

            });
            $('#idTable tfoot th').each(function (i) {
                if (i == 0) {
                    $(this).addClass("NOTVISIBLE");
                }
            });
            //window.location = '/Home/GetFile?file=' + Datas.data;

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}


$('[data-action="GetElementChecked"]').click(function () {
   
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
            GetHistoriques()
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

$('[data-action="SaveV"]').click(function () {
    let CheckList = $(`[compteg-ischecked]:checked`).closest("tr");
    let typeF = $(this).attr(`data-type`);
    idtype = typeF;
    let list = [];
    $.each(CheckList, (k, v) => {
        list.push($(v).attr("compteG-id"));
    });

    if (list.length == 0) {
        alert("Veuillez sélectionner au moins un mandat afin de l'enregistrer et l'envoyer pour validation. ");
        return;
    }

    pass.text('');
    $('#password').val('');
    $('#verification-modal').modal('toggle');
});

$('#get-user-password-btn').on('click', () => {
    let formData = new FormData();
    console.log(idtype);
    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDSOCIETE);

    formData.append("userPassword", $("#password").val());

    $.ajax({
        type: "POST",
        url: Origin + '/Traitement/Password',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function () {
            loader.removeClass('display-none');
        },
        complete: function () {
            //loader.addClass('display-none');
        },
        success: function (result) {
            const res = JSON.parse(result);
            if (res.type === 'error') {
                pass.css({ 'color': 'red' });
                pass.text('Identifiants incorrects.');
            } else {
                // OKOK();
                getCancelWithPsw();
            }
        },
        Error: function (_, e) {
            alert(e);
        }
    });
});

function getCancelWithPsw() {
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
            GetHistoriques();
            $('#verification-modal').modal('toggle');
            //window.location = '/Home/GetFile?file=' + Datas.data;

        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}