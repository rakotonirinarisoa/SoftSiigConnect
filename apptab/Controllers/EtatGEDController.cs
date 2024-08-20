using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using apptab.Data.Entities;
using static apptab.Controllers.UserController;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using static apptab.Controllers.EtatGEDController;

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

        public static Guid[] DeserializeJsonToGuidArray(string jsonString)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Guid[]>(jsonString);
        }

        [HttpPost]
        public ActionResult GETALLSITE(SI_USERS suser, int iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                int crpt = iProjet;

                List<SiteGED> crpto = new List<SiteGED>();

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

                foreach (Guid guid in guidArray)
                {
                    crpto.Add(new SiteGED()
                    {
                        Id = guid,
                        Code = ged.Sites.Any(a => a.Id == guid && a.DeletionDate == null) ? ged.Sites.FirstOrDefault(a => a.Id == guid && a.DeletionDate == null).SiteId : ""
                    });
                }

                //string result = isUserGed.Sites.Replace("[", "").Replace("]", "").Replace("\"", "");

                //foreach (var x in result.Split(',').ToList())
                //{
                //    var aaa = x;
                //}

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { etat = crpto } }, settings));
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
        public ActionResult GETALLTYPEDOCS(SI_USERS suser, int iProjet, string iSite)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                int crpt = iProjet;
                List<string> listSite = iSite.Split(',').ToList();

                List<TypeDoc> crpto = new List<TypeDoc>();

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

        //TB2: Situation des étapes par type de document (état d'avancement)//
        public ActionResult EtapTypeDocs()
        {
            ViewBag.Controller = "Situation des étapes par type de document";

            return View();
        }

        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, string listSite, string TypeDoc)
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

                        List<Guid> TypeDocument = new List<Guid>();
                        foreach (var item in TypeDoc.Split(','))
                        {
                            Guid idd = Guid.Parse(item);
                            TypeDocument.Add(ged.DocumentTypes.FirstOrDefault(a => a.Id == idd && a.ProjectId == idProjet && a.DeletionDate == null).Id);
                        }

                        foreach (var xx in TypeDocument)
                        {
                            //Document type//
                            Guid IdDocTypes = xx;
                            var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.DeletionDate == null);

                            //Mbola misy code eto ijerena ny type de docs en FOREACH izay vao foreach Documents + TYPEDOCS//

                            foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                            && a.DocumentsSenders.Type == 1 && a.DeletionDate == null
                            && a.DocumentsSenders.Users.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                            {
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
                        && a.Users.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
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
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
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
                        && a.Users.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
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
                                    var UsersStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == DocSteps.Id && a.DeletionDate == null);//A verifier le ACTIONTYPE dans ValidationHistory si besoin (0 : validation ou 3 : archivage)

                                    var dateValidation = "";
                                    if (UsersStep.ProcessingDate != null)
                                    {
                                        dateValidation = UsersStep.ProcessingDate.Value.ToShortDateString();
                                    }

                                    dateStep.Add(dateValidation);
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
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

            //dateStep.Add("14/08/2024");
            //dateStep.Add("15/08/2024");
            //list.Add(new TDB
            //{
            //    REFERENCE = "reference",
            //    DOCUMENT = "document",
            //    FOURNISSEUR = "fournisseur",
            //    MONTANT = "2000000",
            //    TYPE = "typedoc.Title",
            //    DATESTEP = dateStep,
            //    ARCHIVEDATE = "ARCHIVEDATE",//archive
            //    RATTACHTOM = "RATTACHTOM"//rattachement TOMATE
            //});

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = new { list = list, listEtape = listEtape, nombreEtape = nombreEtape } }, settings));
        }
    }
}
