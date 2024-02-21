﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Runtime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

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
            ViewBag.Role = exist.ROLE;
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.ROLE == exist.ROLE && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).FirstOrDefault();
                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var users = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur).Select(a => new
                    {
                        LOGIN = a.LOGIN,
                        ROLE = a.ROLE.ToString(),
                        ID = a.ID,
                        PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                        DELETONDATE = a.DELETIONDATE,
                        CREAT = a.CREATIONDATE,

                        MENUPAR1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 : 0,
                        MENUPAR2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 : 0,
                        MENUPAR3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 : 0,
                        MENUPAR4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 : 0,
                        MENUPAR5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 : 0,
                        MENUPAR6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 : 0,
                        MENUPAR7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 : 0,
                        MENUPAR8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 : 0,

                        MT0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 : 0,
                        MT1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 : 0,
                        MT2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 : 0,

                        MP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 : 0,
                        MP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 : 0,
                        MP3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 : 0,
                        MP4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 : 0,

                        TDB0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 : 0,
                        GED = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).GED : 0

                    }).Where(a => a.PROJET != null && a.DELETONDATE == null).OrderBy(a => a.PROJET).OrderBy(a => a.CREAT).ToList();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = users }, settings));
                }
                else
                {
                    var users = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur && x.ROLE != Role.Organe_de_Suivi && x.ROLE != Role.Agent_Comptable && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).Select(a => new
                    {
                        LOGIN = a.LOGIN,
                        ROLE = a.ROLE.ToString(),
                        ID = a.ID,
                        PROJET = db.SI_PROJETS.Where(z => z.ID == exist.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                        DELETONDATE = a.DELETIONDATE,
                        CREAT = a.CREATIONDATE,

                        MENUPAR1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR1 : 0,
                        MENUPAR2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR2 : 0,
                        MENUPAR3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR3 : 0,
                        MENUPAR4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR4 : 0,
                        MENUPAR5 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR5 : 0,
                        MENUPAR6 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR6 : 0,
                        MENUPAR7 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR7 : 0,
                        MENUPAR8 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MENUPAR8 : 0,

                        MT0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT0 : 0,
                        MT1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT1 : 0,
                        MT2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MT2 : 0,

                        MP1 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP1 : 0,
                        MP2 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP2 : 0,
                        MP3 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP3 : 0,
                        MP4 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).MP4 : 0,

                        TDB0 = db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 != null ? db.SI_PRIVILEGE.FirstOrDefault(x => x.IDUSERPRIV == a.ID).TDB0 : 0,
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

                MENUPAR1 = privilege.MENUPAR1,
                MENUPAR2 = privilege.MENUPAR2,
                MENUPAR3 = privilege.MENUPAR3,
                MENUPAR4 = privilege.MENUPAR4,
                MENUPAR5 = privilege.MENUPAR5,
                MENUPAR6 = privilege.MENUPAR6,
                MENUPAR7 = privilege.MENUPAR7,
                MENUPAR8 = privilege.MENUPAR8,

                MT0 = privilege.MT0,
                MT1 = privilege.MT1,
                MT2 = privilege.MT2,

                MP1 = privilege.MP1,
                MP2 = privilege.MP2,
                MP3 = privilege.MP3,
                MP4 = privilege.MP4,

                TDB0 = privilege.TDB0,

                GED = privilege.GED
            };

            db.SI_PRIVILEGE.Add(newPrivilege);
            db.SaveChanges();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = privilege }, settings));
        }
    }
}
