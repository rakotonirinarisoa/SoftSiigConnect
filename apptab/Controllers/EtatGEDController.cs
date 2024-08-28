using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using apptab.Data.Entities;
using static apptab.Controllers.UserController;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Aspose.Zip;
using System.EnterpriseServices.Internal;
using System.Web.Services.Description;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;
using static apptab.Controllers.EtatGEDController;
using Antlr.Runtime.Tree;
using System.Security.Cryptography.Xml;
using static apptab.Controllers.RSFController;

namespace apptab.Controllers
{
    public class EtatGEDController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTOM tom = new SOFTCONNECTOM();
        private readonly SOFTCONNECTGED ged = new SOFTCONNECTGED();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public class SiteGED
        {
            public Guid Id { get; set; }
            public string Code { get; set; }
        }
        public class Fournisseur
        {
            public Guid Id { get; set; }
            public string Nom { get; set; }
        }

        public static Guid[] DeserializeJsonToGuidArray(string jsonString)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Guid[]>(jsonString);
        }

        [HttpPost]
        public ActionResult GETALLSITE(SI_USERS suser, string iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                List<int> idProjet = new List<int>();

                foreach (var item in iProjet.Split(','))
                {
                    idProjet.Add(int.Parse(item));
                }

                List<SiteGED> crpto = new List<SiteGED>();

                if (idProjet != null)
                {
                    foreach (var crpt in idProjet)
                    {
                        if (!db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

                        if (!db.SI_USERS.Any(a => a.IDPROJET == crpt && a.DELETIONDATE == null && a.ID == exist.ID && a.IDUSERGED != null))
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                        else
                        {
                            var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                            if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                        }
                    }

                    foreach (var crpt in idProjet)
                    {
                        var isUserSet = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID);
                        var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                        Guid[] guidArray = DeserializeJsonToGuidArray(isUserGed.Sites);

                        foreach (Guid guid in guidArray)
                        {
                            crpto.Add(new SiteGED()
                            {
                                Id = guid,
                                Code = ged.Sites.Any(a => a.Id == guid && a.DeletionDate == null) ? ged.Sites.FirstOrDefault(a => a.Id == guid && a.DeletionDate == null).SiteId : ""
                            });
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = crpto } }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public ActionResult GETALLFOURNISSEUR(SI_USERS suser, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                int crpt = iProjet;

                List<Fournisseur> supl = new List<Fournisseur>();

                if (!db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

                if (db.SI_USERS.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && a.ID == exist.ID).IDUSERGED == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                else
                {
                    var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                    if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                }

                var isUserSet = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID);
                var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                Guid[] guidArray = DeserializeJsonToGuidArray(isUserGed.Sites);

                var suppliers = ged.Suppliers.Select(r => new {
                    NomSup = r.Name,
                    Id = r.Id,
                }).ToList();

                foreach (var it in suppliers)
                {
                    supl.Add(new Fournisseur()
                    {
                        Id = it.Id,
                        Nom = it.NomSup,
                    });
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = supl } }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
        public class TypeDoc
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public static string[] DeserializeJsonToStringArray(string jsonString)
        {
            return System.Text.Json.JsonSerializer.Deserialize<string[]>(jsonString);
        }

        [HttpPost]
        public ActionResult GETALLTYPEDOCS(SI_USERS suser, string iProjet, string iSite)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                List<int> idProjet = new List<int>();

                foreach (var item in iProjet.Split(','))
                {
                    idProjet.Add(int.Parse(item));
                }

                List<TypeDoc> crpto = new List<TypeDoc>();
                List<string> listSite = iSite.Split(',').ToList();

                if (idProjet != null)
                {
                    foreach (var crpt in idProjet)
                    {
                        if (!db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

                        if (db.SI_USERS.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null && a.ID == exist.ID).IDUSERGED == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                        else
                        {
                            var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                            if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                        }
                    }

                    foreach (var crpt in idProjet)
                    {
                        var isUserSet = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == crpt && b.DELETIONDATE == null && b.ID == exist.ID);
                        var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                        var isListeTypeD = ged.DocumentTypes.Where(a => a.ProjectId == isUserGed.ProjectId && a.DeletionDate == null).ToList();

                        if (isListeTypeD != null)
                        {
                            foreach (var typD in isListeTypeD)
                            {
                                string[] guidArray = DeserializeJsonToStringArray(typD.Sites);
                                var guidArrayList = guidArray.ToList();

                                var inlist = false;

                                foreach (var guid in guidArrayList)
                                {
                                    if (listSite.Contains(guid))
                                        inlist = true;
                                }

                                if (inlist)
                                {
                                    crpto.Add(new TypeDoc()
                                    {
                                        Id = typD.Id,
                                        Title = typD.Title
                                    });
                                }
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = crpto } }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        public class TDB
        {
            public string REFERENCE { get; set; }
            public string DOCUMENT { get; set; }
            public string FOURNISSEUR { get; set; }
            public string MONTANT { get; set; }
            public string TYPE { get; set; }
            public string STEPNOW { get; set; }
            public string STEPNEXT { get; set; }
            public string VALIDATEURNEXT { get; set; }
            public string DUREENEXT { get; set; }
            public string ARCHIVEDATE { get; set; }
            public string RATTACHTOM { get; set; }

            public List<string> DATESTEP { get; set; }
        }

        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, int PROJECTID, DateTime DateDebut, DateTime DateFin, string listSite, int status, string fournisseur, string reference)
        {
            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez parametrer le mappage GED ET PROJET. " }, settings));

            List<string> Projet = new List<string>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }
            try
            {
                if (!db.SI_PROGED.Any(a => a.IDPROJET == PROJECTID))
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

                if (db.SI_USERS.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.ID == exist.ID).IDUSERGED == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                else
                {
                    var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == PROJECTID && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                    if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                }
                Guid iddoc = Guid.Parse("09E1F88D-AB38-4F49-8F3A-B1C600BA31E0");
                var validationhistorique = ged.ValidationsHistory.Where(x => x.DocumentId == iddoc).OrderByDescending(x => x.CreationDate).Join(ged.DocumentSteps, valhist => valhist.ToDocumentStepId, docstep => docstep.Id, (valhist, docstep) => new
                {
                    ID = valhist.DocumentId,
                    IDDOCUMENTSTEP = docstep.Id,

                }).Join(ged.UsersSteps, idsteps => idsteps.IDDOCUMENTSTEP, userstep => userstep.DocumentStepId, (idsteps, userstep) => new
                {
                    userStep = userstep.UserId,
                }).FirstOrDefault();//id documentstep

                var user_validation = validationhistorique != null ? ged.Users.Where(x => x.Id == validationhistorique.userStep).FirstOrDefault() : null;
                var isUserSet = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == PROJECTID && b.DELETIONDATE == null && b.ID == exist.ID);
                var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);
                List<DocS> documentF = new List<DocS>();
                //var isListeTypeD = ged.DocumentTypes.Where(a => a.ProjectId == isUserGed.ProjectId && a.DeletionDate == null).ToList();
                Guid IDsup = Guid.Parse(fournisseur);
                Guid IDref = Guid.Parse(reference);

                var suppliersname = ged.Suppliers.Where(x => x.Id == IDsup).FirstOrDefault();
                var referenS = ged.SuppliersDocumentsAcknowledgements.Where(x => x.Id == IDref).FirstOrDefault();
                var doco = ged.Documents.Where(x => x.DeletionDate == null && x.CreationDate >= DateDebut && x.CreationDate <= DateFin && x.Status == status).ToList();
                var RefDoc = ged.Documents.Where(x => x.DeletionDate == null && x.CreationDate >= DateDebut && x.CreationDate <= DateFin).Join(ged.SuppliersDocumentsAcknowledgements, dcm => dcm.Id, sdal => sdal.Id, (dcm, sdal) => new
                {
                    ID = dcm.SenderId,
                    reference = sdal.ReferenceInterne,
                    Objet = dcm.Object,
                    Fournisseur = "",
                    Acusse = dcm.CreationDate,
                    Validateur = "",
                    Montant = 0,
                    Date = dcm.CreationDate,
                    Encours = dcm.Status,
                    ARCHIVES = "",
                    Lien = dcm.Url,
                    Site = dcm.Site,
                    DocumentID = dcm.Id
                }).Join(ged.Suppliers, dcm => dcm.ID, sup => sup.Id, (dcm, sup) => new
                {
                    ID = dcm.ID,
                    reference = dcm.reference,
                    Objet = dcm.Objet,
                    Fournisseur = sup.Name,
                    Acusse = dcm.Acusse,
                    Montant = 0,
                    Encours = dcm.Encours,
                    ARCHIVES = dcm.Encours == 3 ? dcm.Date.ToString() : "NON ARCHIVER",
                    Lien = dcm.Lien,
                    Site = dcm.Site,
                    DocumentID = dcm.DocumentID,
                    Validateur = ""
                }).Where(x => x.Fournisseur == suppliersname.Name && x.Encours == status && x.reference == referenS.ReferenceInterne).ToList();

                if (RefDoc != null)
                {
                    foreach (var typD in RefDoc)
                    {
                        string uservalidateur = "";
                        uservalidateur = ged.ValidationsHistory.Where(x => x.DocumentId == typD.DocumentID).Join(ged.Users, dc => dc.FromUserId, res => res.Id, (dc, res) => res.Username).FirstOrDefault();
                        string SSITE = typD.Site;
                        documentF.Add(new DocS
                        {
                            REF = typD.reference,
                            Objet = typD.Objet,
                            FOURNISSEUR = typD.Fournisseur,
                            ACCUSE = typD.Acusse,
                            VALIDATEUR = typD.Validateur,
                            Montant = typD.Montant.ToString(),
                            Encours = typD.Encours.ToString(),
                            ARCHIVES = typD.ARCHIVES.ToString(),
                            Lien = typD.Lien.ToString(),
                            Validations = uservalidateur != null ? uservalidateur : "",
                        });
                    }
                }
                documentF.Add(new DocS
                {
                    REF = "0001/24/02",
                    Objet = "20240813042914-Géo.pdf",
                    FOURNISSEUR = "SOFTWELL",
                    ACCUSE = DateTime.Now,
                    VALIDATEUR = "SOFT1",
                    Montant = "50000",
                    Encours = "VALIDATION DU DOCUMENT",
                    ARCHIVES = "PAS ENCORE",
                    Lien = "",
                    Validations =""
                });
                documentF.Add(new DocS
                {
                    REF = "0001/24/02",
                    Objet = "20240813042914-Géo.pdf",
                    FOURNISSEUR = "SOFTWELL",
                    ACCUSE = DateTime.Now,
                    VALIDATEUR = "SOFT1",
                    Montant = "50000",
                    Encours = "VALIDATION CPT",
                    ARCHIVES = "PAS ENCORE",
                    Lien = "",
                    Validations =""
                });
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = documentF }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

        }

        [HttpPost]
        public JsonResult GenereREFERENCE(SI_USERS suser, int PROJECTID, string listSite)
        {
            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez parametrer le mappage GED ET PROJET. " }, settings));

            List<string> Projet = new List<string>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }
            try
            {
                if (!db.SI_PROGED.Any(a => a.IDPROJET == PROJECTID))
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance projet SET-GED. " }, settings));

                if (db.SI_USERS.FirstOrDefault(a => a.IDPROJET == PROJECTID && a.DELETIONDATE == null && a.ID == exist.ID).IDUSERGED == null)
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                else
                {
                    var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == PROJECTID && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED;
                    if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));
                }

                var reference = ged.SuppliersDocumentsAcknowledgements.Select(x => new
                {
                    ID = x.Id,
                    Reference = x.ReferenceInterne,
                }).ToList();
                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = reference }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }


        }

        public class DocS
        {
            public string REF { get; set; }
            public string Objet { get; set; }
            public string FOURNISSEUR { get; set; }
            public DateTime ACCUSE { get; set; }
            public string VALIDATEUR { get; set; }
            public string Montant { get; set; }
            public string Encours { get; set; }
            public string ARCHIVES { get; set; }
            public string Lien { get; set; }
            public string Validations { get; set; }
        }

        //TB2: Situation des étapes par type de document (état d'avancement)//
        public ActionResult EtapTypeDocs()
        {
            ViewBag.Controller = "Situation des étapes par type de document";

            return View();
        }

        [HttpPost]
        public JsonResult GenereLISTERFR(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, string listSite, string TypeDoc)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            List<TDB> list = new List<TDB>();

            List<Guid> Projet = new List<Guid>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }
            foreach (var item in listProjet.Split(','))
            {
                int idd = int.Parse(item);
                Projet.Add(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null).IDGED.Value);
            }

            DateTime DD = new DateTime(DateDebut.Year, DateDebut.Month, DateDebut.Day, 0, 0, 0);
            DateTime DF = new DateTime(DateFin.Year, DateFin.Month, DateFin.Day, 23, 59, 59);

            try
            {
                //Tous//
                if (TypeDoc == "")
                {
                    foreach (var x in Projet)
                    {
                        Guid idProjet = x;

                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            //Document type// na tonga dia document no ijerena ny type misy azy
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id))
                            {
                                Guid? IdDocTypes = ged.DocumentTypeUnion.FirstOrDefault(a => a.DocumentID == y.Id).TypeDocID;
                                var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.DeletionDate == null);

                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) && y.Status == 1) //Status == 1 => Création circuit : OK
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/
                                                        && a.ProjectId == idProjet).Name;//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                    var montant = "0";

                                    //Etape actuel = validation ok actuelle//
                                    var validationHisto = "";
                                    var validationHistoNEXT = "";
                                    var validationHistoNEXTvalidateur = "";
                                    var validationHistoNEXTduree = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && a.ActionType == 0))
                                    {
                                        var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                        var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId && a.DeletionDate == null);

                                        var stepNumber = documentStep.StepNumber;

                                        //Get steps information//
                                        var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber);

                                        validationHisto = "Etape " + stepNumber + " : " + isStepType.ProcessingDescription;

                                        if (ged.DocumentTypesSteps.Any(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber + 1 && a.DeletionDate == null))
                                        {
                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber + 1 && a.DeletionDate == null);

                                            validationHistoNEXT = "Etape " + (stepNumber + 1) + " : " + isStepTypeNEXT.ProcessingDescription;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            validationHistoNEXTvalidateur = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId).Username;

                                            if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                            {
                                                validationHistoNEXTvalidateur = "";
                                                foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
                                                {
                                                    validationHistoNEXTvalidateur += ged.Users.FirstOrDefault(a => a.Id == vhe.UserId).Username + ",";
                                                }
                                                validationHistoNEXTvalidateur.TrimEnd(',');
                                            }
                                        }
                                        else
                                        {
                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                            validationHistoNEXT = validationHisto;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            validationHistoNEXTvalidateur = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId).Username;

                                            if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                            {
                                                validationHistoNEXTvalidateur = "";
                                                foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
                                                {
                                                    validationHistoNEXTvalidateur += ged.Users.FirstOrDefault(a => a.Id == vhe.UserId).Username + ",";
                                                }
                                                validationHistoNEXTvalidateur.TrimEnd(',');
                                            }
                                        }
                                    }

                                    list.Add(new TDB
                                    {
                                        REFERENCE = reference,
                                        DOCUMENT = document,
                                        FOURNISSEUR = fournisseur,
                                        MONTANT = montant,
                                        TYPE = typedoc.Title,
                                        STEPNOW = validationHisto,
                                        STEPNEXT = validationHistoNEXT,
                                        VALIDATEURNEXT = validationHistoNEXTvalidateur,
                                        DUREENEXT = validationHistoNEXTduree
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var x in Projet)
                    {
                        Guid idProjet = x;

                        //Document type//
                        Guid IdDocTypes = Guid.Parse(TypeDoc);
                        var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet && a.DeletionDate == null);

                        //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id && a.TypeDocID == typedoc.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) && y.Status == 1) //Status == 1 => Création circuit : OK pour un document
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/
                                                        && a.ProjectId == idProjet).Name;//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                    var montant = "0";

                                    //Etape actuel = validation ok actuelle//
                                    var validationHisto = "";
                                    var validationHistoNEXT = "";
                                    var validationHistoNEXTvalidateur = "";
                                    var validationHistoNEXTduree = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && a.ActionType == 0))
                                    {
                                        var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                        var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId && a.DeletionDate == null);

                                        var stepNumber = documentStep.StepNumber;

                                        //Get steps information//
                                        var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber);

                                        validationHisto = "Etape " + stepNumber + " : " + isStepType.ProcessingDescription;

                                        if (ged.DocumentTypesSteps.Any(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber + 1 && a.DeletionDate == null))
                                        {
                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber + 1 && a.DeletionDate == null);

                                            validationHistoNEXT = "Etape " + (stepNumber + 1) + " : " + isStepTypeNEXT.ProcessingDescription;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            validationHistoNEXTvalidateur = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId).Username;
                                        }
                                        else
                                        {
                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                            validationHistoNEXT = validationHisto;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            validationHistoNEXTvalidateur = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId).Username;

                                            if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                            {
                                                validationHistoNEXTvalidateur = "";
                                                foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
                                                {
                                                    validationHistoNEXTvalidateur += ged.Users.FirstOrDefault(a => a.Id == vhe.UserId).Username + ",";
                                                }
                                                validationHistoNEXTvalidateur.TrimEnd(',');
                                            }
                                        }
                                    }

                                    list.Add(new TDB
                                    {
                                        REFERENCE = reference,
                                        DOCUMENT = document,
                                        FOURNISSEUR = fournisseur,
                                        MONTANT = montant,
                                        TYPE = typedoc.Title,
                                        STEPNOW = validationHisto,
                                        STEPNEXT = validationHistoNEXT,
                                        VALIDATEURNEXT = validationHistoNEXTvalidateur,
                                        DUREENEXT = validationHistoNEXTduree
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

            //list.Add(new TDB
            //{
            //    REFERENCE = "0001/24/01",
            //    DOCUMENT = "Document01",
            //    FOURNISSEUR = "TELMA",
            //    MONTANT = "200000",
            //    TYPE = "Facture",
            //    STEPNOW = "USER1",
            //    STEPNEXT = "Validation 2",
            //    VALIDATEURNEXT = "USER 2",
            //    DUREENEXT = "24"
            //});
            //list.Add(new TDB
            //{
            //    REFERENCE = "0001/24/02",
            //    DOCUMENT = "Document02",
            //    FOURNISSEUR = "TELMA",
            //    MONTANT = "500000",
            //    TYPE = "BC",
            //    STEPNOW = "USER1",
            //    STEPNEXT = "Validation 2",
            //    VALIDATEURNEXT = "USER 2",
            //    DUREENEXT = "24"
            //});

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
        }

        public ActionResult SituationDetailleDoc()
        {
            ViewBag.Controller = "Situation détaillée par document";
            return View();
        }
        public ActionResult EtapActuelDocs()
        {
            ViewBag.Controller = "Situation actuelle des documents";

            return View();
        }

        [HttpPost]
        public ActionResult GetNIFSTATCIN(SI_USERS suser, int PROJECTID)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var Infos = ged.Suppliers.Where(x => x.DeletionDate == null).Select(x => new {
                ID = x.Id,
                NIF = x.NIF,
                STAT = x.STAT,
                CIN = x.CIN,
            }).ToList();
            return Json(JsonConvert.SerializeObject(new { type = "success", data = Infos }));
        }

        [HttpPost]
        public ActionResult GetSituationsDoc(SI_USERS suser, string listesite, string NIF, string STAT, string CIN, int PROJECTID, string REFERENCE)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();
            List<Documents> Infos = new List<Documents>();
            Guid idDoc = Guid.Parse("028AC262-9CAD-47DC-B88D-D8847BDD939E");
            Guid IDref = Guid.Parse(REFERENCE);
            //var DocTypes = ged.DocumentTypesSteps.Where(x =>  x.DeletionDate == null).ToList();
            Infos = ged.Documents.ToList();
            var documentypes = ged.DocumentTypes.ToList();
            List<DocumentInfos> Infosdoc = new List<DocumentInfos>();

            var referenS = ged.SuppliersDocumentsAcknowledgements.Where(x => x.Id == IDref).FirstOrDefault();
            var informationsDoc = ged.Documents.Join(ged.DocumentsSenders, doc => doc.SenderId, docsend => docsend.Id, (doc, docsend) => new
            {
                IDDOCUMENT = doc.Id,
                SenderId = doc.SenderId,
                CreationDate = doc.CreationDate,
                FileName = doc.Filename,
                Type = docsend.Type,
            }).Join(ged.DocumentSteps, doc => doc.IDDOCUMENT, docstep => docstep.DocumentId, (doc, docstep) => new
            {
                IDDOCUMENT = doc.IDDOCUMENT,
                SenderId = doc.SenderId,
                CreationDate = doc.CreationDate,
                FileName = doc.FileName,
                StepNumber = docstep.StepNumber,
                ProcessingDescription = docstep.ProcessingDescription,
                IDDOCSTEP = docstep.Id,
            }).Join(ged.UsersSteps, res => res.IDDOCSTEP, usrstep => usrstep.DocumentStepId, (res, usrstep) => new
            {
                IDDOCUMENT = res.IDDOCUMENT,
                SenderId = res.SenderId,
                CreationDate = res.CreationDate,
                FileName = res.FileName,
                StepNumber = res.StepNumber,
                ProcessingDescription = res.ProcessingDescription,
                IDDOCSTEP = res.IDDOCSTEP,
                UserID = usrstep.UserId,
            }).Join(ged.ValidationsHistory, res => res.IDDOCUMENT, valHisto => valHisto.DocumentId, (res, valhisto) => new
            {
                IDDOCUMENT = res.IDDOCUMENT,
                SenderId = res.SenderId,
                CreationDate = res.CreationDate,
                FileName = res.FileName,
                StepNumber = res.StepNumber,
                ProcessingDescription = res.ProcessingDescription,
                IDDOCSTEP = res.IDDOCSTEP,
                UserID = res.UserID,
                FromUserID = valhisto.FromUserId,
                Comment = valhisto.Comment,
                DATEValidations = valhisto.CreationDate,
            }).Join(ged.Users, res => res.FromUserID, usr => usr.Id, (res, usr) => new
            {
                IDDOCUMENT = res.IDDOCUMENT,
                SenderId = res.SenderId,
                CreationDate = res.CreationDate,
                FileName = res.FileName,
                StepNumber = res.StepNumber,
                ProcessingDescription = res.ProcessingDescription,
                IDDOCSTEP = res.IDDOCSTEP,
                UserID = res.UserID,
                FromUserID = res.FromUserID,
                Comment = res.Comment,
                DATEValidations = res.CreationDate,
                UserName = usr.Username,
            }).ToList();
            var Fictif = new List<DocumentInfosFicti>();

            Fictif.Add(new DocumentInfosFicti
            {
                IDDOCUMENT = "E782192C-4D49-4C60-B0E8-B1D800AB3524",
                Validateur = "SOFT1",
                CreationDate = "2024-08-26 10:26:03.833",
                DocuMent = "FACT01",
                StepNumber = "0",
                ProcessingDescription = "VISA DU DOCUMENT",
                IDDOCSTEP = "",
                UserID = "",
                FromUserID = "",
                Comment = "OK",
                DATEValidations = "2024-08-26 10:26:03.833",
                UserName = "SOFT1",
            });
            Fictif.Add(new DocumentInfosFicti
            {
                IDDOCUMENT = "E782192C-4D49-4C60-B0E8-B1D800AB3524",
                Validateur = "SOFT1",
                CreationDate = "2024-08-26 10:26:03.833",
                DocuMent = "FACT02",
                StepNumber = "0",
                ProcessingDescription = "VALIDATION DU DOCUMENT",
                IDDOCSTEP = "",
                UserID = "",
                FromUserID = "",
                Comment = "RAS",
                DATEValidations = "2024-08-26 10:26:03.833",
                UserName = "SOFT1",
            });
            return Json(JsonConvert.SerializeObject(new { type = "success", data = Fictif }));
        }
        public class DocumentInfos
        {
            public int StepNumber { get; set; }
            public string Validateur { get; set; }
            public string DocuMent { get; set; }
            public DateTime Datevalidations { get; set; }
            public string Comm { get; set; }
            public Guid DocumentID { get; set; }
            public Guid StepID { get; set; }
            public Guid UserID { get; set; }
        }
        public class DocumentInfosFicti
        {
            public string IDDOCUMENT { get; set; }
            public string Validateur { get; set; }
            public string DocuMent { get; set; }
            public string CreationDate { get; set; }
            public string Comment { get; set; }
            public string StepNumber { get; set; }
            public string ProcessingDescription { get; set; }
            public string IDDOCSTEP { get; set; }
            public string FromUserID { get; set; }
            public string DATEValidations { get; set; }
            public string DocumentID { get; set; }
            public string StepID { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
        }

        //TB2 BIS: Situation des étapes par type de document (état d'avancement)//
        public ActionResult EtapTypeDocsBIS()
        {
            ViewBag.Controller = "Etat d'avancement par type de document";

            return View();
        }

        [HttpPost]
        public JsonResult GenereLISTEBIS(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, string listSite, string TypeDoc)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            List<TDB> list = new List<TDB>();

            List<Guid> Projet = new List<Guid>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }
            foreach (var item in listProjet.Split(','))
            {
                int idd = int.Parse(item);
                Projet.Add(db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null).IDGED.Value);
            }

            DateTime DD = new DateTime(DateDebut.Year, DateDebut.Month, DateDebut.Day, 0, 0, 0);
            DateTime DF = new DateTime(DateFin.Year, DateFin.Month, DateFin.Day, 23, 59, 59);

            int nombreEtape = 0;
            List<string> listEtape = new List<string>();
            List<string> dateStep = new List<string>();

            try
            {
                foreach (var x in Projet)
                {
                    Guid idProjet = x;

                    //Document type//
                    Guid IdDocTypes = Guid.Parse(TypeDoc);
                    var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet && a.DeletionDate == null);

                    nombreEtape = ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).Count();
                    if (nombreEtape != 0)
                    {
                        foreach (var elem in ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).OrderBy(a => a.StepNumber).ToList())
                        {
                            listEtape.Add("Etape " + elem.StepNumber + " : " + elem.ProcessingDescription);
                        }

                        //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id && a.TypeDocID == typedoc.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) && y.Status == 1) //Status == 1 => Création circuit : OK pour un document
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/
                                                        && a.ProjectId == idProjet).Name;//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                    var montant = "0";

                                    var ARCHIVEDATE = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && a.ActionType == 3))
                                        ARCHIVEDATE = ged.ValidationsHistory.FirstOrDefault(a => a.DocumentId == y.Id && a.ActionType == 3).CreationDate.ToShortDateString();

                                    //Rattachement TOMATE
                                    var RATTACHTOM = "";

                                    //Liste des etapes avec date de validation//
                                    foreach (var elem in ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).OrderBy(a => a.StepNumber).ToList())
                                    {
                                        //var DocTypeUserSteps = ged.DocumentTypesUsersSteps.FirstOrDefault(a => a.StepId == elem.Id);
                                        var DocSteps = ged.DocumentSteps.FirstOrDefault(a => a.StepNumber == elem.StepNumber && a.DocumentId == y.Id && a.DeletionDate == null);

                                        if (DocSteps != null)
                                        {
                                            var UsersStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == DocSteps.Id && a.DeletionDate == null);//A verifier le ACTIONTYPE dans ValidationHistory si besoin (0 : validation ou 3 : archivage)

                                            var dateValidation = "";
                                            if (UsersStep.ProcessingDate != null)
                                            {
                                                dateValidation = UsersStep.ProcessingDate.Value.ToShortDateString();
                                            }

                                            dateStep.Add(dateValidation);
                                        }
                                    }

                                    list.Add(new TDB
                                    {
                                        REFERENCE = reference,
                                        DOCUMENT = document,
                                        FOURNISSEUR = fournisseur,
                                        MONTANT = montant,
                                        TYPE = typedoc.Title,
                                        DATESTEP = dateStep,
                                        ARCHIVEDATE = ARCHIVEDATE,//archive
                                        RATTACHTOM = RATTACHTOM//rattachement TOMATE
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

            //listEtape.Add("Validation X");
            //listEtape.Add("Validation Y");

            //nombreEtape = 2;

            //dateStep.Add("14/08/2024");
            //dateStep.Add("15/08/2024");

            //list.Add(new TDB
            //{
            //    REFERENCE = "0001/24/01",
            //    DOCUMENT = "Document01",
            //    FOURNISSEUR = "TELMA",
            //    MONTANT = "200000",
            //    TYPE = "Facture",
            //    DATESTEP = dateStep,
            //    ARCHIVEDATE = "20/08/2024",//archive
            //    RATTACHTOM = "RATTACHTOM"//rattachement TOMATE
            //});
            //list.Add(new TDB
            //{
            //    REFERENCE = "0001/24/02",
            //    DOCUMENT = "Document02",
            //    FOURNISSEUR = "TELMA",
            //    MONTANT = "500000",
            //    TYPE = "BC",
            //    DATESTEP = dateStep,
            //    ARCHIVEDATE = "20/08/2024",//archive
            //    RATTACHTOM = "RATTACHTOM"//rattachement TOMATE
            //});

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = new { list = list, listEtape = listEtape, nombreEtape = nombreEtape } }, settings));
        }
    }
}
