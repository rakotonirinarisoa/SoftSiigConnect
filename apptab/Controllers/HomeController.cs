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
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using System.Web.Services.Description;
using System.Diagnostics;
using Microsoft.SqlServer.Server;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.Bibliography;
using System.Data.SqlClient;
using Renci.SshNet.Common;
using System.Net.Sockets;
using System.Web.UI;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using MimeKit;
using MimeKit.Cryptography;

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
        public ActionResult BanqueCreate()
        {
            ViewBag.Controller = "Creation Liste Banque";
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
            try
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
                    System.IO.File.WriteAllText(pathchemin, stringWriter.ToString(),new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
                //using (StreamWriter stream = new StreamWriter(pathchemin, false, Encoding.GetEncoding("UTF-8")))
                //{
                //    xmlDoc.Save(stream);
                //}
                return null;// (xmlDoc);
            }
            catch (Exception ex)
            {

                throw;
            }

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
        public ActionResult CreateZipFile(SI_USERS suser, string codeproject, int intbasetype, bool devise, string codeJ, string baseName, string listCompte, string journal, int banqueid)
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
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, devise,ftp.FTPPWD, ftp.PATH, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);
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
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, ftp.PATH, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);
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
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, ftp.PATH, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);
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
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);
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
                else if (intbasetype == 4)//testISO20022
                {
                    int typeDevise = 0;
                    var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list, typeDevise, intbasetype, banqueid);
                    if (pathfile.Fichier == null)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = pathfile.NomFichier }, settings));
                    }
                    Anarana = pathfile.Chemin;
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.NomFichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);

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
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec Succées", data = "Fichier Envoyer Avec Success" }, settings));
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
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, journal, ftp.BANQUE);
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
        public ActionResult CreateZipFileISO2022(SI_USERS suser, string codeproject, int intbasetype, bool devise, string codeJ, string baseName, string listCompte, int typeDevise, int banqueid)
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
            List<OPA_REGLEMENTBR> reglementValider = new List<OPA_REGLEMENTBR>();
            foreach (var item in list)
            {
                int b = int.Parse(item.Numereg);
                avalider.AddRange(db.OPA_VALIDATIONS.Where(a => a.IDPROJET == PROJECTID && a.ETAT == 2 && a.IDREGLEMENT == item.Id && a.NUMEREG == b).ToList());
                reglementValider.AddRange(db.OPA_REGLEMENTBR.Where(x => x.IDSOCIETE == PROJECTID && x.ETAT == "0" && x.NUM == item.Id && x.NUMEREG == b).ToList());
            };
            var path = "";
            var Nomfichier = "";
            RBANQUES rbanque = new RBANQUES();
            RJL1 djournal = (from journl in __db.RJL1
                             where journl.CODE == codeJ && journl.JLTRESOR == true && (journl.NATURE == "2" || journl.NATURE == "1")
                             select journl).FirstOrDefault();
            rbanque = __db.RBANQUES.Where(x => x.CODE == djournal.BANQUE).FirstOrDefault();
            string directory = "";
            string projetName = "";
            projetName = db.SI_PROJETS.Where(x => x.ID == PROJECTID).FirstOrDefault().PROJET;
            if (intbasetype == 3 || intbasetype == 4)//ISO crypter RSA
            {

                var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list, typeDevise, intbasetype, banqueid);
                if (pathfile.Fichier == null)
                {
                    return Content(pathfile.NomFichier);
                    //return Json(JsonConvert.SerializeObject(new { type = "error", msg = pathfile.NomFichier }, settings));
                }
                Anarana = pathfile.Chemin;
                path = pathfile.NomFichier;
                //send = CreateAFBTXT(pathfile.Chemin, pathfile.NomFichier);

                //xmlResult = SaveDocument(Anarana, Anarana);
                if (rbanque.CODEBIC.Contains("BOA") || rbanque.CODEBIC.Contains("AFRIMGMG"))
                {
                    xmlResult = SaveDocument(Anarana, Anarana);
                    directory = "BANQUE/" + projetName + "/BOA";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BOA").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("CLMDMGMG"))
                {
                    directory = "BANQUE/" + projetName + "/BNI";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BNI").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("BFAVMGMG"))
                {
                    xmlResult = SaveDocument(Anarana, Anarana);
                    directory = "BANQUE/" + projetName + "/SG";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "SG").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("BMOI"))
                {
                    directory = "BANQUE/" + projetName + "/BMOI";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BMOI").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
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
                    foreach (var item in reglementValider)
                    {
                        try
                        {
                            item.ETAT = "1";
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                            throw;
                        }
                    }
                }
            }
            else if (intbasetype == 5)//mise a disposition
            {
                var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list, typeDevise, intbasetype, banqueid);
                if (pathfile.Fichier == null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = pathfile.NomFichier }, settings));
                }
                Anarana = pathfile.Chemin;
                path = pathfile.NomFichier;
                //send = CreateAFBTXT(pathfile.Chemin, pathfile.NomFichier);

                directory = "BANQUE/" + projetName + "/BOA";
                xmlResult = SaveDocument(Anarana, Anarana);
                if (rbanque.CODEBIC.Contains("BOA") || rbanque.CODEBIC.Contains("AFRIMGMG"))
                {
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BOA").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("BNI"))
                {
                    directory = "BANQUE/" + projetName + "/BNI";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BNI").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("BFAV"))
                {
                    directory = "BANQUE/" + projetName + "/SG";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "SG").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }
                else if (rbanque.CODEBIC.Contains("BMOI"))
                {
                    directory = "BANQUE/" + projetName + "/BMOI";
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BMOI").FirstOrDefault();
                    string pport = ftp.PORT.ToString();
                    SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                }

                if (avalider != null )
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
                    foreach (var item in reglementValider)
                    {
                        try
                        {
                            item.ETAT = "1";
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                            throw;
                        }
                    }
                }
            }
            else if (intbasetype == 0)
            {
                if (avalider != null)
                {
                    var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list, typeDevise, intbasetype, banqueid);

                    if (pathfile.Fichier == null)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = pathfile.NomFichier }, settings));
                    }
                    path = pathfile.NomFichier;
                    //Nomfichier = pathfile.NomFichier + ".xml";
                    if (rbanque.CODEBIC.Contains("CLMDMGMG"))
                    {
                        directory = "BANQUE/" + projetName + "/BNI";
                        Anarana = pathfile.Chemin;
                        //xmlResult = SaveDocument(Anarana, Anarana);
                        //SaveDocument(Anarana, Anarana);
                    }
                    else if (rbanque.CODEBIC.Contains("BF"))
                    {
                        directory = "BANQUE/" + projetName + "/SG";
                        Anarana = pathfile.Chemin;
                        //xmlResult = SaveDocument(Anarana, Anarana);
                        SaveDocument(Anarana, Anarana);
                    }
                    else if (rbanque.CODEBIC.Contains("BMOI"))
                    {
                        directory = "BANQUE/" + projetName + "/BMOI";
                        Anarana = pathfile.Chemin;
                        //xmlResult = SaveDocument(Anarana, Anarana);
                        SaveDocument(Anarana, Anarana);
                    }
                    else
                    {
                        directory = "BANQUE/" + projetName + "/BOA";
                        Anarana = pathfile.Chemin;
                        //xmlResult = SaveDocument(Anarana, Anarana);
                        SaveDocument(Anarana, Anarana);
                    }

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
                        foreach (var item in reglementValider)
                        {
                            try
                            {
                                item.ETAT = "1";
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {
                if (avalider != null)
                {
                    var pathfile = aFB160.CreateISO20022(devise, codeJ, suser, codeproject, list, typeDevise, intbasetype, banqueid);

                    if (pathfile.Fichier == null)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = pathfile.NomFichier }, settings));
                    }

                    path = pathfile.NomFichier;
                    //Nomfichier = pathfile.NomFichier + ".xml";
                    if (rbanque.CODEBIC.Contains("CLMDMGMG"))
                    {
                        directory = "BANQUE/" + projetName + "/BNI";
                    }
                    else if (rbanque.CODEBIC.Contains("BF"))
                    {
                        directory = "BANQUE/" + projetName + "/SG";
                    }
                    else if (rbanque.CODEBIC.Contains("BMOI"))
                    {
                        directory = "BANQUE/" + projetName + "/BMOI";
                    }
                    else
                    {
                        directory = "BANQUE/" + projetName + "/BOA";
                    }
                    Anarana = pathfile.Chemin;
                    //xmlResult = SaveDocument(Anarana, Anarana);
                    SaveDocument(Anarana, Anarana);
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
                        foreach (var item in reglementValider)
                        {
                            try
                            {
                                item.ETAT = "1";
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                                throw;
                            }
                        }
                    }
                    if (rbanque.CODEBIC.Contains("BOA") || rbanque.CODEBIC.Contains("AFRIMGMG"))
                    {
                        var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BOA").FirstOrDefault();
                        string pport = ftp.PORT.ToString();
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                    }
                    else if (rbanque.CODEBIC.Contains("CLMDMGMG"))
                    {
                        directory = "BANQUE/" + projetName + "/BNI";
                        var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BNI").FirstOrDefault();
                        string pport = ftp.PORT.ToString();
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                    }
                    else if (rbanque.CODEBIC.Contains("BFAV"))
                    {
                        directory = "BANQUE/" + projetName + "/SG";
                        var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "SG").FirstOrDefault();
                        string pport = ftp.PORT.ToString();
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                    }
                    else if (rbanque.CODEBIC.Contains("BMOI"))
                    {

                        directory = "BANQUE/" + projetName + "/BMOI";
                        var ftp = db.OPA_FTP.Where(x => x.IDPROJET == PROJECTID && x.BANQUE == "BMOI").FirstOrDefault();
                        string pport = ftp.PORT.ToString();
                        SFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT,devise, ftp.FTPPWD, pathfile.Chemin, pport, intbasetype, PROJECTID, directory, ftp.BANQUE);
                    }
                    if (intbasetype == 0)
                    {
                        Anarana = pathfile.Chemin;
                        SaveDocument(Anarana, Anarana);
                    }
                }
            }
            try
            {
                path = path.Replace("xml", "docx");
                // Chemin complet du fichier
                string pathDown = path;

                // Vérifier si le fichier existe
                if (!System.IO.File.Exists(pathDown))
                {
                    return HttpNotFound("Le fichier n'a pas été trouvé.");
                }

                // Créer un objet FileInfo pour obtenir des informations supplémentaires sur le fichier
                FileInfo fi = new FileInfo(pathDown);
                string fileName = Path.GetFileNameWithoutExtension(fi.Name) + ".docx"; // Forcer l'extension .docx
                Anarana = fileName;
                // Définir le type MIME pour un fichier Word (.docx)
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                // Créer une disposition de contenu pour indiquer le téléchargement
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName, // Nom du fichier à télécharger
                    Inline = false, // Indique que le fichier doit être téléchargé, pas affiché dans le navigateur
                };

                // Effacer les anciens en-têtes HTTP et ajouter le bon en-tête de disposition de contenu
                Response.Clear();
                Response.AppendHeader("Content-Disposition", cd.ToString());
                Response.ContentType = contentType;

                // Utiliser TransmitFile pour envoyer directement le fichier sans le charger en mémoire
                Response.TransmitFile(pathDown);
                Response.End(); // Terminer la réponse HTTP pour éviter d'autres écritures dans la réponse

                return new EmptyResult(); // Fin du traitement
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Erreur lors du téléchargement du fichier : " + ex.Message);
            }
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
            List<string> site = new List<string>();

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            if (siteS == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la site d'utilisateur. " }, settings));
            }
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

            var hstSiig = db.OPA_VALIDATIONS
              .Where(x => x.ETAT != 4 && x.IDPROJET == PROJECTID)
              .Select(x => new
              {
                  IDREGLEMENT = x.IDREGLEMENT.ToString(),
                  NUMEREG = x.NUMEREG.ToString() // Supposons que NUMEREG est une chaîne séparée par des virgules
              }).ToList();

            List<DataListTomOP> list = new List<DataListTomOP>();

            if (hstSiig.Any())
            {
                var idReglements = new HashSet<string>(hstSiig.Select(h => h.IDREGLEMENT ));
                var result = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site);
                var tomproresult = result.Item2.DistinctBy(x => (x.No, x.NUMEREG)).ToList();
                //var tomproresult = result.Item2.ToList();
                if (result.Item1 != "OK")
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = result.Item1 }, settings));
                }
                //var tomproresult = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site)
                //    .DistinctBy(x => (x.No, x.NUMEREG))
                //    .ToList();
                foreach (var s1 in tomproresult)
                {
                    // Vérifier si s1.No n'est pas dans hstSiig
                    if (idReglements.Contains(s1.No))
                    {
                        // Ajouter s1 à la liste s'il n'est pas déjà présent
                        if (list.Any(dp => dp.No == s1.No && dp.NUMEREG == s1.NUMEREG))
                        {
                            list.Add(s1);
                        }
                        else
                        {
                            var correspondingItem = hstSiig.FirstOrDefault(h => h.IDREGLEMENT == s1.No && h.NUMEREG == s1.NUMEREG.ToString());
                            if (correspondingItem != null && correspondingItem.IDREGLEMENT.Contains(s1.No) && !correspondingItem.NUMEREG.Contains(s1.NUMEREG.ToString()))
                            {
                                list.Add(s1);
                            }
                        }
                    }
                    else
                    {
                        list.Add(s1);
                    }
                }
            }
            else
            {
                var result = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site);
                if (result.Item1 != "OK")
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = result.Item1, data = "" }, settings));
                }
                list.AddRange(result.Item2.ToList());
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
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Probléme d'accées(Votre accées ou session sont pérdu). " }, settings));
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

            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == crpt).Select(ST => ST.SITE).FirstOrDefault();
            #region Journal par fid
            //var ruserJournalq = tom.RUSER.ToList();
            //var ruserJournal = tom.RUSER.Where(x => siteS.Contains(x.SITE)).Select(x => x.JOURNAL).FirstOrDefault();
            //List<RUSER> ruserJournal = new List<RUSER>();
            //foreach (var item in siteS.Split(','))
            //{
            //    string a = item.ToString();
            //    ruserJournal.AddRange(tom.RUSER.Where(x => x.SITE.Contains(a)).ToList());
            //}
            //ruserJournal.Where(x=> x.JOURNAL != "" && x.JOURNAL != null);
            //ruserJournal = tom.RUSER
            //    .Where(x => siteS.Contains(x.SITE))
            //    .AsEnumerable() // Force l'exécution en mémoire
            //    .Select(x => x.JOURNAL != null ? x.JOURNAL.ToString() : null)
            //    .FirstOrDefault();
            //List<string>jjournal = new List<string>();
            //foreach (var item in ruserJournal)
            //{
            //    if (item.JOURNAL != "" && item.JOURNAL != null)
            //    {
            //        jjournal.Add(item.JOURNAL);
            //    }
            //}
            //List<string> TempFilter = new List<string>();
            //if (jjournal.Count != 0)
            //{
            //    foreach (var item in jjournal)
            //    {
            //        TempFilter.AddRange(item.Split(';').Distinct());

            //    }
            //    //TempFilter.Distinct();
            //    List<JournalViewModel> journalVirtu = new List<JournalViewModel>();
            //    foreach (var item in TempFilter)
            //    {
            //        journalVirtu.AddRange(tom.RJL1.Where(x => x.JLTRESOR == true && x.NATURE == "2" && x.CODE == item).Select(x => new JournalViewModel
            //        {
            //            CODE = x.CODE,
            //            LIBELLE = x.LIBELLE
            //        }).ToList());
            //    }
            //    journalVirtu = journalVirtu
            //    .GroupBy(j => j.CODE) // Regroupe par CODE
            //    .Select(g => g.First()) // Prend le premier élément de chaque groupe
            //    .ToList();

            //    //var JournalVM = tom.RJL1.Where(x => x.JLTRESOR == true && x.NATURE == "2" && TempFilter.Contains(x.CODE)).Select(x => new
            //    //{
            //    //    CODE = x.CODE,
            //    //    LIBELLE = x.LIBELLE
            //    //}).ToList();

            //    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = journalVirtu }, settings));
            //}
            //else
            //{
            //    var JournalVM = tom.RJL1.Where(x => x.JLTRESOR == true && x.NATURE == "2" ).Select(x => new
            //    {
            //        CODE = x.CODE,
            //        LIBELLE = x.LIBELLE
            //    }).ToList();

            //    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = JournalVM }, settings));
            //}
            #endregion
            var JournalVM = tom.RJL1.Where(x => x.JLTRESOR == true && x.NATURE == "2").Select(x => new
            {
                CODE = x.CODE,
                LIBELLE = x.LIBELLE
            }).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = JournalVM }, settings));
        }
        public class JournalViewModel
        {
            public string CODE { get; set; }
            public string LIBELLE { get; set; }
        }
        [HttpPost]
        public JsonResult GetCompteG(SI_USERS suser, string codeproject)
        {
            int PROJECTID = int.Parse(codeproject);
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Probléme d'accées(Votre accées ou session sont pérdu)." }, settings));
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
                    COGE = x.Key,
                    AUXI = x.Select(y => y.auxi).Distinct().ToList()
                }).ToList();

                if (CompteGBR.Count != 0)
                {
                    var CompteGtmp = CompteGBR.Union(compteGSiig).ToList();
                    var listCompteGENERALE = CompteGtmp.Union(CompteAvance).GroupBy(x => x.COGE).Select(x => new
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
                            avalider.MONTANT = LST.Montant;
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
                        catch (Exception ex)
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez vérifier l'adresse e-mail associée à cette action! Merci" }, settings));
                        }
                    }
                }
            }
            else
            {
                bool compteur = false;
                foreach (var item in siteS.Split(','))
                {

                    if (!compteur)
                    {
                        int countTraitement = 0;
                        MailAdresse = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDMAIL;
                        mdpMail = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item).SENDPWD;

                        string auxi1 = auxi;
                        AFB160 afb160 = new AFB160();//ty miova
                        var hst = db.OPA_HISTORIQUEBR.Where(x => x.SITE == item && x.IDSOCIETE == PROJECTID).Select(x => x.NUMENREG.ToString()).ToArray();
                        if (list.Count > 1)
                        {
                            compteur = false;
                        }
                        else
                        {
                            compteur = true;
                        }
                        var result = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser, PROJECTID, site);
                        int isa = list.Count();
                        foreach (var h in list)
                        {
                            int a = int.Parse(h.Numereg);
                            if (result.Item1 != "OK")
                            {
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = result.Item1, data = "" }, settings));
                            }

                            var listA = result.Item2.Where(x => x.No.ToString() == h.Id && x.NUMEREG == a /*&& item.Contains(x.SITE)*/).ToList();

                            if (listA.Count != 0)
                            {
                                foreach (var Lst in listA)
                                {
                                    var existingRecord = db.OPA_VALIDATIONS
                                        .FirstOrDefault(x => x.IDREGLEMENT == Lst.No.ToString() && x.NUMEREG == Lst.NUMEREG && x.IDPROJET == PROJECTID);

                                    if (existingRecord != null)
                                    {
                                        // Si un enregistrement existe déjà, retourner un message d'erreur
                                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = $"L'enregistrement avec IDREGLEMENT = {Lst.No.ToString()} et NUMREG = {Lst.NUMEREG} existe déjà.", data = "" }, settings));
                                    }

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
                                    //avalider.MONTANT = Convert.ToDecimal(couperText(18, Lst.Montant.ToString()));
                                    avalider.MONTANT = Convert.ToDecimal(Lst.Montant.ToString());
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
                                isa = isa - 1;
                            }
                            if (isa == 0)
                            {
                                compteur = true;
                            }
                        }

                        //SEND MAIL ALERT et NOTIFICATION//
                        //string MailAdresse = "serviceinfo@softwell.mg";
                        // string mdpMail = "09eYpçç0601";

                        if (countTraitement > 0)
                        {
                            try
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

                                    try
                                    {
                                        smtp.Send(mail);
                                    }
                                    catch (Exception ex)
                                    {
                                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez vérifier l'adresse e-mail associée à cette action! Merci Erreur sur" + ex.Message }, settings));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.Message;
                                throw;
                            }
                        }
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Tratement avec succès. ", data = "" }, settings));
        }
        //=========================================================================================TeacherValidation=========================================================================================
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

            List<string> site = new List<string>();
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
                site.Add(item);
                if (db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.SITE == item) == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes). " }, settings));
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
            string resultat = "";
            if (Applicable == 2)
            {
                if (basename == "2")
                {
                    aFB160.SaveValideSelectEcriture(list, true, suser, codeproject, site);
                }
                else
                {
                    foreach (var item in AvaliderList)
                    {
                        resultat = aFB160.SaveValideSelectEcritureBR(item.IDREGLEMENT, item.NUMEREG.ToString(), item.Journal, item.ETAT.ToString(), devise, suser, PROJECTID, (bool)item.AVANCE, site);
                        if (resultat != "Traitement avec Succes !")
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = resultat }, settings));
                        }
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                        resultat =  aFB160.SaveValideSelectEcritureBR(item.IDREGLEMENT, item.NUMEREG.ToString(), item.Journal, item.ETAT.ToString(), devise, suser, PROJECTID, (bool)item.AVANCE, site);
                        if (resultat != "Traitement avec Succes !")
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = resultat }, settings));
                        }
                    }

                }

                List<DataListTompro> listReg = new List<DataListTompro>();
                List<DataListTompro> listReg__ = new List<DataListTompro>();
                List<DataListTomOP> listRegBR = new List<DataListTomOP>();
                List<DataListTomOP> listRegBR__ = new List<DataListTomOP>();

                if (basename == "2")
                {
                    listReg = aFB160.getREGLEMENT(suser, PROJECTID, site);
                }
                else
                {
                    foreach (var item in list)
                    {
                        numeroReg = int.Parse(item.Numereg);
                        var resultValidation = aFB160.getREGLEMENTBR(suser,item.Id,numeroReg, PROJECTID, site);
                        if (resultValidation.Item1 != "OK")
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "success", msg = resultValidation.Item1 }, settings));
                        }
                        listRegBR.AddRange(resultValidation.Item2);
                    }
                   
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                            int b = int.Parse(Lt.Numereg);
                            avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == Lt.Id && a.ETAT == 0 && a.SITE == item && a.NUMEREG == b).FirstOrDefault();
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                            catch (Exception ex) { return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = "" }, settings)); }
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
                        Libelle = item.Libelle,
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
                        Libelle = item.Libelle,
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
                        Libelle = item.Libelle,
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
                listReg = aFB160.getREGLEMENT(suser, PROJECTID, site);
            }
            else
            {
                foreach (var item in list)
                {
                    numeroreg = int.Parse(item.Numereg);
                    var resultatValidation = aFB160.getREGLEMENTBR(suser, item.Id ,numeroreg, PROJECTID, site);
                    if (resultatValidation.Item1 != "OK") return Json(JsonConvert.SerializeObject(new { type = "error", msg = resultatValidation.Item1 }, settings));
                    listRegBR = resultatValidation.Item2;
                }
                
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
        public JsonResult LoadValidateEcriture(SI_USERS suser, string codeproject, string journal)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            //var site = db.SI_SITE.Where(a => a.IDUSER == exist.ID && a.IDPROJET == exist.IDPROJET).Select(a => a.SITE).ToList();

            int PROJECTID = int.Parse(codeproject);

            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == exist.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            if (siteS == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "Error", msg = "Veuillez paramétrer votre Site s'il vous plaît." }, settings));
            }
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
                var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && site.Contains(a.SITE) && a.Journal == journal).ToList();
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
                var val = db.OPA_VALIDATIONS.Where(a => a.DATESEND != null && a.IDPROJET == PROJECTID && a.ETAT == 2 && site.Contains(a.SITE) && a.Journal == journal).ToList();
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
        public JsonResult GetAnomalieBack(SI_USERS suser, string baseName, string codeproject)
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
                        var pp1 = db.SI_PROJETS.Select(a => new
                        {
                            PROJET = a.PROJET,
                            ID = a.ID,
                            DELETIONDATE = a.DELETIONDATE,
                        }).Where(a => a.DELETIONDATE == null && a.ID == test.IDPROJET).ToList();

                        var pp2 = (from usr in db.SI_PROJETS
                                   join prj in db.SI_MAPUSERPROJET on usr.ID equals prj.IDPROJET
                                   where prj.IDUS == test.ID && usr.DELETIONDATE == null
                                   select new
                                   {
                                       PROJET = usr.PROJET,
                                       ID = usr.ID,
                                       DELETIONDATE = usr.DELETIONDATE,
                                   }).ToList();

                        var user = pp1.Union(pp2).ToList();

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
        public JsonResult GetListAFB(string listCompte, SI_USERS suser, int PROJECTID)
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
            var usr = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.IDPROJET == PROJECTID && x.DELETIONDATE == null).FirstOrDefault();
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
                .Where(x => x.IDSOCIETE == PROJECTID)
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
                    .OrderBy(x => x.DATE).DistinctBy(x => x.NUMENREG).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès. ", data = query, databr = queryBr }, settings));
            }

        }
        public JsonResult GetCancel(SI_USERS suser, string listCompte, string codeproject)
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
                    resultBR = db.OPA_HISTORIQUEBR.Where(y => y.NUMENREG == item /*&& y.IDUSER == user.ID*/ && y.IDSOCIETE == PROJECTID).ToList();
                    string numreg = "";
                    foreach (var item2 in resultBR)
                    {
                        var OPABRSAVE = db.OPA_VALIDATIONS.Where(x => x.IDREGLEMENT == item && x.NUMEREG == item2.NUMREG && x.IDPROJET == PROJECTID).FirstOrDefault();
                        var reglementBr = db.OPA_REGLEMENTBR.Where(x => x.NUM == item && x.NUMEREG == item2.NUMREG && x.IDSOCIETE == PROJECTID).FirstOrDefault();
                        if (OPABRSAVE != null)
                        {
                            db.OPA_VALIDATIONS.Remove(OPABRSAVE);
                            db.OPA_REGLEMENTBR.Remove(reglementBr);
                            db.OPA_DELETE.Add(new OPA_DELETE
                            {
                                NUMEROOP = OPABRSAVE.IDREGLEMENT,
                                NUMEREG = (int)OPABRSAVE.NUMEREG,
                                USERMAIL = user.LOGIN,
                                DATEDELETE = DateTime.Now,
                                PROJETID = PROJECTID,
                            });
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
        public void SFTP(string HOTE, string PATH, string USERFTP,bool devise, string PWDFTP, string SOURCE, string port, int intbasetype, int PROJECTID, string directory, string AgenceBanque)
        {
            int pport = int.Parse(port);
            string pth = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + SOURCE;

            string namefile = SOURCE.Split('\\').Last().Split('.').First();
            string remoteFilePath = PATH;
            var res = "";
            string outputFile = "";

            if (AgenceBanque.Contains("BNI"))
            {
                string privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", directory, "Rsakeybni.txt");
                string convertedKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", directory, "Rsakeybni.pem");
                //namefile = namefile + 
                try
                {
                    if (!System.IO.File.Exists(privateKeyPath))
                    {
                        Console.WriteLine("❌ Clé privée OpenSSH introuvable !");
                        return;
                    }

                    // Charger la clé OpenSSH brute (sans conversion en PEM)
                    using (var keyStream = new FileStream(privateKeyPath, FileMode.Open, FileAccess.Read))
                    using (var keyFile = new PrivateKeyFile(keyStream, PWDFTP)) // Ajoutez la passphrase ici
                                                                                //using (var keyFile = new PrivateKeyFile(keyStream, "RsaHostoPic2025")) // Ajoutez la passphrase ici
                                                                                //using (var keyFile = new PrivateKeyFile(keyStream)) // Ajoutez la passphrase ici

                    using (var sftp = new SftpClient(HOTE, pport, USERFTP, keyFile))
                    {
                        try
                        {
                            using (TcpClient tcpClient = new TcpClient())
                            {
                                tcpClient.Connect(HOTE, pport); // Test sur le port 22 (ou le port SFTP utilisé)
                                Console.WriteLine("✅ Le serveur SFTP est joignable !");
                                string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logSuccess.txt";

                                // Créer ou ouvrir le fichier de log
                                using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                                {
                                    // Écrire l'exception dans le fichier de log
                                    writer.WriteLine("--------------------------------------------------");
                                    writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    writer.WriteLine("SOURCE : " + SOURCE);
                                    writer.WriteLine("publicKeyFile : ");
                                    writer.WriteLine("outputFile : " + outputFile);
                                    writer.WriteLine("Message : " + "✅ Le serveur SFTP est joignable !");
                                    writer.WriteLine("StackTrace : " + "✅ Le serveur SFTP est joignable !");
                                    writer.WriteLine("--------------------------------------------------");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Le serveur SFTP est injoignable : {ex.Message}");
                            return;
                        }
                        try
                        {
                            sftp.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
                            sftp.Connect();

                            Console.WriteLine("✅ Connexion SFTP réussie !");
                            // Envoyer le fichier
                            using (var fileStream = new FileStream(SOURCE, FileMode.Open))
                            {
                                sftp.UploadFile(fileStream, remoteFilePath + "/" + namefile + ".xml", x =>
                                {
                                    var az = x.ToString();
                                });
                                //sftp.UploadFile(fileStream, remoteFilePath);
                                Console.WriteLine($"✅ Fichier '{Path.GetFileName(SOURCE)}' envoyé avec succès !");
                            }
                        }
                        catch (Exception ex)
                        {
                            string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                            // Créer ou ouvrir le fichier de log
                            using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                            {
                                // Écrire l'exception dans le fichier de log
                                writer.WriteLine("--------------------------------------------------");
                                writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                writer.WriteLine("SOURCE : " + SOURCE);
                                writer.WriteLine("publicKeyFile : ");
                                writer.WriteLine("outputFile : "+ remoteFilePath + "/" + namefile + ".xml");
                                writer.WriteLine("Message : " + ex.Message);
                                writer.WriteLine("StackTrace : " + ex.StackTrace);
                                writer.WriteLine("--------------------------------------------------");
                            }
                        }
                        sftp.Disconnect();
                        Console.WriteLine("✅ Déconnexion SFTP.");
                    }
                }
                catch (SshAuthenticationException ex)
                {
                    Console.WriteLine($"❌ Erreur d'authentification SFTP : {ex.Message}");
                    string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                    // Créer ou ouvrir le fichier de log
                    using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                    {
                        // Écrire l'exception dans le fichier de log
                        writer.WriteLine("--------------------------------------------------");
                        writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        writer.WriteLine("SOURCE : " + SOURCE);
                        writer.WriteLine("publicKeyFile : ");
                        writer.WriteLine("outputFile : " + outputFile);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);
                        writer.WriteLine("--------------------------------------------------");
                    }
                }
                catch (SshException ex)
                {
                    Console.WriteLine($"❌ Erreur SFTP : {ex.Message}");
                    string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                    // Créer ou ouvrir le fichier de log
                    using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                    {
                        // Écrire l'exception dans le fichier de log
                        writer.WriteLine("--------------------------------------------------");
                        writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        writer.WriteLine("SOURCE : " + SOURCE);
                        writer.WriteLine("publicKeyFile : ");
                        writer.WriteLine("outputFile : " + outputFile);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);
                        writer.WriteLine("--------------------------------------------------");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur générale : {ex.Message}");
                }
            }
            else if (AgenceBanque.Contains("SG"))//BRED
            {
                namefile = "049038JP." + namefile;
                bool chiffrage = true;
                if (devise)
                {
                    chiffrage = false;
                }
                string publicKeyFile = "";
                if (intbasetype == 3)
                {
                    outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", namefile + ".xml.pgp");
                    publicKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", directory, "RSAkeyFile.txt");// Chemin vers le fichier de sortie chiffré
                    EncryptFile(SOURCE, publicKeyFile, outputFile);
                    //DecryptFile(outputFile, privateKeyFile, outputFileDEC);
                    namefile = namefile + ".xml.pgp";
                    try
                    {
                        // Créer une connexion SFTP
                        using (var sftp = new SftpClient(HOTE, pport, USERFTP.ToString(), PWDFTP))
                        //using (var sftp = new SftpClient("72.251.3.20", 22, "tester", "password"))
                        {
                            sftp.Connect();

                            using (var fileStream = new FileStream(outputFile, FileMode.Open))
                            {
                                //var sss =  sftp.ListDirectory("//");
                                // Envoyer le fichier
                                sftp.UploadFile(fileStream, remoteFilePath + "/" + namefile, x =>
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
                        string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                        // Créer ou ouvrir le fichier de log
                        using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                        {
                            // Écrire l'exception dans le fichier de log
                            writer.WriteLine("--------------------------------------------------");
                            writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            writer.WriteLine("SOURCE : " + SOURCE);
                            writer.WriteLine("publicKeyFile : ");
                            writer.WriteLine("outputFile : " + outputFile);
                            writer.WriteLine("Message : " + ex.Message);
                            writer.WriteLine("StackTrace : " + ex.StackTrace);
                            writer.WriteLine("--------------------------------------------------");
                        }
                    }
                }
                else
                {
                    try
                    {
                        // Créer une connexion SFTP
                        using (var sftp = new SftpClient(HOTE, pport, USERFTP.ToString(), PWDFTP))
                        //using (var sftp = new SftpClient("72.251.3.20", 22, "tester", "password"))
                        {
                            sftp.Connect();

                            using (var fileStream = new FileStream(SOURCE, FileMode.Open))
                            {
                                //var sss =  sftp.ListDirectory("//");
                                // Envoyer le fichier
                                sftp.UploadFile(fileStream, remoteFilePath + "/" + namefile + ".xml", x =>
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
                        string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                        // Créer ou ouvrir le fichier de log
                        using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                        {
                            // Écrire l'exception dans le fichier de log
                            writer.WriteLine("--------------------------------------------------");
                            writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            writer.WriteLine("SOURCE : " + SOURCE);
                            writer.WriteLine("publicKeyFile : ");
                            writer.WriteLine("outputFile : " + outputFile);
                            writer.WriteLine("Message : " + ex.Message);
                            writer.WriteLine("StackTrace : " + ex.StackTrace);
                            writer.WriteLine("--------------------------------------------------");
                        }
                    }

                }
            }
            else
            {
                if (intbasetype == 3 || intbasetype == 5)
                {
                    string pthkey = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + directory;

                    if (!Directory.Exists(pthkey))
                    {
                        Directory.CreateDirectory(pthkey);
                        res = "Vous n'avez pas de fichier de cryptage!Veuillez contactez votre administrateur";
                        return;
                    }
                    else
                    {
                        string publicKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", "/RSAkeyFile.asc");//get file clé public
                                                                                                                                    //C:\inetpub\wwwroot\SOFTSETTEST\FILERESULT\BANQUE\PACT\BOA
                        var CryptageType = db.SI_TYPEBANQUE.Where(x => x.IDPROJET == PROJECTID).FirstOrDefault();
                        try
                        {
                            if (!System.IO.File.Exists(pthkey + publicKeyFile))
                            {
                                res = "Vous n'avez pas de fichier de cryptage!Veuillez contactez votre administrateur";
                                //return ;
                            }
                            if (CryptageType.CRYPTAGE == "2")
                            {
                                outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", namefile + ".xml.gpg");
                                publicKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", directory, "RSAkeyFile.asc");// Chemin vers le fichier de sortie chiffré
                                EncryptFileWithGPG(SOURCE, publicKeyFile, outputFile);
                                //DecryptFile(outputFile, privateKeyFile, outputFileDEC);
                                namefile = namefile + ".xml.gpg";
                            }
                            else if (CryptageType.CRYPTAGE == "1")
                            {
                                outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", namefile + ".xml.pgp");
                                publicKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILERESULT", directory, "RSAkeyFile.asc");// Chemin vers le fichier de sortie chiffré
                                EncryptFile(SOURCE, publicKeyFile, outputFile);
                                namefile = namefile + ".xml.pgp";
                            }
                        }
                        catch (Exception ex)
                        {
                            string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreur.txt";

                            // Créer ou ouvrir le fichier de log
                            using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                            {
                                // Écrire l'exception dans le fichier de log
                                writer.WriteLine("--------------------------------------------------");
                                writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                writer.WriteLine("SOURCE : " + SOURCE);
                                writer.WriteLine("publicKeyFile : " + publicKeyFile);
                                writer.WriteLine("outputFile : " + outputFile);
                                writer.WriteLine("Message : " + ex.Message);
                                writer.WriteLine("StackTrace : " + ex.StackTrace);
                                writer.WriteLine("--------------------------------------------------");
                            }
                        }
                        try
                        {
                            // Créer une connexion SFTP
                            using (var sftp = new SftpClient(HOTE, pport, USERFTP.ToString(), PWDFTP))//h2h_pact/IN
                            {
                                sftp.Connect();
                                if (sftp.IsConnected)
                                {
                                    using (var fileStream = new FileStream(outputFile, FileMode.Open))
                                    {
                                        // Envoyer le fichier
                                        sftp.UploadFile(fileStream, remoteFilePath + "/" + namefile, x =>
                                        {
                                            var az = x.ToString();
                                        });
                                        res = "Fichier envoyé avec succès !";
                                    }
                                }

                                sftp.Disconnect();
                            }
                        }
                        catch (Renci.SshNet.Common.SftpPermissionDeniedException ex)
                        {
                            res = $"Erreur de permission : {ex.Message}. Assurez-vous que vous avez les permissions d'écriture sur le répertoire distant.";
                        }
                        catch (Exception ex)
                        {
                            res = $"Erreur générale : {ex.Message}";
                        }
                    }

                }
                //else
                //{//envoye sftp fichier non crypter

                //}
            }
        }
        [HttpPost]
        public JsonResult AlertClient(SI_USERS suser, string codeJournal, string codeproject, string auxi, string comptaG)
        {
            int PROJECTID = int.Parse(codeproject);
            var user = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.PWD == suser.PWD && x.DELETIONDATE == null).FirstOrDefault();
            //var CodeJournalAlert = db.ANOMALIE_G.Where(x => x.IDPROJECT == PROJECTID ).ToList();
            bool Alert = false;
            if (comptaG != "Autre Opérations")
            {
                if (auxi == "Tous")
                {
                    var rjl = __db.RJL1.Where(x => x.CODE == codeJournal).FirstOrDefault();
                    if (rjl.BANQUE == null || rjl.AGENCE == null || rjl.GUICHET == null || rjl.RIB == null || rjl.CLE == null /*|| rjl.IBAN == null*/)
                    {
                        Alert = true;
                    }
                }
                else
                {
                    var rjl = __db.RJL1.Where(x => x.CODE == codeJournal).FirstOrDefault();
                    var tiers = __db.RTIERS.Where(x => x.AUXI == auxi).FirstOrDefault();
                    if (rjl.BANQUE == null || rjl.AGENCE == null || rjl.GUICHET == null || rjl.RIB == null || rjl.CLE == null /*|| rjl.IBAN == null*/)
                    {
                        Alert = true;
                    }
                    if (tiers.PAYS == null || tiers.AD1 == null || tiers.RIB1 == null || tiers.DOM1 == null || tiers.RIBCLE == null || tiers.RIBGUICHET == null)
                    {
                        Alert = true;
                    }
                }
            }
            else
            {
                var rjl = __db.RJL1.Where(x => x.CODE == codeJournal).FirstOrDefault();
                if (rjl.BANQUE == null || rjl.AGENCE == null || rjl.GUICHET == null || rjl.RIB == null || rjl.CLE == null /*|| rjl.IBAN == null*/)
                {
                    Alert = true;
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "Success", msg = "Pourriez-vous s'il vous plaît vérifier votre Parametrage dans TOM² PRO ? Il semble qu'il manque une information importante.", data = Alert }));
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

            var Anomalie = tom.RTIERS.Where(x => x.RIB1 == null || x.NOM == null || x.AD1 == null || x.DOM1 == null).ToList();

            var JournalAnomalie = __db.RJL1.Where(x => x.NATURE == "2" && x.JLTRESOR == true && (x.RIB == null || x.AGENCE == null || x.GUICHET == null || x.BANQUE == null)).ToList();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            ANOMALIE_G SAUVEANOMALIE = new ANOMALIE_G();
            if (db.ANOMALIE_G.Any(x => x.IDPROJECT == PROJECTID))
            {
                var GetallAnomalieProjet = db.ANOMALIE_G.Where(x => x.IDPROJECT == PROJECTID).ToList();
                foreach (var Sup in GetallAnomalieProjet)
                {
                    db.ANOMALIE_G.Remove(Sup);
                }
                //db.ANOMALIE_G.RemoveRange(db.ANOMALIE_G);
                db.SaveChanges();
            }

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
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Traitement avec succès.", data = DataAnomalie, datas = DataAnomalieJournal }, settings));
        }
        public JsonResult SendEmailSuppliersGED(SI_USERS suser, int PROJECTID, string idLiquidation)
        {
            // Connexion à la base de données
            SOFTCONNECTOM.connex = new Data.Extension().GetCon(PROJECTID);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            // Vérification de l'utilisateur
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            if (exist == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion." }));
            }

            // Récupération des paramètres de l'email
            var mailConfig = db.SI_MAIL.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null);
            if (mailConfig == null || string.IsNullOrEmpty(mailConfig.SENDMAIL) || string.IsNullOrEmpty(mailConfig.SENDPWD))
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mail émetteur (Notifications et Alertes)." }));
            }

            string MailAdresse = mailConfig.SENDMAIL;
            string mdpMail = mailConfig.SENDPWD;

            // Récupération des informations de l'historique
            var send = db.OPA_HISTORIQUEBR.FirstOrDefault(x => x.NUMENREG == idLiquidation && x.IDSOCIETE == PROJECTID);
            if (send == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Aucune donnée trouvée pour cette liquidation." }));
            }

            string email = send.LIEN;
            string Obj = send.OBJET;
            string Title = send.TITLE;
            string doc = send.DOC;
            string message = send.MESSAGE;
            string ProjetIntitule = db.SI_PROJETS.Where(a => a.ID == PROJECTID && a.DELETIONDATE == null).Select(a => a.PROJET).FirstOrDefault();

            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    // Configuration du SMTP
                    SmtpClient smtp = new SmtpClient("smtpauth.moov.mg")
                    {
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential(MailAdresse, mdpMail),
                        EnableSsl = true
                    };

                    mail.From = new MailAddress(MailAdresse);
                    //mail.To.Add("fiderana.rakotonirinarisoa@softwell.mg"); // Remplacer par email si nécessaire
                    mail.To.Add(email); // Remplacer par email si nécessaire

                    // Ajout des destinataires supplémentaires
                    if (!string.IsNullOrEmpty(mailConfig.MAILTE))
                    {
                        string[] mailListe = mailConfig.MAILTE.Split(';', (char)StringSplitOptions.RemoveEmptyEntries);
                        foreach (var mailto in mailListe)
                        {
                            mail.To.Add(mailto);
                        }
                    }

                    // Contenu de l'email
                    mail.Subject = "Avis de réglement";
                    mail.IsBodyHtml = true;
                    mail.Body = $"Madame, Monsieur,<br/><br>" +
                                $"Nous vous informons que le paiement en relation avec le document {doc} que vous avez transmis à {ProjetIntitule} a été effectué.<br/><br>" +
                                $"<b><u>Titre du document</u></b>: {Title} <br/>" +
                                $"<b><u>Objet</u></b>: {Obj} <br/>" +
                                $"<b><u>Message</u></b>: {message} <br/><br>" +
                                "Cordialement.";

                    // Envoi de l'email
                    smtp.Send(mail);

                    // Mise à jour de la notification
                    send.NOTIF = true;
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Email envoyé avec succès", data = "" }));
                }
            }
            catch (SmtpException smtpEx)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur SMTP : " + smtpEx.Message }));
            }
            catch (Exception ex)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Une erreur s'est produite : " + ex.Message }));
            }
        }

        public string GetTypeBanque(string codeproject, SI_USERS suser)
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
        public static void EncryptFile(string inputFile, string publicKeyFile, string outputFile)
        {
            PgpPublicKey publicKey = LoadPublicKey(publicKeyFile);

            using (FileStream inputFileStream = System.IO.File.OpenRead(inputFile))
            using (FileStream outputFileStream = System.IO.File.Create(outputFile))
            {
                // Créer un flux de chiffrement sans compression
                PgpEncryptedDataGenerator encryptedDataGenerator = CreateEncryptionStream(publicKey);

                // Ouvrir le flux de chiffrement et écrire directement dans le fichier de sortie
                using (Stream encryptedOut = encryptedDataGenerator.Open(outputFileStream, new byte[1 << 16])) // 64k buffer
                {
                    // Copier les données directement dans le flux de chiffrement
                    inputFileStream.CopyTo(encryptedOut);
                }
            }
        }
        // Fonction pour charger la clé publique à partir du fichier .asc
        public static PgpPublicKey LoadPublicKey(string publicKeyFile)
        {
            using (FileStream keyInStream = System.IO.File.OpenRead(publicKeyFile))
            {
                PgpPublicKeyRingBundle keyRingBundle = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(keyInStream));

                foreach (PgpPublicKeyRing keyRing in keyRingBundle.GetKeyRings())
                {
                    return keyRing.GetPublicKey(); // Retourne la première clé publique trouvée
                }

                throw new Exception("Clé publique non trouvée dans le fichier.");
            }
        }

        // Fonction pour créer un générateur de flux de chiffrement PGP
        public static PgpEncryptedDataGenerator CreateEncryptionStream(PgpPublicKey publicKey)
        {
            PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true);
            encryptedDataGenerator.AddMethod(publicKey);
            return encryptedDataGenerator;
        }
        public static void DecryptFile(string inputFile, string privateKeyFile, string outputFile)
        {
            // Charger la clé privée à partir du fichier sans passphrase
            PgpPrivateKey privateKey = LoadPrivateKey(privateKeyFile);

            // Assurez-vous que la clé privée a été chargée correctement
            if (privateKey == null)
            {
                throw new Exception("La clé privée n'a pas pu être chargée.");
            }

            try
            {
                // Ouvrir le fichier chiffré en lecture
                using (FileStream inputFileStream = System.IO.File.OpenRead(inputFile))
                using (FileStream outputFileStream = System.IO.File.Create(outputFile))
                {
                    // Créer un objet PgpObjectFactory pour analyser les objets PGP du fichier
                    PgpObjectFactory pgpObjectFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputFileStream));

                    // Récupérer la liste des données chiffrées (PgpEncryptedDataList)
                    object pgpObject = pgpObjectFactory.NextPgpObject();
                    PgpEncryptedDataList encryptedDataList = pgpObject as PgpEncryptedDataList;

                    if (encryptedDataList == null)
                    {
                        throw new Exception("Le fichier ne contient pas de données chiffrées PGP.");
                    }

                    // Chercher la première entrée chiffrée valide avec une boucle for
                    PgpPublicKeyEncryptedData encryptedData = null;
                    for (int i = 0; i < encryptedDataList.Count; i++)
                    {
                        if (encryptedDataList[i] is PgpPublicKeyEncryptedData)
                        {
                            encryptedData = (PgpPublicKeyEncryptedData)encryptedDataList[i];
                            break;
                        }
                    }

                    // Vérifier si nous avons trouvé des données chiffrées valides
                    if (encryptedData == null)
                    {
                        throw new Exception("Aucune donnée chiffrée PGP valide trouvée.");
                    }

                    // Déchiffrer les données avec la clé privée
                    using (Stream decryptedDataStream = encryptedData.GetDataStream(privateKey))
                    {
                        // Copier les données déchiffrées dans le fichier de sortie
                        decryptedDataStream.CopyTo(outputFileStream);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Erreur lors de la lecture ou de l'écriture du fichier.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur inconnue est survenue lors du déchiffrement du fichier.", ex);
            }
        }
        // Fonction pour charger la clé privée à partir du fichier .asc (avec passphrase)
        public static PgpPrivateKey LoadPrivateKey(string privateKeyFile)
        {
            using (FileStream keyInStream = System.IO.File.OpenRead(privateKeyFile))
            {
                // Créer le paquet de clés privées à partir du fichier
                PgpSecretKeyRingBundle keyRingBundle = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyInStream));

                // Chercher la première clé secrète dans le paquet
                foreach (PgpSecretKeyRing keyRing in keyRingBundle.GetKeyRings())
                {
                    PgpSecretKey secretKey = keyRing.GetSecretKey();

                    // Extraire la clé privée (sans passphrase, si non protégée)
                    PgpPrivateKey privateKey = secretKey.ExtractPrivateKey(null);  // Passphrase est null car il n'y en a pas

                    if (privateKey != null)
                    {
                        return privateKey; // Retourner la première clé privée trouvée
                    }
                }

                throw new Exception("Clé privée non trouvée dans le fichier.");
            }
        }
        public JsonResult MiseAdisposition(SI_USERS suser, string codeproject)
        {
            int PROJECTID = int.Parse(codeproject);
            var usr = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.IDPROJET == PROJECTID && x.DELETIONDATE == null).FirstOrDefault();
            List<string> site = new List<string>();
            var siteS = db.SI_SITE.Where(ST => ST.IDUSER == usr.ID && ST.IDPROJET == PROJECTID).Select(ST => ST.SITE).FirstOrDefault();
            foreach (var item in siteS.Split(','))
            {
                site.Add(item);
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "", data = "" }, settings));
        }
        public static void EncryptFileWithGPG(string inputFile, string publicKeyFile, string outputFile)
        {
            // Ajouter la clé publique GPG au trousseau de clés
            string importKeyCommand = $"--import \"{publicKeyFile}\"";
            ExecuteGPGCommand(importKeyCommand);

            // Chiffrer le fichier avec la clé publique
            string encryptCommand = $"--output \"{outputFile}\" --encrypt --recipient-file \"{publicKeyFile}\" \"{inputFile}\"";
            ExecuteGPGCommand(encryptCommand);
        }
        public static void DecryptFileWithGPG(string inputFile, string privateKeyFile, string outputFile)
        {
            // Ajouter la clé privée GPG au trousseau de clés
            string importKeyCommand = $"--import \"{privateKeyFile}\"";
            ExecuteGPGCommand(importKeyCommand);

            // Déchiffrer le fichier avec la clé privée
            string decryptCommand = $"--output \"{outputFile}\" --decrypt \"{inputFile}\"";
            ExecuteGPGCommand(decryptCommand);
        }
        private static void ExecuteGPGCommand(string command)
        {
            string gpgPath = @"C:\Program Files (x86)\GnuPG\bin\gpg.exe";
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo
                {
                    //FileName = "gpg", // Exécute le programme GPG
                    FileName = gpgPath, // Exécute le programme GPG
                    Arguments = command, // Ajoute les arguments pour importer ou chiffrer
                    RedirectStandardOutput = true, // Rediriger la sortie pour capturer l'output
                    RedirectStandardError = true, // Rediriger les erreurs pour capturer les messages d'erreur
                    UseShellExecute = false, // Ne pas utiliser le shell (important pour rediriger la sortie)
                    CreateNoWindow = true // Ne pas créer de fenêtre de commande
                };

                using (Process process = Process.Start(pro))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"GPG Error: {error}");
                    }

                    Console.WriteLine(output); // Afficher la sortie standard de GPG
                }
            }
            catch (Exception ex)
            {
                string cheminFichierLog = AppDomain.CurrentDomain.BaseDirectory + "\\FILERESULT\\" + "logErreurCMD.txt";

                // Créer ou ouvrir le fichier de log
                using (StreamWriter writer = new StreamWriter(cheminFichierLog, true)) // 'true' pour ajouter au fichier existant
                {
                    // Écrire l'exception dans le fichier de log
                    writer.WriteLine("--------------------------------------------------");
                    writer.WriteLine("Date et heure : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);
                    writer.WriteLine("--------------------------------------------------");
                }
            }

        }
        public JsonResult GetListeBanqueMAD(SI_USERS suser, string codeproject)
        {
            var PROJECTID = int.Parse(codeproject);
            var B = db.OPA_BANQUE.FirstOrDefault();
            var banque = db.OPA_BANQUE.ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", data = banque, msg = "" }, settings));
        }
        public string GetChoixBtn(string codeproject, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            int PROJECTid = int.Parse(codeproject);
            if (exist == null) return "";

            if (exist.IDPROJET != 0)
            {
                var TypeBtn = db.SI_TYPEBANQUE.FirstOrDefault(a => a.IDPROJET == PROJECTid).TypeBtn;
                return TypeBtn.ToString();
            }
            else
            {
                //var mapuser = db.SI_MAPUSERPROJET.Where(a => a.IDUS == exist.ID).ToList();
                int PROJECTID = int.Parse(codeproject);
                var ii = db.SI_TYPECRITURE.FirstOrDefault(a => a.IDPROJET == PROJECTID);
                var TypeBtn = "";
                if (ii != null)
                {
                    TypeBtn = ii.TYPE.ToString();
                }
                else
                {
                    TypeBtn = "Veuillez parametrer votre type de fichier";
                }
                return TypeBtn.ToString();
            }
            //return TypeFileBQ;
        }
        [HttpGet]
        public ActionResult CheckSession()
        {
            // Vérifie si la session est active
            if (Session["UserId"] == null) // "UserId" est un exemple, remplacez par votre clé de session
            {
                return null;  // Session expirée, retourne un statut non autorisé (401)
            }

            return null;  // Session toujours active, retourne un statut OK (200)
        }
        static bool ConvertToPem(string privateKeyPath, string convertedKeyPath)
        {
            try
            {
                if (!System.IO.File.Exists(privateKeyPath))
                {
                    Console.WriteLine("❌ Clé privée OpenSSH introuvable !");
                    return false;
                }

                // Vérifier si ssh-keygen est disponible
                string sshKeygenPath = "ssh-keygen"; // Assurez-vous que ssh-keygen est dans le PATH
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = sshKeygenPath,
                    Arguments = $"-p -m PEM -f \"{privateKeyPath}\" -N \"\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine($"❌ Erreur ssh-keygen : {error}");
                        return false;
                    }
                }

                // Vérifier que la conversion a réussi
                if (!System.IO.File.Exists(convertedKeyPath))
                {
                    Console.WriteLine("❌ Erreur : La clé convertie n'existe pas !");
                    return false;
                }

                Console.WriteLine("✅ Conversion réussie en format PEM.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception lors de la conversion : {ex.Message}");
                return false;
            }
        }
        public static void EncryptXmlFile(string xmlFilePath, string certFilePath, string outputEncryptedFile)
        {
            // Lire le contenu XML
            string xmlContent = System.IO.File.ReadAllText(xmlFilePath);

            // Construire le corps MIME avec le XML
            var builder = new BodyBuilder();
            // Joindre le fichier XML comme pièce jointe
            var attachment = new MimeKit.MimePart("application", "xml")
            {
                FileName = Path.GetFileName(xmlFilePath),
                Content = new MimeContent(System.IO.File.OpenRead(xmlFilePath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64
            };

            builder.Attachments.Add(attachment);
            var body = builder.ToMessageBody(); // MimeEntity

            // Créer le MimeMessage avec le corps et un sujet
            var message = new MimeMessage();
            message.Subject = "Message XML chiffré";
            message.Body = body; // Associer le corps à un MimeMessage complet

            // Charger le certificat (clé publique RSA)
            var cert = new X509Certificate2(certFilePath);

            // Créer un CmsRecipient à partir du certificat
            var recipient = new MimeKit.Cryptography.CmsRecipient(cert);
            // Créer une CmsRecipientCollection pour le chiffrement
            var recipientCollection = new MimeKit.Cryptography.CmsRecipientCollection { recipient };
            // Créer le contexte de chiffrement
            using (var context = new TemporarySecureMimeContext())
            {
                // Chiffrer le message complet avec les bons arguments
                var encrypted = ApplicationPkcs7Mime.Encrypt(recipientCollection, body);

                // Sauvegarder le fichier chiffré .p7m.
                using (var output = System.IO.File.Create(outputEncryptedFile))
                {
                    encrypted.WriteTo(output);
                }

                Console.WriteLine("✅ Fichier XML chiffré avec succès !");
            }
        }

    }
}
