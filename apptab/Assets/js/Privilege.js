let User;
let Origin;

$(document).ready(() => {
    User = JSON.parse(sessionStorage.getItem("user"));
    if (User == null || User === "undefined") window.location = User.origin;
    Origin = User.origin;

    $(`[data-id="username"]`).text(User.LOGIN);
    GetListUser();

    $("#idTable").DataTable()
});

function GetListUser() {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/FillTable',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            var Datas = JSON.parse(result);
            console.log(Datas);

            if (Datas.type == "error") {
                alert("eeee" + Datas.msg);
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

                let MENUPAR1N = "", MENUPAR1R = "", MENUPAR1A = "";
                if (v.MENUPAR1 == 0) MENUPAR1N = "checked";
                if (v.MENUPAR1 == 1) MENUPAR1R = "checked";
                if (v.MENUPAR1 == 2) MENUPAR1A = "checked";

                let MENUPAR2N = "", MENUPAR2R = "", MENUPAR2A = "";
                if (v.MENUPAR2 == 0) MENUPAR2N = "checked";
                if (v.MENUPAR2 == 1) MENUPAR2R = "checked";
                if (v.MENUPAR2 == 2) MENUPAR2A = "checked";

                let MENUPAR3N = "", MENUPAR3R = "", MENUPAR3A = "";
                if (v.MENUPAR3 == 0) MENUPAR3N = "checked";
                if (v.MENUPAR3 == 1) MENUPAR3R = "checked";
                if (v.MENUPAR3 == 2) MENUPAR3A = "checked";

                let MENUPAR4N = "", MENUPAR4R = "", MENUPAR4A = "";
                if (v.MENUPAR4 == 0) MENUPAR4N = "checked";
                if (v.MENUPAR4 == 1) MENUPAR4R = "checked";
                if (v.MENUPAR4 == 2) MENUPAR4A = "checked";

                let MENUPAR5N = "", MENUPAR5R = "", MENUPAR5A = "";
                if (v.MENUPAR5 == 0) MENUPAR5N = "checked";
                if (v.MENUPAR5 == 1) MENUPAR5R = "checked";
                if (v.MENUPAR5 == 2) MENUPAR5A = "checked";

                let MENUPAR6N = "", MENUPAR6R = "", MENUPAR6A = "";
                if (v.MENUPAR6 == 0) MENUPAR6N = "checked";
                if (v.MENUPAR6 == 1) MENUPAR6R = "checked";
                if (v.MENUPAR6 == 2) MENUPAR6A = "checked";

                let MENUPAR7N = "", MENUPAR7R = "", MENUPAR7A = "";
                if (v.MENUPAR7 == 0) MENUPAR7N = "checked";
                if (v.MENUPAR7 == 1) MENUPAR7R = "checked";
                if (v.MENUPAR7 == 2) MENUPAR7A = "checked";

                let MENUPAR8N = "", MENUPAR8R = "", MENUPAR8A = "";
                if (v.MENUPAR8 == 0) MENUPAR8N = "checked";
                if (v.MENUPAR8 == 1) MENUPAR8R = "checked";
                if (v.MENUPAR8 == 2) MENUPAR8A = "checked";

                let MT0N = "", MT0R = "", MT0A = "";
                if (v.MT0 == 0) MT0N = "checked";
                if (v.MT0 == 1) MT0R = "checked";
                if (v.MT0 == 2) MT0A = "checked";

                let MT1N = "", MT1R = "", MT1A = "";
                if (v.MT1 == 0) MT1N = "checked";
                if (v.MT1 == 1) MT1R = "checked";
                if (v.MT1 == 2) MT1A = "checked";

                let MT2N = "", MT2R = "", MT2A = "";
                if (v.MT2 == 0) MT2N = "checked";
                if (v.MT2 == 1) MT2R = "checked";
                if (v.MT2 == 2) MT2A = "checked";

                let MP1N = "", MP1R = "", MP1A = "";
                if (v.MP1 == 0) MP1N = "checked";
                if (v.MP1 == 1) MP1R = "checked";
                if (v.MP1 == 2) MP1A = "checked";

                let MP2N = "", MP2R = "", MP2A = "";
                if (v.MP2 == 0) MP2N = "checked";
                if (v.MP2 == 1) MP2R = "checked";
                if (v.MP2 == 2) MP2A = "checked";

                let MP3N = "", MP3R = "", MP3A = "";
                if (v.MP3 == 0) MP3N = "checked";
                if (v.MP3 == 1) MP3R = "checked";
                if (v.MP3 == 2) MP3A = "checked";

                let MP4N = "", MP4R = "", MP4A = "";
                if (v.MP4 == 0) MP4N = "checked";
                if (v.MP4 == 1) MP4R = "checked";
                if (v.MP4 == 2) MP4A = "checked";

                let TDB0N = "", TDB0R = "", TDB0A = "";
                if (v.TDB0 == 0) TDB0N = "checked";
                if (v.TDB0 == 1) TDB0R = "checked";
                if (v.TDB0 == 2) TDB0A = "checked";

                let GEDN = "", GEDR = "", GEDA = "";
                if (v.GED == 0) GEDN = "checked";
                if (v.GED == 1) GEDR = "checked";
                if (v.GED == 2) GEDA = "checked";

                code += `
                    <tr data-userId="${v.ID}" class="text-nowrap last-hover">
                        <td>${v.PROJET}</td>
                        <td>${v.LOGIN}</td>
                        <td>${v.ROLE}</td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR1${v.ID}" name="droneMENUPAR1${v.ID}" value="0" ${MENUPAR1N}/><label class="ml-1" for="noneMENUPAR1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR1${v.ID}" name="droneMENUPAR1${v.ID}" value="1" ${MENUPAR1R}/><label class="ml-1" for="readMENUPAR1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR1${v.ID}" name="droneMENUPAR1${v.ID}" value="2" ${MENUPAR1A}/><label class="ml-1" for="writeMENUPAR1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR2${v.ID}" name="droneMENUPAR2${v.ID}" value="0" ${MENUPAR2N}/><label class="ml-1" for="noneMENUPAR2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR2${v.ID}" name="droneMENUPAR2${v.ID}" value="1" ${MENUPAR2R}/><label class="ml-1" for="readMENUPAR2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR2${v.ID}" name="droneMENUPAR2${v.ID}" value="2" ${MENUPAR2A}/><label class="ml-1" for="writeMENUPAR2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR3${v.ID}" name="droneMENUPAR3${v.ID}" value="0" ${MENUPAR3N}/><label class="ml-1" for="noneMENUPAR3${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR3${v.ID}" name="droneMENUPAR3${v.ID}" value="1" ${MENUPAR3R}/><label class="ml-1" for="readMENUPAR3${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR3${v.ID}" name="droneMENUPAR3${v.ID}" value="2" ${MENUPAR3A}/><label class="ml-1" for="writeMENUPAR3${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR4${v.ID}" name="droneMENUPAR4${v.ID}" value="0" ${MENUPAR4N}/><label class="ml-1" for="noneMENUPAR4${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR4${v.ID}" name="droneMENUPAR4${v.ID}" value="1" ${MENUPAR4R}/><label class="ml-1" for="readMENUPAR4${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR4${v.ID}" name="droneMENUPAR4${v.ID}" value="2" ${MENUPAR4A}/><label class="ml-1" for="writeMENUPAR4${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR5${v.ID}" name="droneMENUPAR5${v.ID}" value="0" ${MENUPAR5N}/><label class="ml-1" for="noneMENUPAR5${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR5${v.ID}" name="droneMENUPAR5${v.ID}" value="1" ${MENUPAR5R}/><label class="ml-1" for="readMENUPAR5${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR5${v.ID}" name="droneMENUPAR5${v.ID}" value="2" ${MENUPAR5A}/><label class="ml-1" for="writeMENUPAR5${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR6${v.ID}" name="droneMENUPAR6${v.ID}" value="0" ${MENUPAR6N}/><label class="ml-1" for="noneMENUPAR6${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR6${v.ID}" name="droneMENUPAR6${v.ID}" value="1" ${MENUPAR6R}/><label class="ml-1" for="readMENUPAR6${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR6${v.ID}" name="droneMENUPAR6${v.ID}" value="2" ${MENUPAR6A}/><label class="ml-1" for="writeMENUPAR6${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR7${v.ID}" name="droneMENUPAR7${v.ID}" value="0" ${MENUPAR7N}/><label class="ml-1" for="noneMENUPAR7${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR7${v.ID}" name="droneMENUPAR7${v.ID}" value="1" ${MENUPAR7R}/><label class="ml-1" for="readMENUPAR7${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR7${v.ID}" name="droneMENUPAR7${v.ID}" value="2" ${MENUPAR7A}/><label class="ml-1" for="writeMENUPAR7${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR8${v.ID}" name="droneMENUPAR8${v.ID}" value="0" ${MENUPAR8N}/><label class="ml-1" for="noneMENUPAR8${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR8${v.ID}" name="droneMENUPAR8${v.ID}" value="1" ${MENUPAR8R}/><label class="ml-1" for="readMENUPAR8${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR8${v.ID}" name="droneMENUPAR8${v.ID}" value="2" ${MENUPAR8A}/><label class="ml-1" for="writeMENUPAR8${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMT0${v.ID}" name="droneMT0${v.ID}" value="0" ${MT0N}/><label class="ml-1" for="noneMT0${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMT0${v.ID}" name="droneMT0${v.ID}" value="1" ${MT0R}/><label class="ml-1" for="readMT0${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMT0${v.ID}" name="droneMT0${v.ID}" value="2" ${MT0A}/><label class="ml-1" for="writeMT0${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMT1${v.ID}" name="droneMT1${v.ID}" value="0" ${MT1N}/><label class="ml-1" for="noneMT1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMT1${v.ID}" name="droneMT1${v.ID}" value="1" ${MT1R}/><label class="ml-1" for="readMT1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMT1${v.ID}" name="droneMT1${v.ID}" value="2" ${MT1A}/><label class="ml-1" for="writeMT1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMT2${v.ID}" name="droneMT2${v.ID}" value="0" ${MT2N}/><label class="ml-1" for="noneMT2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMT2${v.ID}" name="droneMT2${v.ID}" value="1" ${MT2R}/><label class="ml-1" for="readMT2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMT2${v.ID}" name="droneMT2${v.ID}" value="2" ${MT2A}/><label class="ml-1" for="writeMT2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMP1${v.ID}" name="droneMP1${v.ID}" value="0" ${MP1N}/><label class="ml-1" for="noneMP1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMP1${v.ID}" name="droneMP1${v.ID}" value="1" ${MP1R}/><label class="ml-1" for="readMP1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMP1${v.ID}" name="droneMP1${v.ID}" value="2" ${MP1A}/><label class="ml-1" for="writeMP1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMP2${v.ID}" name="droneMP2${v.ID}" value="0" ${MP2N}/><label class="ml-1" for="noneMP2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMP2${v.ID}" name="droneMP2${v.ID}" value="1" ${MP2R}/><label class="ml-1" for="readMP2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMP2${v.ID}" name="droneMP2${v.ID}" value="2" ${MP2A}/><label class="ml-1" for="writeMP2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMP3${v.ID}" name="droneMP3${v.ID}" value="0" ${MP3N}/><label class="ml-1" for="noneMP3${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMP3${v.ID}" name="droneMP3${v.ID}" value="1" ${MP3R}/><label class="ml-1" for="readMP3${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMP3${v.ID}" name="droneMP3${v.ID}" value="2" ${MP3A}/><label class="ml-1" for="writeMP3${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMP4${v.ID}" name="droneMP4${v.ID}" value="0" ${MP4N}/><label class="ml-1" for="noneMP4${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMP4${v.ID}" name="droneMP4${v.ID}" value="1" ${MP4R}/><label class="ml-1" for="readMP4${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMP4${v.ID}" name="droneMP4${v.ID}" value="2" ${MP4A}/><label class="ml-1" for="writeMP4${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB0${v.ID}" name="droneTDB0${v.ID}" value="0" ${TDB0N}/><label class="ml-1" for="noneTDB0${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB0${v.ID}" name="droneTDB0${v.ID}" value="1" ${TDB0R}/><label class="ml-1" for="readTDB0${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB0${v.ID}" name="droneTDB0${v.ID}" value="2" ${TDB0A}/><label class="ml-1" for="writeTDB0${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>
                        
                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneGED${v.ID}" name="droneGED${v.ID}" value="0" ${GEDN}/><label class="ml-1" for="noneGED${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readGED${v.ID}" name="droneGED${v.ID}" value="1" ${GEDR}/><label class="ml-1" for="readGED${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeGED${v.ID}" name="droneGED${v.ID}" value="2" ${GEDA}/><label class="ml-1" for="writeGED${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>
                        
                        <td class="elerfr" style="font-weight: bold; text-align:center">
                            <div onclick="SavePRIV('${v.ID}')"><i class="fa fa-save fa-lg text-danger"></i></div>
                        </td>
                    </tr >`;
            });

            $(`[data-id="ubody"]`).append(code);
        },
        error: function () {
            alert("Problème de connexion. ");
        }
    });
}

