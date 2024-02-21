using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using apptab.Extension;
using apptab;
using Aspose.Zip.Saving;
using Aspose.Zip;
using Newtonsoft.Json;
using Renci.SshNet;
using System.Net;
using System.Collections;
using apptab.Models;

namespace apptab.Controllers
{
    public class HomeController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTOM __db = new SOFTCONNECTOM();

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
            return View();
        }
        public ActionResult TeacherValidation() {
            return View();
        }
        public ActionResult FValidation() {
            return View();
        }
        public ActionResult BonPourPaiement() {
            return View();
        }
        [HttpPost]
        public string GetTypeP(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return "";

            var basename = db.SI_TYPECRITURE.FirstOrDefault(a => a.IDUSER == exist.ID && a.IDPROJET == exist.IDPROJET).TYPE;

            return basename.ToString();
        }
        [HttpPost]
        public JsonResult FillTable(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD) != null;
            if (!exist) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var droits = db.OPA_DROITS.Where(a => a.IDSOCIETE == suser.IDPROJET).Select(a => new
                {
                    USER = db.SI_USERS.FirstOrDefault(x => x.ID == a.IDUSER).LOGIN,
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
        public string CreateAFBTXTArch(string pathchemin, string pathfiles, string psw)
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

            Archive archive = new Archive(new ArchiveEntrySettings(encryptionSettings: new TraditionalEncryptionSettings(psw)));

            // Add files to the archive
            archive.CreateEntry(pathchemin + ".txt", pth + pathchemin + ".txt");
            string source = pth + pathchemin + ".zip";
            archive.Save(source);
            return source;
        }
        [HttpPost]
        public ActionResult CreateZipFile(SI_USERS suser/*, List<AnalY> analY*/, int intbasetype, bool devise, string codeJ, string baseName)
        {
            StreamWriter sw = null;
            AFB160 aFB160 = new AFB160();
            var send = "";
            var ps = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.IDPROJET == suser.IDPROJET).Select(x => x.PWD).FirstOrDefault();
            if (baseName == "1")
            {
                var pathfile = aFB160.CreateTOMPAIEAFB160(devise, codeJ, suser);
                if (intbasetype == 0)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                }
                else if (intbasetype == 1)
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                else if (intbasetype == 2)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    //GenererG(send);
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }


                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Archive Success ", data = send }, settings));
            }
            else if (baseName == "2")
            {
                var pathfile = aFB160.CreateTOMPROAFB160(devise, codeJ, suser);
                if (intbasetype == 0)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                }
                else if (intbasetype == 1)
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                else if (intbasetype == 2)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }


                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Archive Success ", data = send }, settings));
            }
            else
            {
                var pathfile = aFB160.CreateBRAFB160(devise, codeJ, suser);
                //var send = "";
                if (intbasetype == 0)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                }
                else if (intbasetype == 1)
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                }
                else if (intbasetype == 2)
                {
                    send = CreateAFBTXT(pathfile.Chemin, pathfile.Fichier);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                else
                {
                    send = CreateAFBTXTArch(pathfile.Chemin, pathfile.Fichier, ps);
                    var ftp = db.OPA_FTP.Where(x => x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                    SENDFTP(ftp.HOTE, ftp.PATH, ftp.IDENTIFIANT, ftp.FTPPWD, send);
                }
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Archive Success ", data = send }, settings));
            }
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

        [HttpPost]
        public JsonResult getelementjs(int ChoixBase, string journal, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP,/* int mois, int annee, string matr1, string matr2, DateTime datePaie,*/ SI_USERS suser)
        {
            var choixBase__ = GetTypeP(suser);
            AFB160 afb160 = new AFB160();
            //var hst = db.OPA_HISTORIQUE.Select(x => x.NUMENREG.ToString()).ToArray();
            var hstSiig = db.OPA_VALIDATIONS.Where(x => x.ETAT != 4).Select(x => x.IDREGLEMENT.ToString()).ToArray();
            var list = afb160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => !hstSiig.Contains(x.No.ToString())).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));

        }
        [HttpPost]
        public JsonResult getelementjsBR(int ChoixBase, string journal, DateTime datein, bool devise, DateTime dateout, string comptaG, string auxi, string auxi1, string etat, DateTime dateP,/* int mois, int annee, string matr1, string matr2, DateTime datePaie,*/ SI_USERS suser)
        {
            var choixBase__ = GetTypeP(suser);
            AFB160 afb160 = new AFB160();
            //var hst = db.OPA_HISTORIQUEBR.Where(x => x.IDSOCIETE == suser.IDPROJET).Select(x => x.NUMENREG.ToString()).ToArray();
            var hstSiig = db.OPA_VALIDATIONS.Where(x => x.ETAT != 4).Select(x => x.IDREGLEMENT.ToString()).ToArray();

            var list = afb160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => !hstSiig.Contains(x.No.ToString())).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));

        }
        [HttpPost]
        public JsonResult GetEtat()
        {

            List<string> listEtat = __db.OP_CHAINETRAITEMENT.Select(x => x.ETAT).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "sucess", msg = "", data = listEtat }));
        }
        [HttpPost]
        public JsonResult GetCODEJournal(/*string baseName,*/SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }
            #region comms
            //if (baseName == "1")
            //{
            //	OPAVITOMATE.connex = "Data Source=FID-INF-PC;Initial Catalog=TOMPAIE;User ID=sa;Password=Soft123well!;";
            //}else 
            //{
            //	OPAVITOMATE.connex = "Data Source=FID-INF-PC;Initial Catalog=PIC3;User ID=sa;Password=Soft123well!;";
            //}
            //OPAVITOMATE.connex = "Data Source=FID-INF-PC;Initial Catalog=TOMPAIE;User ID=sa;Password=Soft123well!;";
            //SOFTCONNECTOM.connex = "Data Source=DESKTOP-N8EMIRC;Initial Catalog=PIC;Integrated Security=True";
            //OPAVITOMATE.connex = "Data Source=NOM-IT-PC;Initial Catalog=PIC3;Integrated Security=True";
            #endregion
            SOFTCONNECTOM __db = new SOFTCONNECTOM();

            int crpt = exist.IDPROJET.Value;
            //Check si le projet est mappé à une base de données TOM²PRO//
            if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt) == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            var JournalVM = __db.RJL1.Select(x => new {
                CODE = x.CODE,
                LIBELLE = x.LIBELLE
            }).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = JournalVM }, settings));
        }
        [HttpPost]
        public JsonResult GetCompteG(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }

            int baseId = int.Parse(basename);
            var CompteG = __db.MCOMPTA.Where(a => a.COGE.StartsWith("4")).GroupBy(x => x.COGE).Select(x => new
            {
                COGE = x.Key,
                AUXI = x.Select(y => y.AUXI).Distinct().ToList()
            }).ToList();

            var CompteGBR = __db.MOP.Where(a => a.COGE.StartsWith("4")).GroupBy(x => x.COGE).Select(x => new
            {
                COGE = x.Key,
                AUXI = x.Select(y => y.AUXI/*new { AUXI = y.AUXI, NOM = y.NOM }*/).Distinct().ToList()
            }).ToList();
            if (baseId == 3)
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = CompteGBR }, settings));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = CompteG }, settings));
            }
            
        }
        [HttpPost]
        public JsonResult GetCheckedCompte(string baseName, DateTime datein, DateTime dateout, string comptaG, string auxi, DateTime dateP, string listCompte, string journal, string etat, bool devise, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }

            if (string.IsNullOrEmpty(listCompte))
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = "" }, settings));

            List<string> list = listCompte.Split(',').ToList();

            #region CommOpavi
            //List<string> numBR = listCompte.Split(',').ToList();
            //List<OPA_REGLEMENTBR> brResult = new List<OPA_REGLEMENTBR>();
            //List<DataListTompro> listReg = new List<DataListTompro>();
            //List<DataListTomOP> listRegBR = new List<DataListTomOP>();
            //AFB160 aFB160 = new AFB160();
            //if (baseName == "3")
            //{

            //    aFB160.SaveValideSelectEcritureBR(numBR, journal, etat, devise, suser);
            //    foreach (var item in numBR)
            //    {
            //        brResult.Add(db.OPA_REGLEMENTBR.Where(x => x.NUM == item && x.CODE_J == journal && x.IDSOCIETE == suser.IDPROJET).FirstOrDefault());
            //    }
            //    listRegBR = aFB160.getREGLEMENTBR(suser);

            //    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = listRegBR }, settings));
            //}
            //else
            //{
            //    aFB160.SaveValideSelectEcriture(list, true, suser);

            //    listReg = aFB160.getREGLEMENT(suser);
            //    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = listReg }, settings));
            //}
            #endregion
            
            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            if (basename == "2")
            {
                string auxi1 = auxi;
                AFB160 afb160 = new AFB160();
                var hst = db.OPA_HISTORIQUE.Select(x => x.NUMENREG.ToString()).ToArray();
                foreach (var h in list)
                {
                    var listA = afb160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => x.No.ToString() == h).ToList();
                    foreach (var item in listA)
                    {
                        avalider.IDREGLEMENT = (int)item.No;
                        avalider.ETAT = 0;
                        avalider.IDPROJET = suser.IDPROJET;
                        avalider.DateIn = datein;
                        avalider.DateOut = dateout;
                        avalider.ComptaG = comptaG;
                        avalider.auxi = auxi;
                        avalider.DateP = dateP;
                        avalider.Journal = journal;
                        avalider.dateOrdre = item.DateOrdre;
                        avalider.NoPiece = item.NoPiece;
                        avalider.Compte = item.Compte;
                        avalider.Libelle = item.Libelle;
                        avalider.Debit = item.Debit;
                        avalider.Credit = item.Credit;
                        avalider.MontantDevise = item.MontantDevise;
                        avalider.Mon = item.Mon;
                        avalider.Rang = item.Rang;
                        avalider.Poste = item.Poste;
                        avalider.FinancementCategorie = item.FinancementCategorie;
                        avalider.Commune = item.Commune;
                        avalider.Plan6 = item.Plan6;
                        avalider.Marche = item.Marche;
                        avalider.Statut = item.Statut;
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
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = "" }, settings));
        }
        [HttpPost]
        public JsonResult GetElementAvalider(string ChoixBase, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (ChoixBase == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 0 && ecriture.ComptaG == comptaG && ecriture.Journal == journal).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 0 && ecriture.ComptaG == comptaG && ecriture.DateIn == datein && ecriture.DateOut == dateout && ecriture.auxi == auxi && ecriture.Journal == journal).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }

        }
        //Envoye Validations
        [HttpPost]
        public JsonResult GetElementAvaliderLoad(SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            
            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }
            if (basename == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 0).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 0 ).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }

        }

        [HttpPost]
        public JsonResult GetElementValiderF(string listCompte , SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            bool devise = false;
            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }
            List<string> list = listCompte.Split(',').ToList();
            List<string> numBR = listCompte.Split(',').ToList();
            var AvaliderList = db.OPA_VALIDATIONS.Where(a=>list.Contains(a.IDREGLEMENT.ToString()) && a.ETAT == 0);
            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();

            if (basename == "2")
            {
                aFB160.SaveValideSelectEcriture(list, true, suser);
            }
            else
            {
                foreach (var item in AvaliderList)
                {
                    aFB160.SaveValideSelectEcritureBR(numBR, item.Journal, item.ETAT.ToString(), devise, suser);
                }
                
            }
            foreach (var item in list)
            {
                int b  = int.Parse(item);
                avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b && a.ETAT == 0).FirstOrDefault();
                if (avalider != null)
                {
                    try
                    {
                        avalider.ETAT = 1;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                        throw;
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = "" }, settings));
        }
        //ETAT = 1
        [HttpPost]
        public JsonResult GetAcceptecriture(string ChoixBase, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (ChoixBase == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 1 && ecriture.ComptaG == comptaG  && ecriture.auxi == auxi && ecriture.Journal == journal).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 1 && ecriture.ComptaG == comptaG && ecriture.DateIn == datein && ecriture.DateOut == dateout && ecriture.auxi == auxi && ecriture.Journal == journal).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
        }
        [HttpPost]
        public JsonResult GetAcceptecritureLoad(SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var basename = GetTypeP(suser);
            if (basename == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez Parametrer le Types D'ecriture avant toutes Opérations." }, settings));
            }

            if (basename == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 1).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 1).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
        }

        [HttpPost]
        public JsonResult GetAcceptecritureF(string listCompte, SI_USERS suser)
        {//validations
            AFB160 aFB160 = new AFB160();
            List<string> list = listCompte.Split(',').ToList();
            List<string> numBR = listCompte.Split(',').ToList();
            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            
            foreach (var item in list)
            {
                int b = int.Parse(item);
                avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b).FirstOrDefault();
                if (avalider != null)
                {
                    try
                    {
                        avalider.IDREGLEMENT = int.Parse(item);
                        avalider.ETAT = 2;
                        avalider.DATESEND = DateTime.Now.Date;
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
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                        throw;
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = "" }, settings));
        }
        [HttpPost]
        public JsonResult EnvoyeValidatioF(string ChoixBase, DateTime datein, DateTime dateout, string comptaG, string auxi, string auxi1, DateTime dateP, string journal, string etat, bool devise, SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (ChoixBase == "2")
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 2 && ecriture.ComptaG == comptaG && ecriture.DateIn == datein && ecriture.DateOut == dateout && ecriture.auxi == auxi && ecriture.Journal == journal).Select(a => a.IDREGLEMENT).ToList();
                //var list = aFB160.getListEcritureCompta(journal, datein, dateout, comptaG, auxi, auxi1, dateP, suser).Where(x => avalider.Contains((int)x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
            else
            {
                var avalider = db.OPA_VALIDATIONS.Where(ecriture => ecriture.IDPROJET == suser.IDPROJET && ecriture.ETAT == 2 && ecriture.ComptaG == comptaG && ecriture.DateIn == datein && ecriture.DateOut == dateout && ecriture.auxi == auxi && ecriture.Journal == journal).Select(a => a.IDREGLEMENT).ToList();
                //var list = aFB160.getListEcritureBR(journal, datein, dateout, devise, comptaG, auxi, etat, dateP, suser).Where(x => avalider.ToString().Contains(x.No)).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = avalider }, settings));
            }
        }
        [HttpPost]
        public JsonResult ValidationsEcrituresF(string baseName, DateTime datein, DateTime dateout, string comptaG, string auxi, DateTime dateP, string listCompte, string journal, string etat, bool devise, SI_USERS suser)
        {
            List<DataListTompro> listReg = new List<DataListTompro>();
            List<DataListTompro> listReg__ = new List<DataListTompro>();
            List<DataListTomOP> listRegBR = new List<DataListTomOP>();
            List<DataListTomOP> listRegBR__ = new List<DataListTomOP>();
            AFB160 aFB160 = new AFB160();
            List<string> list = listCompte.Split(',').ToList();
            if (baseName == "2")
            {
                listReg = aFB160.getREGLEMENT(suser);
            }
            else
            {
                listRegBR = aFB160.getREGLEMENTBR(suser);
            }
            
            OPA_VALIDATIONS avalider = new OPA_VALIDATIONS();
            foreach (var item in list)
            {
                int b = int.Parse(item);
                avalider = db.OPA_VALIDATIONS.Where(a => a.IDREGLEMENT == b).FirstOrDefault();
                if (avalider != null)
                {
                    try
                    {
                        avalider.IDREGLEMENT = b;
                        avalider.ETAT = 3;
                        avalider.DATEVAL = DateTime.Now.Date;
                        avalider.IDPROJET = suser.IDPROJET;
                        avalider.DateIn = datein;
                        avalider.DateOut = dateout;
                        avalider.ComptaG = comptaG;
                        avalider.auxi = auxi;
                        avalider.DateP = dateP;
                        avalider.Journal = journal;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur de connexion", data = ex.Message }, settings));
                        throw;
                    }
                    if (baseName == "2")
                    {
                        listReg__.Add(listReg.Where(a => (int)a.No == int.Parse(item)).FirstOrDefault());
                    }
                    else
                    {

                        listRegBR__.Add(listRegBR.Where(a => a.No == item).FirstOrDefault());
                    }
                }
            }
            if (baseName == "2")
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = listReg__ }, settings));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = listRegBR__ }, settings));
            }
                
        }
        //END ETAT = 1
        [HttpPost]
        public JsonResult LoadValidateEcriture(SI_USERS suser)
        {
            AFB160 aFB160 = new AFB160();
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var val = db.OPA_VALIDATIONS.Where(a=> a.DATESEND != null && a.IDPROJET == exist.IDPROJET && a.ETAT == 2).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "Success", msg = "Connexion avec success. " , data = val}, settings));
        }
        public JsonResult CancelEcriture(int id, string motif, string commentaire, SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            var cancel = db.OPA_VALIDATIONS.Where(x => x.IDREGLEMENT == id).FirstOrDefault();
            //OPA_HCANCEL Hcancel = new OPA_HCANCEL();
            if (cancel != null)
            {
                if (motif != "" && commentaire != "")
                {
                    cancel.ETAT = 4;
                    cancel.MOTIF = motif;
                    cancel.COMS = commentaire;
                    cancel.DATEANNULER = DateTime.Now;
                    cancel.IDUSER = exist.ID;
                    //historique
                    //Hcancel.IDREGLEMENT = id;
                    //Hcancel.COMS = commentaire;
                    //Hcancel.MOTIF = motif;
                    //Hcancel.DATECANCEL = DateTime.Now;
                    //Hcancel.IDUSER = exist.ID;
                    //Hcancel.IDPROJETS = suser.IDPROJET;
                    //Hcancel.ETAT = 4;

                    //db.OPA_HCANCEL.Add(Hcancel);
                    db.SaveChanges();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = "" }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Motif ET COMMENTAIRE OBLIGATOIR.", data = "" }, settings));
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = "" }, settings));
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succés.", data = "" }, settings));
        }
        public JsonResult GetCheckedComptePaie(string baseName, int mois, int annee, string listCompte, string matriculeD, string matriculeF, bool devise, DateTime dateP, string journal, SI_USERS suser)
        {
            if (string.IsNullOrEmpty(listCompte))
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = "" }, settings));

            List<OPA_REGLEMENTBR> brResult = new List<OPA_REGLEMENTBR>();
            List<string> listReg = new List<string>();
            listReg = listCompte.Split(',').ToList();
            AFB160 aFB160 = new AFB160();
            try
            {

                aFB160.SaveValideSelectEcriturePaie(listReg, journal, devise, suser);
                //var zz = aFB160.getListEcriturePaie(journal, mois, annee, matriculeD, matriculeF, dateP, suser);
                var listePaie = aFB160.getREGLEMENTPaie(suser);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = listePaie }, settings));
            }
            catch (Exception ex)
            {

                return Json(JsonConvert.SerializeObject(new { type = "error", msg = ex.Message, data = ex.Message }, settings));
            }


        }
        public JsonResult GetAnomalieBack(SI_USERS suser, string baseName)
        {
            AFB160 Afb = new AFB160();
            if (baseName == "3")
            {
                //var anom = db.OPA_ANOMALIEBR.ToList();
                var resultAnomalies = Afb.getListAnomalieBR(suser);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = resultAnomalies }, settings));
            }
            else
            {
                //var anom = db.OPA_ANOMALIE.ToList();
                var resultAnomalies = Afb.getListAnomalie(suser);
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = resultAnomalies }, settings));
            }

        }
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
            catch (Exception ex)
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
        public JsonResult GetHistoriques(SI_USERS suser)
        {

            var usr = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.IDPROJET == suser.IDPROJET).FirstOrDefault();

            var query = db.OPA_HISTORIQUE
                .Where(x => x.IDSOCIETE == suser.IDPROJET && x.IDUSER == usr.ID)
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
                    LOGIN = user.LOGIN
                })
                .OrderBy(x => x.DATE).ToList();
            var queryBr = db.OPA_HISTORIQUEBR
                .Where(x => x.IDSOCIETE == suser.IDPROJET && x.IDUSER == usr.ID)
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
                    LOGIN = user.LOGIN
                })
                .OrderBy(x => x.DATE).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = query, databr = queryBr }, settings));
        }
        public JsonResult GetCancel(SI_USERS suser, string listCompte)
        {
            var list = listCompte.Split(',');
            List<OPA_HISTORIQUE> result = new List<OPA_HISTORIQUE>();
            List<OPA_HISTORIQUEBR> resultBR = new List<OPA_HISTORIQUEBR>();
            var user = db.SI_USERS.Where(x => x.LOGIN == suser.LOGIN && x.PWD == suser.PWD).FirstOrDefault();
            foreach (var item in list)
            {
                if (item.Contains("BR"))
                {
                    resultBR = db.OPA_HISTORIQUEBR.Where(y => y.NUMENREG == item && y.IDUSER == user.ID && y.IDSOCIETE == suser.IDPROJET).ToList();
                }
                else
                {
                    decimal ii;
                    ii = Convert.ToDecimal(item);
                    result = db.OPA_HISTORIQUE.Where(x => x.NUMENREG == ii && x.IDUSER == user.ID && x.IDSOCIETE == suser.IDPROJET).ToList();

                }


                foreach (var pc in result)
                {
                    db.OPA_HISTORIQUE.Remove(pc);
                }
                foreach (var br in resultBR)
                {
                    db.OPA_HISTORIQUEBR.Remove(br);
                }
            }
            db.SaveChanges();
            return Json(JsonConvert.SerializeObject(new { msg = "success", data = result, datebr = resultBR }));
        }
        public JsonResult SFTP(string HOTE, string PATH, string USERFTP, string PWDFTP, string SOURCE)
        {
            Console.WriteLine("Create client Object");
            using (SftpClient sftpClient = new SftpClient(getSftpConnection(HOTE, USERFTP, 22, SOURCE)))
            {
                Console.WriteLine("Connect to server");
                sftpClient.Connect();
                Console.WriteLine("Creating FileStream object to stream a file");
                using (FileStream fs = new FileStream("filePath", FileMode.Open))
                {
                    sftpClient.BufferSize = 1024;
                    sftpClient.UploadFile(fs, Path.GetFileName("filePath"));
                }
                sftpClient.Dispose();
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès.", data = "" }, settings));
        }

        private ConnectionInfo getSftpConnection(string hOTE, string uSERFTP, int v, string sOURCE)
        {
            throw new NotImplementedException();
        }
        private static AuthenticationMethod[] privateKeyObject(string username, string publicKeyPath)
        {
            PrivateKeyFile privateKeyFile = new PrivateKeyFile(publicKeyPath);
            PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod =
               new PrivateKeyAuthenticationMethod(username, privateKeyFile);
            return new AuthenticationMethod[]
             {
        privateKeyAuthenticationMethod
             };
        }
    }
}
