﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using apptab.Data.Entities;
using apptab.Data;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class BordTraitementController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTOM tom = new SOFTCONNECTOM();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //GET ALL PROJET//
        [HttpPost]
        public ActionResult GetAllPROJET(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();
                var proj = new List<int>();

                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var user = db.SI_PROJETS.Select(a => new
                    {
                        PROJET = a.PROJET,
                        ID = a.ID,
                        DELETIONDATE = a.DELETIONDATE,
                    }).Where(a => a.DELETIONDATE == null).ToList();

                    foreach (var x in user)
                    {
                        proj.Add(x.ID);
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
                }
                else
                {
                    if (test.IDPROJET != 0)
                    {
                        var user = db.SI_PROJETS.Select(a => new
                        {
                            PROJET = a.PROJET,
                            ID = a.ID,
                            DELETIONDATE = a.DELETIONDATE,
                        }).Where(a => a.DELETIONDATE == null && a.ID == test.IDPROJET).ToList();

                        foreach (var x in user)
                        {
                            proj.Add(x.ID);
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
                    }
                    else
                    {
                        var user = (from usr in db.SI_PROJETS
                                    join prj in db.SI_MAPUSERPROJET on usr.ID equals prj.IDPROJET
                                    where prj.IDUS == test.ID && usr.DELETIONDATE == null
                                    select new SI_PROJETS
                                    {
                                        PROJET = usr.PROJET,
                                        ID = usr.ID,
                                        DELETIONDATE = usr.DELETIONDATE,
                                    }).ToList();

                        foreach (var x in user)
                        {
                            proj.Add(x.ID);
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<JsonResult> ModalF(SI_USERS suser, string IdF, string projet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = db.SI_PROJETS.FirstOrDefault(a => a.PROJET == projet && a.DELETIONDATE == null).ID;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF) != null)
                {
                    foreach (var x in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE != "DEF" && a.TYPEPIECE != "TEF" && a.TYPEPIECE != "BE").OrderBy(a => a.RANG).ToList())
                    {
                        var idFGuid = Guid.Parse(IdF);
                        DateTime dpj = tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == idFGuid).FirstOrDefault().DATELIQUIDATION.Value;
                        list.Add(new DATATRPROJET
                        {
                            REF = x.TYPEPIECE,
                            OBJ = x.RANG.ToString(),
                            TITUL = x.NOMBRE.ToString(),
                            DATE = dpj,
                            MONT = Math.Round(x.MONTANT.Value, 2).ToString(),
                            LIEN = x.LIEN
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<JsonResult> ModalD(SI_USERS suser, Guid IdF, string projet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = db.SI_PROJETS.FirstOrDefault(a => a.PROJET == projet && a.DELETIONDATE == null).ID;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.CPTADMIN_MLIQUIDATION.FirstOrDefault(a => a.IDLIQUIDATION == IdF) != null)
                {
                    foreach (var x in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == IdF).ToList())
                    {
                        list.Add(new DATATRPROJET
                        {
                            REF = x.LIBELLE,
                            OBJ = x.COGE.ToString(),
                            TITUL = x.POSTE.ToString(),
                            MONT = Math.Round(x.MONTANTLOCAL.Value, 2).ToString(),
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<JsonResult> ModalLIAS(SI_USERS suser, string IdF, string projet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = db.SI_PROJETS.FirstOrDefault(a => a.PROJET == projet && a.DELETIONDATE == null).ID;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                //List<DATATRPROJET> list = new List<DATATRPROJET>();
                var newElemH = new DATATRPROJET()
                {
                    REF = "",
                    OBJ = "",
                    TITUL = ""
                };

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && (a.TYPEPIECE == "DEF" || a.TYPEPIECE == "TEF" || a.TYPEPIECE == "BE")) != null)
                {
                    var def = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF") != null)
                        def = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF").LIEN;
                    var tef = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF") != null)
                        tef = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF").LIEN;
                    var be = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE") != null)
                        be = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE").LIEN;

                    newElemH = new DATATRPROJET()
                    {
                        REF = String.IsNullOrEmpty(def) ? "" : def,
                        OBJ = String.IsNullOrEmpty(tef) ? "" : tef,
                        TITUL = String.IsNullOrEmpty(be) ? "" : be
                    };
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = newElemH }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        private async Task<string> GetAgent(int? userId)
        {
            if (userId == null)
            {
                return "";
            }

            var agent = await db.SI_USERS.Where(user => user.ID == userId && user.DELETIONDATE == null).Select(user => user.LOGIN).FirstOrDefaultAsync();

            if (agent == null)
            {
                return "";
            }

            return agent;
        }

        //Liste des engagements et paiements//
        public ActionResult BordListeEngaPaie()
        {
            ViewBag.Controller = "Liste des engagements et des paiements";

            return View();
        }

        //Genere liste des engagements et paiements//
        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<TxLISTETRAIT> list = new List<TxLISTETRAIT>();
                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        //int retarDate = 0;
                        //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                        //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELTV.Value;

                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                        SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                        SOFTCONNECTOM tom = new SOFTCONNECTOM();

                        //Check si la correspondance des états est OK//
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                        if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                        //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           });

                                //bool isLate = false;
                                //if (x.DATECRE.Value.AddBusinessDays(retarDate - 1).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                //    isLate = true;

                                list.Add(new TxLISTETRAIT
                                {
                                    No = x.No,
                                    REF = x.REF,
                                    BENEF = x.TITUL,
                                    DATENGAGEMENT = x.DATEMANDAT != null ? x.DATEMANDAT : null,
                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                    DATEPAIE = DateTime.Now.Date,
                                    MONTPAIE = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                    SOA = soa.FirstOrDefault().SOA,
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    //isLATE = isLate
                                });
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.OrderByDescending(a => a.isLATE).ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //Statut des engagements//
        public ActionResult StatutEngagements()
        {
            ViewBag.Controller = "Statut des engagements";

            return View();
        }

        //Genere Statut des engagements//
        [HttpPost]
        public JsonResult GenereSTATLISTE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<TxLISTETRAIT> list = new List<TxLISTETRAIT>();
                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        //int retarDate = 0;
                        //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                        //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELENVOISIIGFP.Value;

                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                        SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                        SOFTCONNECTOM tom = new SOFTCONNECTOM();

                        //Check si la correspondance des états est OK//
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                        if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                        //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           });

                                //bool isLate = false;
                                //if (x.DATECRE.Value.AddBusinessDays(retarDate - 1).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                //    isLate = true;

                                list.Add(new TxLISTETRAIT
                                {
                                    No = x.No,
                                    REF = x.REF,
                                    BENEF = x.TITUL,
                                    //DATENGAGEMENT = x.DATEMANDAT.Value.Date,
                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),

                                    DATETRANSFERTRAF = x.DATECRE != null ? x.DATECRE : null,
                                    DATEVALORDSEC = x.DATEVALIDATION != null ? x.DATEVALIDATION : null,
                                    DATESENDSIIG = x.DATENVOISIIGFP != null ? x.DATENVOISIIGFP : null,
                                    DATESIIGFP = x.DATESIIG != null ? x.DATESIIG : null,

                                    SOA = soa.FirstOrDefault().SOA,
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
                                    //isLATE = isLate
                                });
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //Engagements rejetés//
        public ActionResult EngagementsREJETE()
        {
            ViewBag.Controller = "Liste des engagements rejetés";

            return View();
        }

        //Genere Liste Engagements rejetés//
        [HttpPost]
        public async Task<JsonResult> GenereREJETE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<TxLISTETRAIT> list = new List<TxLISTETRAIT>();
                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        //int retarDate = 0;
                        //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                        //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELENVOISIIGFP.Value;

                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                        SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                        SOFTCONNECTOM tom = new SOFTCONNECTOM();

                        //Check si la correspondance des états est OK//
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                        if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                        //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           });

                                //bool isLate = false;
                                //if (x.DATECRE.Value.AddBusinessDays(retarDate - 1).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                //    isLate = true;

                                var isRejet = (from user in db.SI_USERS
                                               join rejet in db.SI_TRAITANNUL on user.ID equals rejet.IDUSER
                                               where rejet.IDPROJET == crpt && rejet.No == x.No
                                               orderby rejet.DATEANNUL descending
                                               select new
                                               {
                                                   IDUSER = rejet.IDUSER,
                                                   DATEREJE = rejet.DATEANNUL,
                                                   MOTIF = rejet.MOTIF,
                                                   COMMENTAIRE = rejet.COMMENTAIRE
                                               }).FirstOrDefault();

                                list.Add(new TxLISTETRAIT
                                {
                                    No = x.No,
                                    REF = x.REF,
                                    BENEF = x.TITUL,
                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                    AGENTREJETE = isRejet != null ? await GetAgent(isRejet.IDUSER) : "",
                                    DATEREJETE = isRejet != null ? isRejet.DATEREJE : null,
                                    MOTIF = isRejet != null ? isRejet.MOTIF : "",
                                    COMMENTAIRE = isRejet != null ? isRejet.COMMENTAIRE : "",

                                    SOA = soa.FirstOrDefault().SOA,
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
                                    //isLATE = isLate
                                });
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //Suivi des délais de traitement des engagements//
        public ActionResult DelaisTraitementEngagements()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GenereDelaisTraitementEngagements(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var user = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);

            if (user == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            }

            string[] separators = { "," };

            var sProjectsId = listProjet.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

            var iProjectsId = new List<int>();

            for (int i = 0; i < sProjectsId.Count; i += 1)
            {
                int projectId = int.Parse(sProjectsId[i]);

                if ((await db.SI_MAPPAGES.FirstOrDefaultAsync(a => a.IDPROJET == projectId && a.DELETIONDATE == null)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));
                }

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(projectId);

                var tom = new SOFTCONNECTOM();

                var numCaEtapAPP = await db.SI_PARAMETAT.FirstOrDefaultAsync(a => a.IDPROJET == projectId && a.DELETIONDATE == null);

                if (numCaEtapAPP == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.DEF)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.TEF)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.BE)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementEngagement>();

            var lastIndex = -1;

            for (int i = 0; i < iProjectsId.Count; i += 1)
            {
                int projectId = iProjectsId[i];

                var s = await (
                    from soa in db.SI_SOAS
                    join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                    where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                    select new
                    {
                        soa.SOA
                    }
                ).FirstOrDefaultAsync();

                if (s == null)
                {
                    continue;
                }

                var traitprojets = await db.SI_TRAITPROJET.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                if (traitprojets.Count == 0)
                {
                    continue;
                }

                lastIndex += 1;

                result.Add(new TraitementEngagement
                {
                    SOA = s.SOA,
                    TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                });

                for (int j = 0; j < traitprojets.Count; j += 1)
                {
                    result[lastIndex].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                    {
                        NUM_ENGAGEMENT = traitprojets[j].REF,
                        BENEFICIAIRE = traitprojets[j].TITUL,
                        MONTENGAGEMENT = Data.Cipher.Decrypt(traitprojets[j].MONT, "Oppenheimer").ToString(),
                        DATETRANSFERTRAF = traitprojets[j].DATECRE,
                        TRANSFERTRAFAGENT = await GetAgent(traitprojets[j].IDUSERCREATE),
                        DATEVALORDSEC = traitprojets[j].DATEVALIDATION,
                        VALORDSECAGENT = await GetAgent(traitprojets[j].IDUSERVALIDATE),
                        DATESENDSIIG = traitprojets[j].DATENVOISIIGFP,
                        SENDSIIGAGENT = await GetAgent(traitprojets[j].IDUSERENVOISIIGFP),
                        DATESIIGFP = traitprojets[j].DATESIIG,
                        SIIGFPAGENT = "",

                        DUREETRAITEMENTTRANSFERTRAF = Data.Date.GetDifference(traitprojets[j].DATECRE, traitprojets[j].DATEBE),
                        DUREETRAITEMENTVALORDSEC = Data.Date.GetDifference(traitprojets[j].DATEVALIDATION, traitprojets[j].DATECRE),
                        DUREETRAITEMENTSENDSIIG = Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION),
                        DUREETRAITEMENTSIIGFP = Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP)
                    });
                }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
        }

        //Liste des traitements en souffrance (par rapport au délai moyen)//
        public ActionResult SoufTraitement()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GenereSoufTraitement(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var user = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);

            if (user == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            }

            string[] separators = { "," };

            var sProjectsId = listProjet.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

            var iProjectsId = new List<int>();

            for (int i = 0; i < sProjectsId.Count; i += 1)
            {
                int projectId = int.Parse(sProjectsId[i]);

                if ((await db.SI_MAPPAGES.FirstOrDefaultAsync(a => a.IDPROJET == projectId && a.DELETIONDATE == null)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));
                }

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(projectId);

                var tom = new SOFTCONNECTOM();

                var numCaEtapAPP = await db.SI_PARAMETAT.FirstOrDefaultAsync(a => a.IDPROJET == projectId && a.DELETIONDATE == null);

                if (numCaEtapAPP == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.DEF)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.TEF)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                if ((await tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefaultAsync(a => a.NUM == numCaEtapAPP.BE)) == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));
                }

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementEngagement>();

            var lastIndex = -1;

            try
            {
                for (int i = 0; i < iProjectsId.Count; i += 1)
                {
                    int projectId = iProjectsId[i];

                    var durPrevu = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == projectId && a.DELETIONDATE == null);
                    if (durPrevu == null || durPrevu.DELRAF == null || durPrevu.DELTV == null || durPrevu.DELENVOISIIGFP == null || durPrevu.DELSIIGFP == null)
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le délais des traitements. " }, settings));

                    var s = await (
                        from soa in db.SI_SOAS
                        join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                        where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                        select new
                        {
                            soa.SOA
                        }
                    ).FirstOrDefaultAsync();

                    if (s == null)
                    {
                        continue;
                    }

                    var traitprojets = await db.SI_TRAITPROJET.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                    if (traitprojets.Count == 0)
                    {
                        continue;
                    }

                    lastIndex += 1;

                    result.Add(new TraitementEngagement
                    {
                        SOA = s.SOA,
                        TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                    });

                    for (int j = 0; j < traitprojets.Count; j += 1)
                    {
                        result[lastIndex].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                        {
                            NUM_ENGAGEMENT = traitprojets[j].REF,
                            BENEFICIAIRE = traitprojets[j].TITUL,
                            MONTENGAGEMENT = Data.Cipher.Decrypt(traitprojets[j].MONT, "Oppenheimer").ToString(),

                            DATETRANSFERTRAF = traitprojets[j].DATECRE,
                            DATEVALORDSEC = traitprojets[j].DATEVALIDATION,
                            DATESENDSIIG = traitprojets[j].DATENVOISIIGFP,
                            DATESIIGFP = traitprojets[j].DATESIIG,

                            TRANSFERTRAFAGENT = await GetAgent(traitprojets[j].IDUSERCREATE),
                            VALORDSECAGENT = await GetAgent(traitprojets[j].IDUSERVALIDATE),
                            SENDSIIGAGENT = await GetAgent(traitprojets[j].IDUSERENVOISIIGFP),
                            SIIGFPAGENT = "",

                            DUREETRAITEMENTTRANSFERTRAF = Data.Date.GetDifference(traitprojets[j].DATECRE, traitprojets[j].DATEBE),
                            DUREETRAITEMENTVALORDSEC = Data.Date.GetDifference(traitprojets[j].DATEVALIDATION, traitprojets[j].DATECRE),
                            DUREETRAITEMENTSENDSIIG = Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION),
                            DUREETRAITEMENTSIIGFP = Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP),

                            DURPREVUTRANSFERT = durPrevu != null ? durPrevu.DELRAF.Value : 0,
                            DURPREVUVALIDATION = durPrevu != null ? durPrevu.DELTV.Value : 0,
                            DURPREVUTRANSFSIIG = durPrevu != null ? durPrevu.DELENVOISIIGFP.Value : 0,
                            DURPREVUSIIG = durPrevu != null ? durPrevu.DELSIIGFP.Value : 0,

                            DEPASTRANSFERT = durPrevu != null ? durPrevu.DELRAF.Value - Data.Date.GetDifference(traitprojets[j].DATECRE, traitprojets[j].DATEBE) : 0,
                            DEPASVALIDATION = durPrevu != null ? durPrevu.DELTV.Value - Data.Date.GetDifference(traitprojets[j].DATEVALIDATION, traitprojets[j].DATECRE) : 0,
                            DEPASTRANSFSIIG = durPrevu != null ? durPrevu.DELENVOISIIGFP.Value - Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION) : 0,
                            DEPASSIIG = durPrevu != null ? durPrevu.DELSIIGFP.Value - Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP) : 0
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
            }
            catch (Exception) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Problème de connexion. " }, settings)); }
        }
    }
}
