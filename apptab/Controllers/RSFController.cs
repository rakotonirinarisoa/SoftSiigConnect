using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using apptab.Data;
using apptab.Data.Entities;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using static apptab.Controllers.EtatGEDController;

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
        public JsonResult Create(SI_USERS suser, string Title, string Annee, string Periode, string Type, string Lien, int IDProjet, string TITLEDOCS)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            Guid isDocs = Guid.Parse(Lien.Split('/').Last());

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(IDProjet);
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            var isTit = ged.Documents.FirstOrDefault(a => a.Id == isDocs).Title;

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
                    IDUSER = exist.ID,
                    TITLEDOCS = isTit
                };
                db.SI_RSF.Add(newUser);
                db.SaveChanges();

                int ann = int.Parse(words[0]);
                var isElemH = db.SI_RSF.FirstOrDefault(a => a.IDPROJET == IDProjet && a.DELETIONDATE == null && a.TITLE == Title && a.PERIODE == Periode && a.TYPE == Type && a.LIEN == Lien.ToString() && a.TITLEDOCS == isTit && a.ANNEE == ann && a.MOIS == mois && a.IDUSER == exist.ID);
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
                    TITLEDOCS = isTit
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

        public static String[] DeserializeJsonToGuidArray(string jsonString)
        {
            return System.Text.Json.JsonSerializer.Deserialize<String[]>(jsonString);
        }

        //GET ALL LIEN//
        [HttpPost]
        public ActionResult GetAllLien(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez parametrer le mappage GED ET PROJET. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == exist.IDPROJET && a.DELETIONDATE == null).IDPROJET.Value);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            if (!db.SI_PROGED.Any(a => a.IDPROJET == exist.IDPROJET))
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

            if (db.SI_USERS.FirstOrDefault(a => a.IDPROJET == exist.IDPROJET && a.DELETIONDATE == null && a.ID == exist.ID).IDUSERGED == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
            else
            {
                var IDUSERGEDT = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == exist.IDPROJET && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                if (!ged.Users.Any(a => a.Id == IDUSERGEDT && a.DeletionDate == null))
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
            }

            var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == exist.IDPROJET && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;

            List<string> site = new List<string>();
            var allSite = ged.Users.FirstOrDefault(a => a.Id == IDUSERGED).Sites;
            String[] guidArray = DeserializeJsonToGuidArray(allSite);
            foreach (string guid in guidArray)
            {
                site.Add(guid);
            }

            var lienGEd = db.SI_GEDLIEN.FirstOrDefault().LIEN;

            List<PROGED> linkAll = new List<PROGED>();

            linkAll.Add(new PROGED()
            {
                LIEN = "",
                TITLE = "",
                //IDDOC = null
            });

            if (exist.IDPROJET != 0)
            {
                foreach (var x in db.SI_PROGED.Where(a => a.IDPROJET == exist.IDPROJET && a.DELETIONDATE == null))
                {
                    foreach (var y in ged.Users.Where(a => a.ProjectId == x.IDGED && a.DeletionDate == null/* && site.Contains(a.Sites)*/).ToList())
                    {
                        foreach (var z in ged.Documents.Where(a => a.SenderId == y.Id && a.DeletionDate == null && site.Contains(a.Site) && a.RSF == true).ToList())
                        {
                            var islink = lienGEd + "/documents/shared/" + z.Id.ToString();
                            if (!db.SI_RSF.Any(a => a.LIEN == islink))
                            {
                                linkAll.Add(new PROGED()
                                {
                                    LIEN = islink,
                                    TITLE = z.Title,
                                    IDDOC = z.Id
                                });
                            }
                        }
                    }
                }
            }

            //foreach (var x in db.SI_PROGED.Where(a => a.DELETIONDATE == null))
            //{
            //    foreach (var y in ged.Users.Where(a => a.ProjectId == x.IDGED && a.DeletionDate == null).ToList())
            //    {
            //        foreach (var z in ged.Documents.Where(a => a.SenderId == y.Id && a.DeletionDate == null).ToList())
            //        {
            //            var islink = lienGEd + "/documents/shared/" + z.Id.ToString();
            //            if (db.SI_RSF.Where(a => a.LIEN == islink && a.DELETIONDATE == null).Count() == 0)
            //            {
            //                linkAll.Add(new PROGED()
            //                {
            //                    LIEN = islink,
            //                    TITLE = z.Title,
            //                    //IDDOC = z.Id
            //                });
            //            }
            //        }
            //    }
            //}

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = linkAll }, settings));
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

            var lienGEd = db.SI_GEDLIEN.FirstOrDefault();

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
                        IDPROJET = isModif.IDPROJET,
                        TITLEDOCS = isModif.TITLEDOCS,
                    };

                    string ann = mapp.ANNEE.ToString() + '-' + mapp.MOIS.ToString();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { PROJET = mapp.IDPROJET, TITLE = mapp.TITLE, PERIODE = mapp.PERIODE, TYPE = mapp.TYPE, LIEN = mapp.LIEN, TITLEDOCS = mapp.TITLEDOCS, ANNEE = ann } }, settings));
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
        public JsonResult Update(SI_USERS suser, string Title, string Annee, string Periode, string Type/*, string Lien*/, int IDProjet, string UserId/*, string TITLEDOCS*/)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == IDProjet && a.DELETIONDATE == null).IDPROJET.Value);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

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

                if (isModif.TITLE != Title || isModif.PERIODE != Periode || isModif.TYPE != Type /*|| isModif.LIEN != Lien.ToString() || isModif.TITLEDOCS != isTit*/ || isModif.ANNEE != int.Parse(words[0]) || isModif.MOIS != mois)
                {
                    if (isModif.IDUSER == exist.ID && exist.ID != 0)
                    {
                        isModif.TITLE = Title;
                        isModif.PERIODE = Periode;
                        isModif.TYPE = Type;
                        //isModif.LIEN = Lien.ToString();
                        isModif.ANNEE = int.Parse(words[0]);
                        isModif.MOIS = mois;
                        //isModif.TITLEDOCS = isTit;

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
                            //LIEN = Lien.ToString(),
                            ANNEE = int.Parse(words[0]),
                            MOIS = mois,
                            IDPROJET = IDProjet,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = isModif.ID,
                            //TITLEDOCS = isModif.TITLEDOCS,
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

        //GET ALL INSTANCE//
        [HttpPost]
        public ActionResult GetNewInstance(SI_USERS suser, SI_MAPPAGES_GED map)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (String.IsNullOrEmpty(map.INSTANCE))
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle mappage. " }, settings));

            //Get all bases with the instance
            var BaseList = new List<string>();

            try
            {
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                connection.DataSource = map.INSTANCE;
                if (map.CONNEXION != null) connection.UserID = map.CONNEXION;
                if (map.CONNEXPWD != null) connection.Password = map.CONNEXPWD;

                if (map.AUTH != 1)
                    connection.IntegratedSecurity = true;
                else
                    connection.TrustServerCertificate = true;

                String strConn = connection.ToString();
                SqlConnection sqlConn = new SqlConnection(strConn);
                try
                {
                    sqlConn.Open();
                    DataTable tblDatabases = sqlConn.GetSchema("Databases");
                    sqlConn.Close();
                    foreach (DataRow row in tblDatabases.Rows)
                    {
                        String strDatabaseName = row["database_name"].ToString();
                        BaseList.Add(strDatabaseName);
                    }
                    BaseList.Sort();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = BaseList }, settings));
                }
                catch (Exception)
                {
                    if (map.AUTH == 1) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Échec de l'ouverture de session de l'utilisateur 'sa'. Vérifiez vos identifiants pour la connexion à SQL Server. " }, settings));
                    else return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Une erreur liée au réseau ou spécifique à l'instance s'est produite lors de l'établissement d'une connexion à SQL Server. Le serveur est introuvable ou n'est pas accessible. Vérifiez que le nom de l'instance est correct et que SQL Server est configuré pour autoriser les connexions distantes. " }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Une erreur liée au réseau ou spécifique à l'instance s'est produite lors de l'établissement d'une connexion à SQL Server. Le serveur est introuvable ou n'est pas accessible. Vérifiez que le nom de l'instance est correct et que SQL Server est configuré pour autoriser les connexions distantes. " }, settings));
            }
        }

        //MAPPAGE CREATE//
        public ActionResult MappageGED()
        {
            return View();
        }

        [HttpPost]
        public JsonResult MappageCreate(SI_USERS suser, SI_MAPPAGES_GED user)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var isMapp = db.SI_MAPPAGES_GED.FirstOrDefault();
                if (isMapp == null)
                {
                    var newUser = new SI_MAPPAGES_GED()
                    {
                        INSTANCE = user.INSTANCE,
                        AUTH = user.AUTH,
                        CONNEXION = user.CONNEXION,
                        CONNEXPWD = user.CONNEXPWD,
                        DBASE = user.DBASE,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };
                    db.SI_MAPPAGES_GED.Add(newUser);
                    db.SaveChanges();
                }
                else
                {
                    isMapp.INSTANCE = user.INSTANCE;
                    isMapp.AUTH = user.AUTH;
                    isMapp.CONNEXION = user.CONNEXION;
                    isMapp.CONNEXPWD = user.CONNEXPWD;
                    isMapp.DBASE = user.DBASE;
                    isMapp.CREATIONDATE = DateTime.Now;
                    isMapp.IDUSER = exist.ID;

                    db.SaveChanges();
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = user }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public ActionResult DetailsMAPP(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var map = db.SI_MAPPAGES_GED.FirstOrDefault();

                if (map != null)
                {
                    var mapp = new
                    {
                        inst = map.INSTANCE,
                        auth = map.AUTH,
                        conn = map.CONNEXION,
                        mdp = map.CONNEXPWD,
                        baseD = map.DBASE,
                        id = map.ID
                    };

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { INSTANCE = mapp.inst, AUTH = mapp.auth, CONNEXION = mapp.conn, MDP = mapp.mdp, BASED = mapp.baseD, mapp.id } }, settings));
                }
                else
                {
                    int aa = 0;
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "message", data = new { INSTANCE = "", AUTH = "", CONNEXION = "", MDP = "", BASED = "", aa } }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //CORRESPONDANCE SET - GED LISTE//
        public ActionResult PROSOAList()
        {
            ViewBag.Controller = "Correspondance entre SET - GED";
            return View();
        }

        public class PROGED
        {
            public string PROJET { get; set; }
            public string GED { get; set; }
            public int? ID { get; set; }
            public DateTime? DELETIONDATE { get; set; }

            public Guid IDDOC { get; set; }
            public string TITLE { get; set; }
            public string LIEN { get; set; }
        }

        public JsonResult FillTablePROSOA(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(b => b.IDPROJET == exist.IDPROJET && b.DELETIONDATE == null).IDPROJET.Value);
                if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                List<PROGED> a = new List<PROGED>();

                if (db.SI_PROGED.Any(x => x.DELETIONDATE == null))
                {
                    foreach (var x in db.SI_PROGED.Where(x => x.DELETIONDATE == null).ToList())
                    {
                        if (ged.Projects.Any(b => b.Id == x.IDGED && x.DELETIONDATE == null))
                        {
                            if (db.SI_PROJETS.Any(b => b.ID == x.IDPROJET && b.DELETIONDATE == null))
                            {
                                a.Add(new PROGED()
                                {
                                    PROJET = db.SI_PROJETS.FirstOrDefault(b => b.ID == x.IDPROJET && b.DELETIONDATE == null).PROJET,
                                    GED = ged.Projects.Any(b => b.Id == x.IDGED) ? ged.Projects.FirstOrDefault(b => b.Id == x.IDGED).Name : "",
                                    ID = x.ID,
                                    DELETIONDATE = x.DELETIONDATE
                                });
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = a }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        public ActionResult DeleteFPROSOA(SI_USERS suser, string PROSOAID)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IDPROSOA = int.Parse(PROSOAID);
                var PROSOA = db.SI_PROGED.FirstOrDefault(a => a.ID == IDPROSOA && a.DELETIONDATE == null);

                var elemH = db.HSI_PROGED.FirstOrDefault(a => a.IDPARENT == IDPROSOA && a.DELETIONDATE == null);

                if (PROSOA != null)
                {
                    PROSOA.DELETIONDATE = DateTime.Now;
                    PROSOA.IDUSERDEL = exist.ID;

                    db.SaveChanges();

                    if (elemH != null)
                    {
                        elemH.DELETIONDATE = DateTime.Now;
                    }

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

        //CORRESPONDANCE SET - GED CREATE//
        public ActionResult SuperAdminPROSOA()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddSocietePROSOA(SI_USERS suser, SI_PROGED societe)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == exist.IDPROJET && a.DELETIONDATE == null).IDPROJET.Value);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var Projet = db.SI_PROJETS.FirstOrDefault(a => a.ID == societe.IDPROJET && a.DELETIONDATE == null).ID;
            var Soa = ged.Projects.FirstOrDefault(a => a.Id == societe.IDGED && a.DeletionDate == null).Id;

            if (!db.SI_PROGED.Any(a => a.IDPROJET == Projet /*|| a.IDGED == Soa*/ && a.DELETIONDATE == null))
            {
                var newSociete = new SI_PROGED()
                {
                    IDPROJET = Projet,
                    IDGED = Soa,
                    CREATIONDATE = DateTime.Now,
                    IDUSER = exist.ID
                };
                db.SI_PROGED.Add(newSociete);
                db.SaveChanges();

                var isElemH = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == Projet && a.IDGED == Soa && a.DELETIONDATE == null);
                var newSocieteH = new HSI_PROGED()
                {
                    IDPROJET = isElemH.IDPROJET,
                    IDGED = isElemH.IDGED,
                    CREATIONDATE = isElemH.CREATIONDATE,
                    IDUSER = isElemH.IDUSER,
                    IDPARENT = isElemH.ID
                };
                db.HSI_PROGED.Add(newSocieteH);
                db.SaveChanges();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = societe }, settings));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Correspondance déjà existante. " }, settings));
            }
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

        //GET ALL PROJET GED//
        [HttpPost]
        public ActionResult GetAllSOA(SI_USERS suser, string IDPROSOA)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == exist.IDPROJET && a.DELETIONDATE == null).IDPROJET.Value);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            if (IDPROSOA != null)
            {
                int? PROSOAID = int.Parse(IDPROSOA);
                var idsoa = db.SI_PROGED.Where(a => a.ID == PROSOAID && a.DELETIONDATE == null).Select(a => a.IDGED).FirstOrDefault();

                var SOA = ged.Projects.Where(x => x.Id != idsoa && x.DeletionDate == null).Select(a => new
                {
                    SOA = a.Name,
                    ID = a.Id,
                    DELETIONDATE = a.DeletionDate
                }).ToList();

                var soa1 = ged.Projects.Where(x => x.Id == idsoa && x.DeletionDate == null).Select(x => new
                {
                    SOA = x.Name,
                    ID = x.Id,
                    DELETIONDATE = x.DeletionDate
                }).ToList();

                List<SI_SOAS> SOAf = new List<SI_SOAS>();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = soa1, datas = SOA }, settings));
            }
            else
            {
                var SOA = ged.Projects.Where(x => x.DeletionDate == null).Select(a => new
                {
                    SOA = a.Name,
                    ID = a.Id
                }).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = SOA }, settings));
            }
        }

        public ActionResult SuperAdminDetailFPROSOA(SI_USERS suser, string PROSOAID)
        {
            return View();
        }
    }
}
