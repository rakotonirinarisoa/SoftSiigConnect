using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using apptab.Extension;
using Aspose.Zip.Saving;
using Aspose.Zip;
using Newtonsoft.Json;
using Renci.SshNet;
using System.Net;
using apptab.Models;
using Extensions.DateTime;
using System.Net.Mail;
using apptab.Data.Entities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using apptab.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;
using System.Text;
using System.Xml;
using WebGrease.Activities;
using System.Web;
using Microsoft.Build.Framework.XamlTypes;
using System.Security.Policy;
using System.Web.WebPages;
using System.IO.Compression;
using System.Collections;
using static apptab.Controllers.RSFController;
using Microsoft.Ajax.Utilities;
using System.Web.Mail;
using System.Data.Entity;
using System.Configuration;
using Renci.SshNet.Sftp;

namespace apptab.Controllers
{
    public class HomeController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTOM __db = new SOFTCONNECTOM();
        private static string Anarana;


        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TdbAccueil()
        {
            return View();
        }
        public ActionResult PaiementValidation()
        {
            ViewBag.Controller = "Récupération des paiements";
            return View();
        }
        public ActionResult TeacherValidation()
        {
            ViewBag.Controller = "Envoi pour validation";
            return View();
        }
        public ActionResult FValidation()
        {
            ViewBag.Controller = "Génération fichier banque";
            return View();
        }
        public ActionResult BonPourPaiement()
        {
            ViewBag.Controller = "Validation des Payements";
            return View();
        }
        public ActionResult CancelHisto()
        {
            ViewBag.Controller = "Annuler Paiements";
            return View();
        }
        public ActionResult HistoReg()
        {
            ViewBag.Controller = "Historiques Paiements";
            return View();
        }
        public ActionResult AnomaliePaiement()
        {
            ViewBag.Controller = "Anomalie Paiements";
            return View();
        }
        public ActionResult AnomaliePaiementJournal()
        {
            ViewBag.Controller = "Anomalie Journal";
            return View();
        }
        [HttpPost]
        public string GetTypeP(SI_USERS suser, string codeproject)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            int PROJECTid = int.Parse(codeproject);
            if (exist == null) return "";