function SavePRIV(id) {
    let formData = new FormData();

    formData.append("suser.LOGIN", User.LOGIN);
    formData.append("suser.PWD", User.PWD);
    formData.append("suser.ROLE", User.ROLE);
    formData.append("suser.IDPROJET", User.IDPROJET);

    formData.append("UserId", id);

    formData.append("privilege.MENUPAR1", $(`input[name="droneMENUPAR1${id}"]:checked`).val());
    formData.append("privilege.MENUPAR2", $(`input[name="droneMENUPAR2${id}"]:checked`).val());
    formData.append("privilege.MENUPAR3", $(`input[name="droneMENUPAR3${id}"]:checked`).val());
    formData.append("privilege.MENUPAR4", $(`input[name="droneMENUPAR4${id}"]:checked`).val());
    formData.append("privilege.MENUPAR5", $(`input[name="droneMENUPAR5${id}"]:checked`).val());
    formData.append("privilege.MENUPAR6", $(`input[name="droneMENUPAR6${id}"]:checked`).val());
    formData.append("privilege.MENUPAR7", $(`input[name="droneMENUPAR7${id}"]:checked`).val());
    formData.append("privilege.MENUPAR8", $(`input[name="droneMENUPAR8${id}"]:checked`).val());

    formData.append("privilege.MT0", $(`input[name="droneMT0${id}"]:checked`).val());
    formData.append("privilege.MT1", $(`input[name="droneMT1${id}"]:checked`).val());
    formData.append("privilege.MT2", $(`input[name="droneMT2${id}"]:checked`).val());
    
    formData.append("privilege.MP1", $(`input[name="droneMP1${id}"]:checked`).val());
    formData.append("privilege.MP2", $(`input[name="droneMP2${id}"]:checked`).val());
    formData.append("privilege.MP3", $(`input[name="droneMP3${id}"]:checked`).val());
    formData.append("privilege.MP4", $(`input[name="droneMP4${id}"]:checked`).val());

    formData.append("privilege.TDB0", $(`input[name="droneTDB0${id}"]:checked`).val());

    formData.append("privilege.GED", $(`input[name="droneGED${id}"]:checked`).val());
    
    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/AddPRIVILEGE',
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
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
        },
    });
}



