using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class TraitementJustifController : Controller
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
            ViewBag.Controller = "Tris et validation des justificatifs";

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
                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                //int retarDate = 0;
                //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELRAF.Value;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));

                if (tom.CPTADMIN_FAVANCE.Any())
                {
                    foreach (var x in tom.CPTADMIN_FAVANCE.OrderBy(a => a.DATEAVANCE).ToList())
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
                        if (tom.TP_MPIECES_JUSTIFICATIVES.Any(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE"))
                        {
                            foreach (var y in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE").ToList())
                            {
                                MTNPJ += y.MONTANT.Value;
                            }
                        }

                        //MathRound 3 satria kely kokoa ny marge d'erreur no le 2//
                        if (Math.Round(MTN, 3) == Math.Round(MTNPJ, 3))
                        {
                            //Check si F a déjà passé les 3 étapes (DEF, TEF et BE) pour avoir les dates => BE étape finale//
                            var canBe = true;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.DEF) == null)
                                canBe = false;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.TEF) == null)
                                canBe = false;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BE) == null)
                                canBe = false;

                            //TEST que F n'est pas encore traité ou F a été annulé// ETAT annulé = 2//
                            if (canBe)
                            {
                                if (tom.GA_AVANCE_JUSTIFICATIF.Any(a => a.NUMERO_AVANCE == x.NUMEROAVANCE))
                                {
                                    foreach (var y in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.NUMERO_AVANCE == x.NUMEROAVANCE).OrderBy(a => a.DATE).OrderBy(a => a.NUMERO_PIECE).ToList())
                                    {
                                        Guid idJustif = Guid.Parse(y.ID);
                                        if (!db.SI_TRAITJUSTIF.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.IDPROJET == crpt) || db.SI_TRAITJUSTIF.Any(a => a.No == idJustif && a.ETAT == 2 && a.IDPROJET == crpt))
                                        {
                                            var titulaire = "";

                                            var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE);

                                            if (isGA != null)
                                            {
                                                if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE).AUXI != null)
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
                                                       });

                                            //bool isLate = false;
                                            //DateTime DD = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BE).DATETRAITEMENT.Value.Date;
                                            //if (DD.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                            //    isLate = true;

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
                                                PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT).POSTE : "",
                                                DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEF).DATETRAITEMENT,
                                                DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEF).DATETRAITEMENT,
                                                DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BE).DATETRAITEMENT,
                                                SOA = soa.FirstOrDefault().SOA,
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
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
                //Check si le projet est mappé à une base de données TOM²PRO//
                if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

                //int retarDate = 0;
                //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELRAF.Value;

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                //Check si la correspondance des états est OK//
                var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (numCaEtapAPP == null) return Json(JsonConvert.SerializeObject(new { type = "PEtat", msg = "Veuillez paramétrer la correspondance des états. " }, settings));
                //TEST si les états dans les paramètres dans cohérents avec ceux de TOM²PRO//
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));

                if (tom.CPTADMIN_FAVANCE.Any())
                {
                    foreach (var x in tom.CPTADMIN_FAVANCE.OrderBy(a => a.DATEAVANCE).ToList())
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
                        if (tom.TP_MPIECES_JUSTIFICATIVES.Any(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE"))
                        {
                            foreach (var y in tom.TP_MPIECES_JUSTIFICATIVES.Where(a => a.NUMERO_FICHE == IDString && a.MODULLE == "CPTADMINAVANCE").ToList())
                            {
                                MTNPJ += y.MONTANT.Value;
                            }
                        }

                        //MathRound 3 satria kely kokoa ny marge d'erreur no le 2//
                        if (Math.Round(MTN, 3) == Math.Round(MTNPJ, 3))
                        {
                            //Check si F a déjà passé les 3 étapes (DEF, TEF et BE) pour avoir les dates => BE étape finale//
                            var canBe = true;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.DEF) == null)
                                canBe = false;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.TEF) == null)
                                canBe = false;
                            if (tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BE) == null)
                                canBe = false;

                            //TEST que F n'est pas encore traité ou F a été annulé// ETAT annulé = 2//
                            if (canBe)
                            {
                                if (tom.GA_AVANCE_JUSTIFICATIF.Any(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && a.DATE >= DateDebut && a.DATE <= DateFin))
                                {
                                    foreach (var y in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.NUMERO_AVANCE == x.NUMEROAVANCE && a.DATE >= DateDebut && a.DATE <= DateFin).OrderBy(a => a.DATE).OrderBy(a => a.NUMERO_PIECE).ToList())
                                    {
                                        Guid idJustif = Guid.Parse(y.ID);
                                        if (!db.SI_TRAITJUSTIF.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.IDPROJET == crpt) || db.SI_TRAITJUSTIF.Any(a => a.No == idJustif && a.NPIECE == y.NUMERO_PIECE && a.ETAT == 2 && a.IDPROJET == crpt))
                                        {
                                            var titulaire = "";

                                            var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE);

                                            if (isGA != null)
                                            {
                                                if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == y.NUMERO_AVANCE).AUXI != null)
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
                                                       });

                                            //bool isLate = false;
                                            //DateTime DD = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == x.NUMEROAVANCE && a.NUMEROETAPE == numCaEtapAPP.BE).DATETRAITEMENT.Value.Date;
                                            //if (DD.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                                            //    isLate = true;

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
                                                PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == y.NUMERO_AVANCE_MOUVEMENT).POSTE : "",
                                                DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEF).DATETRAITEMENT,
                                                DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEF).DATETRAITEMENT,
                                                DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == y.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BE).DATETRAITEMENT,
                                                SOA = soa.FirstOrDefault().SOA,
                                                PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
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
            var lien = "http://srvapp.softwell.cloud/softconnectsiig/";

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
                    var isPiece = tom.GA_AVANCE_JUSTIFICATIF.FirstOrDefault(a => a.ID == SAV);
                    if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.No == elem && a.NPIECE == isPiece.NUMERO_PIECE && a.ETAT == 2 && a.IDPROJET == crpt) != null)
                    {
                        var ismod = db.SI_TRAITJUSTIF.FirstOrDefault(a => a.No == elem && a.NPIECE == isPiece.NUMERO_PIECE && a.IDPROJET == crpt);
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
                        var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE);
                        if (isGA != null)
                        {
                            if (tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE).COGE != null && tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == isPiece.NUMERO_AVANCE).AUXI != null)
                            {
                                if (tom.RTIERS.Any(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI))
                                    titulaire = tom.RTIERS.FirstOrDefault(a => a.COGE == isGA.COGE && a.AUXI == isGA.AUXI).NOM;
                            }
                        }

                        var numCaEtapAPP = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);

                        var newT = new SI_TRAITJUSTIF()
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
                            PCOP = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == isPiece.NUMERO_AVANCE_MOUVEMENT) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == isPiece.NUMERO_AVANCE_MOUVEMENT).POSTE : "",
                            DATEDEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.DEFA).DATETRAITEMENT,
                            DATETEF = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.TEFA).DATETRAITEMENT,
                            DATEBE = tom.CPTADMIN_TRAITEMENT_AVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isPiece.NUMERO_AVANCE && a.NUMEROETAPE == numCaEtapAPP.BEA).DATETRAITEMENT,
                            DATECRE = DateTime.Now,
                            ETAT = 0,
                            IDUSERCREATE = exist.ID
                        };

                        if (ordsec == 1)
                        {
                            newT.ETAT = 1;
                            newT.DATEVALIDATION = DateTime.Now;
                            newT.IDUSERVALIDATE = exist.ID;
                        }

                        db.SI_TRAITJUSTIF.Add(newT);
                        db.SaveChanges();
                    }

                    countTraitement++;
                }
                catch (Exception e)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
                }
            }

            //SEND MAIL ALERT et NOTIFICATION//
            string MailAdresse = "serviceinfo@softwell.mg";
            string mdpMail = "09eYpçç0601";

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MAILJ0 != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MAILJ0;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }

                mail.Subject = "Attente validation justificatifs du projet " + ProjetIntitule;
                mail.IsBodyHtml = true;

                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " justificatifs en attente de validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT-SIIG CONNECT.<br/><br>" + "Cordialement";

                if (ordsec == 1)
                {
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " justificatifs validées pour le compte du projet " + ProjetIntitule + " et en attente de transfert vers SIIGFP.<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT-SIIG CONNECT.<br/><br>" + "Cordialement";

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

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.GA_AVANCE_JUSTIFICATIF.FirstOrDefault(a => a.ID == IdF) != null)
                {
                    foreach (var x in tom.GA_AVANCE_JUSTIFICATIF.Where(a => a.ID == IdF).ToList())
                    {
                        var isGA = tom.GA_AVANCE.FirstOrDefault(a => a.NUMERO == x.NUMERO_AVANCE);

                        list.Add(new DATATRPROJET
                        {
                            REF = isGA != null ? isGA.LIBELLE : "",
                            OBJ = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT).COGE : "",
                            TITUL = tom.GA_AVANCE_MOUVEMENT.Any(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT) ? tom.GA_AVANCE_MOUVEMENT.FirstOrDefault(a => a.IDENTIFIANT == x.NUMERO_AVANCE_MOUVEMENT).POSTE : "",
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

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                //List<DATATRPROJET> list = new List<DATATRPROJET>();
                var newElemH = new DATATRPROJET()
                {
                    REF = "",
                    OBJ = "",
                    TITUL = ""
                };

                var isJustif = tom.GA_AVANCE_JUSTIFICATIF.FirstOrDefault(a => a.ID == IdF);
                string isAv = tom.CPTADMIN_FAVANCE.FirstOrDefault(a => a.NUMEROAVANCE == isJustif.NUMERO_AVANCE).ID.ToString();

                if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && (a.TYPEPIECE == "DEF" || a.TYPEPIECE == "TEF" || a.TYPEPIECE == "BE") && a.MODULLE == "CPTADMINAVANCE") != null)
                {
                    var def = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "DEF" && a.MODULLE == "CPTADMINAVANCE") != null)
                        def = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "DEF" && a.MODULLE == "CPTADMINAVANCE").LIEN;
                    var tef = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "TEF" && a.MODULLE == "CPTADMINAVANCE") != null)
                        tef = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "TEF" && a.MODULLE == "CPTADMINAVANCE").LIEN;
                    var be = "";
                    if (tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "BE" && a.MODULLE == "CPTADMINAVANCE") != null)
                        be = tom.TP_MPIECES_JUSTIFICATIVES.FirstOrDefault(a => a.NUMERO_FICHE == isAv && a.TYPEPIECE == "BE" && a.MODULLE == "CPTADMINAVANCE").LIEN;

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

                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (tom.GA_AVANCE_JUSTIFICATIF.Any(a => a.ID == IdF))
                {
                    list.Add(new DATATRPROJET
                    {
                        LIEN = tom.GA_AVANCE_JUSTIFICATIF.FirstOrDefault(a => a.ID == IdF).COMMENTAIRE != null ? tom.GA_AVANCE_JUSTIFICATIF.FirstOrDefault(a => a.ID == IdF).COMMENTAIRE : ""
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

                var lien = "http://srvapp.softwell.cloud/softconnectsiig/";

                var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == IdS && a.DELETIONDATE == null).FirstOrDefault().PROJET;

                if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.No == IdF && a.IDPROJET == IdS) != null)
                {
                    var ismod = db.SI_TRAITJUSTIF.FirstOrDefault(a => a.No == IdF && a.IDPROJET == IdS);
                    ismod.ETAT = 2;
                    //ismod.DATECRE = DateTime.Now;
                    ismod.DATEANNUL = DateTime.Now;
                    ismod.IDUSERANNUL = exist.ID;

                    db.SaveChanges();
                }

                var newElemH = new SI_TRAITANNULJUSTIF()
                {
                    No = IdF,
                    DATEANNUL = DateTime.Now,
                    MOTIF = Motif,
                    COMMENTAIRE = Comm,
                    IDPROJET = IdS,
                    IDUSER = exist.ID
                };
                db.SI_TRAITANNULJUSTIF.Add(newElemH);
                db.SaveChanges();

                //SEND MAIL ALERT et NOTIFICATION//
                string MailAdresse = "serviceinfo@softwell.mg";
                string mdpMail = "09eYpçç0601";

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                    smtp.UseDefaultCredentials = true;

                    mail.From = new MailAddress(MailAdresse);

                    mail.To.Add(MailAdresse);
                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null).MAILREJETJUST != null)
                    {
                        string[] separators = { ";" };

                        var Tomail = mail;
                        if (Tomail != null)
                        {
                            string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null).MAILREJETJUST;
                            string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var mailto in mailListe)
                            {
                                mail.To.Add(mailto);
                            }
                        }
                    }

                    mail.Subject = "Rejet justificatif du projet " + ProjetIntitule;
                    mail.IsBodyHtml = true;
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez un justificatif rejeté pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT-SIIG CONNECT.<br/><br>" + "Cordialement";

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
            ViewBag.Controller = "Validation des justificatifs";

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

                //int retarDate = 0;
                //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELAV.Value;

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
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));

                if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 0) != null)
                {
                    foreach (var x in db.SI_TRAITJUSTIF.Where(a => a.IDPROJET == crpt && a.ETAT == 0).OrderBy(a => a.DATECRE).OrderBy(a => a.DATEMANDAT).ToList())
                    {
                        var soa = (from soas in db.SI_SOAS
                                   join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                   where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                   select new
                                   {
                                       soas.SOA
                                   });

                        //bool isLate = false;
                        //if (x.DATECRE.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        //    isLate = true;

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
                            SOA = soa.FirstOrDefault().SOA,
                            PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
                            //isLATE = isLate
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

                //int retarDate = 0;
                //if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null))
                //    retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).DELAV.Value;

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
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.DEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état DEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.TEF) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état TEF n'est pas paramétré sur TOM²PRO. " }, settings));
                if (tom.CPTADMIN_CHAINETRAITEMENT_AVANCE.FirstOrDefault(a => a.NUM == numCaEtapAPP.BE) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "Prese", msg = "L'état BE n'est pas paramétré sur TOM²PRO. " }, settings));

                if (db.SI_TRAITJUSTIF.Any(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 0))
                {
                    foreach (var x in db.SI_TRAITJUSTIF.Where(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == 0).OrderBy(a => a.DATECRE).OrderBy(a => a.DATEMANDAT).ToList())
                    {
                        var soa = (from soas in db.SI_SOAS
                                   join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                   where prj.IDPROJET == crpt && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                   select new
                                   {
                                       soas.SOA
                                   });

                        //bool isLate = false;
                        //if (x.DATECRE.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        //    isLate = true;

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
                            SOA = soa.FirstOrDefault().SOA,
                            PROJET = db.SI_PROJETS.Where(a => a.ID == crpt && a.DELETIONDATE == null).FirstOrDefault().PROJET
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
            var lien = "http://srvapp.softwell.cloud/softconnectsiig/";

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
                    if (db.SI_TRAITJUSTIF.FirstOrDefault(a => a.IDPROJET == crpt && a.No == isSAV) != null)
                    {
                        var isModified = db.SI_TRAITJUSTIF.FirstOrDefault(a => a.IDPROJET == crpt && a.No == isSAV);
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

            //SEND MAIL ALERT et NOTIFICATION//
            string MailAdresse = "serviceinfo@softwell.mg";
            string mdpMail = "09eYpçç0601";

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MAILJ1 != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MAILJ1;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }

                mail.Subject = "Validation justificatifs du projet " + ProjetIntitule;
                mail.IsBodyHtml = true;
                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " justificatifs validés pour le compte du projet " + ProjetIntitule + " .<br/><br>" +
                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT-SIIG CONNECT.<br/><br>" + "Cordialement";

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