            if (exist.IDPROJET != 0)
            {
                var basename = db.SI_TYPECRITURE.FirstOrDefault(a => a.IDPROJET == PROJECTid).TYPE;
                return basename.ToString();
            }
            else
            {
                //var mapuser = db.SI_MAPUSERPROJET.Where(a => a.IDUS == exist.ID).ToList();
                int PROJECTID = int.Parse(codeproject);
                var ii = db.SI_TYPECRITURE.FirstOrDefault(a => a.IDPROJET == PROJECTID);
                var basename = "";
                if (ii != null)
                {
                    basename = ii.TYPE.ToString();
                }
                else
                {
                    basename = "Veuillez parametrer votre Type Ecriture";
                }


                return basename;
            }
        }
        [HttpPost]
        public JsonResult FillTable(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null) != null;
            if (!exist) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var droits = db.OPA_DROITS.Where(a => a.IDSOCIETE == suser.IDPROJET).Select(a => new
                {
                    USER = db.SI_USERS.FirstOrDefault(x => x.ID == a.IDUSER && x.DELETIONDATE == null).LOGIN,
                    INSTANCE = db.SI_MAPPAGES.FirstOrDefault(x => x.ID == a.IDMAPPAGE).INSTANCE,
                    DBASE = db.SI_MAPPAGES.FirstOrDefault(x => x.ID == a.IDMAPPAGE).DBASE,
                    ID = a.ID
                }).ToList();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = droits }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult CreateZipFile_old(SI_USERS suser, string list)
        {
            StreamWriter sw = null;
            var lists = list.Split(',');
            List<AnalY> analY = new List<AnalY>();
            //List<OPA_REGLEMENT> test = new List<OPA_REGLEMENT>();
            sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\OPAVI.txt");
            foreach (var item in lists)
            {
                var tt = Convert.ToDecimal(item);
                var xy = db.OPA_REGLEMENT.Where(x => x.NUM == tt).FirstOrDefault();
                sw.Write(xy.NUM + "\t" + xy.DATE + "\t" + xy.BENEFICIAIRE + "\t" + xy.BANQUE + "\t" + xy.GUICHET + "\t" + xy.RIB + "\t" + xy.MONTANT + "\t" + xy.LIBELLE + "\t" + xy.NUM_ETABLISSEMENT + "\t" + xy.CODE_J + "\t" + xy.DOM1 + "\t" + xy.DOM2 + "\t" + xy.CATEGORIE + "\t" + xy.APPLICATION + "\t" + xy.IDSOCIETE + Environment.NewLine);
            }
            sw.Close();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = "" }, settings));
        }
        public string CreateAFBTXT(string pathchemin, string pathfiles)
        {
            try
            {
                string pth = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\";
                if (!Directory.Exists(pth))
                {
                    Directory.CreateDirectory(pth);
                }
                StreamWriter sw = null;
                sw = new StreamWriter(pth + pathchemin + ".txt");
                sw.Write(pathfiles);
                sw.Close();
                string source = pth + pathchemin + ".txt";
                return source;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public FileResult CreateFileAFBtXt(string pathchemin, string pathfiles)
        {
            try
            {
                string pth = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\";

                if (!Directory.Exists(pth))
                {
                    Directory.CreateDirectory(pth);
                }
                StreamWriter sw = null;

                sw = new StreamWriter(pth + pathchemin + ".txt");
                sw.Write(pathfiles);
                sw.Close();
                byte[] source = System.IO.File.ReadAllBytes(pth + pathchemin + ".txt");
                string s = "application/txt";
                return File(source, System.Net.Mime.MediaTypeNames.Application.Octet, pathfiles);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XmlDocument SaveDocument(string pathchemin, string path)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(pathchemin);
            var address = xmlDoc.GetElementsByTagName("original");
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                    xmlTextWriter.Indentation = 6; // Nombre d'espaces pour l'indentation
                                                   //xmlDoc.WriteTo(xmlTextWriter);
                    xmlDoc.Save(xmlTextWriter);
                }
                System.IO.File.WriteAllText(pathchemin, stringWriter.ToString());
            }
            //using (StreamWriter stream = new StreamWriter(pathchemin, false, Encoding.GetEncoding("UTF-8")))
            //{
            //    xmlDoc.Save(stream);
            //}
            return (xmlDoc);
        }

        public FileResult CreateFileAFBXML(string pathchemin, string pathfiles)
        {
            try
            {
                string pth = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\";

                if (!Directory.Exists(pth))
                {
                    Directory.CreateDirectory(pth);
                }
                byte[] source = System.IO.File.ReadAllBytes(pth + pathchemin);
                string s = "application/xml";

                return File(source, System.Net.Mime.MediaTypeNames.Text.Xml, pathfiles);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public FileContentResult CreateAFBTXTArch(string pathchemin, string pathfiles, string psw)
        {
            string pth = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\";
            if (!Directory.Exists(pth))
            {
                Directory.CreateDirectory(pth);
            }
            StreamWriter sw = null;
            sw = new StreamWriter(pth + pathchemin + ".txt");
            //sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pathchemin + ".txt");
            sw.Write(pathfiles);
            sw.Close();

            byte[] bytearray = null;
            using (var memoryStream = new MemoryStream())
            {
                FileStream zipFile = System.IO.File.Open(pth + pathchemin + ".zip", FileMode.Create);

                var archive = new Archive(new ArchiveEntrySettings(encryptionSettings: new TraditionalEncryptionSettings(psw)));

                FileStream source = System.IO.File.Open(pth + pathchemin + ".txt", FileMode.Open, FileAccess.Read);

                archive.CreateEntry(pathchemin + ".txt", source);
                archive.Save(zipFile);

                zipFile.Dispose();
                // archive.Dispose();
                memoryStream.CopyTo(zipFile);
                bytearray = memoryStream.ToArray();
                memoryStream.Position = 0;
                //return new FileContentResult(bytearray, "application/zip");

                return File(bytearray, System.Net.Mime.MediaTypeNames.Application.Zip, pathfiles + ".zip");
            }
            //return File(source, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
        [HttpPost]
        public ActionResult CreateZipFile(SI_USERS suser, string codeproject, int intbasetype, bool devise, string codeJ, string baseName, string listCompte)
        {
            AFB160 aFB160 = new AFB160();

            var send = "";
            int PROJECTID = int.Parse(codeproject);
            var list = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);

            var ps = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN /*&& x.IDPROJET == PROJECTID*/ && x.PWD == suser.PWD && x.DELETIONDATE == null).Select(x => x.PWD).FirstOrDefault();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var pswftp = db.OPA_CRYPTO.Where(x => x.IDPROJET == PROJECTID && x.IDUSER == suser.ID && x.DELETIONDATE != null).Select(x => x.CRYPTOPWD).FirstOrDefault();
            List<OPA_VALIDATIONS> avalider = new List<OPA_VALIDATIONS>();

            var siteS = db.SI_SITE.Where(x => x.IDUSER == exist.ID && x.IDPROJET == PROJECTID).Select(x => x.SITE).FirstOrDefault();
            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));

            foreach (var item in siteS.Split(','))
            {
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes). " }, settings));
            }
            foreach (var SIT in siteS.Split(','))
            {
                foreach (var item in list)
                {
                    int b = int.Parse(item.Numereg);
                    avalider.AddRange(db.OPA_VALIDATIONS.Where(a => a.IDPROJET == PROJECTID && a.ETAT == 2 && a.IDREGLEMENT == item.Id && a.NUMEREG == b && a.SITE == SIT).ToList());
                };
            }
            
            if (baseName == "2")
            {
                var pathfile = aFB160.CreateTOMPROAFB160(devise, codeJ, suser, codeproject);
                if (intbasetype == 0)
                {
                    Anarana = pathfile.Chemin;
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    return CreateFileAFBtXt(pathfile.Chemin, pathfile.Fichier);
                }
                else if (intbasetype == 1)
                {
                    Anarana = pathfile.Chemin;
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    return CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                else if (intbasetype == 2)
                {
                    Anarana = pathfile.Chemin;
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, ftp.PATH, pport);
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else if (intbasetype == 3)
                {
                    Anarana = pathfile.Chemin;
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, ftp.PATH, pport);
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else
                {
                    Anarana = pathfile.Chemin;
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, ftp.PATH, pport);
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    return CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Archivage avec succès. ", data = send }, settings));
            }
            else
            {
                //var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser, codeproject, list);


                if (intbasetype == 0)
                {
                    var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser, codeproject, list);
                    Anarana = pathfile.Chemin;
                    return CreateFileAFBtXt(pathfile.Chemin, pathfile.Fichier);
                }
                else if (intbasetype == 1)
                {
                    var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser, codeproject, list);
                    Anarana = pathfile.Chemin;
                    return CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                else if (intbasetype == 2)
                {
                    var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser, codeproject, list);
                    Anarana = pathfile.Chemin;
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                    //SFTP(string HOTE, string PATH, string USERFTP, string PWDFTP, string SOURCE)
                    string pport = ftp.PORT.ToString();
                    string res = "";
                    CreateFileAFBtXt(pathfile.Chemin, pathfile.Fichier);
                    try
                    {
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, pathfile.Chemin, pport);
                        if (avalider != null)
                        {
                            foreach (var item in avalider)
                            {
                                try
                                {
                                    item.DATETRANS = DateTime.Now;

                                    item.IDUSTRANS = exist.ID;
                                    item.ETAT = 3;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                        }
                        res = "Fichier bien transferet ! ";
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = res, data = "" }, settings));
                }
                else if (intbasetype == 3)
                {
                    var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list);
                    Anarana = pathfile.Chemin;
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.NomFichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, ftp.PATH, pport);
                    if (avalider != null)
                    {
                        foreach (var item in avalider)
                        {
                            try
                            {
                                item.DATETRANS = DateTime.Now;

                                item.IDUSTRANS = exist.ID;
                                item.ETAT = 3;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    return Json(JsonConvert.SerializeObject(new { type = "sucess", msg = "Traitement avec Succées", data = "" }, settings));
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else
                {
                    var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser, codeproject, list);
                    Anarana = pathfile.Chemin;
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                    string res = "";
                    try
                    {
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, pathfile.Chemin, pport);
                        if (avalider != null)
                        {
                            foreach (var item in avalider)
                            {
                                try
                                {
                                    item.DATETRANS = DateTime.Now;

                                    item.IDUSTRANS = exist.ID;
                                    item.ETAT = 3;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                        }
                        res = "Fichier bien transferet ! ";
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                    //SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = "" }, settings));
                }
            }
        }
        [HttpPost]
        public ActionResult CreateZipFileISO2022(SI_USERS suser, string codeproject, int intbasetype, bool devise, string codeJ, string baseName, string listCompte)
        {
            AFB160 aFB160 = new AFB160();
            XmlDocument xmlResult = new XmlDocument();

            var send = "";
            int PROJECTID = int.Parse(codeproject);
            var list = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);

            var ps = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN /*&& x.IDPROJET == PROJECTID*/ && x.PWD == suser.PWD && x.DELETIONDATE == null).Select(x => x.PWD).FirstOrDefault();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var pswftp = db.OPA_CRYPTO.Where(x => x.IDPROJET == PROJECTID && x.IDUSER == suser.ID && x.DELETIONDATE != null).Select(x => x.CRYPTOPWD).FirstOrDefault();
            List<OPA_VALIDATIONS> avalider = new List<OPA_VALIDATIONS>();
            foreach (var item in list)
            {
                int b = int.Parse(item.Numereg);
                avalider.AddRange(db.OPA_VALIDATIONS.Where(a => a.IDPROJET == PROJECTID && a.ETAT == 2 && a.IDREGLEMENT == item.Id && a.NUMEREG == b).ToList());
            };
            var path = "";
            var Nomfichier = "";
            if (avalider != null)
            {
                var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list);
                path = pathfile.Chemin;
                Nomfichier = pathfile.NomFichier + ".xml";
                if (avalider != null)
                {
                    foreach (var item in avalider)
                    {
                        try
                        {
                            item.DATETRANS = DateTime.Now;
                            item.IDUSTRANS = exist.ID;
                            item.ETAT = 3;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                            throw;
                        }
                    }
                }
                if (intbasetype == 0)
                {
                    Anarana = pathfile.Chemin;
                    //XmlDocument xd = new XmlDocument();
                    //xd.LoadXml(Anarana);
                    //return CreateFileAFBXML(pathfile.Chemin, pathfile.Fichier);
                    
                    xmlResult = SaveDocument(Anarana, Anarana);
                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            FileInfo fi = new FileInfo(path);
            string contentType = MimeMapping.GetMimeMapping(path);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Nomfichier,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Text.Xml);
            return File(fileBytes , System.Net.Mime.MediaTypeNames.Application.Octet);
            //return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Archivage avec succès. ", data = xmlResult  }, settings));
        }
        [HttpPost]
        public JsonResult FileName()
        {
            return Json(JsonConvert.SerializeObject(new { Filename = Anarana }, settings));
        }
        public ActionResult GetFile(string file)
        {
            try
            {
                if (file.Contains(".zip"))
                {
                    string path = file;
                    var fs = new FileStream(path, FileMode.Open);
                    return File(fs, "application/x-7z-compressed", file.Split('\\').Last().Split('.').First() + ".zip");
                }
                else
                {
                    string path = file;
                    var fs = new FileStream(path, FileMode.Open);
                    return File(fs, "application/txt", file.Split('\\').Last().Split('.').First() + ".txt");
                }

            }
            catch { return null; }
        }
        public class AnalY
        {
            public string Numero { get; set; }
            public string Datedordre { get; set; }
            public string NumPiece { get; set; }
            public string Compte { get; set; }
            public string Libelle { get; set; }
            public string debit { get; set; }
            public string credit { get; set; }
            public string Montadevise { get; set; }
            public string Mon { get; set; }
            public string Rang { get; set; }
            public string FinCat { get; set; }
            public string Comm { get; set; }
            public string Plan6 { get; set; }
            public string Journal { get; set; }
            public string Marche { get; set; }
        }
        //=========================================================================================================PaiementsValidations============================================================================
        [HttpPost]
        public JsonResult Getelementjs(int ChoixBase, string codeproject, string journal, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var site = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == exist.IDPROJET).Select(ST => ST.SITE).ToList();

            var basename = GetTypeP(suser, codeproject);
            int PROJECTID = int.Parse(codeproject);

            if (basename == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            if (basename == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            if (codeproject != "")
            {
                PROJECTID = int.Parse(codeproject);
            }
            else
            {
                PROJECTID = exist.IDPROJET.Value;
            }

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            AFB160 afb160 = new AFB160();

            //var hst = db.OPA_HISTORIQUE.Select(x => x.NUMENREG.ToString()).ToArray();
            var hstSiig = db.OPA_VALIDATIONS.Where(x => x.ETAT != 4 && x.IDPROJET == PROJECTID).Select(x => x.IDREGLEMENT.ToString()).ToArray();
            var list = afb160.getListEcritureCompta(journal, PROJECTID, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => !hstSiig.Contains(x.No.ToString())).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = list }, settings));

        }
        [HttpPost]
        public JsonResult getelementjsBR(int ChoixBase, string codeproject, string journal, DateTime datein, bool devise, DateTime dateout, string comptaG, string auxi, string auxi1, string etat, DateTime dateP,/* int mois, int annee, string matr1, string matr2, DateTime datePaie,*/ SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            int PROJECTID = int.Parse(codeproject);
            List<string>site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            var basename = GetTypeP(suser, codeproject);
            //GetAnomalieTomOP(suser, journal, codeproject, datein, dateout, comptaG, auxi, siteS);
            if (basename == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            if (codeproject != "")
            {
                PROJECTID = int.Parse(codeproject);
            }
            else
            {
                PROJECTID = exist.IDPROJET.Value;
            }

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            AFB160 afb160 = new AFB160();

            RJL1 djournal = (from jrnl in tom.RJL1
                             where jrnl.CODE == journal && jrnl.JLTRESOR == true && jrnl.NATURE == "2"
                             select jrnl).Single();

            if (djournal.RIB == null || djournal.RIB == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez remplir les comptes RIB du " + journal + " journal. ", data = "" }, settings));
            }
            if (djournal.RIB.Length < 11)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Votre RIB du journal " + journal + " est incomplet ", data = "" }, settings));
            }

            var hstSiig = db.OPA_VALIDATIONS.Where(x => x.ETAT != 4 && x.IDPROJET == PROJECTID).Select(x => new
            {
                IDREGLEMENT = x.IDREGLEMENT.ToString(),
                NUMEREG = x.NUMEREG.ToString(),
            }).ToList();
            //var hstSiig = db.OPA_VALIDATIONS.Where(x => x.ETAT != 4 && x.IDPROJET == PROJECTID).Select(x => x.IDREGLEMENT.ToString()).ToArray();
            List<DataListTomOP> list = new List<DataListTomOP>();
            List<DataListTomOP> tempList = new List<DataListTomOP>();
            List<string> concat = new List<string>();
            if (hstSiig.Count != 0)
            {
                foreach (var item in hstSiig)
                {
                    List<DataListTomOP> sss = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site).Where(x => (!item.IDREGLEMENT.Contains(x.No) && !item.NUMEREG.Contains(x.NUMEREG.ToString())) || (item.IDREGLEMENT.Contains(x.No) && !item.NUMEREG.Contains(x.NUMEREG.ToString())) || x.No != item.IDREGLEMENT).DistinctBy(x => x.No).ToList();

                    foreach (var s1 in sss)
                    {
                        List<DataListTomOP> dlt = (from dp in list
                                                   where dp.No == s1.No && dp.NUMEREG == s1.NUMEREG
                                                   select dp).ToList();
                        if (dlt.Count() == 0)
                        {
                            list.Add(s1);
                        }
                    }
                }
            }
            else
            {
                list.AddRange(afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site).ToList());
            }


            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = list }, settings));

        }
        [HttpPost]
        public JsonResult GetEtat(string codeproject, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            //var basename = GetTypeP(suser, codeproject);

            int PROJECTID = int.Parse(codeproject);

            if (codeproject != "")
            {
                PROJECTID = int.Parse(codeproject);
            }
            else
            {
                PROJECTID = exist.IDPROJET.Value;
            }

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            List<string> listEtat = __db.OP_CHAINETRAITEMENT.Select(x => x.ETAT).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "sucess", msg = "", data = listEtat }));
        }
        [HttpPost]
        public JsonResult GetCODEJournal(string codeproject, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            var basename = GetTypeP(suser, codeproject);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            int crpt = 0;
            if (codeproject != "")
            {
                crpt = int.Parse(codeproject);
            }
            else
            {
                crpt = exist.IDPROJET.Value;
            }

            //Check si le projet est mappé à une base de données TOM²PRO//
            if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            var JournalVM = tom.RJL1.Where(x => x.JLTRESOR == true && x.NATURE == "2").Select(x => new
            {
                CODE = x.CODE,
                LIBELLE = x.LIBELLE
            }).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = JournalVM }, settings));
        }
        [HttpPost]
        public JsonResult GetCompteG(SI_USERS suser , string codeproject)
        {
            int PROJECTID = int.Parse(codeproject);
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            var basename = GetTypeP(suser, exist.IDPROJET.ToString());
            if (exist.IDPROJET == 0)
            {
                basename = db.SI_TYPECRITURE.Where(x => x.IDPROJET == PROJECTID).Select(x => x.TYPE).FirstOrDefault().ToString();
            }
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            int crpt = 0;
            if (codeproject != "")
            {
                crpt = int.Parse(codeproject);
            }
            else
            {
                crpt = exist.IDPROJET.Value;
            }

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            if (basename == "1")
            {
                var CompteGBR = __db.MOP.Where(a => a.COGEFOURNISSEUR.StartsWith("4")).GroupBy(x => x.COGEFOURNISSEUR).Select(x => new
                {
                    COGE = x.Key,
                    AUXI = x.Select(y => y.AUXIFOURNISSEUR/*new { AUXI = y.AUXI, NOM = y.NOM }*/).Distinct().ToList()
                }).ToList();
                var CompteAvance = __db.GA_AVANCE.Where(a => a.COGE.StartsWith("4")).GroupBy(x => x.COGE).Select(x => new
                {
                    COGE = x.Key,
                    AUXI = x.Select(y => y.AUXI).Distinct().ToList()
                }).ToList();

                var compteGSiig = db.OPA_VALIDATIONS.Where(x => x.ComptaG != null).GroupBy(x => x.ComptaG).Select(x => new
                {
                    COGE = x.Key ,
                    AUXI = x.Select(y => y.auxi).Distinct().ToList()
                }).ToList();

                if (CompteGBR.Count != 0)
                {
                    var CompteGtmp = CompteGBR.Union(compteGSiig).ToList();
                    var listCompteGENERALE  = CompteGtmp.Union(CompteAvance).GroupBy(x => x.COGE).Select(x => new
                    {
                        COGE = x.Key,
                        AUXI = x.Select(y => y.AUXI).Distinct().ToList()
                    }).ToList();

                    //var listCompteGENERALE  = CompteGBR.Union(CompteAvance).GroupBy(x => x.COGE).Select(x => new
                    //{
                    //    COGE = x.Key,
                    //    AUXI = x.Select(y => y.AUXI).Distinct().ToList()
                    //}).ToList();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = listCompteGENERALE }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = CompteAvance }, settings));
                }
            }
            else
            {
                var CompteG = __db.MCOMPTA.Where(a => a.COGE.StartsWith("4")).GroupBy(x => x.COGE).Select(x => new
                {
                    COGE = x.Key,
                    AUXI = x.Select(y => y.AUXI).Distinct().ToList()
                }).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = CompteG }, settings));
            }

        }
      
        [HttpPost]
        public JsonResult GetCheckedCompte(string baseName, string codeproject, DateTime datein, DateTime dateout, string comptaG, string auxi, DateTime dateP, string listCompte, string journal, string etat, bool devise, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            var basename = GetTypeP(suser, codeproject);
            int PROJECTID = int.Parse(codeproject);
            List<string>site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));

            foreach (var item in siteS.Split(','))
            {
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes). " }, settings));
            }
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }
            #region 
            //if (exist.IDPROJET == 0)
            //{
            //    var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID).Select(ST => ST.SITE).FirstOrDefault();
            //    foreach (var item in siteS.Split(','))
            //    {
            //        site.Add(item);
            //    }
            //}
            //else
            //{
            //    var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            //    foreach (var item in siteS.Split(','))
            //    {
            //        site.Add(item);
            //    }
            //}
            #endregion
            if (basename == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }

            if (string.IsNullOrEmpty(listCompte))
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Séléctionner un ecriture.", data = "" }, settings));

            //List<string> list = listCompte.Split(',').ToList();
            var list = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);

            #region CommOpavi

            #endregion
           
            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;
            //var lien = "http://softwellset.softwell.cloud/softsetformation";
            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID).FirstOrDefault().PROJET;

            string MailAdresse = "";
            string mdpMail = "";


            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();

            if (basename == "2")
            {
                foreach (var item in siteS.Split(','))
                {
                    int countTraitement = 0;
                    string auxi1 = auxi;
                    AFB160 afb160 = new AFB160();
                    var hst = db.OPA_HISTORIQUE.Select(x => x.NUMENREG.ToString()).ToArray();

                    foreach (var h in list)
                    {
                        var listA = afb160.getListEcritureCompta(journal, PROJECTID, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => x.No.ToString() == h.Id).ToList();

                        foreach (var LST in listA)
                        {
                            avalider.IDREGLEMENT = LST.No.ToString();
                            avalider.ETAT = 0;
                            avalider.IDPROJET = PROJECTID;
                            avalider.DateIn = datein;
                            avalider.DateOut = dateout;
                            avalider.ComptaG = comptaG;
                            avalider.auxi = LST.Auxi;
                            avalider.DateP = dateP;
                            avalider.Journal = LST.Journal;
                            avalider.dateOrdre = LST.dateOrdre;
                            avalider.NoPiece = LST.NoPiece;
                            avalider.Compte = LST.Compte;
                            avalider.Libelle = LST.Libelle;
                            avalider.Debit = LST.Debit;
                            avalider.Credit = LST.Credit;
                            avalider.MontantDevise = LST.MontantDevise;
                            avalider.Mon = LST.Mon;
                            avalider.Rang = LST.Rang;
                            avalider.Poste = LST.Poste;
                            avalider.FinancementCategorie = LST.FinancementCategorie;
                            avalider.Commune = LST.Commune;
                            avalider.Plan6 = LST.Plan6;
                            avalider.Marche = LST.Marche;
                            avalider.Statut = LST.Statut;
                            avalider.DATECREA = DateTime.Now;
                            avalider.IDUSCREA = exist.ID;
                            avalider.AVANCE = false;

                            try
                            {
                                db.OPA_VALIDATIONS.Add(avalider);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                            }
                        }

                        countTraitement++;
                    }
                    
                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD != null)
                    {
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD;
                    }
                    else
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
                    }

                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                        smtp.UseDefaultCredentials = true;

                        mail.From = new MailAddress(MailAdresse);

                        mail.To.Add(MailAdresse);
                        if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPP != null)
                        {
                            string[] separators = { ";" };

                            var Tomail = mail;
                            if (Tomail != null)
                            {
                                string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPP;
                                string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var mailto in mailListe)
                                {
                                    mail.To.Add(mailto);
                                }
                            }
                        }

                        mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                        mail.IsBodyHtml = true;
                        mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                            "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                        smtp.EnableSsl = true;

                        try { smtp.Send(mail); }
                        catch (Exception) { }
                    }
                }
            }
            else
            {
                foreach (var item in siteS.Split(','))
                {
                    int countTraitement = 0;
                    MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                    mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;

                    string auxi1 = auxi;
                    AFB160 afb160 = new AFB160();//ty miova
                    var hst = db.OPA_HISTORIQUEBR.Where(x => x.SITE == item).Select(x => x.NUMENREG.ToString()).ToArray();
                    foreach (var h in list)
                    {
                        int a = int.Parse(h.Numereg);
                        
                        var listA = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site).Where(x => x.No.ToString() == h.Id && x.NUMEREG == a && x.SITE == item).ToList();
                        foreach (var Lst in listA)
                        {
                            avalider.IDREGLEMENT = Lst.No;
                            avalider.ETAT = 0;
                            avalider.IDPROJET = PROJECTID;
                            avalider.DateIn = datein;
                            avalider.DateOut = dateout;
                            avalider.ComptaG = Lst.CogeFourniseur;
                            avalider.auxi = Lst.Auxi;
                            avalider.DateP = dateP;
                            avalider.Journal = Lst.Journal;
                            avalider.dateOrdre = Lst.Date;
                            avalider.NoPiece = Lst.NoPiece;
                            avalider.Compte = Lst.Compte;
                            avalider.Libelle = Lst.Libelle;
                            avalider.MONTANT = Convert.ToDecimal(couperText(18, Lst.Montant.ToString()));
                            avalider.MontantDevise = Lst.MontantDevise;
                            avalider.Mon = Lst.Mon;
                            avalider.Rang = Lst.Rang;
                            avalider.Poste = Lst.Poste;
                            avalider.FinancementCategorie = Lst.FinancementCategorie;
                            avalider.Commune = Lst.Commune;
                            avalider.Plan6 = Lst.Plan6;
                            avalider.Marche = Lst.Marche;
                            avalider.Statut = Lst.Status;
                            avalider.DATECREA = DateTime.Now;
                            avalider.IDUSCREA = exist.ID;
                            avalider.AVANCE = Lst.Avance;
                            avalider.NUMEROLIQUIDATION = Lst.Mandat;
                            avalider.NUMEREG = Lst.NUMEREG;
                            avalider.AUTREOP = Lst.AUTREOPERATIONS;
                            avalider.SITE = Lst.SITE;
                            try
                            {
                                db.OPA_VALIDATIONS.Add(avalider);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                        countTraitement++;
                    }

                    //SEND MAIL ALERT et NOTIFICATION//
                    //string MailAdresse = "serviceinfo@softwell.mg";
                    // string mdpMail = "09eYpçç0601";

                    if (countTraitement > 0)
                    {
                            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                             {
                                    SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                                    smtp.UseDefaultCredentials = true;

                                    mail.From = new MailAddress(MailAdresse);

                                    mail.To.Add(MailAdresse);
                                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPE != null)
                                    {
                                        string[] separators = { ";" };

                                        var Tomail = mail;
                                        if (Tomail != null)
                                        {
                                            string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPE;
                                            string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                            foreach (var mailto in mailListe)
                                            {
                                                mail.To.Add(mailto);
                                            }
                                        }
                                    }

                                    mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                    mail.IsBodyHtml = true;
                                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                                    smtp.Port = 587;
                                    smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                                    smtp.EnableSsl = true;

                                    try { smtp.Send(mail); }
                                    catch (Exception) { }
                             }
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Tratement avec succès. ", data = "" }, settings));
        }
        //=========================================================================================TeacherValidation======================================================================
        [HttpPost]
        public JsonResult GetElementAvalider(string ChoixBase, string codeproject, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            int PROJECTID = int.Parse(codeproject);
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPE.Value;

            if (comptaG == "Autre Opérations") comptaG = null;

            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();
            if (ChoixBase == "2")
            {
                var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG).ToArray();
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 0 && ecriture.dateOrdre >= datein && ecriture.dateOrdre <= dateout.Date && /*(ecriture.Compte == comptaG || ecriture.Compte == "")*/ ecriture.ComptaG == comptaG && ecriture.Journal == journal && !HistoAFB.Contains(ecriture.IDREGLEMENT.ToString())).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATEVAL.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;
                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        auxi = item.auxi,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Libelle = item.Libelle,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
            else
            {
                //var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG).ToArray();
                //var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => new
                //{
                //    No = x.NUMENREG,
                //    NUMREG = x.NUMREG.ToString()
                //}).ToList();
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 0 && ecriture.ComptaG == comptaG && site.Contains(ecriture.SITE) && ecriture.Journal == journal && ecriture.dateOrdre >= datein && ecriture.dateOrdre <= dateout).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATECREA.Value.AddBusinessDays(retarDate).Date < DateTime.Now.Date/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;
                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        auxi = item.auxi,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Libelle = item.Libelle,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        FinancementCategorie = item.FinancementCategorie,
                        MONTANT = item.MONTANT,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                        NUMEREG = item.NUMEREG
                    });
                }

                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }

        }

        [HttpPost]
        public JsonResult GetElementAvaliderLoad(SI_USERS suser, string codeproject)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            int PROJECTID = int.Parse(codeproject);

            List<string>site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            var basename = GetTypeP(suser, codeproject);
            
            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPE.Value;

            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();
            if (basename == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            if (basename == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 0 && site.Contains(ecriture.SITE)).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATECREA.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        auxi = item.auxi,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Libelle = item.Libelle,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        FinancementCategorie = item.FinancementCategorie,
                        MONTANT = item.MONTANT,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        NUMEREG = item.NUMEREG,
                    });
                }
                //var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 0).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés.", data = list }, settings));
            }
            else
            {
                //var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => new
                //{
                //    No = x.NUMENREG,
                //    NUMREG = x.NUMREG.ToString()
                //}).ToList();
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 0 && site.Contains(ecriture.SITE)).ToList();

                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATECREA.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;
                    if (item.AVANCE == true)
                    {
                        list.Add(new OPA_VALIDATIONS
                        {
                            IDREGLEMENT = item.IDREGLEMENT,
                            dateOrdre = item.dateOrdre,
                            auxi = item.auxi,
                            NoPiece = item.NoPiece,
                            Compte = item.Compte,
                            Journal = item.Journal,
                            Libelle = item.Libelle,
                            Credit = item.Credit,
                            Debit = item.Debit,
                            FinancementCategorie = item.FinancementCategorie,
                            MONTANT = item.MONTANT,
                            Mon = item.Mon,
                            MontantDevise = item.MontantDevise,
                            Rang = item.Rang,
                            Plan6 = item.Plan6,
                            Commune = item.Commune,
                            Marche = item.Marche,
                            isLATE = isLate,
                            AVANCE = item.AVANCE,
                            NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                            NUMEREG = item.NUMEREG,
                            AUTREOP = item.AUTREOP,
                            SITE = item.SITE,
                        });
                    }
                    else
                    {
                        list.Add(new OPA_VALIDATIONS
                        {
                            IDREGLEMENT = item.IDREGLEMENT,
                            dateOrdre = item.dateOrdre,
                            auxi = item.auxi,
                            NoPiece = item.NoPiece,
                            Compte = item.Compte,
                            Journal = item.Journal,
                            Libelle = item.Libelle,
                            Credit = item.Credit,
                            Debit = item.Debit,
                            FinancementCategorie = item.FinancementCategorie,
                            MONTANT = item.MONTANT,
                            Mon = item.Mon,
                            MontantDevise = item.MontantDevise,
                            Rang = item.Rang,
                            Plan6 = item.Plan6,
                            Commune = item.Commune,
                            Marche = item.Marche,
                            isLATE = isLate,
                            AVANCE = item.AVANCE,
                            NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                            NUMEREG = item.NUMEREG,
                            AUTREOP = item.AUTREOP,
                            SITE = item.SITE,
                        });
                    }
                }
               
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés.", data = list }, settings));
            }

        }
        //=========================================================================================TeacherValidation======================================================================

        [HttpPost]
        public JsonResult GetElementValiderF(string listCompte, SI_USERS suser, string codeproject)
        {
            AFB160 aFB160 = new AFB160();
            if (codeproject == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez choisir un projet avant toutes actions. " }, settings));
            }
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int PROJECTID = int.Parse(codeproject);

            string MailAdresse = "";
            string mdpMail = "";

            List<string> site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();

            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));

            foreach (var item in siteS.Split(','))
            {
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes). " }, settings));
            }

            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            bool devise = false;

            var basename = GetTypeP(suser, codeproject);

            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }
            //List<string> list = listCompte.Split(',').ToList();
            //List<string> numBR = listCompte.Split(',').ToList();
            var list = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);
            var numBR = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);

            var AvaliderList = new List<OPA_VALIDATIONS>();
            int numeroReg = 0;
            foreach (var item in list)
            {
                numeroReg = int.Parse(item.Numereg);
                AvaliderList.Add(db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == item.Id && a.ETAT == 0 && a.NUMEREG == numeroReg).FirstOrDefault());
            }

          
            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;

            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID).FirstOrDefault().PROJET;

            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            int ComptablePayeur = int.Parse(Session["PROCESDEPS"].ToString());
            int? Applicable = db.SI_TYPEPROCESSUS.FirstOrDefault(x => x.IDPROJET == PROJECTID && x.DELETIONDATE == null).VALPAIEMENTS;
            
            if (ComptablePayeur == 2)
            {
                Applicable = 2;
            }

            if (Applicable == 2)
            {
                if (basename == "2")
                {
                    aFB160.SaveValideSelectEcriture(list, true, suser, codeproject,site);
                }
                else
                {
                    foreach (var item in AvaliderList)
                    {
                        aFB160.SaveValideSelectEcritureBR(item.IDREGLEMENT,item.NUMEREG.ToString(), item.Journal, item.ETAT.ToString(), devise, suser, PROJECTID, (bool)item.AVANCE,site);
                    }

                }
                if (basename == "2")
                {
                    foreach (var item in siteS.Split(','))
                    {
                        int countTraitement = 0;
                        foreach (var Lt in list)
                        {
                            int b = int.Parse(Lt.Id);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString() && a.ETAT == 0 && a.SITE == item).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.DATEACCEPT = DateTime.Now;
                                    avalider.IDUSSEND = exist.ID;
                                    avalider.ETAT = 1;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                   
                }
                else
                {
                    foreach (var item in siteS.Split(','))
                    {
                        int countTraitement = 0;
                        foreach (var Lt in numBR)
                        {
                            
                            //int b = int.Parse(item);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == Lt.Id && a.ETAT == 0 && a.SITE == item && a.NUMEREG == numeroReg).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.DATEACCEPT = DateTime.Now;
                                    avalider.IDUSSEND = exist.ID;
                                    avalider.ETAT = 1;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                    
                }
            }
            else
            {//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Non applicable////////////////////////////////////////////////////////////////////////
                if (basename == "2")
                {
                    aFB160.SaveValideSelectEcriture(list, true, suser, codeproject, site);
                }
                else
                {
                    foreach (var item in AvaliderList)
                    {
                        aFB160.SaveValideSelectEcritureBR(item.IDREGLEMENT,item.NUMEREG.ToString(), item.Journal, item.ETAT.ToString(), devise, suser, PROJECTID, (bool)item.AVANCE, site);
                    }

                }

                List<DataListTompro> listReg = new List<DataListTompro>();
                List<DataListTompro> listReg__ = new List<DataListTompro>();
                List<DataListTomOP> listRegBR = new List<DataListTomOP>();
                List<DataListTomOP> listRegBR__ = new List<DataListTomOP>();

                if (basename == "2")
                {
                    listReg = aFB160.getREGLEMENT(suser, PROJECTID,site);
                }
                else
                {
                    listRegBR = aFB160.getREGLEMENTBR(suser,numeroReg, PROJECTID,site);
                }

                if (basename == "2")
                {
                    foreach (var item in siteS.Split(','))
                    {
                        int countTraitement = 0;
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;
                        foreach (var Lt in list)
                        {
                            int b = int.Parse(Lt.Id);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString() && a.ETAT == 0 && a.SITE == item && a.NUMEREG == numeroReg).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.DATEACCEPT = DateTime.Now;
                                    avalider.IDUSSEND = exist.ID;
                                    avalider.ETAT = 1;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                }
                else
                {
                    foreach (var item in siteS.Split(','))
                    {
                        int countTraitement = 0;
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;
                        foreach (var Lt in numBR)
                        {
                            //int b = int.Parse(item);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == Lt.Id && a.ETAT == 0 && a.SITE == item && a.NUMEREG == numeroReg).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.DATEACCEPT = DateTime.Now;
                                    avalider.IDUSSEND = exist.ID;
                                    avalider.ETAT = 1;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                }

                //OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
                if (basename == "2")
                {
                    foreach (var item in siteS.Split(','))
                    { 
                        int countTraitement = 0;
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;
                        foreach (var Lt in list)
                        {
                            int b = int.Parse(Lt.Id);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString() && a.ETAT == 1 && a.SITE == item).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.IDREGLEMENT = b.ToString();
                                    avalider.ETAT = 2;
                                    avalider.DATESEND = DateTime.Now.Date;
                                    avalider.IDPROJET = PROJECTID;
                                    avalider.DateIn = avalider.DateIn;
                                    avalider.DateOut = avalider.DateOut;
                                    avalider.ComptaG = avalider.ComptaG;
                                    avalider.auxi = avalider.auxi;
                                    avalider.DateP = avalider.DateP;
                                    avalider.Journal = avalider.Journal;
                                    avalider.DATEVAL = DateTime.Now;
                                    avalider.IDUSVAL = exist.ID;
                                    avalider.SITE = avalider.SITE;

                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                                if (basename == "2")
                                {
                                    listReg__.Add(listReg.Where(a => (int)a.No == int.Parse(Lt.Id)).FirstOrDefault());
                                }
                                else
                                {

                                    listRegBR__.Add(listRegBR.Where(a => a.No == Lt.Id).FirstOrDefault());
                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                }
                else
                {
                    foreach (var item in siteS.Split(','))
                    {
                        int countTraitement = 0;
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;

                        foreach (var Lt in AvaliderList)
                        {
                            //int b = int.Parse(item);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == Lt.IDREGLEMENT && a.ETAT == 1 && a.SITE == item && a.NUMEREG == Lt.NUMEREG).FirstOrDefault();
                            if (avalider != null)
                            {
                                try
                                {
                                    avalider.IDREGLEMENT = Lt.IDREGLEMENT;
                                    avalider.ETAT = 2;
                                    avalider.DATESEND = DateTime.Now.Date;
                                    avalider.IDPROJET = PROJECTID;
                                    avalider.DateIn = avalider.DateIn;
                                    avalider.DateOut = avalider.DateOut;
                                    avalider.ComptaG = avalider.ComptaG;
                                    avalider.auxi = avalider.auxi;
                                    avalider.DateP = avalider.DateP;
                                    avalider.Journal = avalider.Journal;
                                    avalider.DATEVAL = DateTime.Now;
                                    avalider.IDUSVAL = exist.ID;
                                    avalider.SITE = avalider.SITE;
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                    throw;
                                }
                                if (basename == "2")
                                {
                                    listReg__.Add(listReg.Where(a => (int)a.No == int.Parse(Lt.IDREGLEMENT)).FirstOrDefault());
                                }
                                else
                                {
                                    listRegBR__.Add(listRegBR.Where(a => a.No == Lt.IDREGLEMENT).FirstOrDefault());

                                }
                            }
                            countTraitement++;
                        }
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                            smtp.UseDefaultCredentials = true;

                            mail.From = new MailAddress(MailAdresse);

                            mail.To.Add(MailAdresse);
                            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                            {
                                string[] separators = { ";" };

                                var Tomail = mail;
                                if (Tomail != null)
                                {
                                    string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                                    string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var mailto in mailListe)
                                    {
                                        mail.To.Add(mailto);
                                    }
                                }
                            }
                            if (Applicable == 2)
                            {
                                mail.Subject = "Attente validation paiements du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiements en attente validation pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            else
                            {
                                mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                                mail.IsBodyHtml = true;
                                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                                    "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                            }
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                            smtp.EnableSsl = true;

                            try { smtp.Send(mail); }
                            catch (Exception) { }
                        }
                    }
                   
                }
            }
         
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = "" }, settings));
        }
        //ETAT = 1
        //=========================================================================================TeacherValidation======================================================================

        [HttpPost]
        public JsonResult GetAcceptecriture(string ChoixBase, string codeproject, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            int PROJECTID = int.Parse(codeproject);
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (comptaG == "Autre Opérations") comptaG = null;
            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPV.Value;
            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();

            var basename = GetTypeP(suser, codeproject);
            if (basename == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 1 && ecriture.ComptaG == comptaG && ecriture.auxi == auxi && ecriture.Journal == journal).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATEACCEPT.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        auxi = item.auxi,
                        Compte = item.Compte,
                        Libelle = item.Libelle,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        NUMEREG = item.NUMEREG,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 1 && ecriture.ComptaG == comptaG && ecriture.Journal == journal).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATEACCEPT.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        auxi = item.auxi,
                        Compte = item.Compte,
                        Libelle = item.Libelle,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        NUMEREG = item.NUMEREG,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
        }
        [HttpPost]
        public JsonResult GetAcceptecritureLoad(SI_USERS suser, string codeproject)
        {
            AFB160 aFB160 = new AFB160();
            int PROJECTID = int.Parse(codeproject);
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var basename = GetTypeP(suser, codeproject);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }

            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPV.Value;
            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();

            if (basename == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 1).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATEACCEPT.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        auxi = item.auxi,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        Libelle = item.Libelle,
                        FinancementCategorie = item.FinancementCategorie,
                        MONTANT = item.MONTANT,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        NUMEREG = item.NUMEREG,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 1).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATECREA.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        auxi = item.auxi,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        Libelle= item.Libelle,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MONTANT = item.MONTANT,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        NUMEREG = item.NUMEREG,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
        }

        [HttpPost]
        public JsonResult GetAcceptecritureF(string listCompte, SI_USERS suser, string codeproject)
        {//validations
            AFB160 aFB160 = new AFB160();
            List<string> list = listCompte.Split(',').ToList();
            List<string> numBR = listCompte.Split(',').ToList();
            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            int PROJECTID = int.Parse(codeproject);
            int countTraitement = 0;
            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;
            //var lien = "http://softwellset.softwell.cloud/softsetformation";

            foreach (var item in list)
            {
                int b = int.Parse(item);
                avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString()).FirstOrDefault();
                if (avalider != null)
                {
                    try
                    {
                        avalider.IDREGLEMENT = item;
                        avalider.ETAT = 2;
                        avalider.DATESEND = DateTime.Now;
                        avalider.IDPROJET = suser.IDPROJET;
                        avalider.DateIn = avalider.DateIn;
                        avalider.DateOut = avalider.DateOut;
                        avalider.ComptaG = avalider.ComptaG;
                        avalider.auxi = avalider.auxi;
                        avalider.DateP = avalider.DateP;
                        avalider.Journal = avalider.Journal;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion. ", data = ex.Message }, settings));
                        throw;
                    }
                }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = "" }, settings));
        }
        //======================================================================================================EnvoyerValidations============================================================

        [HttpPost]
        public JsonResult EnvoyeValidatioF(string ChoixBase, string codeproject, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            // var basename = GetTypeP(suser, exist.IDPROJET.ToString());

            int PROJECTID = int.Parse(codeproject);

            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPP.Value;
            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();

            if (ChoixBase == "2")
            {
                var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG).ToArray();
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 2 && /*(ecriture.ComptaG == comptaG || ecriture.ComptaG == null)*/ ecriture.ComptaG == comptaG && ecriture.Journal == journal && !HistoAFB.Contains(ecriture.IDREGLEMENT.ToString())).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATESEND.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                        NUMEREG = item.NUMEREG
                    });
                }
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = list }, settings));
            }
            else
            {
                //var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG.ToString()).ToArray();
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == PROJECTID && ecriture.ETAT == 2 && /*(ecriture.ComptaG == comptaG || ecriture.ComptaG == null)*/ ecriture.ComptaG == comptaG && ecriture.Journal == journal /*&& !HistoAFB.Contains(ecriture.IDREGLEMENT.ToString())*/).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                foreach (var item in avalider)
                {
                    bool isLate = false;
                    if (item.DATESEND.Value.AddBusinessDays(retarDate - 1).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        AVANCE = item.AVANCE,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                        NUMEREG = item.NUMEREG
                    });
                }
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés.  ", data = list }, settings));
            }
        }
        //======================================================================================================ValidationsEcrituresF=========================================================

        [HttpPost]
        public JsonResult ValidationsEcrituresF(string baseName, string codeproject, string listCompte, SI_USERS suser)
        {
            List<DataListTompro> listReg = new List<DataListTompro>();
            List<DataListTompro> listReg__ = new List<DataListTompro>();
            List<DataListTomOP> listRegBR = new List<DataListTomOP>();
            List<DataListTomOP> listRegBR__ = new List<DataListTomOP>();
            AFB160 aFB160 = new AFB160();
            int PROJECTID = int.Parse(codeproject);
            int numeroreg = 0;

            int countTraitement = 0;
            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;
            //var lien = "http://softwellset.softwell.cloud/softsetformation";
            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID && a.DELETIONDATE == null).FirstOrDefault().PROJET;

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

           // var site = db.SI_SITE.Where(a => a.IDUSER == exist.ID && a.IDPROJET ==  exist.IDPROJET).Select(a => a.SITE).ToList();
            List<string> site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();

            if (siteS == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer votre site. " }, settings));

            foreach (var item in siteS.Split(','))
            {
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes). " }, settings));
            }

            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }
            baseName = GetTypeP(suser, codeproject);
            if (baseName == "")
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez paramétrer le type d'ecriture avant toutes opérations. " }, settings));
            }

            //List<string> list = listCompte.Split(',').ToList();
            var list = JsonConvert.DeserializeObject<List<AvanceDetails>>(listCompte);
            foreach (var item in list)
            {
                numeroreg = int.Parse(item.Numereg);
            }
            if (baseName == "2")
            {
                listReg = aFB160.getREGLEMENT(suser, PROJECTID,site);
            }
            else
            {
                listRegBR = aFB160.getREGLEMENTBR(suser,numeroreg, PROJECTID,site);
            }

            var AvaliderList = new List<OPA_VALIDATIONS>();

            foreach (var item in list)
            {
                int numereg = int.Parse(item.Numereg);
                AvaliderList.Add(db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == item.Id && a.ETAT == 1 && site.Contains(a.SITE) && a.NUMEREG == numereg).FirstOrDefault());
            }

            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            if (baseName == "2")
            {
                string MailAdresse = "";
                string mdpMail = "";
                foreach (var item in siteS.Split(','))
                {
                    countTraitement = 0;

                    MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                    mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;

                    foreach (var Lt in list)
                    {
                        int b = int.Parse(Lt.Id);
                        int numereg = int.Parse(Lt.Numereg);
                        avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString() && a.SITE == item && a.NUMEREG == numereg).FirstOrDefault();
                       // avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b.ToString() && site.Contains(a.SITE) && a.NUMEREG == numereg).FirstOrDefault();
                        if (avalider != null)
                        {
                            try
                            {
                                avalider.IDREGLEMENT = b.ToString();
                                avalider.ETAT = 2;
                                avalider.DATESEND = DateTime.Now.Date;
                                avalider.IDPROJET = PROJECTID;
                                avalider.DateIn = avalider.DateIn;
                                avalider.DateOut = avalider.DateOut;
                                avalider.ComptaG = avalider.ComptaG;
                                avalider.auxi = avalider.auxi;
                                avalider.DateP = avalider.DateP;
                                avalider.Journal = avalider.Journal;
                                avalider.DATEVAL = DateTime.Now;
                                avalider.IDUSVAL = exist.ID;

                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                            if (baseName == "2")
                            {
                                listReg__.Add(listReg.Where(a => (int)a.No == int.Parse(Lt.Id)).FirstOrDefault());
                            }
                            else
                            {

                                listRegBR__.Add(listRegBR.Where(a => a.No == Lt.Id).FirstOrDefault());
                            }
                        }
                        countTraitement++;
                    }
                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                        smtp.UseDefaultCredentials = true;

                        mail.From = new MailAddress(MailAdresse);

                        mail.To.Add(MailAdresse);
                        if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPV != null)
                        {
                            string[] separators = { ";" };

                            var Tomail = mail;
                            if (Tomail != null)
                            {
                                string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPV;
                                string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var mailto in mailListe)
                                {
                                    mail.To.Add(mailto);
                                }
                            }
                        }

                        mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                        mail.IsBodyHtml = true;
                        mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                            "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                        smtp.EnableSsl = true;

                        try { smtp.Send(mail); }
                        catch (Exception) { }
                    }
                }
               
            }
            else
            {
                string MailAdresse = "";
                string mdpMail = "";
                foreach (var item in list)
                {
                    //int b = int.Parse(item);
                    int numereg = int.Parse(item.Numereg);
                    avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == item.Id && site.Contains(a.SITE) && a.NUMEREG == numereg).FirstOrDefault();
                    if (avalider != null)
                    {
                        try
                        {
                            avalider.IDREGLEMENT = item.Id;
                            avalider.ETAT = 2;
                            avalider.DATESEND = DateTime.Now.Date;
                            avalider.IDPROJET = PROJECTID;
                            avalider.DateIn = avalider.DateIn;
                            avalider.DateOut = avalider.DateOut;
                            avalider.ComptaG = avalider.ComptaG;
                            avalider.auxi = avalider.auxi;
                            avalider.DateP = avalider.DateP;
                            avalider.Journal = avalider.Journal;
                            avalider.DATEVAL = DateTime.Now;
                            avalider.IDUSVAL = exist.ID;

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                            throw;
                        }
                        if (baseName == "2")
                        {
                            listReg__.Add(listReg.Where(a => (int)a.No == int.Parse(item.Id)).FirstOrDefault());
                        }
                        else
                        {

                            listRegBR__.Add(listRegBR.Where(a => a.No == item.Id).FirstOrDefault());
                        }
                    }
                    countTraitement++;
                }
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                    smtp.UseDefaultCredentials = true;

                    mail.From = new MailAddress(MailAdresse);

                    mail.To.Add(MailAdresse);
                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPV != null)
                    {
                        string[] separators = { ";" };

                        var Tomail = mail;
                        if (Tomail != null)
                        {
                            string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILPV;
                            string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var mailto in mailListe)
                            {
                                mail.To.Add(mailto);
                            }
                        }
                    }

                    mail.Subject = "Validation paiement du projet " + ProjetIntitule;
                    mail.IsBodyHtml = true;
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez " + countTraitement + " paiement valider pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                    smtp.EnableSsl = true;

                    try { smtp.Send(mail); }
                    catch (Exception) { }
                }
            }

            if (baseName == "2")
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = listReg__ }, settings));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = listRegBR__ }, settings));
            }

        }
        //END ETAT = 1
        //======================================================================================================Fvalidations=================================================================
        [HttpPost]
        public JsonResult LoadValidateEcriture(SI_USERS suser, string codeproject)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            //var site = db.SI_SITE.Where(a => a.IDUSER == exist.ID && a.IDPROJET == exist.IDPROJET).Select(a => a.SITE).ToList();

            int PROJECTID = int.Parse(codeproject);

            List<string>site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }
            int retarDate = 0;
            if (db.SI_DELAISTRAITEMENT.Any(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null))
                retarDate = db.SI_DELAISTRAITEMENT.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).DELPP.Value;
            List<OPA_VALIDATIONS> list = new List<OPA_VALIDATIONS>();

            var typeEcriture = db.SI_TYPECRITURE.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault().TYPE;

            if (typeEcriture == 1)
            {
                //var HistoAFB = db.OPA_HISTORIQUEBR.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG).ToArray();
                //var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && !HistoAFB.Contains(a.IDREGLEMENT.ToString())).ToList();
                var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && site.Contains(a.SITE)).ToList();
                foreach (var item in val)
                {
                    bool isLate = false;
                    if (item.DATEVAL.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Libelle = item.Libelle,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        NUMEREG = item.NUMEREG,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        AVANCE = item.AVANCE,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
            }
            else
            {
                //var HistoAFB = db.OPA_HISTORIQUE.Where(a => a.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG.ToString()).ToArray();
                //var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && !HistoAFB.Contains(a.IDREGLEMENT.ToString())).ToList();
                var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && site.Contains(a.SITE)).ToList();
                foreach (var item in val)
                {
                    bool isLate = false;
                    if (item.DATEVAL.Value.AddBusinessDays(retarDate).Date < DateTime.Now/* && ((int)DateTime.Now.DayOfWeek) != 6 && ((int)DateTime.Now.DayOfWeek) != 0*/)
                        isLate = true;

                    list.Add(new OPA_VALIDATIONS
                    {
                        IDREGLEMENT = item.IDREGLEMENT,
                        dateOrdre = item.dateOrdre,
                        NoPiece = item.NoPiece,
                        Compte = item.Compte,
                        Libelle = item.Libelle,
                        Journal = item.Journal,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        MONTANT = item.MONTANT,
                        FinancementCategorie = item.FinancementCategorie,
                        Mon = item.Mon,
                        MontantDevise = item.MontantDevise,
                        Rang = item.Rang,
                        Plan6 = item.Plan6,
                        Commune = item.Commune,
                        Marche = item.Marche,
                        isLATE = isLate,
                        NUMEREG = item.NUMEREG,
                        NUMEROLIQUIDATION = item.NUMEROLIQUIDATION,
                        AVANCE = item.AVANCE,
                        AUTREOP = item.AUTREOP,
                        SITE = item.SITE,
                    });
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "Success", msg = "Traitement avec success. ", data = list }, settings));
        }
        //======================================================================================================Cancel========================================================================
        [HttpPost]
        public JsonResult CancelEcriture(string id, string motif, string commentaire, SI_USERS suser, string codeproject)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int PROJECTID = int.Parse(codeproject);
            var cancel = db.OPA_VALIDATIONS.Where(x => x.IDREGLEMENT == id.ToString() && x.IDPROJET == PROJECTID).FirstOrDefault();
            if (cancel != null)
            {
                if (motif != "")
                {
                    cancel.ETAT = 4;
                    cancel.MOTIF = motif;
                    cancel.COMS = commentaire;
                    cancel.DATEANNULER = DateTime.Now;
                    cancel.IDUSER = exist.ID;
                    db.SaveChanges();

                    var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;
                    //var lien = "http://softwellset.softwell.cloud/softsetformation";
                    var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID && a.DELETIONDATE == null).FirstOrDefault().PROJET;
                    //SEND MAIL ALERT et NOTIFICATION//
                    // string MailAdresse = "serviceinfo@softwell.mg";
                    //string mdpMail = "09eYpçç0601";

                    string MailAdresse = "";
                    string mdpMail = "";

                    if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD != null)
                    {
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD;
                    }
                    else
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
                    }

                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                        smtp.UseDefaultCredentials = true;

                        mail.From = new MailAddress(MailAdresse);

                        mail.To.Add(MailAdresse);
                        if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILREJETPAIE != null)
                        {
                            string[] separators = { ";" };

                            var Tomail = mail;
                            if (Tomail != null)
                            {
                                string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILREJETPAIE;
                                string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var mailto in mailListe)
                                {
                                    mail.To.Add(mailto);
                                }
                            }
                        }

                        mail.Subject = "Rejet paiement du projet " + ProjetIntitule;
                        mail.IsBodyHtml = true;
                        mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez un paiement rejeté pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                            "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";

                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                        smtp.EnableSsl = true;

                        try { smtp.Send(mail); }
                        catch (Exception) { }
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés. ", data = "" }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Motif obligatoire. ", data = "" }, settings));
                }
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succés .", data = "" }, settings));
        }
        public JsonResult GetCheckedComptePaie(string baseName, int mois, int annee, string listCompte, string matriculeD, string matriculeF, bool devise, DateTime dateP, string journal, SI_USERS suser)
        {
            if (string.IsNullOrEmpty(listCompte))
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = "" }, settings));

            List<OPA_REGLEMENTBR> brResult = new List<OPA_REGLEMENTBR>();
            List<string> listReg = new List<string>();
            listReg = listCompte.Split(',').ToList();
            AFB160 aFB160 = new AFB160();
            try
            {

                aFB160.SaveValideSelectEcriturePaie(listReg, journal, devise, suser);
                //var zz = aFB160.getListEcriturePaie(journal, mois, annee, matriculeD, matriculeF, dateP, suser);
                var listePaie = aFB160.getREGLEMENTPaie(suser);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = listePaie }, settings));
            }
            catch (Exception ex)
            {

                return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = ex.Message }, settings));
            }

        }
        //======================================================================================================GetAnomalieBack================================================================
        public JsonResult GetAnomalieBack(SI_USERS suser, string baseName,string codeproject)
        {
            AFB160 Afb = new AFB160();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int PROJECTID = int.Parse(codeproject);

            List<string> site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            var type = db.SI_TYPECRITURE.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault().TYPE;
            if (type == 1)
            {
                var anom = db.OPA_ANOMALIEBR.Where(x => x.NUM == null).ToList();
                var resultAnomalies = Afb.getListAnomalieBR(suser, PROJECTID);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = resultAnomalies, dataAnom = anom }, settings));
            }
            else
            {
                //var anom = db.OPA_ANOMALIE.ToList();
                var resultAnomalies = Afb.getListAnomalie(suser);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = resultAnomalies }, settings));
            }

        }
        //======================================================================================================GetAllProjectUser===============================================================
        public JsonResult GetAllProjectUser(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
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

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès.", data = user }, settings));
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

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès.", data = user }, settings));
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

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès.", data = user }, settings));
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
        //========================================================================================================GGetListAFB===================================================================
        [HttpPost]
        public JsonResult GetListAFB(string listCompte,SI_USERS suser,int PROJECTID)
        {

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès." }, settings));
        }
        //======================================================================================================FTP=============================================================================

        public void SENDFTP(string HOTE, string PATH, string USERFTP, string PWDFTP, string SOURCE)
        {
            DateTime now = DateTime.Now;
            FileStream fs = null;
            Stream rs = null;

            try
            {
                GC.Collect(0);
                int Size = 2048;/*8092*/

                Task T = Task.Run(() =>
                {
                    string file = SOURCE;
                    string uploadFileName = new FileInfo(file).Name;
                    string uploadUrl = String.Format("{0}/{1}/", "ftp://" + HOTE, PATH);
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read);

                    string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                    FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                    requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                    requestObj.Credentials = new NetworkCredential(USERFTP, PWDFTP);

                    //ADD 17h Zoma
                    requestObj.UseBinary = true;
                    requestObj.UsePassive = true;
                    requestObj.KeepAlive = true;
                    //

                    rs = requestObj.GetRequestStream();

                    byte[] buffer = new byte[Size];
                    int read = 0;
                    while ((read = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, read);
                    }
                    rs.Flush();

                });

                if (T.IsCompleted)
                    return;

                T.Wait();

                GC.Collect(0);

                throw new OutOfMemoryException();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                if (rs != null)
                {
                    rs.Close();
                    rs.Dispose();
                }
            }
        }
        [HttpPost]
        public JsonResult GetHistoriques(SI_USERS suser, string codeproject)
        {
            int PROJECTID = int.Parse(codeproject);
            var usr = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.IDPROJET == PROJECTID && x.DELETIONDATE == null ).FirstOrDefault();
            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == usr.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }
            if (usr.IDPROJET == 0)
            {
                var query = db.OPA_HISTORIQUE
                .Join(db.OPA_REGLEMENT, histo => histo.NUMENREG, reglement => reglement.NUM, (histo, reglement) => new
                {
                    NUMENREG = histo.NUMENREG,
                    DATEAFB = histo.DATEAFB,
                    IDUSER = histo.IDUSER,
                    IDSOCIETE = histo.IDSOCIETE,
                    CODE_J = reglement.CODE_J,
                    RIB = reglement.RIB,
                    MONTANT = reglement.MONTANT,
                    DATE = reglement.DATE,
                    LIBELLE = reglement.LIBELLE,
                    BANQUE = reglement.BANQUE,
                    GUICHET = reglement.GUICHET,
                    NOMFICHIER = histo.AFB,
                }).Join(db.SI_USERS, x => x.IDUSER, user => user.ID, (x, user) => new
                {
                    NUMENREG = x.NUMENREG,
                    DATEAFB = x.DATEAFB,
                    IDUSER = x.IDUSER,
                    IDSOCIETE = x.IDSOCIETE,
                    CODE_J = x.CODE_J,
                    RIB = x.RIB,
                    MONTANT = x.MONTANT,
                    DATE = x.DATE,
                    LIBELLE = x.LIBELLE,
                    BANQUE = x.BANQUE,
                    GUICHET = x.GUICHET,
                    LOGIN = user.LOGIN,
                    NOMFICHIER = x.NOMFICHIER,
                })
                .OrderBy(x => x.DATE).ToList();
                var queryBr = db.OPA_HISTORIQUEBR
                    .Where(ST => site.Contains(ST.SITE))
                    .Join(db.OPA_REGLEMENTBR, histo => histo.NUMENREG, reglement => reglement.NUM, (histo, reglement) => new
                    {
                        NUMENREG = histo.NUMENREG,
                        DATEAFB = histo.DATEAFB,
                        IDUSER = histo.IDUSER,
                        IDSOCIETE = histo.IDSOCIETE,
                        CODE_J = reglement.CODE_J,
                        RIB = reglement.RIB,
                        MONTANT = reglement.MONTANT,
                        DATE = reglement.DATE,
                        LIBELLE = reglement.LIBELLE,
                        BANQUE = reglement.BANQUE,
                        GUICHET = reglement.GUICHET,
                        SITE = histo.SITE,
                        NOMFICHIER = histo.AFB,
                    }).Join(db.SI_USERS, x => x.IDUSER, user => user.ID, (x, user) => new
                    {
                        NUMENREG = x.NUMENREG,
                        DATEAFB = x.DATEAFB,
                        IDUSER = x.IDUSER,
                        IDSOCIETE = x.IDSOCIETE,
                        CODE_J = x.CODE_J,
                        RIB = x.RIB,
                        MONTANT = x.MONTANT,
                        DATE = x.DATE,
                        LIBELLE = x.LIBELLE,
                        BANQUE = x.BANQUE,
                        GUICHET = x.GUICHET,
                        LOGIN = user.LOGIN,
                        SITE = x.SITE,
                        NOMFICHIER = x.NOMFICHIER,
                    })
                    .OrderBy(x => x.DATE).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = query, databr = queryBr }, settings));
            }
            else
            {
                var query = db.OPA_HISTORIQUE
                .Where(x => x.IDSOCIETE == PROJECTID )
                .Join(db.OPA_REGLEMENT, histo => histo.NUMENREG, reglement => reglement.NUM, (histo, reglement) => new
                {
                    NUMENREG = histo.NUMENREG,
                    DATEAFB = histo.DATEAFB,
                    IDUSER = histo.IDUSER,
                    IDSOCIETE = histo.IDSOCIETE,
                    CODE_J = reglement.CODE_J,
                    RIB = reglement.RIB,
                    MONTANT = reglement.MONTANT,
                    DATE = reglement.DATE,
                    LIBELLE = reglement.LIBELLE,
                    BANQUE = reglement.BANQUE,
                    GUICHET = reglement.GUICHET,
                }).Join(db.SI_USERS, x => x.IDUSER, user => user.ID, (x, user) => new
                {
                    NUMENREG = x.NUMENREG,
                    DATEAFB = x.DATEAFB,
                    IDUSER = x.IDUSER,
                    IDSOCIETE = x.IDSOCIETE,
                    CODE_J = x.CODE_J,
                    RIB = x.RIB,
                    MONTANT = x.MONTANT,
                    DATE = x.DATE,
                    LIBELLE = x.LIBELLE,
                    BANQUE = x.BANQUE,
                    GUICHET = x.GUICHET,
                    LOGIN = user.LOGIN,
                    NOTIFICATION = x,
                })
                .OrderBy(x => x.DATE).DistinctBy(x => x.NUMENREG).ToList();
                var queryBr = db.OPA_HISTORIQUEBR
                    .Where(x => x.IDSOCIETE == PROJECTID && site.Contains(x.SITE))
                    .Join(db.OPA_REGLEMENTBR, histo => histo.NUMENREG, reglement => reglement.NUM, (histo, reglement) => new
                    {
                        NUMENREG = histo.NUMENREG,
                        DATEAFB = histo.DATEAFB,
                        IDUSER = histo.IDUSER,
                        IDSOCIETE = histo.IDSOCIETE,
                        CODE_J = reglement.CODE_J,
                        RIB = reglement.RIB,
                        MONTANT = reglement.MONTANT,
                        DATE = reglement.DATE,
                        LIBELLE = histo.AFB,
                        BANQUE = reglement.BANQUE,
                        GUICHET = reglement.GUICHET,
                        SITE = histo.SITE,
                        LIEN = histo.LIEN,
                        NOTIF = histo.NOTIF,
                    }).Join(db.SI_USERS, x => x.IDUSER, user => user.ID, (x, user) => new
                    {
                        NUMENREG = x.NUMENREG,
                        DATEAFB = x.DATEAFB,
                        IDUSER = x.IDUSER,
                        IDSOCIETE = x.IDSOCIETE,
                        CODE_J = x.CODE_J,
                        RIB = x.RIB,
                        MONTANT = x.MONTANT,
                        DATE = x.DATE,
                        LIBELLE = x.LIBELLE,
                        BANQUE = x.BANQUE,
                        GUICHET = x.GUICHET,
                        LOGIN = user.LOGIN,
                        SITE = x.SITE,
                        NOTIF = x.NOTIF,
                        NOTIFICATION = x.NOTIF == true ? true : false,
                    })
                    .OrderBy(x => x.DATE).DistinctBy(x=> x.NUMENREG).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = query, databr = queryBr }, settings));
            }
            
        }
        public JsonResult GetCancel(SI_USERS suser, string listCompte,string codeproject)
        {
            var list = listCompte.Split(',');
            int PROJECTID = int.Parse(codeproject); 
            List<OPA_HISTORIQUE> result = new List<OPA_HISTORIQUE>();
            List<OPA_HISTORIQUEBR> resultBR = new List<OPA_HISTORIQUEBR>();
            List<OPA_VALIDATIONS> OPABR = new List<OPA_VALIDATIONS>();
            var user = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.PWD == suser.PWD && x.DELETIONDATE == null).FirstOrDefault();
            var TYPE = db.SI_TYPECRITURE.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault().TYPE;

            int countTraitement = 0;
            var lien = db.SI_SETLIEN.FirstOrDefault().LIEN;
           // var lien = "http://srvapp.softwell.cloud/softconnectsiig/";
            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID && a.DELETIONDATE == null).FirstOrDefault().PROJET;

            foreach (var item in list)
            {
                if (TYPE == 1)
                {
                    resultBR = db.OPA_HISTORIQUEBR.Where(y => y.NUMENREG == item && y.IDUSER == user.ID && y.IDSOCIETE == PROJECTID).ToList();
                    var OPABRSAVE = db.OPA_VALIDATIONS.Where(x => x.IDREGLEMENT == item && x.IDPROJET == PROJECTID).FirstOrDefault();
                    if (OPABRSAVE != null)
                    {
                        db.OPA_VALIDATIONS.Remove(OPABRSAVE);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    decimal ii;
                    ii = Convert.ToDecimal(item);
                    result = db.OPA_HISTORIQUE.Where(x => x.NUMENREG == ii && x.IDUSER == user.ID && x.IDSOCIETE == PROJECTID).ToList();

                }

                foreach (var pc in result)
                {
                    db.OPA_HISTORIQUE.Remove(pc);
                    
                }
                foreach (var br in resultBR)
                {
                    db.OPA_HISTORIQUEBR.Remove(br);

                }
                countTraitement++;
            }
            db.SaveChanges();
            string MailAdresse = "";
            string mdpMail = "";

            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD != null)
            {
                MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL;
                mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD;
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
            }
            
            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }
               
                    mail.Subject = "Annulation paiement du projet " + ProjetIntitule;
                    mail.IsBodyHtml = true;
                    mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que vous avez annulée" + countTraitement + " paiement pour le compte du projet " + ProjetIntitule + ".<br/><br>" +
                        "Nous vous remercions de cliquer <a href='" + lien + "'>(ici)</a> pour accéder à la plate-forme SOFT EXPENDITURES TRACKERS.<br/><br>" + "Cordialement";
                
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                smtp.EnableSsl = true;

                try { smtp.Send(mail); }
                catch (Exception) { }
            }
            return Json(JsonConvert.SerializeObject(new { msg = "success", data = result, datebr = resultBR }));
        }
        public JsonResult SFTP(string HOTE, string PATH, string USERFTP, string PWDFTP, string SOURCE,string port)
        {
            int pport = int.Parse(port);
            string pth = AppDomain.CurrentDomain.BaseDirectory + "FILERESULT\\" + SOURCE + ".txt";
            string remoteFilePath = @"\public\";
            var res = ""; 
            try
            {
                // Créer une connexion SFTP
                //using (var sftp = new SftpClient(HOTE, pport, USERFTP.ToString(), PWDFTP))
                using (var sftp = new SftpClient("151.80.218.41", 22, "tester", "password"))
                {
                    sftp.Connect();

                    using (var fileStream = new FileStream(pth, FileMode.Open))
                    {
                       //var sss =  sftp.ListDirectory("//");
                        // Envoyer le fichier
                        sftp.UploadFile(fileStream, remoteFilePath + Path.GetFileName(pth), x =>
                        {
                            var az = x.ToString();
                        });
                        //Console.WriteLine("Fichier envoyé avec succès !");
                        res = "Fichier envoyé avec succès !";
                    }

                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                res = ex.Message;
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = res, data = "" }, settings));
        }

        private ConnectionInfo getSftpConnection(string hOTE, string username, int port, string sOURCE)
        {
            string pth = AppDomain.CurrentDomain.BaseDirectory + "KeyP.txt";
            string keypath = Convert.ToBase64String(System.IO.File.ReadAllBytes(pth));
            try
            {
                return new ConnectionInfo(hOTE, port, username, privateKeyObject(username, keypath)); //
            }
            catch (Exception ex)
            {

                throw new NotImplementedException();
            }
            
        }
        private static AuthenticationMethod[] privateKeyObject(string username, string publicKeyPath)
        {
            PrivateKeyFile privateKeyFile = new PrivateKeyFile(publicKeyPath);
            PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod =
                 new PrivateKeyAuthenticationMethod(username, privateKeyFile);
            return new AuthenticationMethod[] { privateKeyAuthenticationMethod };
        }
        private string couperText(int x, string str)
        {
            string s = "";
            int n = 0;
            try
            {
                if (str != null)
                {
                    n = str.Length;
                }

            }
            catch (Exception) { }

            if (n > x)
            {
                int y = n - x;
                s = str.Remove(x, y);
            }
            else
            {
                s = str;
            }
            return s;
        }
        
        //public JsonResult GetAnomalieTomOP(SI_USERS suser,string journal, string codeproject, DateTime datein, DateTime dateout, string compteG,string Auxi,string site)
        public JsonResult GetAnomalieTomOP(SI_USERS suser, string codeproject)
        {
            int PROJECTID = int.Parse(codeproject);
            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            var Anomalie = tom.RTIERS.Where(x => x.RIB1 == null || x.NOM == null || x.AD1 == null || x.DOM1 == null ).ToList();

            var JournalAnomalie = __db.RJL1.Where(x => x.NATURE == "2" && x.JLTRESOR == true && (x.RIB == null || x.AGENCE == null || x.GUICHET == null || x.BANQUE == null)).ToList();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
           
            ANOMALIE_G SAUVEANOMALIE = new ANOMALIE_G();
            var GetallAnomalieProjet = db.ANOMALIE_G.Where(x => x.IDPROJECT == PROJECTID).ToList();
            foreach (var Sup in GetallAnomalieProjet)
            {
                db.ANOMALIE_G.Remove(Sup);
            }
            //db.ANOMALIE_G.RemoveRange(db.ANOMALIE_G);
            db.SaveChanges();
            if (Anomalie.Count != 0)
            {
                foreach (var item in Anomalie)
                {
                    SAUVEANOMALIE.COMPTE_BANQUE = item.NOM;
                    SAUVEANOMALIE.RIB = item.RIB1;
                    SAUVEANOMALIE.COMPTEG = item.COGE;
                    SAUVEANOMALIE.IDPROJECT = PROJECTID;
                    SAUVEANOMALIE.AUXI = item.AUXI;
                    SAUVEANOMALIE.AD1 = item.AD1;
                    SAUVEANOMALIE.AD2 = item.AD2;
                    SAUVEANOMALIE.DOM1 = item.DOM1;
                    SAUVEANOMALIE.TYPE = "TIERS";
                    SAUVEANOMALIE.GUICHET = "";
                    SAUVEANOMALIE.AGENCE = "";
                    try
                    {
                        db.ANOMALIE_G.Add(SAUVEANOMALIE);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                }
            }
            if (JournalAnomalie.Count != 0)
            {
                foreach (var item in JournalAnomalie)
                {
                    SAUVEANOMALIE.COMPTE_BANQUE = item.BANQUE;
                    SAUVEANOMALIE.RIB = item.RIB;
                    SAUVEANOMALIE.IDPROJECT = PROJECTID;
                    SAUVEANOMALIE.GUICHET = item.GUICHET;
                    SAUVEANOMALIE.AGENCE = item.AGENCE;
                    SAUVEANOMALIE.TYPE = "JOURNAL";
                    SAUVEANOMALIE.JOURNAL = item.CODE;
                    SAUVEANOMALIE.LIBELLE = item.LIBELLE;
                    SAUVEANOMALIE.COMPTEG = item.COMPTEASSOCIE;
                    SAUVEANOMALIE.AD1 = "";
                    SAUVEANOMALIE.AD2 = "";
                    SAUVEANOMALIE.AUXI = "";
                    SAUVEANOMALIE.DOM1 = "";
                    SAUVEANOMALIE.AD2 = "";
                    try
                    {
                        db.ANOMALIE_G.Add(SAUVEANOMALIE);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                }
            }
            var DataAnomalie = db.ANOMALIE_G.Where(x => x.IDPROJECT == PROJECTID && x.TYPE == "TIERS").Select(x => new
            {
                ID = x.ID,
                COMPTE_BANQUE = x.COMPTE_BANQUE,
                RIB = x.RIB,
                AUXI = x.AUXI,
                COMPTEG = x.COMPTEG,
                IDPROJECT = x.IDPROJECT,
                AD1 = x.AD1,
                AD2 = x.AD2,
                DOM1 = x.DOM1,
                GUICHET = x.GUICHET,
                AGENCE = x.AGENCE,
                TYPE = x.TYPE,


            }).Join(db.SI_PROJETS,anomalie => anomalie.IDPROJECT,projet => projet.ID ,(anomalie, projet) => new
            {
                ID = anomalie.ID,
                COMPTE_BANQUE = anomalie.COMPTE_BANQUE,
                RIB = anomalie.RIB,
                AUXI = anomalie.AUXI,
                COMPTEG = anomalie.COMPTEG,
                IDPROJECT = projet.PROJET,
                AD1 = anomalie.AD1,
                AD2 = anomalie.AD2,
                DOM1 = anomalie.DOM1,
                GUICHET = anomalie.GUICHET,
                AGENCE = anomalie.AGENCE,
                TYPE = anomalie.TYPE,

            }).ToList();
            var DataAnomalieJournal = db.ANOMALIE_G.Where(x => x.IDPROJECT == PROJECTID && x.TYPE == "JOURNAL").Select(x => new
            {
                ID = x.ID,
                COMPTE_BANQUE = x.COMPTE_BANQUE,
                RIB = x.RIB,
                AUXI = x.AUXI,
                COMPTEG = x.COMPTEG,
                IDPROJECT = x.IDPROJECT,
                AD1 = x.AD1,
                AD2 = x.AD2,
                DOM1 = x.DOM1,
                GUICHET = x.GUICHET,
                AGENCE = x.AGENCE,
                TYPE = x.TYPE,
                JOURNAL = x.JOURNAL,
                LIBELLE = x.LIBELLE

            }).Join(db.SI_PROJETS, anomalie => anomalie.IDPROJECT, projet => projet.ID, (anomalie, projet) => new
            {
                ID = anomalie.ID,
                COMPTE_BANQUE = anomalie.COMPTE_BANQUE,
                RIB = anomalie.RIB,
                AUXI = anomalie.AUXI,
                COMPTEG = anomalie.COMPTEG,
                IDPROJECT = projet.PROJET,
                AD1 = anomalie.AD1,
                AD2 = anomalie.AD2,
                DOM1 = anomalie.DOM1,
                GUICHET = anomalie.GUICHET,
                AGENCE = anomalie.AGENCE,
                TYPE = anomalie.TYPE,
                JOURNAL = anomalie.JOURNAL,
                LIBELLE = anomalie.LIBELLE

            }).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès.", data = DataAnomalie , datas = DataAnomalieJournal }, settings));
        }
        public JsonResult SendEmailSuppliersGED(SI_USERS suser,int PROJECTID, string idLiquidation)
        {
            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            string MailAdresse = "";
            string mdpMail = "";

            if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL != null && db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD != null)
            {
                MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDMAIL;
                mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).SENDPWD;
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)" }, settings));
            }

            var send = db.OPA_HISTORIQUEBR.Where(x => x.NUMENREG == idLiquidation && x.IDSOCIETE == PROJECTID).FirstOrDefault();
            string email = send.LIEN;
            string Obj = send.OBJET;
            string Title = send.TITLE;
            string doc = send.DOC;
            string message = send.MESSAGE;
            var ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID && a.DELETIONDATE == null).FirstOrDefault().PROJET;
            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                SmtpClient smtp = new SmtpClient("smtpauth.moov.mg");
                smtp.UseDefaultCredentials = true;

                mail.From = new MailAddress(MailAdresse);

                mail.To.Add(MailAdresse);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE != null)
                {
                    string[] separators = { ";" };

                    var Tomail = mail;
                    if (Tomail != null)
                    {
                        string listUser = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null).MAILTE;
                        string[] mailListe = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }
                }

                mail.Subject = "Avis de réglement";
                mail.IsBodyHtml = true;
                mail.Body = "Madame, Monsieur,<br/><br>" + "Nous vous informons que le paiement en relation avec  le document " + doc + " que vous avez transmis à " + ProjetIntitule + " a été efféctué.<br/><br>" +
                            " <b><u>Titre du document</u></b>: " + Title + " <br/>" +
                            " <b><u>Objet</u></b>: " + Obj + " <br/>" +
                            " <b><u>Message</u></b>: " + message + " <br/>" +
                            "'<br/><br>" + "Cordialement";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail);
                smtp.EnableSsl = true;

                try {
                    smtp.Send(mail);
                    send.NOTIF = true;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                }
                catch (Exception) { }
            }
            return Json(JsonConvert.SerializeObject(new { msg = "Email envoyer avec Succes", data = "" }));
        }
        public string GetTypeBanque(string codeproject , SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            int PROJECTid = int.Parse(codeproject);
            if (exist == null) return "";

            if (exist.IDPROJET != 0)
            {
                var TypeFileBQ = db.SI_TYPEBANQUE.FirstOrDefault(a => a.IDPROJET == PROJECTid).TYPE;
                return TypeFileBQ.ToString();
            }
            else
            {
                //var mapuser = db.SI_MAPUSERPROJET.Where(a => a.IDUS == exist.ID).ToList();
                int PROJECTID = int.Parse(codeproject);
                var ii = db.SI_TYPECRITURE.FirstOrDefault(a => a.IDPROJET == PROJECTID);
                var TypeFileBQ = "";
                if (ii != null)
                {
                    TypeFileBQ = ii.TYPE.ToString();
                }
                else
                {
                    TypeFileBQ = "Veuillez parametrer votre type de fichier";
                }
                return TypeFileBQ.ToString();
            }
            //return TypeFileBQ;
        }
    }
}
