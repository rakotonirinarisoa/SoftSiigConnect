using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using apptab.Data.Entities;
using apptab.Data;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

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
                                    select new
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

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.MODULLE == "LIQUIDATION") != null)
                {
                    foreach (var x in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE != "DEF" && a.TYPEPIECE != "TEF" && a.TYPEPIECE != "BE" && a.MODULLE == "LIQUIDATION").OrderBy(a => a.RANG).ToList())
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

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && (a.TYPEPIECE == "DEF" || a.TYPEPIECE == "TEF" || a.TYPEPIECE == "BE") && a.MODULLE == "LIQUIDATION") != null)
                {
                    var def = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF" && a.MODULLE == "LIQUIDATION") != null)
                        def = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF" && a.MODULLE == "LIQUIDATION").LIEN;
                    var tef = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF" && a.MODULLE == "LIQUIDATION") != null)
                        tef = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF" && a.MODULLE == "LIQUIDATION").LIEN;
                    var be = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE" && a.MODULLE == "LIQUIDATION") != null)
                        be = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE" && a.MODULLE == "LIQUIDATION").LIEN;

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

        //Liste des engagements, avances et paiements//
        public ActionResult BordListeEngaPaie()
        {
            ViewBag.Controller = "Liste des engagements, avances et des paiements";

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
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
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
                                           }).FirstOrDefault();
                                var paiement = db.OPA_VALIDATIONS.Where(pai => pai.ETAT == 3 && pai.IDPROJET == crpt && pai.IDREGLEMENT == x.REF).FirstOrDefault();
                                if (paiement != null)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        No = x.No,
                                        REF = x.REF,
                                        BENEF = x.TITUL,
                                        DATENGAGEMENT = x.DATEMANDAT != null ? x.DATEMANDAT : null,
                                        MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                        DATEPAIE = paiement.DATEVAL,
                                        MONTPAIE = string.Format("{0:0.00}", Math.Round(paiement.MONTANT.Value)),
                                        SOA = soa != null ? soa.SOA : "",
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                        TYPE = "Engagement",
                                        SITE = x.SITE
                                    });
                                }
                                else
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        No = x.No,
                                        REF = x.REF,
                                        BENEF = x.TITUL,
                                        DATENGAGEMENT = x.DATEMANDAT != null ? x.DATEMANDAT : null,
                                        MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                        DATEPAIE = null,
                                        MONTPAIE = "",
                                        SOA = soa != null ? soa.SOA : "",
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                        TYPE = "Engagement",
                                        SITE = x.SITE
                                    });
                                }
                            }
                        }
                        if (db.SI_TRAITAVANCE.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITAVANCE.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();
                                var paiement = db.OPA_VALIDATIONS.Where(pai => pai.ETAT == 3 && pai.IDPROJET == crpt && pai.IDREGLEMENT == x.REF && pai.AVANCE == true).FirstOrDefault();
                                if (paiement != null)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        No = x.No,
                                        REF = x.REF,
                                        BENEF = x.TITUL,
                                        DATENGAGEMENT = x.DATEMANDAT != null ? x.DATEMANDAT : null,
                                        MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                        DATEPAIE = paiement.DATEVAL,
                                        MONTPAIE = string.Format("{0:0.00}", Math.Round(paiement.MONTANT.Value)),
                                        SOA = soa != null ? soa.SOA : "",
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                        TYPE = "Avance",
                                        SITE = x.SITE
                                    });
                                }
                                else
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        No = x.No,
                                        REF = x.REF,
                                        BENEF = x.TITUL,
                                        DATENGAGEMENT = x.DATEMANDAT != null ? x.DATEMANDAT : null,
                                        MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                        DATEPAIE = null,
                                        MONTPAIE = "",
                                        SOA = soa != null ? soa.SOA : "",
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                        TYPE = "Avance",
                                        SITE = x.SITE
                                    });
                                }
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.OrderByDescending(a => a.DATENGAGEMENT).ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //Statut des engagements//
        public ActionResult StatutEngagements()
        {
            ViewBag.Controller = "Statut des engagements et avances";

            return View();
        }

        //Genere Statut des engagements et avances//
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
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
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
                                           }).FirstOrDefault();

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

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Engagement",
                                    SITE = x.SITE
                                });
                            }
                        }
                        if (db.SI_TRAITAVANCE.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITAVANCE.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

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

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Avance",
                                    SITE = x.SITE
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

        //Engagements et avances rejetés//
        public ActionResult EngagementsREJETE()
        {
            ViewBag.Controller = "Liste des engagements et avances rejetés";

            return View();
        }

        //Genere Liste Engagements et avances rejetés//
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
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
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
                                           }).FirstOrDefault();

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

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Engagement",
                                    SITE = x.SITE
                                    //isLATE = isLate
                                });
                            }
                        }
                        if (db.SI_TRAITAVANCE.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITAVANCE.Where(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                var isRejet = (from user in db.SI_USERS
                                               join rejet in db.SI_TRAITANNULAVANCE on user.ID equals rejet.IDUSER
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

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Avance",
                                    SITE = x.SITE
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

        //Suivi des délais de traitement des engagements et avances//
        public ActionResult DelaisTraitementEngagements()
        {
            ViewBag.Controller = "Suvi des délais de traitement des engagements et avances";

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

                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementEngagement>();

            var lastIndex = -1;
            var lastIndexAVANCE = -1;

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

                //ENGAGEMENTS//
                var traitprojets = await db.SI_TRAITPROJET.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                if (traitprojets.Count == 0)
                {
                    continue;
                }

                lastIndex += 1;

                result.Add(new TraitementEngagement
                {
                    SOA = s != null ? s.SOA : "",
                    TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                });

                for (int j = 0; j < traitprojets.Count; j += 1)
                {
                    result[lastIndex].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                    {
                        PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                        TYPE = "Engagement",
                        NUM_ENGAGEMENT = traitprojets[j].REF,
                        SITE = traitprojets[j].SITE,
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

                //AVANCES//
                var traitprojetsAVANCE = await db.SI_TRAITAVANCE.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                if (traitprojetsAVANCE.Count == 0)
                {
                    continue;
                }

                lastIndexAVANCE += 1;

                result.Add(new TraitementEngagement
                {
                    SOA = s != null ? s.SOA : "",
                    TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                });

                for (int j = 0; j < traitprojetsAVANCE.Count; j += 1)
                {
                    result[lastIndexAVANCE].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                    {
                        PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                        TYPE = "Avance",
                        NUM_ENGAGEMENT = traitprojetsAVANCE[j].REF,
                        SITE = traitprojetsAVANCE[j].SITE,
                        BENEFICIAIRE = traitprojetsAVANCE[j].TITUL,
                        MONTENGAGEMENT = Data.Cipher.Decrypt(traitprojetsAVANCE[j].MONT, "Oppenheimer").ToString(),
                        DATETRANSFERTRAF = traitprojetsAVANCE[j].DATECRE,
                        TRANSFERTRAFAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERCREATE),
                        DATEVALORDSEC = traitprojetsAVANCE[j].DATEVALIDATION,
                        VALORDSECAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERVALIDATE),
                        DATESENDSIIG = traitprojetsAVANCE[j].DATENVOISIIGFP,
                        SENDSIIGAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERENVOISIIGFP),
                        DATESIIGFP = traitprojetsAVANCE[j].DATESIIG,
                        SIIGFPAGENT = "",

                        DUREETRAITEMENTTRANSFERTRAF = Data.Date.GetDifference(traitprojetsAVANCE[j].DATECRE, traitprojetsAVANCE[j].DATEBE),
                        DUREETRAITEMENTVALORDSEC = Data.Date.GetDifference(traitprojetsAVANCE[j].DATEVALIDATION, traitprojetsAVANCE[j].DATECRE),
                        DUREETRAITEMENTSENDSIIG = Data.Date.GetDifference(traitprojetsAVANCE[j].DATENVOISIIGFP, traitprojetsAVANCE[j].DATEVALIDATION),
                        DUREETRAITEMENTSIIGFP = Data.Date.GetDifference(traitprojetsAVANCE[j].DATESIIG, traitprojetsAVANCE[j].DATENVOISIIGFP)
                    });
                }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
        }

        //Liste des traitements en souffrance (par rapport au délai moyen)//
        public ActionResult SoufTraitement()
        {
            ViewBag.Controller = "Liste des traitements en souffrance (par rapport au délai moyen)";

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

                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementEngagement>();

            var lastIndex = -1;
            var lastIndexAVANCE = -1;

            try
            {
                for (int i = 0; i < iProjectsId.Count; i += 1)
                {
                    int projectId = iProjectsId[i];

                    var durPrevu = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == projectId && a.DELETIONDATE == null);
                    if (durPrevu == null || durPrevu.DELRAF == null || durPrevu.DELTV == null /*|| durPrevu.DELENVOISIIGFP == null || durPrevu.DELSIIGFP == null*/)
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le délais des traitements. " }, settings));

                    var durPrevuAVANCE = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == projectId && a.DELETIONDATE == null);
                    if (durPrevuAVANCE == null || durPrevuAVANCE.DELARAF == null || durPrevuAVANCE.DELAV == null /*|| durPrevuAVANCE.DELAENVOISIIGFP == null || durPrevuAVANCE.DELASIIGFP == null*/)
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

                    //ENGAGEMENTS//
                    var traitprojets = await db.SI_TRAITPROJET.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                    //AVANCES//
                    var traitprojetsAVANCE = await db.SI_TRAITAVANCE.Where(a => a.IDPROJET == projectId && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToListAsync();

                    if (traitprojets.Count == 0 && traitprojetsAVANCE.Count == 0)
                    {
                        continue;
                    }

                    lastIndex += 1;

                    result.Add(new TraitementEngagement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                    });

                    for (int j = 0; j < traitprojets.Count; j += 1)
                    {
                        result[lastIndex].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            TYPE = "Engagement",
                            NUM_ENGAGEMENT = traitprojets[j].REF,
                            SITE = traitprojets[j].SITE,
                            BENEFICIAIRE = traitprojets[j].TITUL,
                            MONTENGAGEMENT = Data.Cipher.Decrypt(traitprojets[j].MONT, "Oppenheimer").ToString(),

                            DATETRANSFERTRAF = traitprojets[j].DATECRE,
                            DATEVALORDSEC = traitprojets[j].DATEVALIDATION,
                            //DATESENDSIIG = traitprojets[j].DATENVOISIIGFP,
                            //DATESIIGFP = traitprojets[j].DATESIIG,

                            TRANSFERTRAFAGENT = await GetAgent(traitprojets[j].IDUSERCREATE),
                            VALORDSECAGENT = await GetAgent(traitprojets[j].IDUSERVALIDATE),
                            //SENDSIIGAGENT = await GetAgent(traitprojets[j].IDUSERENVOISIIGFP),
                            //SIIGFPAGENT = "",

                            DUREETRAITEMENTTRANSFERTRAF = Data.Date.GetDifference(traitprojets[j].DATECRE, traitprojets[j].DATEBE),
                            DUREETRAITEMENTVALORDSEC = Data.Date.GetDifference(traitprojets[j].DATEVALIDATION, traitprojets[j].DATECRE),
                            //DUREETRAITEMENTSENDSIIG = Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION),
                            //DUREETRAITEMENTSIIGFP = Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP),

                            DURPREVUTRANSFERT = durPrevu != null ? durPrevu.DELRAF.Value : 0,
                            DURPREVUVALIDATION = durPrevu != null ? durPrevu.DELTV.Value : 0,
                            //DURPREVUTRANSFSIIG = durPrevu != null ? durPrevu.DELENVOISIIGFP.Value : 0,
                            //DURPREVUSIIG = durPrevu != null ? durPrevu.DELSIIGFP.Value : 0,

                            DEPASTRANSFERT = durPrevu != null ? Data.Date.GetDifference(traitprojets[j].DATECRE, traitprojets[j].DATEBE) - durPrevu.DELRAF.Value : 0,
                            DEPASVALIDATION = durPrevu != null ? Data.Date.GetDifference(traitprojets[j].DATEVALIDATION, traitprojets[j].DATECRE) - durPrevu.DELTV.Value : 0,
                            //DEPASTRANSFSIIG = durPrevu != null ? Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION) - durPrevu.DELENVOISIIGFP.Value : 0,
                            //DEPASSIIG = durPrevu != null ? Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP) - durPrevu.DELSIIGFP.Value : 0
                        });
                    }

                    lastIndexAVANCE += 1;

                    result.Add(new TraitementEngagement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementsEngagementsDetails = new List<TraitementEngagementDetails>()
                    });

                    for (int j = 0; j < traitprojetsAVANCE.Count; j += 1)
                    {
                        result[lastIndexAVANCE].TraitementsEngagementsDetails.Add(new TraitementEngagementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            TYPE = "Avance",
                            NUM_ENGAGEMENT = traitprojetsAVANCE[j].REF,
                            SITE = traitprojetsAVANCE[j].SITE,
                            BENEFICIAIRE = traitprojetsAVANCE[j].TITUL,
                            MONTENGAGEMENT = Data.Cipher.Decrypt(traitprojetsAVANCE[j].MONT, "Oppenheimer").ToString(),

                            DATETRANSFERTRAF = traitprojetsAVANCE[j].DATECRE,
                            DATEVALORDSEC = traitprojetsAVANCE[j].DATEVALIDATION,
                            //DATESENDSIIG = traitprojetsAVANCE[j].DATENVOISIIGFP,
                            //DATESIIGFP = traitprojetsAVANCE[j].DATESIIG,

                            TRANSFERTRAFAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERCREATE),
                            VALORDSECAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERVALIDATE),
                            //SENDSIIGAGENT = await GetAgent(traitprojetsAVANCE[j].IDUSERENVOISIIGFP),
                            //SIIGFPAGENT = "",

                            DUREETRAITEMENTTRANSFERTRAF = Data.Date.GetDifference(traitprojetsAVANCE[j].DATECRE, traitprojetsAVANCE[j].DATEBE),
                            DUREETRAITEMENTVALORDSEC = Data.Date.GetDifference(traitprojetsAVANCE[j].DATEVALIDATION, traitprojetsAVANCE[j].DATECRE),
                            //DUREETRAITEMENTSENDSIIG = Data.Date.GetDifference(traitprojetsAVANCE[j].DATENVOISIIGFP, traitprojetsAVANCE[j].DATEVALIDATION),
                            //DUREETRAITEMENTSIIGFP = Data.Date.GetDifference(traitprojetsAVANCE[j].DATESIIG, traitprojetsAVANCE[j].DATENVOISIIGFP),

                            DURPREVUTRANSFERT = durPrevuAVANCE != null ? durPrevuAVANCE.DELRAF.Value : 0,
                            DURPREVUVALIDATION = durPrevuAVANCE != null ? durPrevuAVANCE.DELTV.Value : 0,
                            //DURPREVUTRANSFSIIG = durPrevuAVANCE != null ? durPrevuAVANCE.DELENVOISIIGFP.Value : 0,
                            //DURPREVUSIIG = durPrevuAVANCE != null ? durPrevuAVANCE.DELSIIGFP.Value : 0,

                            DEPASTRANSFERT = durPrevuAVANCE != null ? Data.Date.GetDifference(traitprojetsAVANCE[j].DATECRE, traitprojetsAVANCE[j].DATEBE) - durPrevuAVANCE.DELRAF.Value : 0,
                            DEPASVALIDATION = durPrevuAVANCE != null ? Data.Date.GetDifference(traitprojetsAVANCE[j].DATEVALIDATION, traitprojetsAVANCE[j].DATECRE) - durPrevuAVANCE.DELTV.Value : 0,
                            //DEPASTRANSFSIIG = durPrevuAVANCE != null ? Data.Date.GetDifference(traitprojets[j].DATENVOISIIGFP, traitprojets[j].DATEVALIDATION) - durPrevuAVANCE.DELENVOISIIGFP.Value : 0,
                            //DEPASSIIG = durPrevuAVANCE != null ? Data.Date.GetDifference(traitprojets[j].DATESIIG, traitprojets[j].DATENVOISIIGFP) - durPrevuAVANCE.DELSIIGFP.Value : 0
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
            }
            catch (Exception) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Problème de connexion. " }, settings)); }
        }

        public ActionResult StatutPaiement()
        {
            ViewBag.Controller = "Status des paiements";
            return View();
        }

        [HttpPost]
        public JsonResult GenereSTATPAIEMENT(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<TxtPAIEMENT> list = new List<TxtPAIEMENT>();
                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);
                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                    }
                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        //var paielst = db.OPA_VALIDATIONS.Where(a => a.IDPROJET == crpt).Join().ToList();
                        var typeEcriture = db.SI_TYPECRITURE.Where(x => x.IDPROJET == crpt).FirstOrDefault().TYPE;
                        if (typeEcriture == 1)
                        {
                            var paielst = (
                               from r in db.OPA_REGLEMENTBR
                               join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                               where r.NUM == v.IDREGLEMENT && r.IDSOCIETE == crpt
                               select new
                               {
                                   BENEFICIAIRE = r.BENEFICIAIRE,
                                   MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                   NUM = r.NUM,
                                   DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                   DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                   DATETRANS = v.DATETRANS != null ? v.DATETRANS : null,
                                   TYPE = v.AVANCE == true ? "Avance" : "Réglement",
                                   SITE = v.SITE
                               }
                           ).ToList();

                            foreach (var item in paielst)
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();


                                list.Add(new TxtPAIEMENT
                                {
                                    No = item.NUM,
                                    BENEF = item.BENEFICIAIRE,
                                    MONTANT = item.MONTANT.ToString(),
                                    DATEVALIDATIONOP = item.DATECREA,
                                    DATEVALIDATIONAC = item.DATESEND,
                                    DATEPAIEBANQUE = item.DATETRANS,
                                    SOA = soa.SOA != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = item.TYPE == "1" ? "Avance" : "Réglement",
                                    SITE = item.SITE
                                });
                            }
                        }
                        else
                        {
                            var paielst = (
                               from r in db.OPA_REGLEMENT
                               join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                               where r.NUM.ToString() == v.IDREGLEMENT && r.IDSOCIETE == crpt
                               select new
                               {
                                   BENEFICIAIRE = r.BENEFICIAIRE,
                                   MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                   NUM = r.NUM,
                                   DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                   DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                   DATETRANS = v.DATETRANS != null ? v.DATETRANS : null,
                                   TYPE = v.AVANCE == true ? "Avance" : "Réglement",
                                   SITE = v.SITE
                               }
                           ).ToList();

                            foreach (var item in paielst)
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();


                                list.Add(new TxtPAIEMENT
                                {
                                    No = item.NUM.ToString(),
                                    BENEF = item.BENEFICIAIRE,
                                    MONTANT = item.MONTANT.ToString(),
                                    DATEVALIDATIONOP = item.DATECREA,
                                    DATEVALIDATIONAC = item.DATESEND,
                                    DATEPAIEBANQUE = item.DATETRANS,
                                    SOA = soa.SOA != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = item.TYPE == "1" ? "Avance" : "Réglement",
                                    SITE = item.SITE
                                });
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
        public ActionResult TraitementsPaiement()
        {
            ViewBag.Controller = "Suvi des délais de traitement des paiements";
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GenereDelaisTraitementPaiement(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
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

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementPaiement>();

            var lastIndex = -1;

            for (int i = 0; i < iProjectsId.Count; i += 1)
            {
                int projectId = iProjectsId[i];

                var typeEcriture = db.SI_TYPECRITURE.Where(x => x.IDPROJET == projectId).FirstOrDefault().TYPE;

                if (typeEcriture == 1)
                {
                    var s = await (
                            from soa in db.SI_SOAS
                            join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                            where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                            select new
                            {
                                soa.SOA
                            }
                            ).FirstOrDefaultAsync();

                    var paielst = (
                                   from r in db.OPA_REGLEMENTBR
                                   join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                                   where r.NUM == v.IDREGLEMENT && r.IDSOCIETE == projectId
                                   select new
                                   {
                                       BENEFICIAIRE = r.BENEFICIAIRE,
                                       MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                       NUM = r.NUM,
                                       DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                       DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                       DATEVAL = v.DATEVAL != null ? v.DATEVAL : null,
                                       IDUSCREA = v.IDUSCREA != null ? v.IDUSCREA : null,
                                       IDUSSEND = v.IDUSSEND != null ? v.IDUSSEND : null,
                                       IDUSVAL = v.IDUSVAL != null ? v.IDUSVAL : null,
                                       TYPE = v.AVANCE == true ? "Avance" : "Réglement",
                                       SITE = v.SITE
                                   }
                               ).ToList();

                    lastIndex += 1;

                    result.Add(new TraitementPaiement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementPaiementDetails = new List<TraitementPaiementDetails>()
                    });

                    for (int j = 0; j < paielst.Count; j += 1)
                    {
                        result[lastIndex].TraitementPaiementDetails.Add(new TraitementPaiementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            NUM_ENGAGEMENT = paielst[j].NUM,
                            BENEFICIAIRE = paielst[j].BENEFICIAIRE,
                            MONTENGAGEMENT = paielst[j].MONTANT.ToString(),
                            DATETRANSFERTRAF = paielst[j].DATECREA,
                            TRANSFERTRAFAGENT = await GetAgent(paielst[j].IDUSCREA),
                            DATEVALORDSEC = paielst[j].DATEVAL,
                            VALORDSECAGENT = await GetAgent(paielst[j].IDUSVAL),
                            DATESENDSIIG = paielst[j].DATESEND,
                            SENDSIIGAGENT = await GetAgent(paielst[j].IDUSSEND),
                            DUREETRAITEMENTTRANSFERTRAF = Date.GetDifference(paielst[j].DATECREA, paielst[j].DATESEND),
                            DUREETRAITEMENTVALORDSEC = Date.GetDifference(paielst[j].DATESEND, paielst[j].DATEVAL),
                            TYPE = paielst[j].TYPE,
                            SITE = paielst[j].SITE
                        });
                    }
                }
                else
                {
                    var s = await (
                            from soa in db.SI_SOAS
                            join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                            where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                            select new
                            {
                                soa.SOA
                            }
                        ).FirstOrDefaultAsync();

                    var paielst = (
                                   from r in db.OPA_REGLEMENT
                                   join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                                   where r.NUM.ToString() == v.IDREGLEMENT && r.IDSOCIETE == projectId
                                   select new
                                   {
                                       BENEFICIAIRE = r.BENEFICIAIRE,
                                       MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                       NUM = r.NUM,
                                       DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                       DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                       DATEVAL = v.DATEVAL != null ? v.DATEVAL : null,
                                       IDUSCREA = v.IDUSCREA != null ? v.IDUSCREA : null,
                                       IDUSSEND = v.IDUSSEND != null ? v.IDUSSEND : null,
                                       IDUSVAL = v.IDUSVAL != null ? v.IDUSVAL : null,
                                       TYPE = v.AVANCE == true ? "Avance" : "Réglement",
                                       SITE = v.SITE != null ? v.SITE : null
                                   }
                               ).ToList();

                    lastIndex += 1;

                    result.Add(new TraitementPaiement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementPaiementDetails = new List<TraitementPaiementDetails>()
                    });

                    for (int j = 0; j < paielst.Count; j += 1)
                    {
                        result[lastIndex].TraitementPaiementDetails.Add(new TraitementPaiementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            NUM_ENGAGEMENT = paielst[j].NUM.ToString(),
                            BENEFICIAIRE = paielst[j].BENEFICIAIRE,
                            MONTENGAGEMENT = paielst[j].MONTANT.ToString(),
                            DATETRANSFERTRAF = paielst[j].DATECREA,
                            TRANSFERTRAFAGENT = await GetAgent(paielst[j].IDUSCREA),
                            DATEVALORDSEC = paielst[j].DATEVAL,
                            VALORDSECAGENT = await GetAgent(paielst[j].IDUSVAL),
                            DATESENDSIIG = paielst[j].DATESEND,
                            SENDSIIGAGENT = await GetAgent(paielst[j].IDUSSEND),
                            DUREETRAITEMENTTRANSFERTRAF = Date.GetDifference(paielst[j].DATECREA, paielst[j].DATESEND),
                            DUREETRAITEMENTVALORDSEC = Date.GetDifference(paielst[j].DATESEND, paielst[j].DATEVAL),
                            TYPE = db.OPA_VALIDATIONS.Where(a => a.ID == projectId && a.IDREGLEMENT == paielst[j].NUM.ToString() && a.NUMEREG == int.Parse(paielst[j].TYPE)).FirstOrDefault().AVANCE == true ? "Avance" : "Réglement",
                            SITE = paielst[j].SITE
                        });
                    }
                }

            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
        }
        public ActionResult TraitementPaiementSoufrance()
        {
            ViewBag.Controller = "Liste des traitements en souffrance(par rapport au délai moyen)";
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GenereTraitementPaiementSouffrance(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
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

                iProjectsId.Add(projectId);
            }

            var result = new List<TraitementPaiement>();

            var lastIndex = -1;

            for (int i = 0; i < iProjectsId.Count; i += 1)
            {
                int projectId = iProjectsId[i];

                var typeEcriture = db.SI_TYPECRITURE.Where(x => x.IDPROJET == projectId).FirstOrDefault().TYPE;
                if (typeEcriture == 1)
                {
                    var s = await (
                    from soa in db.SI_SOAS
                    join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                    where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                    select new
                    {
                        soa.SOA
                    }
                ).FirstOrDefaultAsync();


                    var paielst = (
                                   from r in db.OPA_REGLEMENTBR
                                   join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                                   where r.NUM == v.IDREGLEMENT && r.IDSOCIETE == projectId
                                   select new
                                   {
                                       BENEFICIAIRE = r.BENEFICIAIRE,
                                       MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                       NUM = r.NUM,
                                       DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                       DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                       DATEVAL = v.DATEVAL != null ? v.DATEVAL : null,
                                       IDUSCREA = v.IDUSCREA != null ? v.IDUSCREA : null,
                                       IDUSSEND = v.IDUSSEND != null ? v.IDUSSEND : null,
                                       IDUSVAL = v.IDUSVAL != null ? v.IDUSVAL : null,
                                       SITE = v.SITE != null ? v.SITE : null,
                                   }
                               ).ToList();

                    var durerTraite = db.SI_DELAISTRAITEMENT.Where(x => x.IDPROJET == projectId).Select(x => new {
                        DELAISOP = x.DELPP,
                        DELAISAC = x.DELPV,
                        DELAISBK = x.DELPB,
                    }).ToList();

                    lastIndex += 1;

                    result.Add(new TraitementPaiement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementPaiementDetails = new List<TraitementPaiementDetails>()
                    });

                    for (int j = 0; j < paielst.Count; j += 1)
                    {
                        double dateOP = Date.GetDifference(paielst[j].DATESEND, paielst[j].DATECREA);
                        double dateAC = Date.GetDifference(paielst[j].DATEVAL, paielst[j].DATESEND);
                        double dateBK = Date.GetDifference(paielst[j].DATEVAL, paielst[j].DATESEND);
                        result[lastIndex].TraitementPaiementDetails.Add(new TraitementPaiementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            NUM_ENGAGEMENT = paielst[j].NUM,
                            BENEFICIAIRE = paielst[j].BENEFICIAIRE,
                            MONTENGAGEMENT = paielst[j].MONTANT.ToString(),
                            DATETRANSFERTRAF = paielst[j].DATECREA,
                            TRANSFERTRAFAGENT = await GetAgent(paielst[j].IDUSCREA),
                            DATEVALORDSEC = paielst[j].DATEVAL,
                            VALORDSECAGENT = await GetAgent(paielst[j].IDUSVAL),
                            DATESENDSIIG = paielst[j].DATESEND,
                            SENDSIIGAGENT = await GetAgent(paielst[j].IDUSSEND),
                            DUREETRAITEMENTTRANSFERTOP = dateOP,
                            DUREETRAITEMENTTRANSFERTAC = dateAC,
                            DUREETRAITEMENTTRANSFERTBK = dateBK,

                            DUREETRAITEMENTPREVUEOP = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISOP),
                            DUREETRAITEMENTPREVUEAC = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISAC),
                            DUREETRAITEMENTPREVUEBK = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISBK),

                            DEPASSEMENTOP = durerTraite.FirstOrDefault().DELAISOP != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISOP) - dateOP : 0,
                            DEPASSEMENTAC = durerTraite.FirstOrDefault().DELAISAC != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISAC) - dateAC : 0,
                            DEPASSEMENTBK = durerTraite.FirstOrDefault().DELAISBK != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISBK) - dateBK : 0,
                            SITE = paielst[j].SITE,
                        });
                    }
                }
                else
                {
                    var s = await (
                    from soa in db.SI_SOAS
                    join prosoa in db.SI_PROSOA on soa.ID equals prosoa.IDSOA
                    where prosoa.IDPROJET == projectId && prosoa.DELETIONDATE == null && soa.DELETIONDATE == null
                    select new
                    {
                        soa.SOA
                    }
                ).FirstOrDefaultAsync();


                    var paielst = (
                                   from r in db.OPA_REGLEMENT
                                   join v in db.OPA_VALIDATIONS on r.IDSOCIETE equals v.IDPROJET
                                   where r.NUM.ToString() == v.IDREGLEMENT && r.IDSOCIETE == projectId
                                   select new
                                   {
                                       BENEFICIAIRE = r.BENEFICIAIRE,
                                       MONTANT = r.MONTANT != null ? r.MONTANT : null,
                                       NUM = r.NUM,
                                       DATECREA = v.DATECREA != null ? v.DATECREA : null,
                                       DATESEND = v.DATESEND != null ? v.DATESEND : null,
                                       DATEVAL = v.DATEVAL != null ? v.DATEVAL : null,
                                       IDUSCREA = v.IDUSCREA != null ? v.IDUSCREA : null,
                                       IDUSSEND = v.IDUSSEND != null ? v.IDUSSEND : null,
                                       IDUSVAL = v.IDUSVAL != null ? v.IDUSVAL : null,
                                       SITE = v.SITE != null ? v.SITE : null,
                                   }
                               ).ToList();
                    var durerTraite = db.SI_DELAISTRAITEMENT.Where(x => x.IDPROJET == projectId).Select(x => new {
                        DELAISOP = x.DELPP,
                        DELAISAC = x.DELPV,
                        DELAISBK = x.DELPB,
                    }).ToList();
                    lastIndex += 1;

                    result.Add(new TraitementPaiement
                    {
                        SOA = s != null ? s.SOA : "",
                        TraitementPaiementDetails = new List<TraitementPaiementDetails>()
                    });

                    for (int j = 0; j < paielst.Count; j += 1)
                    {
                        result[lastIndex].TraitementPaiementDetails.Add(new TraitementPaiementDetails
                        {
                            PROJET = db.SI_PROJETS.FirstOrDefault(a => a.ID == projectId && a.DELETIONDATE == null).PROJET,
                            NUM_ENGAGEMENT = paielst[j].NUM.ToString(),
                            BENEFICIAIRE = paielst[j].BENEFICIAIRE,
                            MONTENGAGEMENT = paielst[j].MONTANT.ToString(),
                            DATETRANSFERTRAF = paielst[j].DATECREA,
                            TRANSFERTRAFAGENT = await GetAgent(paielst[j].IDUSCREA),
                            DATEVALORDSEC = paielst[j].DATEVAL,
                            VALORDSECAGENT = await GetAgent(paielst[j].IDUSVAL),
                            DATESENDSIIG = paielst[j].DATESEND,
                            SENDSIIGAGENT = await GetAgent(paielst[j].IDUSSEND),
                            DUREETRAITEMENTTRANSFERTOP = Date.GetDifference(paielst[j].DATECREA, paielst[j].DATESEND),
                            DUREETRAITEMENTTRANSFERTAC = Date.GetDifference(paielst[j].DATESEND, paielst[j].DATEVAL),
                            DUREETRAITEMENTTRANSFERTBK = Date.GetDifference(paielst[j].DATESEND, paielst[j].DATEVAL),

                            DUREETRAITEMENTPREVUEOP = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISOP),
                            DUREETRAITEMENTPREVUEAC = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISAC),
                            DUREETRAITEMENTPREVUEBK = Convert.ToDouble(durerTraite.FirstOrDefault().DELAISBK),

                            DEPASSEMENTOP = durerTraite.FirstOrDefault().DELAISOP != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISOP) - Convert.ToDouble(Date.GetDifference(paielst[j].DATECREA, paielst[j].DATESEND)) : 0,
                            DEPASSEMENTAC = durerTraite.FirstOrDefault().DELAISAC != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISAC) - Convert.ToDouble(Date.GetDifference(paielst[j].DATESEND, paielst[j].DATEVAL)) : 0,
                            DEPASSEMENTBK = durerTraite.FirstOrDefault().DELAISBK != null ? Convert.ToDouble(durerTraite.FirstOrDefault().DELAISBK) - Convert.ToDouble(Date.GetDifference(paielst[j].DATEVAL, paielst[j].DATEVAL)) : 0,

                            SITE = paielst[j].SITE,
                        });
                    }
                }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = result }, settings));
        }
        public ActionResult TraitementPaiementRejet()
        {
            ViewBag.Controller = "Liste des paiements rejetés";
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GenerePaiementREJETE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<TxtPAIEMENT> list = new List<TxtPAIEMENT>();
                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        var typeEcriture = db.SI_TYPECRITURE.Where(x => x.IDPROJET == crpt).FirstOrDefault().TYPE;
                        if (db.OPA_VALIDATIONS.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DateIn >= DateDebut && a.DateOut <= DateFin) != null)
                        {
                            foreach (var x in db.OPA_VALIDATIONS.Where(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DateIn >= DateDebut && a.DateOut <= DateFin).OrderBy(a => a.DateIn).OrderBy(a => a.DATECREA).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                var cancel = db.OPA_VALIDATIONS.Where(a => a.IDPROJET == crpt && x.ETAT == 4).Join(db.SI_USERS, z => z.IDUSER, e => e.IDUSER, (z, e) => new
                                {
                                    AGENT = e.LOGIN,
                                    NUMENGAGEMENT = z.IDREGLEMENT,
                                    DATEREJETPAIEMENT = z.DATEANNULER,
                                    MOTIF = z.MOTIF,
                                    COMMENTAIRE = z.COMS,
                                    SITE = z.SITE

                                }).Join(db.OPA_REGLEMENTBR, t => t.NUMENGAGEMENT, p => p.NUM, (t, p) => new
                                {
                                    AGENT = t.AGENT,
                                    NUMENGAGEMENT = t.NUMENGAGEMENT,
                                    DATEREJETPAIEMENT = t.DATEREJETPAIEMENT,
                                    MOTIF = t.MOTIF,
                                    COMMENTAIRE = t.MOTIF,
                                    BENEFICIAIRE = p.BENEFICIAIRE,
                                    MONTANT = p.MONTANT,
                                    SITE = t.SITE
                                }).ToList();

                                foreach (var item in cancel)
                                {
                                    list.Add(new TxtPAIEMENT
                                    {
                                        No = item.NUMENGAGEMENT,
                                        BENEF = item.BENEFICIAIRE,
                                        MONTANT = item.MONTANT.ToString(),
                                        DATEREJETAC = item.DATEREJETPAIEMENT,
                                        SOA = soa.SOA != null ? soa.SOA : "",
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                        SITE = item.SITE
                                    });
                                }
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

        //Statut des Justificatifs et reversements//
        public ActionResult StatutJR()
        {
            ViewBag.Controller = "Statut des justificatifs et reversements";

            return View();
        }

        //Genere Statut des Justificatifs et reversements//
        [HttpPost]
        public JsonResult GenereStatutJR(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
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
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITJUSTIF.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                list.Add(new TxLISTETRAIT
                                {
                                    No = x.No,
                                    REF = x.REF,
                                    BENEF = x.TITUL,
                                    NPIECE = x.NPIECE,

                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),

                                    DATETRANSFERTRAF = x.DATECRE != null ? x.DATECRE : null,
                                    DATEVALORDSEC = x.DATEVALIDATION != null ? x.DATEVALIDATION : null,

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Justificatif",
                                    SITE = x.SITE
                                });
                            }
                        }
                        if (db.SI_TRAITREVERS.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITREVERS.Where(a => a.IDPROJET == crpt && a.ETAT != 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                list.Add(new TxLISTETRAIT
                                {
                                    No = x.No,
                                    REF = x.REF,
                                    BENEF = x.TITUL,

                                    NPIECE = x.NPIECE,

                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),

                                    DATETRANSFERTRAF = x.DATECRE != null ? x.DATECRE : null,
                                    DATEVALORDSEC = x.DATEVALIDATION != null ? x.DATEVALIDATION : null,

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Reversement",
                                    SITE = x.SITE
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

        //Justificatifs et reversements rejetés//
        public ActionResult StatutJRA()
        {
            ViewBag.Controller = "Liste des justificatifs et reversements rejetés";

            return View();
        }

        //Genere Liste Engagements et avances rejetés//
        [HttpPost]
        public async Task<JsonResult> GenereStatutJRA(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin)
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

                        //Check si le projet est mappé à une base de données TOM²PRO//
                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                        SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                        SOFTCONNECTOM tom = new SOFTCONNECTOM();

                        //Check si la correspondance des états est OK//
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                        if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                        //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                        if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Avance). " }, settings));
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITJUSTIF.Where(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                var isRejet = (from user in db.SI_USERS
                                               join rejet in db.SI_TRAITANNULJUSTIF on user.ID equals rejet.IDUSER
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
                                    NPIECE = x.NPIECE,
                                    BENEF = x.TITUL,
                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                    AGENTREJETE = isRejet != null ? await GetAgent(isRejet.IDUSER) : "",
                                    DATEREJETE = isRejet != null ? isRejet.DATEREJE : null,
                                    MOTIF = isRejet != null ? isRejet.MOTIF : "",
                                    COMMENTAIRE = isRejet != null ? isRejet.COMMENTAIRE : "",

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Justificatif",
                                    SITE = x.SITE
                                });
                            }
                        }
                        if (db.SI_TRAITREVERS.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                        {
                            foreach (var x in db.SI_TRAITREVERS.Where(a => a.IDPROJET == crpt && a.ETAT == 2 && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).OrderBy(a => a.DATEMANDAT).OrderBy(a => a.DATECRE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                var isRejet = (from user in db.SI_USERS
                                               join rejet in db.SI_TRAITANNULREVERS on user.ID equals rejet.IDUSER
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
                                    NPIECE = x.NPIECE,
                                    BENEF = x.TITUL,
                                    MONTENGAGEMENT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                    AGENTREJETE = isRejet != null ? await GetAgent(isRejet.IDUSER) : "",
                                    DATEREJETE = isRejet != null ? isRejet.DATEREJE : null,
                                    MOTIF = isRejet != null ? isRejet.MOTIF : "",
                                    COMMENTAIRE = isRejet != null ? isRejet.COMMENTAIRE : "",

                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = "Reversement",
                                    SITE = x.SITE
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

        //Suivi budgétaire et crédit ouvert//
        public ActionResult BudgetCreditOuvert()
        {
            ViewBag.Controller = "Suivi budgétaire";

            return View();
        }

        //GENERER Suivi budgétaire et crédit ouvert//
        [HttpPost]
        public JsonResult GenerePTBA(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, int numbud)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            //if (DateDebut.Year != DateFin.Year) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous ne pouvez pas générer deux années différentes. " }, settings));

            var anneeDeb = DateDebut.Year;
            var anneeFin = DateFin.Year;
            List<string> listAnnee = new List<string>();
            for (int i = anneeDeb; i <= anneeFin; i++)
            { listAnnee.Add(i.ToString()); }

            try
            {
                List<TxLISTETRAIT> list = new List<TxLISTETRAIT>();
                var pro = listProjet;

                List<string> autrePCOP = new List<string>();

                if (pro != null)
                {
                    int crpt = int.Parse(pro);

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
                        return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                    if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                        return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));
                    if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                        return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO (Liquidation). " }, settings));

                    //Check PROCESS (PAD et PCOP)//
                    if (db.SI_TYPEPROCESSUS.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la table PAD et la colonne PCOP. " }, settings));

                    var isParam = db.SI_TYPEPROCESSUS.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                    //ACTI//
                    if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                    {
                        foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                        {
                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                            {
                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                                {
                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID).ToList())
                                    {
                                        if (tom.MBUDGET.Where(a => a.NUMBUD == numbud && a.ACTI == yy.ACTI).Count() == 0)
                                        {
                                            if (!autrePCOP.Contains(yy.ACTI) && !String.IsNullOrEmpty(yy.ACTI))
                                                autrePCOP.Add(yy.ACTI);
                                        }
                                    }
                                }
                            }
                        }

                        if (tom.MBUDGET.Any(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)))
                        {
                            foreach (var x in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)).Select(a => a.ACTI).Distinct().ToList())
                            {
                                if (!autrePCOP.Contains(x) && !String.IsNullOrEmpty(x))
                                    autrePCOP.Add(x);
                            }

                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeT = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADT = 0;
                            //TOTAL Liquidation validé SET//
                            decimal MTNENGAT = 0;
                            //TOTAL Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                            decimal MTNPAYET = 0;
                            //TOTAL % sur PAD (% Solde sur PAD)
                            decimal PADT = 0;
                            //TOTAL % sur PTBA (% Solde sur PTBA)
                            decimal PTBAT = 0;
                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeTP = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADTP = 0;
                            //SITE
                            string SITE = "";

                            foreach (var x in autrePCOP.Distinct().OrderBy(x => x).ToList())
                            {
                                //PTBA
                                decimal MTNTOTALPeriode = 0;
                                //PCOP et INTITULE PCOP//
                                var PCOP = x;
                                var PCOPINTITUL = !String.IsNullOrEmpty(x) ? tom.RACTI1.FirstOrDefault(a => a.CODE == x).LIBELLE : "";
                                //PAD
                                decimal MTNTOTALPAD = 0;
                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                decimal MTNENGA = 0;
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                decimal MTNPAYE = 0;

                                foreach (var s in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE) && a.ACTI == x).ToList())
                                {
                                    //PAD
                                    if (MTNTOTALPAD == 0) if (tom.RREPACTI.Any(a => a.CODE == s.ACTI)) MTNTOTALPAD = tom.RREPACTI.FirstOrDefault(a => a.CODE == s.ACTI).MONTREP1.Value;

                                    //PTBA
                                    foreach (var y in tom.MBUDALLOC.Where(a => a.NUMBUD == numbud && a.NUMENREG == s.NUMENREG && (a.MOIS >= DateDebut && a.MOIS <= DateFin) && listAnnee.Contains(a.ANNEE)).ToList())
                                    {
                                        MTNTOTALPeriode += y.MONTANT.Value;
                                    }

                                    //SITE
                                    if (String.IsNullOrEmpty(SITE))
                                        SITE = s.SITE;
                                }

                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                                        {
                                            foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.ACTI == PCOP).ToList())
                                            {
                                                MTNENGA += yy.MONTANTLOCAL.Value;
                                            }
                                        }
                                    }
                                }
                                //Justif validé SET//
                                foreach (var z in db.SI_TRAITJUSTIF.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                                        {
                                            foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.ACTI == PCOP).ToList())
                                            {
                                                MTNENGA += xx.MONTANT.Value;
                                            }
                                        }
                                    }
                                }
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                var ListTraitement = db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList();
                                foreach (var opaV in db.OPA_VALIDATIONS.Where(a => a.ETAT == 2 && a.dateOrdre >= DateDebut && a.dateOrdre <= DateFin && a.NUMEROLIQUIDATION != null))
                                {
                                    if (opaV.AVANCE == false)
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                                                {
                                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.ACTI == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "ACTI")
                                                {
                                                    foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.ACTI == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                if (MTNTOTALPAD != 0 || MTNTOTALPeriode != 0 || MTNENGA != 0 || MTNPAYE != 0)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        //No = "",//ID
                                        SOA = soa != null ? soa.SOA : "",//SOA
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//PROJET

                                        SITE = SITE,

                                        REF = PCOP,//PCOP
                                        INTITUT = PCOPINTITUL,//Intitulé PCOP

                                        BENEF = Math.Round(MTNTOTALPAD, 2).ToString(),//PAD
                                        MONTENGAGEMENT = Math.Round(MTNTOTALPeriode, 2).ToString(),//Montant PTBA
                                        MONTPAIE = Math.Round(MTNENGA, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                        TYPE = Math.Round(MTNPAYE, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                        NPIECE = Math.Round((MTNTOTALPAD - MTNENGA), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        AGENTREJETE = MTNTOTALPAD != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        MOTIF = Math.Round((MTNTOTALPeriode - MTNENGA), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        COMMENTAIRE = MTNTOTALPeriode != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        SoldPadPayé = Math.Round((MTNTOTALPAD - MTNPAYE), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        SoldPadPayéP = MTNTOTALPAD != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        SoldPtbaPayé = Math.Round((MTNTOTALPeriode - MTNPAYE), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        SoldPtbaPayéP = MTNTOTALPeriode != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        PayeEngage = MTNPAYE != 0 && MTNENGA != 0 ? Math.Round(((MTNPAYE * 100) / MTNENGA), 2).ToString() : "0",//% sur payé et engagé
                                    });

                                    //TOTAL
                                    MTNTOTALPeriodeT += MTNTOTALPeriode;
                                    MTNTOTALPADT += MTNTOTALPAD;
                                    MTNENGAT += MTNENGA;
                                    MTNPAYET += MTNPAYE;
                                }
                            }

                            list.Add(new TxLISTETRAIT
                            {
                                //No = "",//ID
                                SOA = "",//SOA
                                PROJET = "",//PROJET

                                SITE = "",

                                REF = "",//PCOP
                                INTITUT = "TOTAL",//Intitulé PCOP

                                BENEF = Math.Round(MTNTOTALPADT, 2).ToString(),//PAD
                                MONTENGAGEMENT = Math.Round(MTNTOTALPeriodeT, 2).ToString(),//Montant PTBA
                                MONTPAIE = Math.Round(MTNENGAT, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                TYPE = Math.Round(MTNPAYET, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                NPIECE = Math.Round((MTNTOTALPADT - MTNENGAT), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                AGENTREJETE = MTNTOTALPADT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                MOTIF = Math.Round((MTNTOTALPeriodeT - MTNENGAT), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                COMMENTAIRE = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                SoldPadPayé = Math.Round((MTNTOTALPADT - MTNPAYET), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                SoldPadPayéP = MTNTOTALPADT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                SoldPtbaPayé = Math.Round((MTNTOTALPeriodeT - MTNPAYET), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                SoldPtbaPayéP = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                PayeEngage = MTNPAYET != 0 && MTNENGAT != 0 ? Math.Round(((MTNPAYET * 100) / MTNENGAT), 2).ToString() : "0",//% sur payé et engagé
                            });
                        }
                    }

                    //GEO//
                    if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                    {
                        foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                        {
                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                            {
                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                                {
                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID).ToList())
                                    {
                                        if (tom.MBUDGET.Where(a => a.NUMBUD == numbud && a.GEO == yy.GEO).Count() == 0)
                                        {
                                            if (!autrePCOP.Contains(yy.GEO) && !String.IsNullOrEmpty(yy.GEO))
                                                autrePCOP.Add(yy.GEO);
                                        }
                                    }
                                }
                            }
                        }

                        if (tom.MBUDGET.Any(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)))
                        {
                            foreach (var x in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)).Select(a => a.GEO).Distinct().ToList())
                            {
                                if (!autrePCOP.Contains(x) && !String.IsNullOrEmpty(x))
                                    autrePCOP.Add(x);
                            }

                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeT = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADT = 0;
                            //TOTAL Liquidation validé SET//
                            decimal MTNENGAT = 0;
                            //TOTAL Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                            decimal MTNPAYET = 0;
                            //TOTAL % sur PAD (% Solde sur PAD)
                            decimal PADT = 0;
                            //TOTAL % sur PTBA (% Solde sur PTBA)
                            decimal PTBAT = 0;
                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeTP = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADTP = 0;
                            //SITE
                            string SITE = "";

                            foreach (var x in autrePCOP.OrderBy(x => x).Distinct().ToList())
                            {
                                //PTBA
                                decimal MTNTOTALPeriode = 0;
                                //PCOP et INTITULE PCOP//
                                var PCOP = x;
                                var PCOPINTITUL = !String.IsNullOrEmpty(x) ? tom.RGEO1.FirstOrDefault(a => a.CODE == x).LIBELLE : "";
                                //PAD
                                decimal MTNTOTALPAD = 0;
                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                decimal MTNENGA = 0;
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                decimal MTNPAYE = 0;

                                foreach (var s in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE) && a.GEO == x).ToList())
                                {
                                    //PAD
                                    if (MTNTOTALPAD == 0) if (tom.RREPGEO.Any(a => a.CODE == s.GEO)) MTNTOTALPAD = tom.RREPGEO.FirstOrDefault(a => a.CODE == s.GEO).MONTREP1.Value;
                                    //PTBA
                                    foreach (var y in tom.MBUDALLOC.Where(a => a.NUMBUD == numbud && a.NUMENREG == s.NUMENREG && (a.MOIS >= DateDebut && a.MOIS <= DateFin) && listAnnee.Contains(a.ANNEE)).ToList())
                                    {
                                        MTNTOTALPeriode += y.MONTANT.Value;
                                    }
                                    //SITE
                                    if (String.IsNullOrEmpty(SITE))
                                        SITE = s.SITE;
                                }

                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                                        {
                                            foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.GEO == PCOP).ToList())
                                            {
                                                MTNENGA += yy.MONTANTLOCAL.Value;
                                            }
                                        }
                                    }
                                }
                                //Justif validé SET//
                                foreach (var z in db.SI_TRAITJUSTIF.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                                        {
                                            foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.GEO == PCOP).ToList())
                                            {
                                                MTNENGA += xx.MONTANT.Value;
                                            }
                                        }
                                    }
                                }
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                var ListTraitement = db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList();
                                foreach (var opaV in db.OPA_VALIDATIONS.Where(a => a.ETAT == 2 && a.dateOrdre >= DateDebut && a.dateOrdre <= DateFin && a.NUMEROLIQUIDATION != null))
                                {
                                    if (opaV.AVANCE == false)
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                                                {
                                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.GEO == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "GEO")
                                                {
                                                    foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.GEO == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                if (MTNTOTALPAD != 0 || MTNTOTALPeriode != 0 || MTNENGA != 0 || MTNPAYE != 0)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        //No = "",//ID
                                        SOA = soa != null ? soa.SOA : "",//SOA
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//PROJET

                                        REF = PCOP,//PCOP
                                        INTITUT = PCOPINTITUL,//Intitulé PCOP

                                        SITE = SITE,

                                        BENEF = Math.Round(MTNTOTALPAD, 2).ToString(),//PAD
                                        MONTENGAGEMENT = Math.Round(MTNTOTALPeriode, 2).ToString(),//Montant PTBA
                                        MONTPAIE = Math.Round(MTNENGA, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                        TYPE = Math.Round(MTNPAYE, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                        NPIECE = Math.Round((MTNTOTALPAD - MTNENGA), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        AGENTREJETE = MTNTOTALPAD != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        MOTIF = Math.Round((MTNTOTALPeriode - MTNENGA), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        COMMENTAIRE = MTNTOTALPeriode != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        SoldPadPayé = Math.Round((MTNTOTALPAD - MTNPAYE), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        SoldPadPayéP = MTNTOTALPAD != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        SoldPtbaPayé = Math.Round((MTNTOTALPeriode - MTNPAYE), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        SoldPtbaPayéP = MTNTOTALPeriode != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        PayeEngage = MTNPAYE != 0 && MTNENGA != 0 ? Math.Round(((MTNPAYE * 100) / MTNENGA), 2).ToString() : "0",//% sur payé et engagé
                                    });

                                    //TOTAL
                                    MTNTOTALPeriodeT += MTNTOTALPeriode;
                                    MTNTOTALPADT += MTNTOTALPAD;
                                    MTNENGAT += MTNENGA;
                                    MTNPAYET += MTNPAYE;
                                }
                            }

                            list.Add(new TxLISTETRAIT
                            {
                                //No = "",//ID
                                SOA = "",//SOA
                                PROJET = "",//PROJET

                                SITE = "",

                                REF = "",//PCOP
                                INTITUT = "TOTAL",//Intitulé PCOP

                                BENEF = Math.Round(MTNTOTALPADT, 2).ToString(),//PAD
                                MONTENGAGEMENT = Math.Round(MTNTOTALPeriodeT, 2).ToString(),//Montant PTBA
                                MONTPAIE = Math.Round(MTNENGAT, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                TYPE = Math.Round(MTNPAYET, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                NPIECE = Math.Round((MTNTOTALPADT - MTNENGAT), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                AGENTREJETE = MTNTOTALPADT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                MOTIF = Math.Round((MTNTOTALPeriodeT - MTNENGAT), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                COMMENTAIRE = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                SoldPadPayé = Math.Round((MTNTOTALPADT - MTNPAYET), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                SoldPadPayéP = MTNTOTALPADT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                SoldPtbaPayé = Math.Round((MTNTOTALPeriodeT - MTNPAYET), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                SoldPtbaPayéP = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                PayeEngage = MTNPAYET != 0 && MTNENGAT != 0 ? Math.Round(((MTNPAYET * 100) / MTNENGAT), 2).ToString() : "0",//% sur payé et engagé
                            });
                        }
                    }

                    //PLAN6//
                    if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                    {
                        foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                        {
                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                            {
                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                                {
                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID).ToList())
                                    {
                                        if (tom.MBUDGET.Where(a => a.NUMBUD == numbud && a.PLAN6 == yy.PLAN6).Count() == 0)
                                        {
                                            if (!autrePCOP.Contains(yy.PLAN6) && !String.IsNullOrEmpty(yy.PLAN6))
                                                autrePCOP.Add(yy.PLAN6);
                                        }
                                    }
                                }
                            }
                        }

                        if (tom.MBUDGET.Any(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)))
                        {
                            foreach (var x in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)).Select(a => a.PLAN6).Distinct().ToList())
                            {
                                if (!autrePCOP.Contains(x) && !String.IsNullOrEmpty(x))
                                    autrePCOP.Add(x);
                            }

                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeT = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADT = 0;
                            //TOTAL Liquidation validé SET//
                            decimal MTNENGAT = 0;
                            //TOTAL Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                            decimal MTNPAYET = 0;
                            //TOTAL % sur PAD (% Solde sur PAD)
                            decimal PADT = 0;
                            //TOTAL % sur PTBA (% Solde sur PTBA)
                            decimal PTBAT = 0;
                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeTP = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADTP = 0;
                            //SITE
                            string SITE = "";

                            foreach (var x in autrePCOP.OrderBy(x => x).Distinct().ToList())
                            {
                                //PTBA
                                decimal MTNTOTALPeriode = 0;
                                //PCOP et INTITULE PCOP//
                                var PCOP = x;
                                var PCOPINTITUL = !String.IsNullOrEmpty(x) ? tom.RPLAN6.FirstOrDefault(a => a.CODE == x).LIBELLE : "";
                                //PAD
                                decimal MTNTOTALPAD = 0;
                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                decimal MTNENGA = 0;
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                decimal MTNPAYE = 0;

                                foreach (var s in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE) && a.PLAN6 == x).ToList())
                                {
                                    //PAD
                                    if (MTNTOTALPAD == 0) if (tom.RREPPLAN6.Any(a => a.CODE == s.PLAN6)) MTNTOTALPAD = tom.RREPPLAN6.FirstOrDefault(a => a.CODE == s.PLAN6).MONTREP1.Value;
                                    //PTBA
                                    foreach (var y in tom.MBUDALLOC.Where(a => a.NUMBUD == numbud && a.NUMENREG == s.NUMENREG && (a.MOIS >= DateDebut && a.MOIS <= DateFin) && listAnnee.Contains(a.ANNEE)).ToList())
                                    {
                                        MTNTOTALPeriode += y.MONTANT.Value;
                                    }
                                    //SITE
                                    if (String.IsNullOrEmpty(SITE))
                                        SITE = s.SITE;
                                }

                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                                        {
                                            foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.PLAN6 == PCOP).ToList())
                                            {
                                                MTNENGA += yy.MONTANTLOCAL.Value;
                                            }
                                        }
                                    }
                                }
                                //Justif validé SET//
                                foreach (var z in db.SI_TRAITJUSTIF.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                                        {
                                            foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.PLAN6 == PCOP).ToList())
                                            {
                                                MTNENGA += xx.MONTANT.Value;
                                            }
                                        }
                                    }
                                }
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                var ListTraitement = db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList();
                                foreach (var opaV in db.OPA_VALIDATIONS.Where(a => a.ETAT == 2 && a.dateOrdre >= DateDebut && a.dateOrdre <= DateFin && a.NUMEROLIQUIDATION != null))
                                {
                                    if (opaV.AVANCE == false)
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                                                {
                                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.PLAN6 == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "PLAN6")
                                                {
                                                    foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.PLAN6 == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                if (MTNTOTALPAD != 0 || MTNTOTALPeriode != 0 || MTNENGA != 0 || MTNPAYE != 0)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        //No = "",//ID
                                        SOA = soa != null ? soa.SOA : "",//SOA
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//PROJET

                                        SITE = SITE,

                                        REF = PCOP,//PCOP
                                        INTITUT = PCOPINTITUL,//Intitulé PCOP

                                        BENEF = Math.Round(MTNTOTALPAD, 2).ToString(),//PAD
                                        MONTENGAGEMENT = Math.Round(MTNTOTALPeriode, 2).ToString(),//Montant PTBA
                                        MONTPAIE = Math.Round(MTNENGA, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                        TYPE = Math.Round(MTNPAYE, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                        NPIECE = Math.Round((MTNTOTALPAD - MTNENGA), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        AGENTREJETE = MTNTOTALPAD != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        MOTIF = Math.Round((MTNTOTALPeriode - MTNENGA), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        COMMENTAIRE = MTNTOTALPeriode != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        SoldPadPayé = Math.Round((MTNTOTALPAD - MTNPAYE), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        SoldPadPayéP = MTNTOTALPAD != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        SoldPtbaPayé = Math.Round((MTNTOTALPeriode - MTNPAYE), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        SoldPtbaPayéP = MTNTOTALPeriode != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        PayeEngage = MTNPAYE != 0 && MTNENGA != 0 ? Math.Round(((MTNPAYE * 100) / MTNENGA), 2).ToString() : "0",//% sur payé et engagé
                                    });

                                    //TOTAL
                                    MTNTOTALPeriodeT += MTNTOTALPeriode;
                                    MTNTOTALPADT += MTNTOTALPAD;
                                    MTNENGAT += MTNENGA;
                                    MTNPAYET += MTNPAYE;
                                }
                            }

                            list.Add(new TxLISTETRAIT
                            {
                                //No = "",//ID
                                SOA = "",//SOA
                                PROJET = "",//PROJET

                                SITE = "",

                                REF = "",//PCOP
                                INTITUT = "TOTAL",//Intitulé PCOP

                                BENEF = Math.Round(MTNTOTALPADT, 2).ToString(),//PAD
                                MONTENGAGEMENT = Math.Round(MTNTOTALPeriodeT, 2).ToString(),//Montant PTBA
                                MONTPAIE = Math.Round(MTNENGAT, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                TYPE = Math.Round(MTNPAYET, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                NPIECE = Math.Round((MTNTOTALPADT - MTNENGAT), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                AGENTREJETE = MTNTOTALPADT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                MOTIF = Math.Round((MTNTOTALPeriodeT - MTNENGAT), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                COMMENTAIRE = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                SoldPadPayé = Math.Round((MTNTOTALPADT - MTNPAYET), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                SoldPadPayéP = MTNTOTALPADT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                SoldPtbaPayé = Math.Round((MTNTOTALPeriodeT - MTNPAYET), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                SoldPtbaPayéP = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                PayeEngage = MTNPAYET != 0 && MTNENGAT != 0 ? Math.Round(((MTNPAYET * 100) / MTNENGAT), 2).ToString() : "0",//% sur payé et engagé
                            });
                        }
                    }

                    //POSTE//
                    if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                    {
                        foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                        {
                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                            {
                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                                {
                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID).ToList())
                                    {
                                        if (tom.MBUDGET.Where(a => a.NUMBUD == numbud && a.POSTE == yy.POSTE).Count() == 0)
                                        {
                                            if (!autrePCOP.Contains(yy.POSTE) && !String.IsNullOrEmpty(yy.POSTE))
                                                autrePCOP.Add(yy.POSTE);
                                        }
                                    }
                                }
                            }
                        }

                        if (tom.MBUDGET.Any(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)))
                        {
                            foreach (var x in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE)).Select(a => a.POSTE).Distinct().ToList())
                            {
                                if (!autrePCOP.Contains(x) && !String.IsNullOrEmpty(x))
                                    autrePCOP.Add(x);
                            }

                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeT = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADT = 0;
                            //TOTAL Liquidation validé SET//
                            decimal MTNENGAT = 0;
                            //TOTAL Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                            decimal MTNPAYET = 0;
                            //TOTAL % sur PAD (% Solde sur PAD)
                            decimal PADT = 0;
                            //TOTAL % sur PTBA (% Solde sur PTBA)
                            decimal PTBAT = 0;
                            //TOTAL PTBA
                            decimal MTNTOTALPeriodeTP = 0;
                            //TOTAL PAD
                            decimal MTNTOTALPADTP = 0;
                            //SITE
                            string SITE = "";

                            foreach (var x in autrePCOP.OrderBy(x => x).Distinct().ToList())
                            {
                                //PTBA
                                decimal MTNTOTALPeriode = 0;
                                //PCOP et INTITULE PCOP//
                                var PCOP = x;
                                var PCOPINTITUL = !String.IsNullOrEmpty(x) ? tom.RPOST1.FirstOrDefault(a => a.CODE == x).LIBELLE : "";

                                //PAD
                                decimal MTNTOTALPAD = 0;
                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                decimal MTNENGA = 0;
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                decimal MTNPAYE = 0;

                                foreach (var s in tom.MBUDGET.Where(a => a.NUMBUD == numbud && listAnnee.Contains(a.ANNEE) && a.POSTE == x).ToList())
                                {
                                    //PAD
                                    if (MTNTOTALPAD == 0) if (tom.RREPPOSTE.Any(a => a.CODE == s.POSTE)) MTNTOTALPAD = tom.RREPPOSTE.FirstOrDefault(a => a.CODE == s.POSTE).MONTREP1.Value;
                                    //PTBA
                                    foreach (var y in tom.MBUDALLOC.Where(a => a.NUMBUD == numbud && a.NUMENREG == s.NUMENREG && (a.MOIS >= DateDebut && a.MOIS <= DateFin) && listAnnee.Contains(a.ANNEE)).ToList())
                                    {
                                        MTNTOTALPeriode += y.MONTANT.Value;
                                    }
                                    //SITE
                                    if (String.IsNullOrEmpty(SITE))
                                        SITE = s.SITE;
                                }

                                //Montant engagé (Liquidation + Justif validé SET//
                                //Liquidation validé SET//
                                foreach (var z in db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                                        {
                                            foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.POSTE == PCOP).ToList())
                                            {
                                                MTNENGA += yy.MONTANTLOCAL.Value;
                                            }
                                        }
                                    }
                                }
                                //Justif validé SET//
                                foreach (var z in db.SI_TRAITJUSTIF.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList())
                                {
                                    foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                    {
                                        if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                                        {
                                            foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.POSTE == PCOP).ToList())
                                            {
                                                MTNENGA += xx.MONTANT.Value;
                                            }
                                        }
                                    }
                                }
                                //Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)//
                                var ListTraitement = db.SI_TRAITPROJET.Where(a => a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 1 && a.IDPROJET == crpt).ToList();
                                foreach (var opaV in db.OPA_VALIDATIONS.Where(a => a.ETAT == 2 && a.dateOrdre >= DateDebut && a.dateOrdre <= DateFin && a.NUMEROLIQUIDATION != null))
                                {
                                    if (opaV.AVANCE == false)
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == z.No).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                                                {
                                                    foreach (var yy in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == xx.ID && a.POSTE == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var z in ListTraitement.Where(a => a.REF == opaV.NUMEROLIQUIDATION).ToList())
                                        {
                                            foreach (var xx in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == z.No.ToString()).ToList())
                                            {
                                                if (db.SI_PCOP.Any(a => a.ID == isParam.PCOP) && db.SI_PCOP.FirstOrDefault(a => a.ID == isParam.PCOP).PCOP == "POSTE")
                                                {
                                                    foreach (var yy in tom.GA_AVANCE_MOUVEMENT.Where(a => a.IDENTIFIANT == xx.NUMERO_AVANCE_MOUVEMENT && a.POSTE == PCOP).ToList())
                                                    {
                                                        MTNPAYE += opaV.MONTANT.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                if (MTNTOTALPAD != 0 || MTNTOTALPeriode != 0 || MTNENGA != 0 || MTNPAYE != 0)
                                {
                                    list.Add(new TxLISTETRAIT
                                    {
                                        //No = "",//ID
                                        SOA = soa != null ? soa.SOA : "",//SOA
                                        PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//PROJET

                                        REF = PCOP,//PCOP
                                        INTITUT = PCOPINTITUL,//Intitulé PCOP

                                        SITE = SITE,

                                        BENEF = Math.Round(MTNTOTALPAD, 2).ToString(),//PAD
                                        MONTENGAGEMENT = Math.Round(MTNTOTALPeriode, 2).ToString(),//Montant PTBA
                                        MONTPAIE = Math.Round(MTNENGA, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                        TYPE = Math.Round(MTNPAYE, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                        NPIECE = Math.Round((MTNTOTALPAD - MTNENGA), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        AGENTREJETE = MTNTOTALPAD != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        MOTIF = Math.Round((MTNTOTALPeriode - MTNENGA), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        COMMENTAIRE = MTNTOTALPeriode != 0 ? Math.Round(((MTNENGA * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        SoldPadPayé = Math.Round((MTNTOTALPAD - MTNPAYE), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                        SoldPadPayéP = MTNTOTALPAD != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPAD), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                        SoldPtbaPayé = Math.Round((MTNTOTALPeriode - MTNPAYE), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                        SoldPtbaPayéP = MTNTOTALPeriode != 0 ? Math.Round(((MTNPAYE * 100) / MTNTOTALPeriode), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                        PayeEngage = MTNPAYE != 0 && MTNENGA != 0 ? Math.Round(((MTNPAYE * 100) / MTNENGA), 2).ToString() : "0",//% sur payé et engagé
                                    });

                                    //TOTAL
                                    MTNTOTALPeriodeT += MTNTOTALPeriode;
                                    MTNTOTALPADT += MTNTOTALPAD;
                                    MTNENGAT += MTNENGA;
                                    MTNPAYET += MTNPAYE;
                                }
                            }

                            list.Add(new TxLISTETRAIT
                            {
                                //No = "",//ID
                                SOA = "",//SOA
                                PROJET = "",//PROJET

                                SITE = "",

                                REF = "",//PCOP
                                INTITUT = "TOTAL",//Intitulé PCOP

                                BENEF = Math.Round(MTNTOTALPADT, 2).ToString(),//PAD
                                MONTENGAGEMENT = Math.Round(MTNTOTALPeriodeT, 2).ToString(),//Montant PTBA
                                MONTPAIE = Math.Round(MTNENGAT, 2).ToString(),//Montant engagé (Liquidation + Justif validé SET//
                                TYPE = Math.Round(MTNPAYET, 2).ToString(),//Montant payé (OPA_VALIDATIONS (MONTANT, ETAT = 2, dateOrdre)
                                NPIECE = Math.Round((MTNTOTALPADT - MTNENGAT), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                AGENTREJETE = MTNTOTALPADT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                MOTIF = Math.Round((MTNTOTALPeriodeT - MTNENGAT), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                COMMENTAIRE = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNENGAT * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                SoldPadPayé = Math.Round((MTNTOTALPADT - MTNPAYET), 2).ToString(),//Solde sur PAD (PAD - Montant engagé)
                                SoldPadPayéP = MTNTOTALPADT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPADT), 2).ToString() : "0",//% sur PAD (% Solde sur PAD)
                                SoldPtbaPayé = Math.Round((MTNTOTALPeriodeT - MTNPAYET), 2).ToString(),//Solde sur PTBA (Montant PTBA - Montant engagé)
                                SoldPtbaPayéP = MTNTOTALPeriodeT != 0 ? Math.Round(((MTNPAYET * 100) / MTNTOTALPeriodeT), 2).ToString() : "0",//% sur PTBA (% Solde sur PTBA)

                                PayeEngage = MTNPAYET != 0 && MTNENGAT != 0 ? Math.Round(((MTNPAYET * 100) / MTNENGAT), 2).ToString() : "0",//% sur payé et engagé
                            });
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.OrderByDescending(a => a.DATENGAGEMENT).ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<ActionResult> DetailsBUDGET(SI_USERS suser, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                //int crpt = exist.IDPROJET.Value;
                int crpt = iProjet;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                var user = tom.RBUDGET.Select(a => new
                {
                    BUDGET = a.LIBELLE,
                    ID = a.NUMBUD
                }).ToList();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user } }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //GET ALL Période//
        [HttpPost]
        public ActionResult GetAllPeriode(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var user = db.SI_RSFPERIOD.Select(a => new
            {
                PERIODE = a.PERIODE
            }).OrderBy(a => a.PERIODE).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
        }

        //GET ALL Years//
        [HttpPost]
        public ActionResult GetAllYears(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var user = db.SI_RSF.Select(a => new
            {
                ANNEE = a.ANNEE
            }).Distinct().OrderBy(a => a.ANNEE).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
        }

        //GET ALL Type//
        [HttpPost]
        public ActionResult GetAllType(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var user = db.SI_RSFTYPE.Select(a => new
            {
                TYPE = a.TYPE
            }).OrderBy(a => a.TYPE).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
        }

        //RSF//
        public ActionResult RSF()
        {
            ViewBag.Controller = "Liste des RSF";

            return View();
        }

        //Genere Liste RSF//
        [HttpPost]
        public JsonResult GenereRSF(SI_USERS suser, string listProjet, string Annee/*, string Mois*/, string Periode, string Type)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var lienGEd = db.SI_GEDLIEN.FirstOrDefault();

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

                        if (db.SI_RSF.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                        {
                            foreach (var x in db.SI_RSF.Where(a => a.IDPROJET == crpt && a.DELETIONDATE == null).OrderBy(a => a.ANNEE).OrderBy(a => a.MOIS).OrderBy(a => a.PERIODE).OrderBy(a => a.TYPE).OrderBy(a => a.CREATIONDATE).ToList())
                            {
                                var soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault();

                                list.Add(new TxLISTETRAIT
                                {
                                    IDRSF = x.ID,
                                    SOA = soa != null ? soa.SOA : "",
                                    PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    TYPE = x.TITLE,
                                    REF = x.TYPE,
                                    NPIECE = x.ANNEE.ToString(),
                                    BENEF = x.MOIS,
                                    INTITUT = lienGEd + "/documents/shared/" + x.LIEN.ToString(),
                                    COMMENTAIRE = x.PERIODE
                                });
                            }
                        }
                    }

                    if (Annee.ToString() != "Tous")
                    {
                        list = list.Where(a => a.NPIECE == Annee.ToString()).ToList();
                    }
                    //if (Mois != "Tous")
                    //{
                    //    list = list.Where(a => a.BENEF == Mois).ToList();
                    //}
                    if (Periode.ToString() != "Tous")
                    {
                        list = list.Where(a => a.COMMENTAIRE == Periode).ToList();
                    }
                    if (Type.ToString() != "Tous")
                    {
                        list = list.Where(a => a.REF == Type).ToList();
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
