using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using apptab.Data;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Extensions.DateTime;
using Newtonsoft.Json;
using static apptab.Controllers.RSFController;

namespace apptab.Controllers
{
    public class AnomalieController : Controller
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
            var site = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == exist.IDPROJET).Select(x => x.SITE).ToList();

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
            var site = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == exist.IDPROJET).Select(x => x.SITE).ToList();

            try
            {
                int crpt = db.SI_PROJETS.FirstOrDefault(a => a.PROJET == projet && a.DELETIONDATE == null).ID;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)) != null)
                {
                    foreach (var x in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE != "DEF" && a.TYPEPIECE != "TEF" && a.TYPEPIECE != "BE" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)).OrderBy(a => a.RANG).ToList())
                    {
                        var idFGuid = Guid.Parse(IdF);
                        DateTime dpj = tom.CPTADMIN_FLIQUIDATION.Where(a => a.ID == idFGuid && site.Contains(a.SITE)).FirstOrDefault().DATELIQUIDATION.Value;
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
            var site = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == exist.IDPROJET).Select(x => x.SITE).ToList();

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
            var site = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == exist.IDPROJET).Select(x => x.SITE).ToList();

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

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && (a.TYPEPIECE == "DEF" || a.TYPEPIECE == "TEF" || a.TYPEPIECE == "BE") && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)) != null)
                {
                    var def = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)) != null)
                        def = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "DEF" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)).LIEN;
                    var tef = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)) != null)
                        tef = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "TEF" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)).LIEN;
                    var be = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)) != null)
                        be = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == IdF && a.TYPEPIECE == "BE" && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)).LIEN;

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

        //Anomalie//
        public ActionResult Anomalie()
        {
            ViewBag.Controller = "Anomalie générale des dépenses à payer et avances";

            return View();
        }

        //Genere liste des engagements et paiements//
        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, string listSite)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

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
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                        if (tom.CPTADMIN_FLIQUIDATION.Any(a => a.DATELIQUIDATION >= DateDebut && a.DATELIQUIDATION <= DateFin && site.Contains(a.SITE)))
                        {
                            foreach (var x in tom.CPTADMIN_FLIQUIDATION.Where(a => a.DATELIQUIDATION >= DateDebut && a.DATELIQUIDATION <= DateFin && site.Contains(a.SITE)).OrderBy(a => a.DATELIQUIDATION).ToList())
                            {
                                decimal MTN = 0;
                                decimal MTNPJ = 0;
                                var PCOP = "";

                                var COGEBENEFICIAIRE = "";
                                var AUXIBENEFICIAIRE = "";

                                //Get total MTN dans CPTADMIN_MLIQUIDATION pour vérification du SOMMES MTN M = SOMMES MTN MPJ//
                                if (tom.CPTADMIN_MLIQUIDATION.Any(a => a.IDLIQUIDATION == x.ID))
                                {
                                    foreach (var y in tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == x.ID).ToList())
                                    {
                                        MTN += y.MONTANTLOCAL.Value;

                                        if (String.IsNullOrEmpty(PCOP))
                                            PCOP = y.POSTE;

                                        if (String.IsNullOrEmpty(COGEBENEFICIAIRE))
                                            COGEBENEFICIAIRE = y.COGEFRNS;

                                        if (String.IsNullOrEmpty(AUXIBENEFICIAIRE))
                                            AUXIBENEFICIAIRE = y.AUXIFRNS;
                                    }
                                }

                                //TEST SI SOMMES MTN M = SOMMES MTN MPJ//
                                var IDString = x.ID.ToString();
                                if (tom.TP_MPIECES_JUSTIFICATIVES.Any(a => a.NUMERO_FICHE == IDString && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)))
                                {
                                    foreach (var y in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IDString && a.MODULLE == "LIQUIDATION" && site.Contains(a.CODE_SITE)).ToList())
                                    {
                                        MTNPJ += y.MONTANT.Value;
                                    }
                                }

                                //MathRound 3 satria kely kokoa ny marge d'erreur no le 2//
                                if (Math.Round(MTN, 3) != Math.Round(MTNPJ, 3))
                                {
                                    //Check si F a déjà passé les 3 étapes (DEF, TEF et BE) pour avoir les dates => BE étape finale//
                                    var canBe = true;
                                    if (tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.DEF && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;
                                    if (tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.TEF && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;
                                    if (tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.BE && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;

                                    //TEST que F n'est pas encore traité ou F a été annulé// ETAT annulé = 2//
                                    if (canBe)
                                    {
                                        if (!db.SI_TRAITPROJET.Any(a => a.No == x.ID && a.IDPROJET == crpt && site.Contains(a.SITE)) || db.SI_TRAITPROJET.Any(a => a.No == x.ID && a.ETAT == 2 && a.IDPROJET == crpt && site.Contains(a.SITE)))
                                        {
                                            var titulaire = "";
                                            if (tom.RTIERS.Any(a => a.COGE == COGEBENEFICIAIRE && a.AUXI == AUXIBENEFICIAIRE))
                                                titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == COGEBENEFICIAIRE && a.AUXI == AUXIBENEFICIAIRE).NOM;

                                            var soa = (from soas in db.SI_SOAS
                                                       join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                       where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                       select new
                                                       {
                                                           soas.SOA
                                                       }).FirstOrDefault() != null ? (from soas in db.SI_SOAS
                                                                                      join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                                                      where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                                                      select new
                                                                                      {
                                                                                          soas.SOA
                                                                                      }).FirstOrDefault().SOA : "MULTIPLE";

                                            list.Add(new TxLISTETRAIT
                                            {
                                                No = x.ID,
                                                SOA = soa,//SOA
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//Projet
                                                TYPE = "Dépenses à payer",//Type
                                                SITE = x.SITE,//Site
                                                REF = x.NUMEROCA,//Référence
                                                OBJ = x.DESCRIPTION,//Objet
                                                TITUL = titulaire,//Titulaire
                                                DATENGAGEMENT = x.DATELIQUIDATION.Value.Date,//Date mandat
                                                COMPTE = COGEBENEFICIAIRE,//Compte bénéficiaire
                                                PCOP = PCOP,//PCOP

                                                MONT = Math.Round(MTN, 2).ToString(),//Montant imputations
                                                MONTi = Math.Round(MTN, 2).ToString(),//Montant pièces justificatives

                                                DATEDEF = tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.DEF && site.Contains(a.CODE_SITE)).DATECA,//Date STATUT 1
                                                DATETEF = tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.TEF && site.Contains(a.CODE_SITE)).DATECA,//Date STATUT 2
                                                DATEBE = tom.CPTADMIN_TRAITEMENT.FirstOrDefault(a => a.NUMEROCA == x.NUMEROCA && a.NUMCAETAPE == numCaEtapAPP.BE && site.Contains(a.CODE_SITE)).DATECA,//Date STATUT 3
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);
                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                        if (tom.CPTADMIN_FAVANCE.Any(a => a.DATEAVANCE >= DateDebut && a.DATEAVANCE <= DateFin && site.Contains(a.SITE)))
                        {
                            foreach (var x in tom.CPTADMIN_FAVANCE.Where(a => a.DATEAVANCE >= DateDebut && a.DATEAVANCE <= DateFin && site.Contains(a.SITE)).OrderBy(a => a.DATEAVANCE).ToList())
                            {
                                decimal MTN = 0;
                                decimal MTNPJ = 0;
                                var PCOP = "";

                                //Get total MTN dans CPTADMIN_MLIQUIDATION pour vérification du SOMMES MTN M = SOMMES MTN MPJ//
                                if (tom.CPTADMIN_MAVANCE.Any(a => a.IDAVANCE == x.ID))
                                {
                                    foreach (var y in tom.CPTADMIN_MAVANCE.Where(a => a.IDAVANCE == x.ID).ToList())
                                    {
                                        MTN += y.MONTANTLOCAL.Value;

                                        if (String.IsNullOrEmpty(PCOP))
                                            PCOP = y.POSTE;
                                    }
                                }

                                //TEST SI SOMMES MTN M = SOMMES MTN MPJ//
                                var IDString = x.ID.ToString();
                                if (tom.TP_MPIECES_JUSTIFICATIVES.Any(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)))
                                {
                                    foreach (var y in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)).ToList())
                                    {
                                        MTNPJ += y.MONTANT.Value;
                                    }
                                }

                                //MathRound 3 satria kely kokoa ny marge d'erreur no le 2//
                                if (Math.Round(MTN, 3) != Math.Round(MTNPJ, 3))
                                {
                                    //Check si F a déjà passé les 3 étapes (DEFA, TEFA et BEA) pour avoir les dates => BEA étape finale//
                                    var canBe = true;
                                    if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;
                                    if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;
                                    if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)) == null)
                                        canBe = false;

                                    //TEST que F n'est pas encore traité ou F a été annulé// ETAT annulé = 2//
                                    if (canBe)
                                    {
                                        if (!db.SI_TRAITAVANCE.Any(a => a.No == x.ID && a.IDPROJET == crpt && site.Contains(a.SITE)) || db.SI_TRAITAVANCE.Any(a => a.No == x.ID && a.ETAT == 2 && a.IDPROJET == crpt && site.Contains(a.SITE)))
                                        {
                                            var titulaire = "";
                                            if (tom.RTIERS.Any(a => a.COGE == x.COGEBENEFICIAIRE && a.AUXI == x.AUXIBENEFICIAIRE))
                                                titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == x.COGEBENEFICIAIRE && a.AUXI == x.AUXIBENEFICIAIRE).NOM;

                                            var soa = (from soas in db.SI_SOAS
                                                       join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                       where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                       select new
                                                       {
                                                           soas.SOA
                                                       }).FirstOrDefault() != null ? (from soas in db.SI_SOAS
                                                                                      join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                                                      where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                                                      select new
                                                                                      {
                                                                                          soas.SOA
                                                                                      }).FirstOrDefault().SOA : "MULTIPLE";

                                            list.Add(new TxLISTETRAIT
                                            {
                                                No = x.ID,
                                                SOA = soa,//SOA
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,//Projet
                                                TYPE = "Dépenses à payer",//Type
                                                SITE = x.SITE,//Site
                                                REF = x.NUMEROAVANCE,//Référence
                                                OBJ = x.DESCRIPTION,//Objet
                                                TITUL = titulaire,//Titulaire
                                                DATENGAGEMENT = x.DATEAVANCE.Value.Date,//Date mandat
                                                COMPTE = x.COGEBENEFICIAIRE,//Compte bénéficiaire
                                                PCOP = PCOP,//PCOP

                                                MONT = Math.Round(MTN, 2).ToString(),//Montant imputations
                                                MONTi = Math.Round(MTN, 2).ToString(),//Montant pièces justificatives

                                                DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,//Date STATUT 1
                                                DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,//Date STATUT 2
                                                DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,//Date STATUT 3
                                            });
                                        }
                                    }
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
    }
}
