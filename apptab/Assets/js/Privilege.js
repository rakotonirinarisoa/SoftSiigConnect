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
        url: Origin + '/Privilege/FillTable',
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

                let MENUPAR9N = "", MENUPAR9R = "", MENUPAR9A = "";
                if (v.MENUPAR9 == 0) MENUPAR9N = "checked";
                if (v.MENUPAR9 == 1) MENUPAR9R = "checked";
                if (v.MENUPAR9 == 2) MENUPAR9A = "checked";

                let MENUPAR10N = "", MENUPAR10R = "", MENUPAR10A = "";
                if (v.MENUPAR10 == 0) MENUPAR10N = "checked";
                if (v.MENUPAR10 == 1) MENUPAR10R = "checked";
                if (v.MENUPAR10 == 2) MENUPAR10A = "checked";

                let MTNONN = "", MTNONR = "", MTNONA = "";
                if (v.MTNON == 0) MTNONN = "checked";
                if (v.MTNON == 1) MTNONR = "checked";
                if (v.MTNON == 2) MTNONA = "checked";

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

                let JRN = "", JRR = "", JRA = "";
                if (v.JR == 0) JRN = "checked";
                if (v.JR == 1) JRR = "checked";
                if (v.JR == 2) JRA = "checked";

                let JRAN = "", JRAR = "", JRAA = "";
                if (v.JRA == 0) JRAN = "checked";
                if (v.JRA == 1) JRAR = "checked";
                if (v.JRA == 2) JRAA = "checked";

                let TDB1N = "", TDB1R = "", TDB1A = "";
                if (v.TDB1 == 0) TDB1N = "checked";
                if (v.TDB1 == 1) TDB1R = "checked";
                if (v.TDB1 == 2) TDB1A = "checked";

                let TDB2N = "", TDB2R = "", TDB2A = "";
                if (v.TDB2 == 0) TDB2N = "checked";
                if (v.TDB2 == 1) TDB2R = "checked";
                if (v.TDB2 == 2) TDB2A = "checked";

                let TDB3N = "", TDB3R = "", TDB3A = "";
                if (v.TDB3 == 0) TDB3N = "checked";
                if (v.TDB3 == 1) TDB3R = "checked";
                if (v.TDB3 == 2) TDB3A = "checked";

                let TDB4N = "", TDB4R = "", TDB4A = "";
                if (v.TDB4 == 0) TDB4N = "checked";
                if (v.TDB4 == 1) TDB4R = "checked";
                if (v.TDB4 == 2) TDB4A = "checked";

                let TDB5N = "", TDB5R = "", TDB5A = "";
                if (v.TDB5 == 0) TDB5N = "checked";
                if (v.TDB5 == 1) TDB5R = "checked";
                if (v.TDB5 == 2) TDB5A = "checked";

                let TDB6N = "", TDB6R = "", TDB6A = "";
                if (v.TDB6 == 0) TDB6N = "checked";
                if (v.TDB6 == 1) TDB6R = "checked";
                if (v.TDB6 == 2) TDB6A = "checked";

                let TDB7N = "", TDB7R = "", TDB7A = "";
                if (v.TDB7 == 0) TDB7N = "checked";
                if (v.TDB7 == 1) TDB7R = "checked";
                if (v.TDB7 == 2) TDB7A = "checked";

                let TDB8N = "", TDB8R = "", TDB8A = "";
                if (v.TDB8 == 0) TDB8N = "checked";
                if (v.TDB8 == 1) TDB8R = "checked";
                if (v.TDB8 == 2) TDB8A = "checked";

                let GEDN = "", GEDR = "", GEDA = "";
                if (v.GED == 0) GEDN = "checked";
                if (v.GED == 1) GEDR = "checked";
                if (v.GED == 2) GEDA = "checked";

                let MD0N = "", MD0R = "", MD0A = "";
                if (v.MD0 == 0) MD0N = "checked";
                if (v.MD0 == 1) MD0R = "checked";
                if (v.MD0 == 2) MD0A = "checked";

                let MD1N = "", MD1R = "", MD1A = "";
                if (v.MD1 == 0) MD1N = "checked";
                if (v.MD1 == 1) MD1R = "checked";
                if (v.MD1 == 2) MD1A = "checked";

                let MD2N = "", MD2R = "", MD2A = "";
                if (v.MD2 == 0) MD2N = "checked";
                if (v.MD2 == 1) MD2R = "checked";
                if (v.MD2 == 2) MD2A = "checked";

                let MD3N = "", MD3R = "", MD3A = "";
                if (v.MD3 == 0) MD3N = "checked";
                if (v.MD3 == 1) MD3R = "checked";
                if (v.MD3 == 2) MD3A = "checked";

                let J0N = "", J0R = "", J0A = "";
                if (v.J0 == 0) J0N = "checked";
                if (v.J0 == 1) J0R = "checked";
                if (v.J0 == 2) J0A = "checked";

                let J1N = "", J1R = "", J1A = "";
                if (v.J1 == 0) J1N = "checked";
                if (v.J1 == 1) J1R = "checked";
                if (v.J1 == 2) J1A = "checked";

                let J2N = "", J2R = "", J2A = "";
                if (v.J2 == 0) J2N = "checked";
                if (v.J2 == 1) J2R = "checked";
                if (v.J2 == 2) J2A = "checked";

                let J3N = "", J3R = "", J3A = "";
                if (v.J3 == 0) J3N = "checked";
                if (v.J3 == 1) J3R = "checked";
                if (v.J3 == 2) J3A = "checked";

                let MOP0N = "", MOP0R = "", MOP0A = "";
                if (v.MOP0 == 0) MOP0N = "checked";
                if (v.MOP0 == 1) MOP0R = "checked";
                if (v.MOP0 == 2) MOP0A = "checked";

                let MOP1N = "", MOP1R = "", MOP1A = "";
                if (v.MOP1 == 0) MOP1N = "checked";
                if (v.MOP1 == 1) MOP1R = "checked";
                if (v.MOP1 == 2) MOP1A = "checked";

                let MOP2N = "", MOP2R = "", MOP2A = "";
                if (v.MOP2 == 0) MOP2N = "checked";
                if (v.MOP2 == 1) MOP2R = "checked";
                if (v.MOP2 == 2) MOP2A = "checked";

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
                                <input type="radio" id="noneMENUPAR9${v.ID}" name="droneMENUPAR9${v.ID}" value="0" ${MENUPAR9N}/><label class="ml-1" for="noneMENUPAR9${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR9${v.ID}" name="droneMENUPAR9${v.ID}" value="1" ${MENUPAR9R}/><label class="ml-1" for="readMENUPAR9${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR9${v.ID}" name="droneMENUPAR9${v.ID}" value="2" ${MENUPAR9A}/><label class="ml-1" for="writeMENUPAR9${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMENUPAR10${v.ID}" name="droneMENUPAR10${v.ID}" value="0" ${MENUPAR10N}/><label class="ml-1" for="noneMENUPAR10${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMENUPAR10${v.ID}" name="droneMENUPAR10${v.ID}" value="1" ${MENUPAR10R}/><label class="ml-1" for="readMENUPAR10${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMENUPAR10${v.ID}" name="droneMENUPAR10${v.ID}" value="2" ${MENUPAR10A}/><label class="ml-1" for="writeMENUPAR10${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMTNON${v.ID}" name="droneMTNON${v.ID}" value="0" ${MTNONN}/><label class="ml-1" for="noneMTNON${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMTNON${v.ID}" name="droneMTNON${v.ID}" value="1" ${MTNONR}/><label class="ml-1" for="readMTNON${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMTNON${v.ID}" name="droneMTNON${v.ID}" value="2" ${MTNONA}/><label class="ml-1" for="writeMTNON${v.ID}" style="font-weight:normal">All</label>
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
                                <input type="radio" id="noneMD0${v.ID}" name="droneMD0${v.ID}" value="0" ${MD0N}/><label class="ml-1" for="noneMD0${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMD0${v.ID}" name="droneMD0${v.ID}" value="1" ${MD0R}/><label class="ml-1" for="readMD0${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMD0${v.ID}" name="droneMD0${v.ID}" value="2" ${MD0A}/><label class="ml-1" for="writeMD0${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMD1${v.ID}" name="droneMD1${v.ID}" value="0" ${MD1N}/><label class="ml-1" for="noneMD1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMD1${v.ID}" name="droneMD1${v.ID}" value="1" ${MD1R}/><label class="ml-1" for="readMD1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMD1${v.ID}" name="droneMD1${v.ID}" value="2" ${MD1A}/><label class="ml-1" for="writeMD1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMD2${v.ID}" name="droneMD2${v.ID}" value="0" ${MD2N}/><label class="ml-1" for="noneMD2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMD2${v.ID}" name="droneMD2${v.ID}" value="1" ${MD2R}/><label class="ml-1" for="readMD2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMD2${v.ID}" name="droneMD2${v.ID}" value="2" ${MD2A}/><label class="ml-1" for="writeMD2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMD3${v.ID}" name="droneMD3${v.ID}" value="0" ${MD3N}/><label class="ml-1" for="noneMD3${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMD3${v.ID}" name="droneMD3${v.ID}" value="1" ${MD3R}/><label class="ml-1" for="readMD3${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMD3${v.ID}" name="droneMD3${v.ID}" value="2" ${MD3A}/><label class="ml-1" for="writeMD3${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneJ0${v.ID}" name="droneJ0${v.ID}" value="0" ${J0N}/><label class="ml-1" for="noneJ0${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJ0${v.ID}" name="droneJ0${v.ID}" value="1" ${J0R}/><label class="ml-1" for="readJ0${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJ0${v.ID}" name="droneJ0${v.ID}" value="2" ${J0A}/><label class="ml-1" for="writeJ0${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneJ1${v.ID}" name="droneJ1${v.ID}" value="0" ${J1N}/><label class="ml-1" for="noneJ1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJ1${v.ID}" name="droneJ1${v.ID}" value="1" ${J1R}/><label class="ml-1" for="readJ1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJ1${v.ID}" name="droneJ1${v.ID}" value="2" ${J1A}/><label class="ml-1" for="writeJ1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneJ2${v.ID}" name="droneJ2${v.ID}" value="0" ${J2N}/><label class="ml-1" for="noneJ2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJ2${v.ID}" name="droneJ2${v.ID}" value="1" ${J2R}/><label class="ml-1" for="readJ2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJ2${v.ID}" name="droneJ2${v.ID}" value="2" ${J2A}/><label class="ml-1" for="writeJ2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneJ3${v.ID}" name="droneJ3${v.ID}" value="0" ${J3N}/><label class="ml-1" for="noneJ3${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJ3${v.ID}" name="droneJ3${v.ID}" value="1" ${J3R}/><label class="ml-1" for="readJ3${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJ3${v.ID}" name="droneJ3${v.ID}" value="2" ${J3A}/><label class="ml-1" for="writeJ3${v.ID}" style="font-weight:normal">All</label>
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
                                <input type="radio" id="noneMOP0${v.ID}" name="droneMOP0${v.ID}" value="0" ${MOP0N}/><label class="ml-1" for="noneMOP0${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMOP0${v.ID}" name="droneMOP0${v.ID}" value="1" ${MOP0R}/><label class="ml-1" for="readMOP0${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMOP0${v.ID}" name="droneMOP0${v.ID}" value="2" ${MOP0A}/><label class="ml-1" for="writeMOP0${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMOP1${v.ID}" name="droneMOP1${v.ID}" value="0" ${MOP1N}/><label class="ml-1" for="noneMOP1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMOP1${v.ID}" name="droneMOP1${v.ID}" value="1" ${MOP1R}/><label class="ml-1" for="readMOP1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMOP1${v.ID}" name="droneMOP1${v.ID}" value="2" ${MOP1A}/><label class="ml-1" for="writeMOP1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneMOP2${v.ID}" name="droneMOP2${v.ID}" value="0" ${MOP2N}/><label class="ml-1" for="noneMOP2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readMOP2${v.ID}" name="droneMOP2${v.ID}" value="1" ${MOP2R}/><label class="ml-1" for="readMOP2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeMOP2${v.ID}" name="droneMOP2${v.ID}" value="2" ${MOP2A}/><label class="ml-1" for="writeMOP2${v.ID}" style="font-weight:normal">All</label>
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
                                <input type="radio" id="noneJR${v.ID}" name="droneJR${v.ID}" value="0" ${JRN}/><label class="ml-1" for="noneJR${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJR${v.ID}" name="droneJR${v.ID}" value="1" ${JRR}/><label class="ml-1" for="readJR${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJR${v.ID}" name="droneJR${v.ID}" value="2" ${JRA}/><label class="ml-1" for="writeJR${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneJRA${v.ID}" name="droneJRA${v.ID}" value="0" ${JRAN}/><label class="ml-1" for="noneJRA${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readJRA${v.ID}" name="droneJRA${v.ID}" value="1" ${JRAR}/><label class="ml-1" for="readJRA${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeJRA${v.ID}" name="droneJRA${v.ID}" value="2" ${JRAA}/><label class="ml-1" for="writeJRA${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB1${v.ID}" name="droneTDB1${v.ID}" value="0" ${TDB1N}/><label class="ml-1" for="noneTDB1${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB1${v.ID}" name="droneTDB1${v.ID}" value="1" ${TDB1R}/><label class="ml-1" for="readTDB1${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB1${v.ID}" name="droneTDB1${v.ID}" value="2" ${TDB1A}/><label class="ml-1" for="writeTDB1${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB2${v.ID}" name="droneTDB2${v.ID}" value="0" ${TDB2N}/><label class="ml-1" for="noneTDB2${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB2${v.ID}" name="droneTDB2${v.ID}" value="1" ${TDB2R}/><label class="ml-1" for="readTDB2${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB2${v.ID}" name="droneTDB2${v.ID}" value="2" ${TDB2A}/><label class="ml-1" for="writeTDB2${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB3${v.ID}" name="droneTDB3${v.ID}" value="0" ${TDB3N}/><label class="ml-1" for="noneTDB3${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB3${v.ID}" name="droneTDB3${v.ID}" value="1" ${TDB3R}/><label class="ml-1" for="readTDB3${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB3${v.ID}" name="droneTDB3${v.ID}" value="2" ${TDB3A}/><label class="ml-1" for="writeTDB3${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB4${v.ID}" name="droneTDB4${v.ID}" value="0" ${TDB4N}/><label class="ml-1" for="noneTDB4${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB4${v.ID}" name="droneTDB4${v.ID}" value="1" ${TDB4R}/><label class="ml-1" for="readTDB4${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB4${v.ID}" name="droneTDB4${v.ID}" value="2" ${TDB4A}/><label class="ml-1" for="writeTDB4${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB5${v.ID}" name="droneTDB5${v.ID}" value="0" ${TDB5N}/><label class="ml-1" for="noneTDB5${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB5${v.ID}" name="droneTDB5${v.ID}" value="1" ${TDB5R}/><label class="ml-1" for="readTDB5${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB5${v.ID}" name="droneTDB5${v.ID}" value="2" ${TDB5A}/><label class="ml-1" for="writeTDB5${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB6${v.ID}" name="droneTDB6${v.ID}" value="0" ${TDB6N}/><label class="ml-1" for="noneTDB6${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB6${v.ID}" name="droneTDB6${v.ID}" value="1" ${TDB6R}/><label class="ml-1" for="readTDB6${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB6${v.ID}" name="droneTDB6${v.ID}" value="2" ${TDB6A}/><label class="ml-1" for="writeTDB6${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB7${v.ID}" name="droneTDB7${v.ID}" value="0" ${TDB7N}/><label class="ml-1" for="noneTDB7${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB7${v.ID}" name="droneTDB7${v.ID}" value="1" ${TDB7R}/><label class="ml-1" for="readTDB7${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB7${v.ID}" name="droneTDB7${v.ID}" value="2" ${TDB7A}/><label class="ml-1" for="writeTDB7${v.ID}" style="font-weight:normal">All</label>
                            </div>
                        </td>

                        <td text-align:center>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="noneTDB8${v.ID}" name="droneTDB8${v.ID}" value="0" ${TDB8N}/><label class="ml-1" for="noneTDB8${v.ID}" style="font-weight:normal">None</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="readTDB8${v.ID}" name="droneTDB8${v.ID}" value="1" ${TDB8R}/><label class="ml-1" for="readTDB8${v.ID}" style="font-weight:normal">Read</label>
                            </div></br>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="writeTDB8${v.ID}" name="droneTDB8${v.ID}" value="2" ${TDB8A}/><label class="ml-1" for="writeTDB8${v.ID}" style="font-weight:normal">All</label>
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
    formData.append("privilege.MENUPAR9", $(`input[name="droneMENUPAR9${id}"]:checked`).val());
    formData.append("privilege.MENUPAR10", $(`input[name="droneMENUPAR10${id}"]:checked`).val());

    formData.append("privilege.MTNON", $(`input[name="droneMTNON${id}"]:checked`).val());
    formData.append("privilege.MT0", $(`input[name="droneMT0${id}"]:checked`).val());
    formData.append("privilege.MT1", $(`input[name="droneMT1${id}"]:checked`).val());
    formData.append("privilege.MT2", $(`input[name="droneMT2${id}"]:checked`).val());

    formData.append("privilege.MP1", $(`input[name="droneMP1${id}"]:checked`).val());
    formData.append("privilege.MP2", $(`input[name="droneMP2${id}"]:checked`).val());
    formData.append("privilege.MP3", $(`input[name="droneMP3${id}"]:checked`).val());
    formData.append("privilege.MP4", $(`input[name="droneMP4${id}"]:checked`).val());

    formData.append("privilege.MD0", $(`input[name="droneMD0${id}"]:checked`).val());
    formData.append("privilege.MD1", $(`input[name="droneMD1${id}"]:checked`).val());
    formData.append("privilege.MD2", $(`input[name="droneMD2${id}"]:checked`).val());
    formData.append("privilege.MD3", $(`input[name="droneMD3${id}"]:checked`).val());

    formData.append("privilege.J0", $(`input[name="droneJ0${id}"]:checked`).val());
    formData.append("privilege.J1", $(`input[name="droneJ1${id}"]:checked`).val());
    formData.append("privilege.J2", $(`input[name="droneJ2${id}"]:checked`).val());
    formData.append("privilege.J3", $(`input[name="droneJ3${id}"]:checked`).val());

    formData.append("privilege.MOP0", $(`input[name="droneMOP0${id}"]:checked`).val());
    formData.append("privilege.MOP1", $(`input[name="droneMOP1${id}"]:checked`).val());
    formData.append("privilege.MOP2", $(`input[name="droneMOP2${id}"]:checked`).val());

    formData.append("privilege.TDB0", $(`input[name="droneTDB0${id}"]:checked`).val());
    formData.append("privilege.TDB1", $(`input[name="droneTDB1${id}"]:checked`).val());
    formData.append("privilege.TDB2", $(`input[name="droneTDB2${id}"]:checked`).val());
    formData.append("privilege.TDB3", $(`input[name="droneTDB3${id}"]:checked`).val());
    formData.append("privilege.TDB4", $(`input[name="droneTDB4${id}"]:checked`).val());
    formData.append("privilege.TDB5", $(`input[name="droneTDB5${id}"]:checked`).val());
    formData.append("privilege.TDB6", $(`input[name="droneTDB6${id}"]:checked`).val());
    formData.append("privilege.TDB7", $(`input[name="droneTDB7${id}"]:checked`).val());
    formData.append("privilege.TDB8", $(`input[name="droneTDB8${id}"]:checked`).val());
    formData.append("privilege.JR", $(`input[name="droneJR${id}"]:checked`).val());
    formData.append("privilege.JRA", $(`input[name="droneJRA${id}"]:checked`).val());

    formData.append("privilege.GED", $(`input[name="droneGED${id}"]:checked`).val());

    $.ajax({
        type: "POST",
        url: Origin + '/Privilege/AddPRIVILEGE',
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
            }
            if (Datas.type == "login") {
                alert(Datas.msg);
                window.location = window.location.origin;
                return;
            }
        },
    });
}
