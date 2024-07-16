using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using apptab.Data.Entities;
using apptab.Models;
using Newtonsoft.Json;
using static apptab.Controllers.RSFController;

namespace apptab.Controllers
{
    public class PrivilegeController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ActionResult List()
        {
            ViewBag.Controller = "Liste des Privilèges";
            return View();
        }

        [HttpPost]
        public JsonResult FillTable(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            //ViewBag.Role = exist.ROLE;
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();
                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var users = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur && x.ROLE != Role.Administrateur).Select(a => new
                    {
                        LOGIN = a.LOGIN,
                        ROLE = a.ROLE.ToString(),
                        ID = a.ID,
                        PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                        DELETONDATE = a.DELETIONDATE,
                        CREAT = a.CREATIONDATE,

                        //MENUPAR1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 : 0,
                        //MENUPAR2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 : 0,
                        //MENUPAR3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 : 0,
                        //MENUPAR4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 : 0,
                        //MENUPAR5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 : 0,
                        //MENUPAR6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 : 0,
                        //MENUPAR7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 : 0,
                        //MENUPAR8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 : 0,
                        //MENUPAR9 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR9 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR9 : 0,
                        //MENUPAR10 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR10 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR10 : 0,

                        MT0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 : 0,
                        MT1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 : 0,
                        //MT2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 : 0,
                        MTNON = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MTNON != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MTNON : 0,

                        MP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 : 0,
                        MP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 : 0,
                        MP3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 : 0,
                        MP4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 : 0,

                        MD0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD0 : 0,
                        MD1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD1 : 0,
                        MD2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD2 : 0,
                        //MD3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD3 : 0,

                        //MOP0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP0 : 0,
                        //MOP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP1 : 0,
                        //MOP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP2 : 0,

                        TDB0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 : 0,
                        TDB1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB1 : 0,
                        TDB2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB2 : 0,
                        TDB3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB3 : 0,
                        TDB4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB4 : 0,
                        TDB5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB5 : 0,
                        TDB6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB6 : 0,
                        TDB7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB7 : 0,
                        TDB8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB8 : 0,

                        J0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J0 : 0,
                        J1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J1 : 0,
                        J2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J2 : 0,
                        J3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J3 : 0,
                        JR = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JR != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JR : 0,
                        JRA = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JRA != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JRA : 0,

                        RSF = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSF != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSF : 0,
                        RSFT = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSFT != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSFT : 0,
                        TDB9 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB9 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB9 : 0,

                        GED = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED : 0

                    }).Where(a => a.PROJET != null && a.DELETONDATE == null).OrderBy(a => a.PROJET).OrderBy(a => a.CREAT).ToList();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = users }, settings));
                }
                else
                {
                    var users = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur && x.ROLE != Role.Administrateur && x.ROLE != Role.Organe_de_Suivi && x.ROLE != Role.Validateur_paiements && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).Select(a => new
                    {
                        LOGIN = a.LOGIN,
                        ROLE = a.ROLE.ToString(),
                        ID = a.ID,
                        PROJET = db.SI_PROJETS.Where(z => z.ID == exist.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                        DELETONDATE = a.DELETIONDATE,
                        CREAT = a.CREATIONDATE,

                        //MENUPAR1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 : 0,
                        //MENUPAR2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 : 0,
                        //MENUPAR3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 : 0,
                        //MENUPAR4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 : 0,
                        //MENUPAR5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 : 0,
                        //MENUPAR6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 : 0,
                        //MENUPAR7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 : 0,
                        //MENUPAR8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 : 0,
                        //MENUPAR9 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR9 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR9 : 0,
                        //MENUPAR10 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR10 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR10 : 0,

                        MT0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 : 0,
                        MT1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 : 0,
                        //MT2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 : 0,
                        MTNON = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MTNON != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MTNON : 0,

                        MP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 : 0,
                        MP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 : 0,
                        MP3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 : 0,
                        MP4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 : 0,

                        MD0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD0 : 0,
                        MD1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD1 : 0,
                        MD2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD2 : 0,
                        //MD3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MD3 : 0,

                        //MOP0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP0 : 0,
                        //MOP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP1 : 0,
                        //MOP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MOP2 : 0,

                        TDB0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 : 0,
                        TDB1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB1 : 0,
                        TDB2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB2 : 0,
                        TDB3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB3 : 0,
                        TDB4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB4 : 0,
                        TDB5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB5 : 0,
                        TDB6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB6 : 0,
                        TDB7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB7 : 0,
                        TDB8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB8 : 0,

                        J0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J0 : 0,
                        J1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J1 : 0,
                        J2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J2 : 0,
                        J3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).J3 : 0,
                        JR = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JR != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JR : 0,
                        JRA = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JRA != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).JRA : 0,

                        RSF = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSF != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSF : 0,
                        RSFT = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSFT != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).RSFT : 0,
                        TDB9 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB9 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB9 : 0,

                        GED = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED : 0

                    }).OrderBy(a => a.CREAT).Where(a => a.DELETONDATE == null).ToList();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = users }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult AddPRIVILEGE(SI_USERS suser, SI_PRIVILEGE privilege, int UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var societeExist = db.SI_PRIVILEGE.FirstOrDefault(a => a.IDUSERPRIV == UserId);

            if (societeExist != null)
            {
                db.SI_PRIVILEGE.Remove(societeExist);
                db.SaveChanges();
            }

            var newPrivilege = new SI_PRIVILEGE()
            {
                IDUSERPRIV = UserId,
                IDUSER = exist.ID,
                CREATIONDATE = DateTime.Now,

                //MENUPAR1 = privilege.MENUPAR1,
                //MENUPAR2 = privilege.MENUPAR2,
                //MENUPAR3 = privilege.MENUPAR3,
                //MENUPAR4 = privilege.MENUPAR4,
                //MENUPAR5 = privilege.MENUPAR5,
                //MENUPAR6 = privilege.MENUPAR6,
                //MENUPAR7 = privilege.MENUPAR7,
                //MENUPAR8 = privilege.MENUPAR8,
                //MENUPAR9 = privilege.MENUPAR9,
                //MENUPAR10 = privilege.MENUPAR10,

                MT0 = privilege.MT0,
                MT1 = privilege.MT1,
                //MT2 = privilege.MT2,
                MTNON = privilege.MTNON,

                MP1 = privilege.MP1,
                MP2 = privilege.MP2,
                MP3 = privilege.MP3,
                MP4 = privilege.MP4,

                MD0 = privilege.MD0,
                MD1 = privilege.MD1,
                MD2 = privilege.MD2,
                //MD3 = privilege.MD3,

                //MOP0 = privilege.MOP0,
                //MOP1 = privilege.MOP1,
                //MOP2 = privilege.MOP2,

                TDB0 = privilege.TDB0,
                TDB1 = privilege.TDB1,
                TDB2 = privilege.TDB2,
                TDB3 = privilege.TDB3,
                TDB4 = privilege.TDB4,
                TDB5 = privilege.TDB5,
                TDB6 = privilege.TDB6,
                TDB7 = privilege.TDB7,
                TDB8 = privilege.TDB8,

                J0 = privilege.J0,
                J1 = privilege.J1,
                J2 = privilege.J2,
                J3 = privilege.J3,
                JR = privilege.JR,
                JRA = privilege.JRA,

                RSF = privilege.RSF,
                RSFT = privilege.RSFT,
                TDB9 = privilege.TDB9,

                GED = privilege.GED
            };

            db.SI_PRIVILEGE.Add(newPrivilege);
            db.SaveChanges();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = privilege }, settings));
        }

        //GET ALL PROJET//
        [HttpPost]
        public ActionResult GetAllPROJET(SI_USERS suser, string IDPROSOA)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (IDPROSOA != null)
            {
                int? PROSOAID = int.Parse(IDPROSOA);
                var idPro = db.SI_PROSOA.Where(a => a.ID == PROSOAID && a.DELETIONDATE == null).Select(a => a.IDPROJET).FirstOrDefault();
                var FProfet = db.SI_PROJETS.Where(a => a.ID != idPro && a.DELETIONDATE == null).Select(a => new
                {
                    PROJET = a.PROJET,
                    ID = a.ID

                }).ToList();
                var FprojetFirst = db.SI_PROJETS.Where(a => a.ID == idPro && a.DELETIONDATE == null).Select(a => new
                {
                    PROJET = a.PROJET,
                    ID = a.ID
                }).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = FprojetFirst, datas = FProfet }, settings));
            }
            else
            {
                var user = db.SI_PROJETS
                    .Where(a => a.DELETIONDATE == null).Select(a => new
                    {
                        PROJET = a.PROJET,
                        ID = a.ID
                    }).ToList();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user, datas = "" }, settings));
            }
        }

        //Liste//
        public ActionResult SiteList()
        {
            ViewBag.Controller = "Liste des privilèges SITE";

            return View();
        }

        [HttpPost]
        public JsonResult FillTableSITE(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            ViewBag.Role = exist.ROLE;
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<SITE> a = new List<SITE>();

                var test = db.SI_USERS.Where(x => x.ROLE == exist.ROLE && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).FirstOrDefault();

                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    foreach (var x in db.SI_SITE.Where(x => x.DELETIONDATE == null).ToList())
                    {
                        a.Add(new SITE()
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(b => b.ID == x.IDPROJET && b.DELETIONDATE == null).PROJET,
                            ID = x.ID,
                            USER = db.SI_USERS.FirstOrDefault(z => z.ID == x.IDUSER && z.DELETIONDATE == null).LOGIN,
                            SITES = x.SITE
                        });
                    }
                }
                else
                {
                    foreach (var x in db.SI_SITE.Where(x => x.DELETIONDATE == null && x.IDPROJET == exist.IDPROJET).ToList())
                    {
                        a.Add(new SITE()
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(b => b.ID == x.IDPROJET && b.DELETIONDATE == null).PROJET,
                            ID = x.ID,
                            USER = db.SI_USERS.FirstOrDefault(z => z.ID == x.IDUSER && z.DELETIONDATE == null).LOGIN,
                            SITES = x.SITE
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = a }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        public class SITE
        {
            public string PROJET { get; set; }
            public string USER { get; set; }
            public string SITES { get; set; }
            public int? ID { get; set; }
        }

        public ActionResult DeleteSITE(SI_USERS suser, string SITE)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IDPROSOA = int.Parse(SITE);
                var PROSOA = db.SI_SITE.FirstOrDefault(a => a.ID == IDPROSOA && a.DELETIONDATE == null);

                if (PROSOA != null)
                {
                    PROSOA.DELETIONDATE = DateTime.Now;

                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Suppression avec succès. " }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de Supression" }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public ActionResult GETALLUSER(SI_USERS suser, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                var crpto = db.SI_USERS.Where(a => a.IDPROJET == crpt && a.DELETIONDATE == null).ToList();

                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = crpto, IDP = crpt } }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "notYet", msg = "Veuillez créer des utilisateurs. ", data = new { etat = crpto, IDP = crpt } }, settings));
                }

                //var crpto = db.SI_SITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                //if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                //    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                //SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                //SOFTCONNECTOM tom = new SOFTCONNECTOM();

                //var etat = tom.RSITE.OrderBy(a => a.CODE).ToList();

                //if (crpto != null)
                //{
                //    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = etat, IDP = crpt } }, settings));
                //}
                //else
                //{
                //    return Json(JsonConvert.SerializeObject(new { type = "notYet", msg = "Veuillez créer les privilèges SITE. ", data = new { etat = etat, IDP = crpt } }, settings));
                //}
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public ActionResult GETALLSITE(SI_USERS suser, int iProjet, int iUser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                var crpto = db.SI_SITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                var etat = tom.RSITE.OrderBy(a => a.CODE).ToList();

                if (crpto != null)
                {
                    var SITE = new List<string>();

                    if (db.SI_SITE.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null && a.IDUSER == iUser))
                    {
                        var isSITE = db.SI_SITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && a.IDUSER == iUser).SITE;

                        string[] separators = { "," };
                        var pro = isSITE;
                        if (pro != null)
                        {
                            string listUser = pro.ToString();
                            string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var a in lst)
                            {
                                SITE.Add(a);
                            }
                        }
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = etat, IDP = crpt, SITE = SITE } }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "notYet", msg = "Veuillez créer les privilèges SITE. ", data = new { etat = etat, IDP = crpt } }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //Creation site//
        public ActionResult CreateSITE()
        {
            ViewBag.Controller = "Nouveau site";

            return View();
        }

        [HttpPost]
        public JsonResult AddSite(SI_USERS suser, SI_SITE site, string listSite)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                if (db.SI_SITE.Any(a => a.IDUSER == site.IDUSER))
                {
                    var isUs = db.SI_SITE.FirstOrDefault(a => a.IDUSER == site.IDUSER);
                    db.SI_SITE.Remove(isUs);
                    db.SaveChanges();
                }

                var isSITE = new SI_SITE()
                {
                    IDPROJET = site.IDPROJET,
                    IDUSER = site.IDUSER,
                    SITE = listSite,
                    CREATIONDATE = DateTime.Now
                };

                db.SI_SITE.Add(isSITE);
                db.SaveChanges();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = isSITE }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
