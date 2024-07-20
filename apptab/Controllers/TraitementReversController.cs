using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using Extensions.DateTime;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class TraitementReversController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTOM tom = new SOFTCONNECTOM();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //Traitement justificatifs PROJET//
        public ActionResult TraitementPROJET()
        {
            ViewBag.Controller = "Tris et validation des reversements";

            return View();
        }

        //GET ALL PROJET//
        [HttpPost]
        public ActionResult GetAllPROJET(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();
                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var user = db.SI_PROJETS.Select(a => new
                    {
                        PROJET = a.PROJET,
                        ID = a.ID,
                        DELETIONDATE = a.DELETIONDATE,
                    }).Where(a => a.DELETIONDATE == null).ToList();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
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

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
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

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<JsonResult> GenerationLOAD(SI_USERS suser, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                int retarDate = 0;
                if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELARAF.Value;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO. " }, settings));

                if (tom.CPTADMIN_FAVANCE.Any(a => site.Contains(a.SITE)
))
                {
                    foreach (var x in tom.CPTADMIN_FAVANCE.Where(a => site.Contains(a.SITE)).OrderBy(a => a.DATEAVANCE).ToList())
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
                        if (Math.Round(MTN, 3) == Math.Round(MTNPJ, 3))
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
                                if (tom.GA_AVANCE_REVERSEMENT.Any(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && site.Contains(a.SITE)))
                                {
                                    foreach (var y in tom.GA_AVANCE_REVERSEMENT.Where(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && site.Contains(a.SITE)).OrderBy(a => a.DATE).OrderBy(a => a.NUMERO_PIECE).ToList())
                                    {
                                        Guid idJustif = Guid.Parse(y.ID);
                                        if (!db.SI_TRAITREVERS.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.IDPROJET == crpt && site.Contains(a.SITE)) || db.SI_TRAITREVERS.Any(a => a.No == idJustif && a.ETAT == 2 && a.IDPROJET == crpt && site.Contains(a.SITE)))
                                        {
                                            var titulaire = "";

                                            var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE));

                                            if (isGA != null)
                                            {
                                                if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE)).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE)).AUXI != null)
                                                {
                                                    if (tom.RTIERS.Any(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI))
                                                        titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI).NOM;
                                                }
                                            }

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

                                            bool isLate = false;
                                            DateTime DD = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT.Value.Date;
                                            if (DD.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                                isLate = true;

                                            list.Add(new DATATRPROJET
                                            {
                                                No = idJustif,
                                                REF = y.NUMERO_AVANCE,
                                                NPIECE = y.NUMERO_PIECE,
                                                OBJ = isGA != null ? isGA.LIBELLE : "",
                                                TITUL = titulaire,
                                                MONT = Math.Round(y.MONTANT.Value, 2).ToString(),
                                                COMPTE = isGA != null ? isGA.COGE : "",
                                                DATE = y.DATE.Value.Date,
                                                PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)).POSTE : "",
                                                DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                SOA = soa,
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                                isLATE = isLate,
                                                SITE = y.SITE
                                            });
                                        }
                                    }
                                }
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

        [HttpPost]
        public async Task<JsonResult> Generation(SI_USERS suser, DateTime DateDebut, DateTime DateFin, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }


                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                int retarDate = 0;
                if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELARAF.Value;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO. " }, settings));

                if (tom.CPTADMIN_FAVANCE.Any(a => site.Contains(a.SITE)))
                {
                    foreach (var x in tom.CPTADMIN_FAVANCE.Where(a => site.Contains(a.SITE)).OrderBy(a => a.DATEAVANCE).ToList())
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
                        if (Math.Round(MTN, 3) == Math.Round(MTNPJ, 3))
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
                                if (tom.GA_AVANCE_REVERSEMENT.Any(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && a.DATE >= DateDebut && a.DATE <= DateFin && site.Contains(a.SITE)))
                                {
                                    foreach (var y in tom.GA_AVANCE_REVERSEMENT.Where(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && a.DATE >= DateDebut && a.DATE <= DateFin && site.Contains(a.SITE)).OrderBy(a => a.DATE).OrderBy(a => a.NUMERO_PIECE).ToList())
                                    {
                                        Guid idJustif = Guid.Parse(y.ID);
                                        if (!db.SI_TRAITREVERS.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.IDPROJET == crpt && site.Contains(a.SITE)) || db.SI_TRAITREVERS.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.ETAT == 2 && a.IDPROJET == crpt && site.Contains(a.SITE)))
                                        {
                                            var titulaire = "";

                                            var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE));

                                            if (isGA != null)
                                            {
                                                if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE)).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE && site.Contains(a.SITE)).AUXI != null)
                                                {
                                                    if (tom.RTIERS.Any(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI))
                                                        titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI).NOM;
                                                }
                                            }

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

                                            bool isLate = false;
                                            DateTime DD = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT.Value.Date;
                                            if (DD.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                                isLate = true;

                                            list.Add(new DATATRPROJET
                                            {
                                                No = idJustif,
                                                REF = y.NUMERO_AVANCE,
                                                NPIECE = y.NUMERO_PIECE,
                                                OBJ = isGA != null ? isGA.LIBELLE : "",
                                                TITUL = titulaire,
                                                MONT = Math.Round(y.MONTANT.Value, 2).ToString(),
                                                COMPTE = isGA != null ? isGA.COGE : "",
                                                DATE = y.DATE.Value.Date,
                                                PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)).POSTE : "",
                                                DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                                                SOA = soa,
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                                                isLATE = isLate,
                                                SITE = y.SITE
                                            });
                                        }
                                    }
                                }
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

        [HttpPost]
        public JsonResult GetCheckedEcritureF(SI_USERS suser, string listCompte, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int countTraitement = 0;
            int crpt = iProjet;

            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;

            //SEND MAIL ALERT et NOTIFICATION//
            string MailAdresse = "";
            string mdpMail = "";

            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD != null)
            {
                MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL;
                mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD;
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
            }

            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET;

            int ordsec = int.Parse(Session["PROCESDEPS"].ToString());

            var listCompteS = listCompte.Split(',');
            foreach (var SAV in listCompteS)
            {
                try
                {
                    SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                    SOFTCONNECTOM tom = new SOFTCONNECTOM();
                    var FSauv = new SI_TRAITAVANCE();

                    List<DATATRPROJET> list = new List<DATATRPROJET>();

                    Guid elem = Guid.Parse(SAV);
                    var isPiece = tom.GA_AVANCE_REVERSEMENT.FirstOrDefault(a => a.ID == SAV && site.Contains(a.SITE));
                    if (db.SI_TRAITREVERS.FirstOrDefault(a => a.No == elem && a.NPIECE == isPiece.NUMERO_PIECE && a.ETAT == 2 && a.IDPROJET == crpt && site.Contains(a.SITE)) != null)
                    {
                        var ismod = db.SI_TRAITREVERS.FirstOrDefault(a => a.No == elem && a.NPIECE == isPiece.NUMERO_PIECE && a.IDPROJET == crpt && site.Contains(a.SITE));
                        ismod.ETAT = 0;
                        ismod.DATECRE = DateTime.Now;
                        ismod.DATEANNUL = null;
                        ismod.IDUSERANNUL = null;

                        //SANS ORDSEC//
                        if (ordsec == 1)
                        {
                            ismod.ETAT = 1;
                            ismod.DATEVALIDATION = DateTime.Now;
                            ismod.IDUSERVALIDATE = exist.ID;
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        var titulaire = "";
                        var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE && site.Contains(a.SITE));
                        if (isGA != null)
                        {
                            if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE && site.Contains(a.SITE)).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE && site.Contains(a.SITE)).AUXI != null)
                            {
                                if (tom.RTIERS.Any(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI))
                                    titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI).NOM;
                            }
                        }

                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                        var newT = new SI_TRAITREVERS()
                        {
                            IDPROJET = crpt,
                            No = elem,
                            REF = isPiece.NUMERO_AVANCE,
                            NPIECE = isPiece.NUMERO_PIECE,
                            OBJ = isGA != null ? isGA.LIBELLE : "",
                            TITUL = titulaire,
                            MONT = Data.Cipher.Encrypt((Math.Round(isPiece.MONTANT.Value, 2)).ToString(), "Oppenheimer"),
                            COMPTE = isGA != null ? isGA.COGE : "",
                            DATEMANDAT = isPiece.DATE.Value.Date,
                            PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == isPiece.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == isPiece.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)).POSTE : "",
                            DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                            DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                            DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA && site.Contains(a.CODE_SITE)).DATETRAITEMENT,
                            DATECRE = DateTime.Now,
                            ETAT = 0,
                            IDUSERCREATE = exist.ID,
                            SITE = isGA.SITE
                        };

                        if (ordsec == 1)
                        {
                            newT.ETAT = 1;
                            newT.DATEVALIDATION = DateTime.Now;
                            newT.IDUSERVALIDATE = exist.ID;
                        }

                        db.SI_TRAITREVERS.Add(newT);
                        db.SaveChanges();
                    }

                    countTraitement++;
                }
                catch (Exception e)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
                }
            }

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILJ2 != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILJ2;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }

                mail.Subject = "Attente validation reversements du projet " + ProjetIntitule;
                mail.IsBodyHtml = true;

                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " reversements en attente de validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                if (ordsec == 1)
                {
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " reversements validées pour le compte du projet " + ProjetIntitule + " et en attente de transfert vers SIIGFP.<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                }

                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                smtp.EnableSsl = true;

                try { smtp.Send(mail); }
                catch (Exception) { }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = "" }, settings));
        }

        [HttpPost]
        public async Task<JsonResult> ModalD(SI_USERS suser, string IdF, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.GA_AVANCE_REVERSEMENT.FirstOrDefault(a => a.ID == IdF && site.Contains(a.SITE)) != null)
                {
                    foreach (var x in tom.GA_AVANCE_REVERSEMENT.Where(a => a.ID == IdF && site.Contains(a.SITE)).ToList())
                    {
                        var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == x.NUMERO_AVANCE && site.Contains(a.SITE));

                        list.Add(new DATATRPROJET
                        {
                            REF = isGA != null ? isGA.LIBELLE : "",
                            OBJ = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)).COGE : "",
                            TITUL = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT && site.Contains(a.SITE)).POSTE : "",
                            MONT = x.MONTANT != null ? Math.Round(x.MONTANT.Value, 2).ToString() : "0",
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
        public async Task<JsonResult> ModalLIAS(SI_USERS suser, string IdF, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                //List<DATATRPROJET> list = new List<DATATRPROJET>();
                var newElemH = new DATATRPROJET()
                {
                    REF = "",
                    OBJ = "",
                    TITUL = ""
                };

                var isJustif = tom.GA_AVANCE_REVERSEMENT.FirstOrDefault(a => a.ID == IdF && site.Contains(a.SITE));
                string isAv = tom.CPTADMIN_FAVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isJustif.NUMERO_AVANCE && site.Contains(a.SITE)).ID.ToString();

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && (a.TYPEPIECE == "DEF" || a.TYPEPIECE == "TEF" || a.TYPEPIECE == "BE") && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)) != null)
                {
                    var def = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "DEF" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)) != null)
                        def = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "DEF" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)).LIEN;
                    var tef = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "TEF" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)) != null)
                        tef = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "TEF" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)).LIEN;
                    var be = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "BE" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)) != null)
                        be = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "BE" && a.MODULLE == "CPTADMINAVANCE" && site.Contains(a.CODE_SITE)).LIEN;

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

        [HttpPost]
        public async Task<JsonResult> ModalF(SI_USERS suser, string IdF, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.GA_AVANCE_REVERSEMENT.Any(a => a.ID == IdF))
                {
                    list.Add(new DATATRPROJET
                    {
                        LIEN = tom.GA_AVANCE_REVERSEMENT.FirstOrDefault(a => a.ID == IdF && site.Contains(a.SITE)).COMMENTAIRE != null ? tom.GA_AVANCE_REVERSEMENT.FirstOrDefault(a => a.ID == IdF && site.Contains(a.SITE)).COMMENTAIRE : ""
                    });
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult GetIsMotif()
        {
            try
            {
                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (db.SI_MOTIF.Any())
                {
                    string[] separators = { "," };

                    var Tomail = db.SI_MOTIF.FirstOrDefault().MOTIFTRAIT;
                    if (Tomail != null)
                    {
                        string listUser = Tomail.ToString();
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            list.Add(new DATATRPROJET
                            {
                                REF = mailto
                            });
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

        [HttpPost]
        public JsonResult AnnulationJustif(SI_USERS suser, Guid IdF, string Comm, string Motif, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == IdS).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;

                //SEND MAIL ALERT et NOTIFICATION//
                string MailAdresse = "";
                string mdpMail = "";

                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD != null)
                {
                    MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL;
                    mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD;
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
                }

                var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == IdS && a.DELETIONDATE == null).FirstOrDefault().PROJET;

                if (db.SI_TRAITREVERS.FirstOrDefault(a => a.No == IdF && a.IDPROJET == IdS && site.Contains(a.SITE)) != null)
                {
                    var ismod = db.SI_TRAITREVERS.FirstOrDefault(a => a.No == IdF && a.IDPROJET == IdS && site.Contains(a.SITE));
                    ismod.ETAT = 2;
                    //ismod.DATECRE = DateTime.Now;
                    ismod.DATEANNUL = DateTime.Now;
                    ismod.IDUSERANNUL = exist.ID;

                    db.SaveChanges();
                }

                var newElemH = new SI_TRAITANNULREVERS()
                {
                    No = IdF,
                    DATEANNUL = DateTime.Now,
                    MOTIF = Motif,
                    COMMENTAIRE = Comm,
                    IDPROJET = IdS,
                    IDUSER = exist.ID
                };
                db.SI_TRAITANNULREVERS.Add(newElemH);
                db.SaveChanges();

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                    smtp.UseDefaultCredentials = true;

                    mail.From = new MailAddress(MailAdresse);

                    mail.To.Add(MailAdresse);
                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILREJETREV != null)
                    {
                        string[] separators = { ";" };

                        var Tomail = mail;
                        if (Tomail != null)
                        {
                            string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILREJETREV;
                            string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var mailto in mailListe)
                            {
                                mail.To.Add(mailto);
                            }
                        }
                    }

                    mail.Subject = "Rejet reversement du projet " + ProjetIntitule;
                    mail.IsBodyHtml = true;
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez un reversement rejeté pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                    smtp.EnableSsl = true;

                    try { smtp.Send(mail); }
                    catch (Exception) { }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Rejet avec succès. ", data = Comm }, settings));
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Traitement justificatifs ORDSEC//
        public ActionResult TraitementORDSEC()
        {
            ViewBag.Controller = "Validation des reversements";

            return View();
        }

        //GENERATION SIIGLOAD//
        [HttpPost]
        public JsonResult GenerationSIIGLOAD(SI_USERS suser, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                int retarDate = 0;
                if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELAV.Value;

                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO. " }, settings));

                if (db.SI_TRAITREVERS.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 0 && site.Contains(a.SITE)) != null)
                {
                    foreach (var x in db.SI_TRAITREVERS.Where(a => a.IDPROJET == crpt && a.ETAT == 0 && site.Contains(a.SITE)).OrderBy(a => a.DATECRE).OrderBy(a => a.DATEMANDAT).ToList())
                    {
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

                        bool isLate = false;
                        if (x.DATECRE.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                            isLate = true;

                        list.Add(new DATATRPROJET
                        {
                            No = x.No,
                            REF = x.REF,
                            NPIECE = x.NPIECE,
                            OBJ = x.OBJ,
                            TITUL = x.TITUL,
                            MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                            COMPTE = x.COMPTE,
                            DATE = x.DATEMANDAT.Value.Date,
                            PCOP = x.PCOP,
                            DATEDEF = x.DATEDEF.Value.Date,
                            DATETEF = x.DATETEF.Value.Date,
                            DATEBE = x.DATEBE.Value.Date,
                            LIEN = db.SI_USERS.FirstOrDefault(a => a.ID == x.IDUSERCREATE).LOGIN,
                            DATECREATION = x.DATECRE.Value.Date,
                            SOA = soa,
                            PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                            isLATE = isLate,
                            SITE = x.SITE
                        });
                    }
                    //listORDER = list.OrderByDescending(a => a.isLATE).ToList();
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult GenerationSIIG(SI_USERS suser, DateTime DateDebut, DateTime DateFin, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = iProjet;

                List<string> site = new List<string>();
                var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
                if (siteS == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
                foreach (var item in siteS.Split(','))
                {
                    site.Add(item);
                }

                int retarDate = 0;
                if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELAV.Value;

                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 1 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEFA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 2 n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BEA) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état du STATUT 3 n'est pas paramétré sur TOM²PRO. " }, settings));

                if (db.SI_TRAITREVERS.Any(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 0 && site.Contains(a.SITE)))
                {
                    foreach (var x in db.SI_TRAITREVERS.Where(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 0 && site.Contains(a.SITE)).OrderBy(a => a.DATECRE).OrderBy(a => a.DATEMANDAT).ToList())
                    {
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

                        bool isLate = false;
                        if (x.DATECRE.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                            isLate = true;

                        list.Add(new DATATRPROJET
                        {
                            No = x.No,
                            REF = x.REF,
                            NPIECE = x.NPIECE,
                            OBJ = x.OBJ,
                            TITUL = x.TITUL,
                            MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                            COMPTE = x.COMPTE,
                            DATE = x.DATEMANDAT.Value.Date,
                            PCOP = x.PCOP,
                            DATEDEF = x.DATEDEF.Value.Date,
                            DATETEF = x.DATETEF.Value.Date,
                            DATEBE = x.DATEBE.Value.Date,
                            LIEN = db.SI_USERS.FirstOrDefault(a => a.ID == x.IDUSERCREATE).LOGIN,
                            DATECREATION = x.DATECRE.Value.Date,
                            SOA = soa,
                            PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET,
                            isLATE = isLate,
                            SITE = x.SITE
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.OrderByDescending(a => a.isLATE).ToList() }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult GetCheckedEcritureORDSEC(SI_USERS suser, string listCompte, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int countTraitement = 0;
            int crpt = iProjet;

            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == crpt).Select(x => x.SITE).FirstOrDefault();
            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;

            //SEND MAIL ALERT et NOTIFICATION//
            string MailAdresse = "";
            string mdpMail = "";

            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD != null)
            {
                MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDMAIL;
                mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).SENDPWD;
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
            }

            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET;

            var listCompteS = listCompte.Split(',');
            foreach (var SAV in listCompteS)
            {
                try
                {
                    SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                    SOFTCONNECTOM tom = new SOFTCONNECTOM();

                    List<DATATRPROJET> list = new List<DATATRPROJET>();

                    Guid isSAV = Guid.Parse(SAV);
                    if (db.SI_TRAITREVERS.FirstOrDefault(a => a.IDPROJET == crpt && a.No == isSAV && site.Contains(a.SITE)) != null)
                    {
                        var isModified = db.SI_TRAITREVERS.FirstOrDefault(a => a.IDPROJET == crpt && a.No == isSAV && site.Contains(a.SITE));
                        isModified.ETAT = 1;
                        isModified.DATEVALIDATION = DateTime.Now;
                        isModified.DATEANNUL = null;
                        isModified.IDUSERANNUL = null;
                        isModified.IDUSERVALIDATE = exist.ID;
                        db.SaveChanges();

                        countTraitement++;
                    }
                }
                catch (Exception e)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
                }
            }

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILJ3 != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && site.Contains(a.SITE)).MAILJ3;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }

                mail.Subject = "Validation reversements du projet " + ProjetIntitule;
                mail.IsBodyHtml = true;
                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " reversements validés pour le compte du projet " + ProjetIntitule + " .<br/><br>" +
                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                smtp.EnableSsl = true;

                try { smtp.Send(mail); }
                catch (Exception) { }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = "" }, settings));
        }
    }
}
