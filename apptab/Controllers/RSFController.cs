using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using apptab.Data.Entities;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class RSFController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        // GET: RSF
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult FillTable(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();

                var rsf = db.SI_RSF.Select(a => new
                {
                    ID = a.ID,
                    PROJET = db.SI_PROJETS.FirstOrDefault(x => x.ID == a.IDPROJET && a.DELETIONDATE == null).PROJET,
                    IDP = 0,
                    SOA = (from soas in db.SI_SOAS
                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                           where prj.IDPROJET == a.IDPROJET && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                           select new
                           {
                               soas.SOA
                           }).FirstOrDefault().SOA,
                    TITRE = a.TITLE,
                    ANNEE = a.ANNEE,
                    MOIS = a.MOIS,
                    PERIODE = a.PERIODE,
                    TYPE = a.TYPE,
                    LIEN = a.LIEN,
                    DELETEDATE = a.DELETIONDATE
                }).Where(a => a.DELETEDATE == null).ToList();

                if (test.IDPROJET != 0)
                {
                    rsf = db.SI_RSF.Select(a => new
                    {
                        ID = a.ID,
                        PROJET = db.SI_PROJETS.FirstOrDefault(x => x.ID == a.IDPROJET && a.DELETIONDATE == null).PROJET,
                        IDP = db.SI_PROJETS.FirstOrDefault(x => x.ID == a.IDPROJET && a.DELETIONDATE == null).ID,
                        SOA = (from soas in db.SI_SOAS
                               join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                               where prj.IDPROJET == a.IDPROJET && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                               select new
                               {
                                   soas.SOA
                               }).FirstOrDefault().SOA,
                        TITRE = a.TITLE,
                        ANNEE = a.ANNEE,
                        MOIS = a.MOIS,
                        PERIODE = a.PERIODE,
                        TYPE = a.TYPE,
                        LIEN = a.LIEN,
                        DELETEDATE = a.DELETIONDATE
                    }).Where(a => a.IDP == test.IDPROJET.Value && a.DELETEDATE == null).ToList();
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = rsf }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //DELETE//
        [HttpPost]
        public JsonResult Delete(SI_USERS suser, string MAPPId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int useID = int.Parse(MAPPId);
                var user = db.SI_RSF.FirstOrDefault(a => a.ID == useID);
                if (user != null)
                {
                    if (user.IDUSER == exist.ID && exist.ID != 0)
                    {
                        user.DELETIONDATE = DateTime.Now;
                        db.SaveChanges();

                        if (db.HSI_RSF.Any(a => a.IDPARENT == user.ID))
                        {
                            var hsi = db.HSI_RSF.FirstOrDefault(a => a.IDPARENT == user.ID);
                            hsi.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Suppression avec succès. " }, settings));
                    }
                    else
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous n'avez pas le droit de supprimer ce document. " }, settings));
                    }
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "message" }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //MAPPAGE//
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(SI_USERS suser, string Title, string Annee, string Periode, string Type, string Lien, int IDProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                string phrase = Annee;
                string[] words = phrase.Split('-');

                string mois = "";
                int moisInt = int.Parse(words[1]);
                switch (moisInt)
                {
                    case 1:
                        mois = "JANVIER";
                        break;
                    case 2:
                        mois = "FEVRIER";
                        break;
                    case 3:
                        mois = "MARS";
                        break;
                    case 4:
                        mois = "AVRIL";
                        break;
                    case 5:
                        mois = "MAI";
                        break;
                    case 6:
                        mois = "JUIN";
                        break;
                    case 7:
                        mois = "JUILLET";
                        break;
                    case 8:
                        mois = "AOUT";
                        break;
                    case 9:
                        mois = "SEPTEMBRE";
                        break;
                    case 10:
                        mois = "OCTOBRE";
                        break;
                    case 11:
                        mois = "NOVEMBRE";
                        break;
                    case 12:
                        mois = "DECEMBRE";
                        break;
                    default:
                        // code block
                        break;
                }

                var newUser = new SI_RSF()
                {
                    TITLE = Title,
                    PERIODE = Periode,
                    TYPE = Type,
                    LIEN = Lien,
                    ANNEE = int.Parse(words[0]),
                    MOIS = mois,
                    IDPROJET = IDProjet,
                    CREATIONDATE = DateTime.Now,
                    IDUSER = exist.ID
                };
                db.SI_RSF.Add(newUser);
                db.SaveChanges();

                int ann = int.Parse(words[0]);
                var isElemH = db.SI_RSF.FirstOrDefault(a => a.IDPROJET == IDProjet && a.DELETIONDATE == null && a.TITLE == Title && a.PERIODE == Periode && a.TYPE == Type && a.LIEN == Lien && a.ANNEE == ann && a.MOIS == mois && a.IDUSER == exist.ID);
                var newSocieteH = new HSI_RSF()
                {
                    TITLE = isElemH.TITLE,
                    PERIODE = isElemH.PERIODE,
                    TYPE = isElemH.TYPE,
                    LIEN = isElemH.LIEN,
                    ANNEE = ann,
                    MOIS = isElemH.MOIS,
                    IDPROJET = IDProjet,
                    CREATIONDATE = isElemH.CREATIONDATE,
                    IDUSER = exist.ID,
                    IDPARENT = isElemH.ID,
                };
                db.HSI_RSF.Add(newSocieteH);
                db.SaveChanges();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = newUser }, settings));
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

        //DETAILS//
        public ActionResult Details(string UserId)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Details(SI_USERS suser, string UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int useID = int.Parse(UserId);
                var isModif = db.SI_RSF.FirstOrDefault(a => a.ID == useID);

                string mois = isModif.MOIS;
                string moisInt = "00";
                switch (mois)
                {
                    case "JANVIER":
                        moisInt = "01";
                        break;
                    case "FEVRIER":
                        moisInt = "02";
                        break;
                    case "MARS":
                        moisInt = "03";
                        break;
                    case "AVRIL":
                        moisInt = "04";
                        break;
                    case "MAI":
                        moisInt = "05";
                        break;
                    case "JUIN":
                        moisInt = "06";
                        break;
                    case "JUILLET":
                        moisInt = "07";
                        break;
                    case "AOUT":
                        moisInt = "08";
                        break;
                    case "SEPTEMBRE":
                        moisInt = "09";
                        break;
                    case "OCTOBRE":
                        moisInt = "10";
                        break;
                    case "NOVEMBRE":
                        moisInt = "11";
                        break;
                    case "DECEMBRE":
                        moisInt = "12";
                        break;
                    default:
                        // code block
                        break;
                }

                if (isModif != null)
                {
                    var mapp = new
                    {
                        TITLE = isModif.TITLE,
                        PERIODE = isModif.PERIODE,
                        TYPE = isModif.TYPE,
                        LIEN = isModif.LIEN,
                        ANNEE = isModif.ANNEE,
                        MOIS = moisInt,
                        IDPROJET = isModif.IDPROJET
                    };

                    string ann = mapp.ANNEE.ToString() + '-' + mapp.MOIS.ToString();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { PROJET = mapp.IDPROJET, TITLE = mapp.TITLE, PERIODE = mapp.PERIODE, TYPE = mapp.TYPE, LIEN = mapp.LIEN, ANNEE = ann } }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "message" }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //UPDATE//
        [HttpPost]
        public JsonResult Update(SI_USERS suser, string Title, string Annee, string Periode, string Type, string Lien, int IDProjet, string UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int UserIdI = int.Parse(UserId);
                var isModif = db.SI_RSF.FirstOrDefault(a => a.IDPROJET == IDProjet && a.ID == UserIdI);

                string phrase = Annee;
                string[] words = phrase.Split('-');

                string mois = "";
                int moisInt = int.Parse(words[1]);
                switch (moisInt)
                {
                    case 1:
                        mois = "JANVIER";
                        break;
                    case 2:
                        mois = "FEVRIER";
                        break;
                    case 3:
                        mois = "MARS";
                        break;
                    case 4:
                        mois = "AVRIL";
                        break;
                    case 5:
                        mois = "MAI";
                        break;
                    case 6:
                        mois = "JUIN";
                        break;
                    case 7:
                        mois = "JUILLET";
                        break;
                    case 8:
                        mois = "AOUT";
                        break;
                    case 9:
                        mois = "SEPTEMBRE";
                        break;
                    case 10:
                        mois = "OCTOBRE";
                        break;
                    case 11:
                        mois = "NOVEMBRE";
                        break;
                    case 12:
                        mois = "DECEMBRE";
                        break;
                    default:
                        // code block
                        break;
                }

                if (isModif.TITLE != Title || isModif.PERIODE != Periode || isModif.TYPE != Type || isModif.LIEN != Lien || isModif.ANNEE != int.Parse(words[0]) || isModif.MOIS != mois)
                {
                    if (isModif.IDUSER == exist.ID && exist.ID != 0)
                    {
                        isModif.TITLE = Title;
                        isModif.PERIODE = Periode;
                        isModif.TYPE = Type;
                        isModif.LIEN = Lien;
                        isModif.ANNEE = int.Parse(words[0]);
                        isModif.MOIS = mois;

                        db.SaveChanges();

                        var H = db.HSI_RSF.FirstOrDefault(a => a.IDPARENT == isModif.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newSocieteH = new HSI_RSF()
                        {
                            TITLE = Title,
                            PERIODE = Periode,
                            TYPE = Type,
                            LIEN = Lien,
                            ANNEE = int.Parse(words[0]),
                            MOIS = mois,
                            IDPROJET = IDProjet,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = isModif.ID,
                        };
                        db.HSI_RSF.Add(newSocieteH);
                        db.SaveChanges();

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = isModif }, settings));
                    }
                    else
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous n'avez pas le droit de modifier ce document. " }, settings));
                    }
                }
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = isModif }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
