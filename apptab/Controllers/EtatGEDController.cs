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
using apptab.Models;
using System.Diagnostics.SymbolStore;

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

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(int.Parse(iProjet));
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
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
        public ActionResult GETALLFOURNISSEUR(SI_USERS suser, string iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(int.Parse(iProjet));
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                List<int> idProjet = new List<int>();

                foreach (var item in iProjet.Split(','))
                {
                    idProjet.Add(int.Parse(item));
                }

                List<Fournisseur> supl = new List<Fournisseur>();

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
                    }
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

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(int.Parse(iProjet));
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
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
                                if (ged.DocumentTypesSteps.Any(a => a.DocumentTypeId == typD.Id && a.DeletionDate == null))
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
            public string COMM { get; set; }

            public List<string> DATESTEP { get; set; }
        }

        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, int PROJECTID, DateTime DateDebut, DateTime DateFin, string listSite, int status, string fournisseur /*string reference*/)
        {
            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(PROJECTID);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez parametrer le mappage GED ET PROJET. " }, settings));

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
                    var IDUSERGED = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == PROJECTID && b.DELETIONDATE == null && b.ID == exist.ID).IDUSERGED; ;
                    if (!ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings)); ;
                }
               
                var isUserSet = db.SI_USERS.FirstOrDefault(b => b.IDPROJET == PROJECTID && b.DELETIONDATE == null && b.ID == exist.ID); ;
                var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null); ;
                List<DocS> documentF = new List<DocS>(); ;
                List<string> suppliersnameList = new List<string>();
                Guid IDsup = new Guid();
                if (fournisseur == "0")
                {
                    suppliersnameList.AddRange(ged.Suppliers.Select(a => a.Name).ToList()); ;
                }
                else
                {
                    IDsup = Guid.Parse(fournisseur); ;
                    suppliersnameList.Add(ged.Suppliers.Where(a => a.Id == IDsup).Select(a => a.Name).FirstOrDefault()); ;
                }
                var suppliersname = ged.Suppliers.FirstOrDefault(); ;
                var RefDoc = ged.Documents.Where(x => x.DeletionDate == null && x.CreationDate >= DateDebut && x.CreationDate <= DateFin).Join(ged.SuppliersDocumentsAcknowledgements, dcm => dcm.Id, sdal => sdal.Id, (dcm, sdal) => new
                {
                    ID = dcm.SenderId,
                    reference = sdal.ReferenceInterne,
                    Objet = dcm.Object,
                    Fournisseur = "",
                    Acusse = dcm.CreationDate,
                    Validateur = "",
                    Montant = dcm.Montant,
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
                    Montant = dcm.Montant,
                    Encours = dcm.Encours,
                    ARCHIVES = dcm.Date.ToString(),
                    Lien = dcm.Lien,
                    Site = dcm.Site,
                    DocumentID = dcm.DocumentID,
                    Validateur = ""
                }).Join(ged.DocumentSteps , dcm => dcm.DocumentID , docstep => docstep.DocumentId ,(dcm,docstep) => new
                {
                    ID = dcm.ID,
                    reference = dcm.reference,
                    Objet = dcm.Objet,
                    Fournisseur = dcm.Fournisseur,
                    Acusse = dcm.Acusse,
                    Montant = dcm.Montant,
                    Encours = dcm.Encours,
                    Etape = docstep.ProcessingDescription,
                    ARCHIVES = dcm.Encours == 3 ? dcm.ARCHIVES.ToString() : "",
                    Lien = dcm.Lien,
                    Site = dcm.Site,
                    DocumentID = dcm.DocumentID,
                    Validateur = "",
                    DocumentStepID = docstep.Id,
                }).Join(ged.UsersSteps, dcm => dcm.DocumentStepID, userStep => userStep.DocumentStepId, (dcm, userStep) => new
                {
                    ID = dcm.ID,
                    reference = dcm.reference,
                    Objet = dcm.Objet,
                    Fournisseur = dcm.Fournisseur,
                    Acusse = dcm.Acusse,
                    Montant = dcm.Montant,
                    Encours = dcm.Encours,
                    Etape = dcm.Etape,
                    ARCHIVES = dcm.Encours == 3 ? dcm.ARCHIVES.ToString() : "",
                    Lien = dcm.Lien,
                    Site = dcm.Site,
                    DocumentID = dcm.DocumentID,
                    IDvalidateur = userStep.UserId,
                    Validateur = "",
                    DocumentStepID = dcm.DocumentStepID,
                    IsValidator = userStep.IsValidator,
                }).Join(ged.Users, dcm => dcm.IDvalidateur, us => us.Id, (dcm, us) => new
                {
                    ID = dcm.ID,
                    reference = dcm.reference,
                    Objet = dcm.Objet,
                    Fournisseur = dcm.Fournisseur,
                    Acusse = dcm.Acusse,
                    Montant = dcm.Montant,
                    Encours = dcm.Encours,
                    Etape = dcm.Etape,
                    ARCHIVES = dcm.Encours == 3 ? dcm.ARCHIVES.ToString() : "",
                    Lien = dcm.Lien,
                    Site = dcm.Site,
                    DocumentID = dcm.DocumentID,
                    IDvalidateur = dcm.IDvalidateur,
                    Validateur = us.FirstName,
                    DocumentStepID = dcm.DocumentStepID,
                    IsValidator = dcm.IsValidator,
                }).Where(x => x.IsValidator == true ).DistinctBy(x => x.Etape).ToList();// x.Fournisseur == suppliersname.Name && x.Encours == status &&

                if (RefDoc != null)
                {
                    if (status != 4 || fournisseur != "0")
                    {
                        //foreach (var typD in RefDoc.Where(x => x.Encours == status && x.Fournisseur == suppliersname.Name))
                        foreach (var typD in RefDoc.Where(x => x.Encours == status && suppliersnameList.Contains(x.Fournisseur)))
                        {
                            string uservalidateur = "";
                            string SSITE = typD.Site;
                            documentF.Add(new DocS
                            {
                                REF = typD.reference,
                                Objet = typD.Objet,
                                FOURNISSEUR = typD.Fournisseur,
                                ACCUSE = typD.Acusse,
                                VALIDATEUR = typD.Validateur,
                                Montant = typD.Montant.ToString(),
                                Encours = typD.Etape.ToString(),
                                ARCHIVES = typD.ARCHIVES.ToString(),
                                Lien = typD.Lien.ToString(),
                                //Validations = uservalidateur != null ? uservalidateur : "",
                            });
                        }
                    }else if(status != 4 || fournisseur == "0")
                    {
                        foreach (var typD in RefDoc)
                        {
                            string uservalidateur = "";
                            string SSITE = typD.Site;
                            documentF.Add(new DocS
                            {
                                REF = typD.reference,
                                Objet = typD.Objet,
                                FOURNISSEUR = typD.Fournisseur,
                                ACCUSE = typD.Acusse,
                                VALIDATEUR = typD.Validateur,
                                Montant = typD.Montant.ToString(),
                                Encours = typD.Etape.ToString(),
                                ARCHIVES = typD.ARCHIVES.ToString(),
                                Lien = typD.Lien.ToString(),
                                //Validations = uservalidateur != null ? uservalidateur : "",
                            });
                        }
                    }else if (status == 4 || fournisseur != "0")
                    {
                        foreach (var typD in RefDoc.Where(x => suppliersnameList.Contains(x.Fournisseur)))
                        {
                            string uservalidateur = "";
                            string SSITE = typD.Site;
                            documentF.Add(new DocS
                            {
                                REF = typD.reference,
                                Objet = typD.Objet,
                                FOURNISSEUR = typD.Fournisseur,
                                ACCUSE = typD.Acusse,
                                VALIDATEUR = typD.Validateur,
                                Montant = typD.Montant.ToString(),
                                Encours = typD.Etape.ToString(),
                                ARCHIVES = typD.ARCHIVES.ToString(),
                                Lien = typD.Lien.ToString(),
                                //Validations = uservalidateur != null ? uservalidateur : "",
                            });
                        }
                    }
                    else
                    {
                        foreach (var typD in RefDoc)
                        {
                            string uservalidateur = "";
                            string SSITE = typD.Site;
                            documentF.Add(new DocS
                            {
                                REF = typD.reference,
                                Objet = typD.Objet,
                                FOURNISSEUR = typD.Fournisseur,
                                ACCUSE = typD.Acusse,
                                VALIDATEUR = typD.Validateur,
                                Montant = typD.Montant.ToString(),
                                Encours = typD.Etape.ToString(),
                                ARCHIVES = typD.ARCHIVES.ToString(),
                                Lien = typD.Lien.ToString(),
                                //Validations = uservalidateur != null ? uservalidateur : "",
                            });
                        }
                    }
                }

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
            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(PROJECTID);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez parametrer le mappage GED ET PROJET. " }, settings));

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

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);

                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(isPG.IDPROJET.Value);
                if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                SOFTCONNECTGED ged = new SOFTCONNECTGED();
            }

            DateTime DD = new DateTime(DateDebut.Year, DateDebut.Month, DateDebut.Day, 0, 0, 0);
            DateTime DF = new DateTime(DateFin.Year, DateFin.Month, DateFin.Day, 23, 59, 59);

            try
            {
                //Tous//
                if (String.IsNullOrEmpty(TypeDoc))
                {
                    foreach (var x in Projet)
                    {
                        Guid idProjet = x;

                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDGED == idProjet && a.DELETIONDATE == null).IDPROJET.Value);
                        if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            //Document type// na tonga dia document no ijerena ny type misy azy
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id))
                            {
                                Guid? IdDocTypes = ged.DocumentTypeUnion.FirstOrDefault(a => a.DocumentID == y.Id).TypeDocID;
                                var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes /*&& a.DeletionDate == null*/);

                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = "";
                                    if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                        fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet).Name;

                                    var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                    //Etape actuel = validation ok actuelle//
                                    var validationHisto = "";
                                    var validationHistoNEXT = "";
                                    var validationHistoNEXTvalidateur = "";
                                    var validationHistoNEXTduree = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)))
                                    {
                                        var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                        var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId /*&& a.DeletionDate == null*/);

                                        if (documentStep == null)
                                        {
                                            var validationInProgressFin = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                            var documentStepFin = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgressFin.ToDocumentStepId /*&& a.DeletionDate == null*/);
                                            var stepNumberFin = documentStepFin.StepNumber;

                                            validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin /*&& a.DeletionDate == null*/).ProcessingDescription;
                                            validationHistoNEXT = "Terminé";
                                            validationHistoNEXTvalidateur = "Terminé";
                                            validationHistoNEXTduree = "0";
                                        }
                                        else
                                        {
                                            var stepNumber = documentStep.StepNumber;

                                            //Get steps information//
                                            var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber - 1) /*&& a.DeletionDate == null*/);
                                            if (isStepType == null)
                                            {
                                                validationHisto = "Etape " + stepNumber + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/).ProcessingDescription;
                                                validationHistoNEXT = "Terminé";
                                                validationHistoNEXTvalidateur = "Terminé";
                                                validationHistoNEXTduree = "0";
                                            }
                                            else
                                            {
                                                validationHisto = "Etape " + (stepNumber - 1) + " : " + isStepType.ProcessingDescription;

                                                var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/);

                                                validationHistoNEXT = "Etape " + (stepNumber) + " : " + isStepTypeNEXT.ProcessingDescription;

                                                validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                                var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/);
                                                var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/);
                                                var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                                validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                            (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                            (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                            (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                                if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/).Count() > 1)
                                                {
                                                    validationHistoNEXTvalidateur = "";
                                                    foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/).ToList())
                                                    {
                                                        var isUser = ged.Users.FirstOrDefault(a => a.Id == vhe.UserId);
                                                        validationHistoNEXTvalidateur += "<li>" + ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                            (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                            (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                            (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName)) + "</li>";
                                                    }
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
                else
                {
                    foreach (var x in Projet)
                    {
                        Guid idProjet = x;

                        //Document type//
                        Guid IdDocTypes = Guid.Parse(TypeDoc);
                        var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet /*&& a.DeletionDate == null*/);

                        //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id && a.TypeDocID == typedoc.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = "";
                                    if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                        fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet).Name;

                                    var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                    //Etape actuel = validation ok actuelle//
                                    var validationHisto = "";
                                    var validationHistoNEXT = "";
                                    var validationHistoNEXTvalidateur = "";
                                    var validationHistoNEXTduree = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)))
                                    {
                                        var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                        var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId /*&& a.DeletionDate == null*/);

                                        if (documentStep == null)
                                        {
                                            var validationInProgressFin = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                            var documentStepFin = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgressFin.ToDocumentStepId /*&& a.DeletionDate == null*/);
                                            var stepNumberFin = documentStepFin.StepNumber;

                                            validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin /*&& a.DeletionDate == null*/).ProcessingDescription;
                                            validationHistoNEXT = "Terminé";
                                            validationHistoNEXTvalidateur = "Terminé";
                                            validationHistoNEXTduree = "0";
                                        }
                                        else
                                        {
                                            var stepNumber = documentStep.StepNumber;

                                            //Get steps information//
                                            var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber - 1) /*&& a.DeletionDate == null*/);
                                            if (isStepType == null)
                                            {
                                                validationHisto = "Etape " + stepNumber + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/).ProcessingDescription;
                                                validationHistoNEXT = "Terminé";
                                                validationHistoNEXTvalidateur = "Terminé";
                                                validationHistoNEXTduree = "0";
                                            }
                                            else
                                            {
                                                validationHisto = "Etape " + (stepNumber - 1) + " : " + isStepType.ProcessingDescription;

                                                var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/);

                                                validationHistoNEXT = "Etape " + (stepNumber) + " : " + isStepTypeNEXT.ProcessingDescription;

                                                validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                                var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber /*&& a.DeletionDate == null*/);
                                                var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/);
                                                var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                                validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                            (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                            (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                            (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                                if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/).Count() > 1)
                                                {
                                                    validationHistoNEXTvalidateur = "";
                                                    foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id /*&& a.DeletionDate == null*/).ToList())
                                                    {
                                                        var isUser = ged.Users.FirstOrDefault(a => a.Id == vhe.UserId);
                                                        validationHistoNEXTvalidateur += "<li>" + ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                            (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                            (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                            (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName)) + "</li>";
                                                    }
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
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }

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

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(PROJECTID);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
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

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(PROJECTID);
            if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
            SOFTCONNECTGED ged = new SOFTCONNECTGED();


            List<Documents> Infos = new List<Documents>();

            Guid IDref = Guid.Parse(REFERENCE);

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
            }).Join(ged.SuppliersDocumentsAcknowledgements , doc => doc.IDDOCUMENT, ackn => ackn.Id ,(doc,ackn) => new
            {
                IDDOCUMENT = doc.IDDOCUMENT,
                SenderId = doc.SenderId,
                CreationDate = doc.CreationDate,
                FileName = doc.FileName,
                Type = doc.Type,
                referenceinterne = ackn.ReferenceInterne,
            }).Join(ged.DocumentSteps, doc => doc.IDDOCUMENT, docstep => docstep.DocumentId, (doc, docstep) => new
            {
                IDDOCUMENT = doc.IDDOCUMENT,
                SenderId = doc.SenderId,
                CreationDate = doc.CreationDate,
                FileName = doc.FileName,
                StepNumber = docstep.StepNumber,
                ProcessingDescription = docstep.ProcessingDescription,
                IDDOCSTEP = docstep.Id,
                referenceinterne =doc.referenceinterne,
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
                referenceinterne = res.referenceinterne,
                Isvalidator = usrstep.IsValidator,
                commentaire = usrstep.Comment
            }).Where(usrstep => usrstep.Isvalidator == true).Join(ged.ValidationsHistory, res => res.IDDOCUMENT, valHisto => valHisto.DocumentId, (res, valhisto) => new
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
                //Comment = valhisto.Comment,
                Comment = res.commentaire,
                DATEValidations = valhisto.CreationDate,
                referenceinterne = res.referenceinterne,
                Isvalidator = res.Isvalidator
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
                referenceinterne = res.referenceinterne,
                Isvalidator = res.Isvalidator
            }).Join(ged.Suppliers , res => res.SenderId , supl => supl.Id, (res,supl) => new
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
                UserName = res.UserName,
                referenceinterne = res.referenceinterne,
                Fournisseur = supl.Name,
                Isvalidator = res.Isvalidator
            }).Where(x => x.referenceinterne == referenS.ReferenceInterne && x.Isvalidator == true).DistinctBy(x => new
            {
                ProcessingDescription = x.ProcessingDescription,
                Comment = x.Comment
            }).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", data = informationsDoc }));
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

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);

                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(isPG.IDPROJET.Value);
                if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                SOFTCONNECTGED ged = new SOFTCONNECTGED();
            }

            DateTime DD = new DateTime(DateDebut.Year, DateDebut.Month, DateDebut.Day, 0, 0, 0);
            DateTime DF = new DateTime(DateFin.Year, DateFin.Month, DateFin.Day, 23, 59, 59);

            int nombreEtape = 0;
            List<string> listEtape = new List<string>();

            try
            {
                foreach (var x in Projet)
                {
                    Guid idProjet = x;

                    SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDGED == idProjet && a.DELETIONDATE == null).IDPROJET.Value);
                    if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                    SOFTCONNECTGED ged = new SOFTCONNECTGED();

                    nombreEtape = 0;

                    //Document type//
                    Guid IdDocTypes = Guid.Parse(TypeDoc);
                    var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet /*&& a.DeletionDate == null*/);

                    nombreEtape = ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).Count();
                    if (nombreEtape != 0)
                    {
                        listEtape = new List<string>();

                        foreach (var elem in ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id /*&& a.DeletionDate == null*/).OrderBy(a => a.StepNumber).ToList())
                        {
                            listEtape.Add("Etape " + elem.StepNumber + " : " + elem.ProcessingDescription);
                        }

                        //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id && a.TypeDocID == typedoc.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK pour un document
                                {
                                    List<string> dateStep = new List<string>();

                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                    var document = y.Object;

                                    var fournisseur = "";
                                    if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                        fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet).Name;

                                    var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                    var ARCHIVEDATE = "";
                                    if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && a.ActionType == 3))
                                        ARCHIVEDATE = ged.ValidationsHistory.FirstOrDefault(a => a.DocumentId == y.Id && a.ActionType == 3).CreationDate.ToShortDateString();

                                    //Rattachement TOMATE
                                    var RATTACHTOM = "";

                                    //Liste des etapes avec date de validation//
                                    foreach (var elem in ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id /*&& a.DeletionDate == null*/).OrderBy(a => a.StepNumber).ToList())
                                    {
                                        //var DocTypeUserSteps = ged.DocumentTypesUsersSteps.FirstOrDefault(a => a.StepId == elem.Id);
                                        var DocSteps = ged.DocumentSteps.FirstOrDefault(a => a.StepNumber == elem.StepNumber && a.DocumentId == y.Id /*&& a.DeletionDate == null*/);

                                        if (DocSteps != null)
                                        {
                                            var UsersStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == DocSteps.Id && a.ProcessingDate != null && a.IsValidator == true/*&& a.DeletionDate == null*/);//A verifier le ACTIONTYPE dans ValidationHistory si besoin (0 : validation ou 3 : archivage)

                                            var dateValidation = " ";
                                            if (UsersStep != null)
                                            {
                                                var isuu = ged.Users.FirstOrDefault(a => a.Id == UsersStep.UserId /*&& a.DeletionDate == null*/);
                                                dateValidation = UsersStep.ProcessingDate.Value.ToShortDateString() + "<br/>" +
                                                    ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                    (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                    (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                    (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName)) + "<br/>" +
                                                    UsersStep.Comment;
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

            //list = new List<TDB>();
            //listEtape = new List<string>();
            //List<string> dateStep2 = new List<string>();
            //nombreEtape = 2;

            //listEtape.Add("Validation X");
            //listEtape.Add("Validation Y");

            //dateStep2.Add("14/08/2024");
            //dateStep2.Add("15/08/2024");

            //list.Add(new TDB
            //{
            //    REFERENCE = "0001/24/01",
            //    DOCUMENT = "Document01",
            //    FOURNISSEUR = "TELMA",
            //    MONTANT = "200000",
            //    TYPE = "Facture",
            //    DATESTEP = dateStep2,
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
            //    DATESTEP = dateStep2,
            //    ARCHIVEDATE = "20/08/2024",//archive
            //    RATTACHTOM = "RATTACHTOM"//rattachement TOMATE
            //});

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = new { list = list, listEtape = listEtape, nombreEtape = nombreEtape } }, settings));
        }

        //TB5: Statistique des factures fournisseur rejetées//
        public ActionResult EtapFactRejete()
        {
            ViewBag.Controller = "Statistique des factures fournisseur rejetées";

            return View();
        }

        [HttpPost]
        public JsonResult GenereLISTEREFUS(SI_USERS suser, string listProjet, DateTime DateDebut, DateTime DateFin, string listSite, string ListFournisseur)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            if (exist.IDUSERGED == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer la correspondance utilisateur SET-GED. " }, settings));

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

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);

                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(isPG.IDPROJET.Value);
                if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                SOFTCONNECTGED ged = new SOFTCONNECTGED();
            }

            DateTime DD = new DateTime(DateDebut.Year, DateDebut.Month, DateDebut.Day, 0, 0, 0);
            DateTime DF = new DateTime(DateFin.Year, DateFin.Month, DateFin.Day, 23, 59, 59);

            try
            {
                //Tous//
                if (String.IsNullOrEmpty(ListFournisseur))
                {
                    foreach (var x in Projet)
                    {
                        Guid idProjet = x;

                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDGED == idProjet && a.DELETIONDATE == null).IDPROJET.Value);
                        if (SOFTCONNECTGED.connex == "") return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le mappage SET-GED. " }, settings));
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.DocumentsSenders.Type == 1 && a.DeletionDate == null && a.Status == 2
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            //Document type// na tonga dia document no ijerena ny type misy azy
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = "";
                                    if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                        fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet).Name;

                                    var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                    //Etape d'annulation//
                                    var DescriptAnn = "";
                                    var DateAnn = "";
                                    //var EtapeNumberAnn = "";
                                    var UserAnn = "";
                                    var comm = "";
                                    if (ged.DocumentSteps.Any(a => a.DocumentId == y.Id /*&& a.DeletionDate == null*/))
                                    {
                                        foreach (var s in ged.DocumentSteps.Where(a => a.DocumentId == y.Id /*&& a.DeletionDate == null*/).ToList())
                                        {
                                            if (ged.UsersSteps.Any(a => a.DocumentStepId == s.Id && a.DeletionDate != null && a.IsValidator == true))// != NULL suite requête de GED : GetTotalNumberOfCanceledDocuments
                                            {
                                                var isAnnul = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == s.Id /*&& a.DeletionDate != null */ && a.IsValidator == true);

                                                DescriptAnn = "Etape " + s.StepNumber.ToString() + " : " + s.ProcessingDescription;

                                                DateAnn = isAnnul.ProcessingDate.Value.ToString();

                                                var isuu = ged.Users.FirstOrDefault(a => a.Id == isAnnul.UserId);
                                                UserAnn = ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                    (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                    (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                    (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName));

                                                comm = isAnnul.Comment;
                                            }
                                        }
                                    }

                                    list.Add(new TDB
                                    {
                                        REFERENCE = reference,
                                        DOCUMENT = document,
                                        FOURNISSEUR = fournisseur,
                                        MONTANT = montant,
                                        TYPE = DescriptAnn,
                                        STEPNOW = DateAnn,
                                        STEPNEXT = UserAnn,
                                        COMM = comm
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

                        //Fournisseur//
                        Guid IdDocTypes = Guid.Parse(ListFournisseur);
                        var Idfournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet /*&& a.DeletionDate == null*/);

                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        && a.SenderId == Idfournisseur.Id && a.DocumentsSenders.Type == 1 && a.DeletionDate == null && a.Status == 2
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            //Document type// na tonga dia document no ijerena ny type misy azy
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id))
                            {
                                //Accusé de récéption//
                                if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) && y.Status == 1 /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                {
                                    var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                    var document = y.Object;
                                    var fournisseur = "";
                                    if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                        fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id /*&& a.DeletionDate == null*/ && a.ProjectId == idProjet).Name;

                                    var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                    //Etape d'annulation//
                                    var DescriptAnn = "";
                                    var DateAnn = "";
                                    //var EtapeNumberAnn = "";
                                    var UserAnn = "";
                                    var comm = "";
                                    if (ged.DocumentSteps.Any(a => a.DocumentId == y.Id /*&& a.DeletionDate == null*/))
                                    {
                                        foreach (var s in ged.DocumentSteps.Where(a => a.DocumentId == y.Id /*&& a.DeletionDate == null*/).ToList())
                                        {
                                            if (ged.UsersSteps.Any(a => a.DocumentStepId == s.Id && a.DeletionDate != null && a.IsValidator == true))// != NULL suite requête de GED : GetTotalNumberOfCanceledDocuments
                                            {
                                                var isAnnul = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == s.Id /*&& a.DeletionDate != null */ && a.IsValidator == true);

                                                DescriptAnn = "Etape " + s.StepNumber.ToString() + " : " + s.ProcessingDescription;

                                                DateAnn = isAnnul.ProcessingDate.Value.ToString();

                                                var isuu = ged.Users.FirstOrDefault(a => a.Id == isAnnul.UserId);
                                                UserAnn = ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                    (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                    (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                    (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName));

                                                comm = isAnnul.Comment;
                                            }
                                        }
                                    }

                                    list.Add(new TDB
                                    {
                                        REFERENCE = reference,
                                        DOCUMENT = document,
                                        FOURNISSEUR = fournisseur,
                                        MONTANT = montant,
                                        TYPE = DescriptAnn,
                                        STEPNOW = DateAnn,
                                        STEPNEXT = UserAnn,
                                        COMM = comm
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

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
        }
    }
}
