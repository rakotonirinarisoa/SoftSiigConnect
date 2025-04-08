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
using System.Windows.Forms;
using System.Web.Helpers;
using Microsoft.CodeAnalysis;
using OpenXmlPowerTools;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

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

            try
            {
                List<int> idProjet = new List<int>();

                int i = 0;
                foreach (var item in iProjet.Split(','))
                {
                    if (i < 2)
                    {
                        idProjet.Add(int.Parse(item));
                    }
                    i++;
                }

                List<SiteGED> crpto = new List<SiteGED>();

                bool mappingErreur = false;
                bool projetErreur = false;
                bool utilisateurErreur = false;

                if (idProjet != null)
                {
                    foreach (var crpt in idProjet)
                    {
                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                        if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                        {
                            if (mappingErreur == false)
                                mappingErreur = true;
                        }

                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                        {
                            if (projetErreur == false)
                                projetErreur = true;
                        }

                        var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                        var IDUSERGED = userT?.IDUSERGED;

                        if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        {
                            if (utilisateurErreur == false)
                                utilisateurErreur = true;
                        }
                    }

                    if (!mappingErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer le mappage SET-GED."
                        }, settings));
                    }

                    if (!projetErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance projet SET-GED."
                        }, settings));
                    }

                    if (!utilisateurErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                        }, settings));
                    }

                    if (exist.ROLE == Role.Administrateur || exist.ROLE == Role.Autre)
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            var isUserSet = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt && */b.DELETIONDATE == null && b.ID == exist.ID);
                            var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                            Guid[] guidArray = DeserializeJsonToGuidArray(isUserGed.Sites);

                            var validSites = ged.Sites.Where(a => a.DeletionDate == null).ToDictionary(a => a.Id, a => a);

                            foreach (Guid guid in guidArray)
                            {
                                if (validSites.TryGetValue(guid, out var site))
                                {
                                    var newSite = new SiteGED()
                                    {
                                        Id = guid,
                                        Code = site.SiteId + "-" + site.Name
                                    };

                                    if (!crpto.Any(s => s.Id == newSite.Id && s.Code == newSite.Code))
                                    {
                                        crpto.Add(newSite);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            var validSites = ged.Sites.Where(a => a.DeletionDate == null).ToList();

                            foreach (var guid in validSites.ToList())
                            {
                                var newSite = new SiteGED()
                                {
                                    Id = guid.Id,
                                    Code = guid.SiteId + "-" + guid.Name
                                };

                                if (!crpto.Any(s => s.Id == newSite.Id && s.Code == newSite.Code))
                                {
                                    crpto.Add(newSite);
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

        [HttpPost]
        public ActionResult GETALLFOURNISSEUR(SI_USERS suser, string iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<int> idProjet = new List<int>();

                foreach (var item in iProjet.Split(','))
                {
                    idProjet.Add(int.Parse(item));
                }

                List<Fournisseur> supl = new List<Fournisseur>();

                bool mappingErreur = false;
                bool projetErreur = false;
                bool utilisateurErreur = false;

                if (idProjet != null)
                {
                    foreach (var crpt in idProjet)
                    {
                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                        if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                        {
                            if (mappingErreur == false)
                                mappingErreur = true;
                        }

                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                        {
                            if (projetErreur == false)
                                projetErreur = true;
                        }

                        var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                        var IDUSERGED = userT?.IDUSERGED;

                        if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        {
                            if (utilisateurErreur == false)
                                utilisateurErreur = true;
                        }
                    }

                    if (!mappingErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer le mappage SET-GED."
                        }, settings));
                    }

                    if (!projetErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance projet SET-GED."
                        }, settings));
                    }

                    if (!utilisateurErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                        }, settings));
                    }

                    if (exist.ROLE == Role.Administrateur || exist.ROLE == Role.Autre)
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            var isUserSet = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt && */b.DELETIONDATE == null && b.ID == exist.ID);
                            var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                            var suppliers1 = ged.Suppliers.Where(a => a.ProjectId == isUserGed.ProjectId && a.DeletionDate == null).ToList();
                            var autreprojet = ged.Users.Where(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null).FirstOrDefault().ProjectIdOth.Split(',').ToList();
                            var suppliers2 = ged.Suppliers.Where(a => autreprojet.Contains(a.ProjectId.ToString()) && a.DeletionDate == null).ToList();

                            var suppliers = suppliers1.Union(suppliers2).ToList();

                            foreach (var it in suppliers)
                            {
                                if (!supl.Any(c => c.Id == it.Id && c.Nom == it.Name))  // Vérification si l'élément existe déjà
                                {
                                    supl.Add(new Fournisseur()
                                    {
                                        Id = it.Id,
                                        Nom = it.Name,
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            var suppliers1 = ged.Suppliers.Where(a => a.DeletionDate == null).ToList();

                            //var autreprojet = ged.Users.Where(a => a.DeletionDate == null).FirstOrDefault().ProjectIdOth.Split(',').ToList();
                            //var suppliers2 = ged.Suppliers.Where(a => autreprojet.Contains(a.ProjectId.ToString()) && a.DeletionDate == null).ToList();
                            //var suppliers = suppliers1.Union(suppliers2).ToList();

                            var suppliers = suppliers1;

                            foreach (var it in suppliers)
                            {
                                if (!supl.Any(c => c.Id == it.Id && c.Nom == it.Name))  // Vérification si l'élément existe déjà
                                {
                                    supl.Add(new Fournisseur()
                                    {
                                        Id = it.Id,
                                        Nom = it.Name,
                                    });
                                }
                            }
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

            try
            {
                List<int> idProjet = new List<int>();

                int i = 0;
                foreach (var item in iProjet.Split(','))
                {
                    if (i < 2)
                    {
                        idProjet.Add(int.Parse(item));
                    }
                    i++;
                }

                List<TypeDoc> crpto = new List<TypeDoc>();
                List<string> listSite = iSite.Split(',').ToList();

                bool mappingErreur = false;
                bool projetErreur = false;
                bool utilisateurErreur = false;

                if (idProjet != null)
                {
                    foreach (var crpt in idProjet)
                    {
                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                        if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                        {
                            if (mappingErreur == false)
                                mappingErreur = true;
                        }

                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                        {
                            if (projetErreur == false)
                                projetErreur = true;
                        }

                        var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                        var IDUSERGED = userT?.IDUSERGED;

                        if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                        {
                            if (utilisateurErreur == false)
                                utilisateurErreur = true;
                        }
                    }

                    if (!mappingErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer le mappage SET-GED."
                        }, settings));
                    }

                    if (!projetErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance projet SET-GED."
                        }, settings));
                    }

                    if (!utilisateurErreur)
                    {
                        return Json(JsonConvert.SerializeObject(new
                        {
                            type = "error",
                            msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                        }, settings));
                    }

                    if (exist.ROLE == Role.Administrateur || exist.ROLE == Role.Autre)
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            var isUserSet = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt && */b.DELETIONDATE == null && b.ID == exist.ID);
                            var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                            var isListeTypeD1 = ged.DocumentTypes.Where(a => a.ProjectId == isUserGed.ProjectId && a.DeletionDate == null).ToList();
                            var autreprojet = ged.Users.Where(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null).FirstOrDefault().ProjectIdOth.Split(',').ToList();
                            var isListeTypeD2 = ged.DocumentTypes.Where(a => autreprojet.Contains(a.ProjectId.ToString()) && a.DeletionDate == null).ToList();

                            var isListeTypeD = isListeTypeD1.Union(isListeTypeD2).ToList();

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
                                            if (!crpto.Any(c => c.Id == typD.Id && c.Title == typD.Title))  // Vérification si l'élément existe déjà
                                            {
                                                crpto.Add(new TypeDoc
                                                {
                                                    Id = typD.Id,
                                                    Title = typD.Title
                                                });
                                            }
                                            //crpto.Add(new TypeDoc()
                                            //{
                                            //    Id = typD.Id,
                                            //    Title = typD.Title
                                            //});
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var crpt in idProjet)
                        {
                            SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);
                            SOFTCONNECTGED ged = new SOFTCONNECTGED();

                            //var isUserSet = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt && */b.DELETIONDATE == null && b.ID == exist.ID);
                            //var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                            var isListeTypeD1 = ged.DocumentTypes.Where(a => a.DeletionDate == null).ToList();

                            //var autreprojet = ged.Users.Where(a => a.DeletionDate == null).FirstOrDefault().ProjectIdOth.Split(',').ToList();
                            //var isListeTypeD2 = ged.DocumentTypes.Where(a => autreprojet.Contains(a.ProjectId.ToString()) && a.DeletionDate == null).ToList();
                            //var isListeTypeD = isListeTypeD1.Union(isListeTypeD2).ToList();

                            var isListeTypeD = isListeTypeD1;

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
                                            if (!crpto.Any(c => c.Id == typD.Id && c.Title == typD.Title))  // Vérification si l'élément existe déjà
                                            {
                                                crpto.Add(new TypeDoc
                                                {
                                                    Id = typD.Id,
                                                    Title = typD.Title
                                                });
                                            }
                                            //crpto.Add(new TypeDoc()
                                            //{
                                            //    Id = typD.Id,
                                            //    Title = typD.Title
                                            //});
                                        }
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
            public string PROJET { get; set; }
            public string TYPEEXPEDITEUR { get; set; }
            public string SITE { get; set; }

            public List<string> DATESTEP { get; set; }
        }

        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, string PROJECTID, DateTime DateDebut, DateTime DateFin, string listSite, int status, string fournisseur /*string reference*/)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<string> Projet = new List<string>();
            List<string> site = new List<string>();
            List<DocS> documentF = new List<DocS>(); ;
            Guid IDsup;
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var xyz in PROJECTID.Split(','))
            {
                int crpt = int.Parse(xyz);

                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var PRJS in PROJECTID.Split(','))
            {
                int proj = int.Parse(PRJS);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(proj);
                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                try
                {
                    var isUserSet = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == proj &&*/ b.DELETIONDATE == null && b.ID == exist.ID); ;
                    var isUserGed = ged.Users.FirstOrDefault(a => a.Id == isUserSet.IDUSERGED && a.DeletionDate == null);

                    if (fournisseur != "0")
                    {
                        IDsup = Guid.Parse(fournisseur); ;
                    }
                    else IDsup = new Guid();
                    DateTime datetemp = new DateTime(DateFin.Date.Year,DateFin.Date.Month,DateFin.Date.Day,23,59,59);

                    Guid xs = Guid.Empty;
                    var ddoc = ged.Documents.Where(x => x.CreationDate >= DateDebut.Date && x.CreationDate <= datetemp)
                    .Join(ged.SuppliersDocumentsAcknowledgements.DefaultIfEmpty(), dcm => dcm.Id, sdal => sdal.Id, (dcm, sdal) => new
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
                        Site = dcm.Site.ToString(),
                        DocumentID = dcm.Id,
                        ProjectId = dcm.ProjectId,
                        Project = "",
                        SenderId = dcm.SenderId,
                    })
                    .Join(ged.Projects.DefaultIfEmpty(), dcm => dcm.ProjectId, prj => prj.Id, (dcm, prj) => new
                    {
                        ID = dcm.ID,
                        reference = dcm.reference,
                        Objet = dcm.Objet,
                        Fournisseur = "",
                        Acusse = dcm.Acusse,
                        Validateur = "",
                        Montant = dcm.Montant,
                        Date = dcm.Date,
                        Encours = dcm.Encours,
                        ARCHIVES = "",
                        Lien = dcm.Lien,
                        Site = dcm.Site,
                        DocumentID = dcm.DocumentID,
                        ProjectId = dcm.ProjectId,
                        Project = prj.Name,
                        SenderId = dcm.SenderId,
                    }).Join(ged.Sites.DefaultIfEmpty(), dcm => dcm.Site, prj => prj.Id.ToString(), (dcm, prj) => new{
                        ID = dcm.ID,
                        reference = dcm.reference,
                        Objet = dcm.Objet,
                        Fournisseur = "",
                        Acusse = dcm.Acusse,
                        Validateur = "",
                        Montant = dcm.Montant,
                        Date = dcm.Date,
                        Encours = dcm.Encours,
                        ARCHIVES = "",
                        Lien = dcm.Lien,
                        Site = prj.SiteId + prj.Name,
                        DocumentID = dcm.DocumentID,
                        ProjectId = dcm.ProjectId,
                        Project = prj.Name,
                        SenderId = dcm.SenderId,
                    }).Join(ged.DocumentsSenders.DefaultIfEmpty(), dcm => dcm.ID, prj => prj.Id, (dcm, prj) => new
                    {
                        ID = dcm.ID,
                        reference = dcm.reference,
                        Objet = dcm.Objet,
                        Fournisseur = "",
                        Acusse = dcm.Acusse,
                        Validateur = "",
                        Montant = dcm.Montant,
                        Date = dcm.Date,
                        Encours = dcm.Encours,
                        ARCHIVES = "",
                        Lien = dcm.Lien,
                        Site = dcm.Site,
                        DocumentID = dcm.DocumentID,
                        Project = dcm.Project,
                        SenderId = dcm.SenderId,
                        TYPEEXPEDITEUR = prj.Type == 1 ? "Fournisseur" : "Interne",
                    }).Join(ged.Suppliers.DefaultIfEmpty(), dcm => dcm.ID, sup => sup.Id, (dcm, sup) => new
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
                        Validateur = "",
                        Project = dcm.Project,
                        TYPEEXPEDITEUR = dcm.TYPEEXPEDITEUR,
                    }).ToList();


                    //ddoc

                    //var ress = ddoc.Join(ged.DocumentSteps.DefaultIfEmpty(), dcm => dcm.DocumentID, dcs => dcs.DocumentId, (dcm, dcs) => new
                    //{
                    //    ID = dcm.ID,
                    //    reference = dcm.reference,
                    //    Objet = dcm.Objet,
                    //    Fournisseur = dcm.Fournisseur,
                    //    Acusse = dcm.Acusse,
                    //    Montant = dcm.Montant,
                    //    Encours = dcm.Encours,
                    //    ARCHIVES = dcm.ARCHIVES,
                    //    Lien = dcm.Lien,
                    //    Site = dcm.Site,
                    //    DocumentID = dcm.DocumentID,
                    //    Validateur = dcm.Validateur,
                    //    Project = dcm.Project,
                    //    TYPEEXPEDITEUR = dcm.TYPEEXPEDITEUR,
                    //    DocumentStepID = dcs.Id,
                    //    Etape = dcs.ProcessingDescription,
                    //}).ToList();
                    List<DocS> ress = new List<DocS>();

                    if (ddoc != null )
                    {
                        foreach (var Do in ddoc)
                        {
                            var DocsStep = ged.DocumentSteps.Where(x => x.DocumentId == Do.DocumentID).FirstOrDefault();

                            if (DocsStep != null)
                            {
                                var userEtapeLast = ged.UsersSteps.Where(x => x.DocumentStepId == DocsStep.Id && x.IsValidator == true).OrderBy(x => x.ProcessingDate).FirstOrDefault();
                                if (userEtapeLast != null)
                                {
                                    var userVal = ged.Users.Where(x => x.Id == userEtapeLast.UserId).FirstOrDefault();
                                    ress.Add( new DocS
                                    {
                                        REF = Do.reference,
                                        Objet = Do.Objet,
                                        FOURNISSEUR = Do.Fournisseur,
                                        ACCUSE = Do.Acusse,
                                        VALIDATEUR = userVal.Email,
                                        Montant = Do.Montant.ToString(),
                                        Encours = Do.Encours.ToString(),//DocsStep.ProcessingDescription
                                        ARCHIVES = Do.ARCHIVES.ToString(),
                                        Lien =  Do.Lien.ToString(),
                                        PROJET = Do.Project,
                                        SITE = Do.Site,
                                        TYPEEXPEDITEUR = Do.TYPEEXPEDITEUR,
                                        //Etape = DocsStep.ProcessingDescription,
                                    });
                                }
                            }
                        }
                    }
                    var links = db.SI_GEDLIEN.Where(x => x.IDPROJET == proj).Select(x => x.LIEN).FirstOrDefault();

                    if (ress != null)
                    {
                        if (status != 4 || fournisseur != "0")
                        {
                            var suppliersname = ged.Suppliers.Where(x => x.Id == IDsup).FirstOrDefault(); ;
                            foreach (var typD in ress.Where(x => x.Encours == status.ToString() && x.FOURNISSEUR == suppliersname.Name))
                            {
                                string uservalidateur = "";
                                string SSITE = typD.SITE;
                                documentF.Add(new DocS
                                {
                                    REF = typD.REF,
                                    Objet = typD.Objet,
                                    FOURNISSEUR = typD.FOURNISSEUR,
                                    ACCUSE = typD.ACCUSE,
                                    VALIDATEUR = typD.VALIDATEUR,
                                    Montant = typD.Montant.ToString(),
                                    Encours = typD.Encours.ToString(),
                                    ARCHIVES = typD.ARCHIVES.ToString(),
                                    Lien = links + "/" + typD.Lien.ToString(),
                                    PROJET = typD.PROJET,
                                    SITE = typD.SITE,
                                    TYPEEXPEDITEUR = typD.TYPEEXPEDITEUR,
                                    //Validations = uservalidateur != null ? uservalidateur : "",
                                });
                            }
                        }
                        else if (fournisseur == "0")
                        {
                            foreach (var typD in ress)
                            {
                                string uservalidateur = "";
                                string SSITE = typD.SITE;
                                documentF.Add(new DocS
                                {
                                    REF = typD.REF,
                                    Objet = typD.Objet,
                                    FOURNISSEUR = typD.FOURNISSEUR,
                                    ACCUSE = typD.ACCUSE,
                                    VALIDATEUR = typD.VALIDATEUR,
                                    Montant = typD.Montant.ToString(),
                                    Encours = typD.Encours.ToString(),
                                    ARCHIVES = typD.ARCHIVES.ToString(),
                                    Lien = links + "/" + typD.Lien.ToString(),
                                    PROJET = typD.PROJET,
                                    SITE = typD.SITE,
                                    TYPEEXPEDITEUR = typD.TYPEEXPEDITEUR,
                                    //Validations = uservalidateur != null ? uservalidateur : "",
                                });
                            }
                        }
                        else if (status == 4 || fournisseur != "0")
                        {
                            var suppliersname = ged.Suppliers.Where(x => x.Id == IDsup).FirstOrDefault(); ;
                            foreach (var typD in ress.Where(x => x.FOURNISSEUR == suppliersname.Name))
                            {
                                string uservalidateur = "";
                                string SSITE = typD.SITE;
                                documentF.Add(new DocS
                                {
                                    REF = typD.REF,
                                    Objet = typD.Objet,
                                    FOURNISSEUR = typD.FOURNISSEUR,
                                    ACCUSE = typD.ACCUSE,
                                    VALIDATEUR = typD.VALIDATEUR,
                                    Montant = typD.Montant.ToString(),
                                    Encours = typD.Encours.ToString(),
                                    ARCHIVES = typD.ARCHIVES.ToString(),
                                    Lien = links + "/" + typD.Lien.ToString(),
                                    PROJET = typD.PROJET,
                                    SITE = typD.SITE,
                                    TYPEEXPEDITEUR = typD.TYPEEXPEDITEUR,
                                    //Validations = uservalidateur != null ? uservalidateur : "",
                                });
                            }
                        }
                        else
                        {
                            foreach (var typD in ress)
                            {
                                string uservalidateur = "";
                                string SSITE = typD.SITE;
                                documentF.Add(new DocS
                                {
                                    REF = typD.REF,
                                    Objet = typD.Objet,
                                    FOURNISSEUR = typD.FOURNISSEUR,
                                    ACCUSE = typD.ACCUSE,
                                    VALIDATEUR = typD.VALIDATEUR,
                                    Montant = typD.Montant.ToString(),
                                    Encours = typD.Encours.ToString(),
                                    ARCHIVES = typD.ARCHIVES.ToString(),
                                    Lien = links + "/" + typD.Lien.ToString(),
                                    PROJET = typD.PROJET,
                                    SITE = typD.SITE,
                                    TYPEEXPEDITEUR = typD.TYPEEXPEDITEUR,
                                    //Validations = uservalidateur != null ? uservalidateur : "",
                                });
                            }
                        }
                    }

                    //return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = documentF }, settings));
                }
                catch (Exception e)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
                }
            }

            List<DocS> list = documentF.DistinctBy(x => x.REF).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = list }, settings));
        }

        [HttpPost]
        public JsonResult GenereREFERENCE(SI_USERS suser, string PROJECTID, string listSite)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<string> Projet = new List<string>();
            List<string> site = new List<string>();
            List<REFF> refff = new List<REFF>();

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

            foreach (var Proj in PROJECTID.Split(','))
            {
                int crpt = int.Parse(Proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var Proj in PROJECTID.Split(','))
            {
                int crpt = int.Parse(Proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                refff = ged.SuppliersDocumentsAcknowledgements.Select(x => new REFF
                {
                    ID = x.Id,
                    Reference = x.ReferenceInterne,
                }).ToList();
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = refff }, settings));
        }

        public class REFF
        {
            public Guid ID { get; set; }
            public string Reference { get; set; }
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
            public string PROJET { get; set; }
            public string SITE { get; set; }
            public string TYPEEXPEDITEUR { get; set; }
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

            List<TDB> list = new List<TDB>();

            List<Guid> Projet = new List<Guid>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var item in listProjet.Split(','))
            {
                int crpt = int.Parse(item);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var item in listProjet.Split(','))
            {
                int idd = int.Parse(item);

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);
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
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        var projetIntitule = ged.Projects.FirstOrDefault(a => a.Id == x && a.DeletionDate == null).Name;

                        foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                        /*&& a.DocumentsSenders.Type == 1*/ && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
                        && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                        {
                            //Document type// na tonga dia document no ijerena ny type misy azy
                            if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id))
                            {
                                Guid? IdDocTypes = ged.DocumentTypeUnion.FirstOrDefault(a => a.DocumentID == y.Id).TypeDocID;

                                if (ged.DocumentTypes.Any(a => a.Id == IdDocTypes))
                                {
                                    var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.DeletionDate == null);

                                    //Accusé de récéption//
                                    if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                    {
                                        var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                        var document = y.Object;

                                        //Site//
                                        Guid siteId = Guid.Parse(y.Site);
                                        var siteTest = ged.Sites.FirstOrDefault(a => a.Id == siteId);
                                        var siteIntitule = siteTest != null ? $"{siteTest.SiteId} - {siteTest.Name}" : "";

                                        //TYPE EXPEDITEUR//
                                        var TYPEEXPEDITEUR = y.DocumentsSenders.Type == 1 ? "Fournisseur" : "Interne";

                                        var fournisseur = "";
                                        if (y.DocumentsSenders.Type == 1)
                                        {
                                            if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                                fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id).Name;
                                        }
                                        else
                                        {
                                            var userInfo = ged.Users.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id);
                                            fournisseur = (!String.IsNullOrEmpty(userInfo.Fonction) ? userInfo.Fonction.ToString() : "SANS FONCTION") + " : " + userInfo.Username.ToString() + " : " + userInfo.LastName.ToString() + " " + userInfo.FirstName.ToString();
                                        }

                                        var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                        //Etape actuel = validation ok actuelle//
                                        var validationHisto = "";
                                        var validationHistoNEXT = "";
                                        var validationHistoNEXTvalidateur = "";
                                        var validationHistoNEXTduree = "";
                                        if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)))
                                        {
                                            var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                            var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId && a.DeletionDate == null);

                                            if (documentStep == null)
                                            {
                                                var validationInProgressFin = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();

                                                if (validationInProgressFin != null)
                                                {
                                                    var documentStepFin = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgressFin.ToDocumentStepId && a.DeletionDate == null);
                                                    if (documentStepFin != null)
                                                    {
                                                        var stepNumberFin = documentStepFin.StepNumber;

                                                        validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin /*&& a.DeletionDate == null*/).ProcessingDescription;
                                                        validationHistoNEXT = "Terminé";
                                                        validationHistoNEXTvalidateur = "Terminé";
                                                        validationHistoNEXTduree = "0";
                                                    }
                                                    else
                                                    {
                                                        documentStepFin = ged.DocumentSteps.Where(a => a.DocumentId == y.Id).OrderByDescending(a => a.StepNumber).FirstOrDefault();

                                                        var stepNumberFin = documentStepFin.StepNumber;

                                                        validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin /*&& a.DeletionDate == null*/).ProcessingDescription;
                                                        validationHistoNEXT = "Terminé";
                                                        validationHistoNEXTvalidateur = "Terminé";
                                                        validationHistoNEXTduree = "0";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var stepNumber = documentStep.StepNumber;

                                                //Get steps information//
                                                var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber - 1) && a.DeletionDate == null);
                                                if (isStepType == null)
                                                {
                                                    validationHisto = "Etape " + stepNumber + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null).ProcessingDescription;
                                                    validationHistoNEXT = "Terminé";
                                                    validationHistoNEXTvalidateur = "Terminé";
                                                    validationHistoNEXTduree = "0";
                                                }
                                                else
                                                {
                                                    validationHisto = "Etape " + (stepNumber - 1) + " : " + isStepType.ProcessingDescription;

                                                    var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                                    validationHistoNEXT = "Etape " + (stepNumber) + " : " + isStepTypeNEXT.ProcessingDescription;

                                                    validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                                    var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber && a.DeletionDate == null);
                                                    var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                                    var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                                    validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                                (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                                (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                                (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                                    if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                                    {
                                                        validationHistoNEXTvalidateur = "";
                                                        foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
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
                                                DUREENEXT = validationHistoNEXTduree,
                                                PROJET = projetIntitule,
                                                TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                                SITE = siteIntitule
                                            });
                                        }
                                        else
                                        {
                                            var stepNumber = 1;

                                            var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                            validationHisto = "Etape " + stepNumber + " : " + isStepType.ProcessingDescription;

                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);

                                            validationHistoNEXT = "Etape " + (stepNumber + 1) + " : " + isStepTypeNEXT.ProcessingDescription;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                            validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                        (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                        (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                        (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                            if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                            {
                                                validationHistoNEXTvalidateur = "";
                                                foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
                                                {
                                                    var isUser = ged.Users.FirstOrDefault(a => a.Id == vhe.UserId);
                                                    validationHistoNEXTvalidateur += "<li>" + ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                        (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                        (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                        (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName)) + "</li>";
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
                                                DUREENEXT = validationHistoNEXTduree,
                                                PROJET = projetIntitule,
                                                TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                                SITE = siteIntitule
                                            });
                                        }
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

                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDGED == idProjet && a.DELETIONDATE == null).IDPROJET.Value);
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        var projetIntitule = ged.Projects.FirstOrDefault(a => a.Id == x && a.DeletionDate == null).Name;

                        //Document type//
                        Guid IdDocTypes = Guid.Parse(TypeDoc);
                        //var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes && a.ProjectId == idProjet /*&& a.DeletionDate == null*/);

                        if (ged.DocumentTypes.Any(a => a.Id == IdDocTypes))
                        {
                            var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes);

                            //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                            foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                            /*&& a.DocumentsSenders.Type == 1*/ && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
                            && a.ProjectId == idProjet))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                            {
                                if (ged.DocumentTypeUnion.Any(a => a.DocumentID == y.Id && a.TypeDocID == typedoc.Id))
                                {
                                    //Accusé de récéption//
                                    if (ged.SuppliersDocumentsAcknowledgements.Any(a => a.Id == y.Id) /*&& y.DeletionDate == null*/) //Status == 1 => Création circuit : OK
                                    {
                                        var reference = ged.SuppliersDocumentsAcknowledgements.FirstOrDefault(a => a.Id == y.Id /*&& y.DeletionDate == null*/).ReferenceInterne;
                                        var document = y.Object;

                                        //Site//
                                        Guid siteId = Guid.Parse(y.Site);
                                        var siteTest = ged.Sites.FirstOrDefault(a => a.Id == siteId);
                                        var siteIntitule = siteTest != null ? $"{siteTest.SiteId} - {siteTest.Name}" : "";

                                        //TYPE EXPEDITEUR//
                                        var TYPEEXPEDITEUR = y.DocumentsSenders.Type == 1 ? "Fournisseur" : "Interne";

                                        var fournisseur = "";
                                        if (y.DocumentsSenders.Type == 1)
                                        {
                                            if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                                fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id).Name;
                                        }
                                        else
                                        {
                                            var userInfo = ged.Users.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id);
                                            fournisseur = (!String.IsNullOrEmpty(userInfo.Fonction) ? userInfo.Fonction.ToString() : "SANS FONCTION") + " : " + userInfo.Username.ToString() + " : " + userInfo.LastName.ToString() + " " + userInfo.FirstName.ToString();
                                        }

                                        var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

                                        //Etape actuel = validation ok actuelle//
                                        var validationHisto = "";
                                        var validationHistoNEXT = "";
                                        var validationHistoNEXTvalidateur = "";
                                        var validationHistoNEXTduree = "";
                                        if (ged.ValidationsHistory.Any(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)))
                                        {
                                            var validationInProgress = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && (a.ActionType == 0 || a.ActionType == 3)).OrderByDescending(a => a.CreationDate).FirstOrDefault();
                                            var documentStep = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgress.ToDocumentStepId && a.DeletionDate == null);

                                            if (documentStep == null)
                                            {
                                                var validationInProgressFin = ged.ValidationsHistory.Where(a => a.DocumentId == y.Id && a.ActionType == 0).OrderByDescending(a => a.CreationDate).FirstOrDefault();

                                                if (validationInProgressFin != null)
                                                {
                                                    var documentStepFin = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgressFin.ToDocumentStepId && a.DeletionDate == null);
                                                    if (documentStepFin != null)
                                                    {
                                                        var stepNumberFin = documentStepFin.StepNumber;

                                                        validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin && a.DeletionDate == null).ProcessingDescription;
                                                        validationHistoNEXT = "Terminé";
                                                        validationHistoNEXTvalidateur = "Terminé";
                                                        validationHistoNEXTduree = "0";
                                                    }
                                                    else
                                                    {
                                                        documentStepFin = ged.DocumentSteps.Where(a => a.DocumentId == y.Id).OrderByDescending(a => a.StepNumber).FirstOrDefault();

                                                        var stepNumberFin = documentStepFin.StepNumber;

                                                        validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin && a.DeletionDate == null).ProcessingDescription;
                                                        validationHistoNEXT = "Terminé";
                                                        validationHistoNEXTvalidateur = "Terminé";
                                                        validationHistoNEXTduree = "0";
                                                    }
                                                }

                                                //var documentStepFin = ged.DocumentSteps.FirstOrDefault(a => a.Id == validationInProgressFin.ToDocumentStepId /*&& a.DeletionDate == null*/);
                                                //var stepNumberFin = documentStepFin.StepNumber;

                                                //validationHisto = "Etape " + stepNumberFin + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumberFin /*&& a.DeletionDate == null*/).ProcessingDescription;
                                                //validationHistoNEXT = "Terminé";
                                                //validationHistoNEXTvalidateur = "Terminé";
                                                //validationHistoNEXTduree = "0";
                                            }
                                            else
                                            {
                                                var stepNumber = documentStep.StepNumber;

                                                //Get steps information//
                                                var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber - 1) && a.DeletionDate == null);
                                                if (isStepType == null)
                                                {
                                                    validationHisto = "Etape " + stepNumber + " : " + ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null).ProcessingDescription;
                                                    validationHistoNEXT = "Terminé";
                                                    validationHistoNEXTvalidateur = "Terminé";
                                                    validationHistoNEXTduree = "0";
                                                }
                                                else
                                                {
                                                    validationHisto = "Etape " + (stepNumber - 1) + " : " + isStepType.ProcessingDescription;

                                                    var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                                    validationHistoNEXT = "Etape " + (stepNumber) + " : " + isStepTypeNEXT.ProcessingDescription;

                                                    validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                                    var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == stepNumber && a.DeletionDate == null);
                                                    var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                                    var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                                    validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                                (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                                (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                                (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                                    if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                                    {
                                                        validationHistoNEXTvalidateur = "";
                                                        foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
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
                                                DUREENEXT = validationHistoNEXTduree,
                                                PROJET = projetIntitule,
                                                TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                                SITE = siteIntitule
                                            });
                                        }
                                        else
                                        {
                                            var stepNumber = 1;

                                            var isStepType = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == stepNumber && a.DeletionDate == null);

                                            validationHisto = "Etape " + stepNumber + " : " + isStepType.ProcessingDescription;

                                            var isStepTypeNEXT = ged.DocumentTypesSteps.FirstOrDefault(a => a.DocumentTypeId == typedoc.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);

                                            validationHistoNEXT = "Etape " + (stepNumber + 1) + " : " + isStepTypeNEXT.ProcessingDescription;

                                            validationHistoNEXTduree = isStepTypeNEXT.ProcessingDuration.ToString();

                                            var isStepNext = ged.DocumentSteps.FirstOrDefault(a => a.DocumentId == y.Id && a.StepNumber == (stepNumber + 1) && a.DeletionDate == null);
                                            var userStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null);
                                            var isuu = ged.Users.FirstOrDefault(a => a.Id == userStep.UserId /*&& a.DeletionDate == null*/);
                                            validationHistoNEXTvalidateur = (String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                        (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                        (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                        (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName);

                                            if (ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).Count() > 1)
                                            {
                                                validationHistoNEXTvalidateur = "";
                                                foreach (var vhe in ged.UsersSteps.Where(a => a.DocumentStepId == isStepNext.Id && a.DeletionDate == null).ToList())
                                                {
                                                    var isUser = ged.Users.FirstOrDefault(a => a.Id == vhe.UserId);
                                                    validationHistoNEXTvalidateur += "<li>" + ((String.IsNullOrEmpty(isuu.Fonction) ? "SANS FONCTION" : isuu.Fonction + " : ") +
                                                        (String.IsNullOrEmpty(isuu.Username) ? "" : isuu.Username + " : ") +
                                                        (String.IsNullOrEmpty(isuu.LastName) ? "" : isuu.LastName + " ") +
                                                        (String.IsNullOrEmpty(isuu.FirstName) ? "" : isuu.FirstName)) + "</li>";
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
                                                DUREENEXT = validationHistoNEXTduree,
                                                PROJET = projetIntitule,
                                                TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                                SITE = siteIntitule
                                            });
                                        }
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
        public ActionResult GetNIFSTATCIN(SI_USERS suser, string PROJECTID)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));
            List<supplierRAF> InfosS = new List<supplierRAF>();

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var proj in PROJECTID.Split(','))
            {
                int crpt = int.Parse(proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var proj in PROJECTID.Split(','))
            {
                int projS = int.Parse(proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(projS);

                SOFTCONNECTGED ged = new SOFTCONNECTGED();
                var Infos = ged.Suppliers.Where(x => x.DeletionDate == null).Select(x => new supplierRAF
                {
                    ID = x.Id,
                    NIF = x.NIF,
                    STAT = x.STAT,
                    CIN = x.CIN,
                }).ToList();
                InfosS.AddRange(Infos);
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", data = InfosS }));
        }
        public class supplierRAF
        {
            public Guid ID { get; set; }
            public string NIF { get; set; }
            public string STAT { get; set; }
            public string CIN { get; set; }
        }

        [HttpPost]
        public ActionResult GetSituationsDoc(SI_USERS suser, string listesite, string NIF, string STAT, string CIN, string PROJECTID, string REFERENCE)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<Documents> Infos = new List<Documents>();

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var proj in PROJECTID.Split(','))
            {
                int crpt = int.Parse(proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            Guid IDref = Guid.Parse(REFERENCE);

            Infos = ged.Documents.ToList();
            var documentypes = ged.DocumentTypes.ToList();
            List<DocumentInfos> Infosdoc = new List<DocumentInfos>();
            List<documentFdR> resultat = new List<documentFdR>();
            var referenS = ged.SuppliersDocumentsAcknowledgements.Where(x => x.Id == IDref).FirstOrDefault();

            foreach (var proj in PROJECTID.Split(','))
            {
                int projId = int.Parse(proj);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(projId);
                SOFTCONNECTGED ged = new SOFTCONNECTGED();
                string links = db.SI_GEDLIEN.Where(x=> x.IDPROJET == projId).Select(x => x.LIEN).FirstOrDefault();
                var docc = ged.Documents.Join(ged.Projects.DefaultIfEmpty(),doc => doc.ProjectId , dcm => dcm.Id , (doc,dcm) => new
                {
                    IDDOCUMENT = doc.Id,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.Filename,
                    Projet = dcm.Name,
                })
                .Join(ged.DocumentsSenders.DefaultIfEmpty(), doc => doc.SenderId, docsend => docsend.Id, (doc, docsend) => new
                {
                    IDDOCUMENT = doc.IDDOCUMENT,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.FileName,
                    Projet = doc.Projet,
                    Type = docsend.Type,
                }).Join(ged.SuppliersDocumentsAcknowledgements.DefaultIfEmpty(), doc => doc.IDDOCUMENT, ackn => ackn.Id, (doc, ackn) => new
                {
                    IDDOCUMENT = doc.IDDOCUMENT,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.FileName,
                    Projet = doc.Projet,
                    Type = doc.Type,
                    referenceinterne = ackn.ReferenceInterne,
                }).Join(ged.DocumentSteps.DefaultIfEmpty(), doc => doc.IDDOCUMENT, docstep => docstep.DocumentId, (doc, docstep) => new
                {
                    IDDOCUMENT = doc.IDDOCUMENT,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.FileName,
                    StepNumber = docstep.StepNumber,
                    ProcessingDescription = docstep.ProcessingDescription,
                    IDDOCSTEP = docstep.Id,
                    Projet = doc.Projet,
                    referenceinterne = doc.referenceinterne,
                }).Join(ged.UsersSteps.DefaultIfEmpty(), res => res.IDDOCSTEP, usrstep => usrstep.DocumentStepId, (res, usrstep) => new
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
                    Projet = res.Projet,
                    commentaire = usrstep.Comment
                }).ToList();



                var informationsDoc = ged.Documents.Join(ged.DocumentsSenders.DefaultIfEmpty(), doc => doc.SenderId, docsend => docsend.Id, (doc, docsend) => new
                {
                    IDDOCUMENT = doc.Id,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.Filename,
                    Type = docsend.Type,
                }).Join(ged.SuppliersDocumentsAcknowledgements.DefaultIfEmpty(), doc => doc.IDDOCUMENT, ackn => ackn.Id, (doc, ackn) => new
                {
                    IDDOCUMENT = doc.IDDOCUMENT,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.FileName,
                    Type = doc.Type,
                    referenceinterne = ackn.ReferenceInterne,
                }).Join(ged.DocumentSteps.DefaultIfEmpty(), doc => doc.IDDOCUMENT, docstep => docstep.DocumentId, (doc, docstep) => new
                {
                    IDDOCUMENT = doc.IDDOCUMENT,
                    SenderId = doc.SenderId,
                    CreationDate = doc.CreationDate,
                    FileName = doc.FileName,
                    StepNumber = docstep.StepNumber,
                    ProcessingDescription = docstep.ProcessingDescription,
                    IDDOCSTEP = docstep.Id,
                    referenceinterne = doc.referenceinterne,
                }).Join(ged.UsersSteps.DefaultIfEmpty(), res => res.IDDOCSTEP, usrstep => usrstep.DocumentStepId, (res, usrstep) => new
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
                }).Where(usrstep => usrstep.Isvalidator == true)
                .Join(ged.ValidationsHistory.DefaultIfEmpty(), res => res.IDDOCUMENT, valHisto => valHisto.DocumentId, (res, valhisto) => new
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
                }).Join(ged.Users.DefaultIfEmpty(), res => res.FromUserID, usr => usr.Id, (res, usr) => new
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
                }).Join(ged.Suppliers.DefaultIfEmpty(), res => res.SenderId, supl => supl.Id, (res, supl) => new documentFdR
                {
                    IDDOCUMENT = res.IDDOCUMENT,
                    SenderId = res.SenderId,
                    CreationDate = res.CreationDate,
                    FileName = links + "/" + res.FileName,
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
                resultat.AddRange(informationsDoc);
            }

            return Json(JsonConvert.SerializeObject(new { type = "success", data = resultat }));
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
        public class documentFdR
        {
            public Guid IDDOCUMENT { get; set; }
            public int Type { get; set; }
            public Guid SenderId { get; set; }
            public DateTime? CreationDate { get; set; }
            public string FileName { get; set; }
            public int? StepNumber { get; set; }
            public string ProcessingDescription { get; set; }
            public Guid IDDOCSTEP { get; set; }
            public Guid UserID { get; set; }
            public Guid FromUserID { get; set; }
            public string Comment { get; set; }
            public DateTime? DATEValidations { get; set; }
            public string UserName { get; set; }
            public string referenceinterne { get; set; }
            public string Fournisseur { get; set; }
            public bool? Isvalidator { get; set; }
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

            List<TDB> list = new List<TDB>();

            List<Guid> Projet = new List<Guid>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var item in listProjet.Split(','))
            {
                int crpt = int.Parse(item);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var item in listProjet.Split(','))
            {
                int idd = int.Parse(item);

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);
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
                    SOFTCONNECTGED ged = new SOFTCONNECTGED();

                    var projetIntitule = ged.Projects.FirstOrDefault(a => a.Id == x && a.DeletionDate == null).Name;

                    //nombreEtape = 0;

                    //Document type//
                    Guid IdDocTypes = Guid.Parse(TypeDoc);

                    if (ged.DocumentTypes.Any(a => a.Id == IdDocTypes))
                    {
                        var typedoc = ged.DocumentTypes.FirstOrDefault(a => a.Id == IdDocTypes);

                        nombreEtape = ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).Count();

                        if (nombreEtape != 0)
                        {
                            listEtape = new List<string>();

                            foreach (var elem in ged.DocumentTypesSteps.Where(a => a.DocumentTypeId == typedoc.Id && a.DeletionDate == null).OrderBy(a => a.StepNumber).ToList())
                            {
                                listEtape.Add("Etape " + elem.StepNumber + " : " + elem.ProcessingDescription);
                            }

                            //Mbola misy code eto ijerena ny type de docs WHERE @io ambany io//
                            foreach (var y in ged.Documents.Where(a => a.CreationDate >= DD && a.CreationDate <= DF && site.Contains(a.Site)
                            /*&& a.DocumentsSenders.Type == 1*/ && a.DeletionDate == null && (a.Status == 1 || a.Status == 3)
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

                                        //Site//
                                        Guid siteId = Guid.Parse(y.Site);
                                        var siteTest = ged.Sites.FirstOrDefault(a => a.Id == siteId);
                                        var siteIntitule = siteTest != null ? $"{siteTest.SiteId} - {siteTest.Name}" : "";

                                        //TYPE EXPEDITEUR//
                                        var TYPEEXPEDITEUR = y.DocumentsSenders.Type == 1 ? "Fournisseur" : "Interne";

                                        var fournisseur = "";
                                        if (y.DocumentsSenders.Type == 1)
                                        {
                                            if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                                fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id).Name;
                                        }
                                        else
                                        {
                                            var userInfo = ged.Users.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id);
                                            fournisseur = (!String.IsNullOrEmpty(userInfo.Fonction) ? userInfo.Fonction.ToString() : "SANS FONCTION") + " : " + userInfo.Username.ToString() + " : " + userInfo.LastName.ToString() + " " + userInfo.FirstName.ToString();
                                        }

                                        var montant = y.Montant != null ? Math.Round(y.Montant.Value, 2).ToString() : "0";

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
                                                var dateValidation = " ";

                                                if (ged.UsersSteps.Any(a => a.DocumentStepId == DocSteps.Id && a.ProcessingDate != null && a.IsValidator == true && a.DeletionDate == null))
                                                {
                                                    var UsersStep = ged.UsersSteps.FirstOrDefault(a => a.DocumentStepId == DocSteps.Id && a.ProcessingDate != null && a.IsValidator == true && a.DeletionDate == null);//A verifier le ACTIONTYPE dans ValidationHistory si besoin (0 : validation ou 3 : archivage)

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

                                        while (dateStep.Count < nombreEtape)
                                        {
                                            dateStep.Add("");
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
                                            RATTACHTOM = RATTACHTOM,//rattachement TOMATE,
                                            TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                            SITE = siteIntitule,
                                            PROJET = projetIntitule
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

            List<TDB> list = new List<TDB>();

            List<Guid> Projet = new List<Guid>();
            List<string> site = new List<string>();
            foreach (var item in listSite.Split(','))
            {
                site.Add(item);
            }

            bool mappingErreur = false;
            bool projetErreur = false;
            bool utilisateurErreur = false;

            foreach (var item in listProjet.Split(','))
            {
                int crpt = int.Parse(item);
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED(crpt);

                if (!string.IsNullOrEmpty(SOFTCONNECTGED.connex))
                {
                    if (mappingErreur == false)
                        mappingErreur = true;
                }

                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (db.SI_PROGED.Any(a => a.IDPROJET == crpt))
                {
                    if (projetErreur == false)
                        projetErreur = true;
                }

                var userT = db.SI_USERS.FirstOrDefault(b => /*b.IDPROJET == crpt &&*/ b.DELETIONDATE == null && b.ID == exist.ID);
                var IDUSERGED = userT?.IDUSERGED;

                if (IDUSERGED != null && ged.Users.Any(a => a.Id == IDUSERGED && a.DeletionDate == null))
                {
                    if (utilisateurErreur == false)
                        utilisateurErreur = true;
                }
            }

            if (!mappingErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer le mappage SET-GED."
                }, settings));
            }

            if (!projetErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance projet SET-GED."
                }, settings));
            }

            if (!utilisateurErreur)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    type = "error",
                    msg = "Veuillez paramétrer la correspondance utilisateur SET-GED."
                }, settings));
            }

            foreach (var item in listProjet.Split(','))
            {
                int idd = int.Parse(item);

                var isPG = db.SI_PROGED.FirstOrDefault(a => a.IDPROJET == idd && a.DELETIONDATE == null);

                Projet.Add(isPG.IDGED.Value);
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
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        var projetIntitule = ged.Projects.FirstOrDefault(a => a.Id == x && a.DeletionDate == null).Name;

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

                                    /// Site//
                                    Guid siteId = Guid.Parse(y.Site);
                                    var siteTest = ged.Sites.FirstOrDefault(a => a.Id == siteId);
                                    var siteIntitule = siteTest != null ? $"{siteTest.SiteId} - {siteTest.Name}" : "";

                                    //TYPE EXPEDITEUR//
                                    var TYPEEXPEDITEUR = y.DocumentsSenders.Type == 1 ? "Fournisseur" : "Interne";

                                    var fournisseur = "";
                                    if (y.DocumentsSenders.Type == 1)
                                    {
                                        if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                            fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id).Name;
                                    }
                                    else
                                    {
                                        var userInfo = ged.Users.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id);
                                        fournisseur = (!String.IsNullOrEmpty(userInfo.Fonction) ? userInfo.Fonction.ToString() : "SANS FONCTION") + " : " + userInfo.Username.ToString() + " : " + userInfo.LastName.ToString() + " " + userInfo.FirstName.ToString();
                                    }

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
                                            if (ged.UsersSteps.Any(a => a.DocumentStepId == s.Id && a.DeletionDate == null && a.IsValidator == true))// != NULL suite requête de GED : GetTotalNumberOfCanceledDocuments
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
                                        SITE = siteIntitule,
                                        TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                        PROJET = projetIntitule,
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

                        SOFTCONNECTGED.connex = new Data.Extension().GetConGED(db.SI_PROGED.FirstOrDefault(a => a.IDGED == idProjet && a.DELETIONDATE == null).IDPROJET.Value);
                        SOFTCONNECTGED ged = new SOFTCONNECTGED();

                        var projetIntitule = ged.Projects.FirstOrDefault(a => a.Id == x && a.DeletionDate == null).Name;

                        //Fournisseur//
                        Guid IdDocTypes = Guid.Parse(ListFournisseur);

                        if (ged.Suppliers.Any(a => a.Id == IdDocTypes/* && a.ProjectId == idProjet && a.DeletionDate == null*/))
                        {
                            var Idfournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == IdDocTypes/* && a.ProjectId == idProjet && a.DeletionDate == null*/);

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

                                        //Site//
                                        Guid siteId = Guid.Parse(y.Site);
                                        var siteTest = ged.Sites.FirstOrDefault(a => a.Id == siteId);
                                        var siteIntitule = siteTest != null ? $"{siteTest.SiteId} - {siteTest.Name}" : "";

                                        //TYPE EXPEDITEUR//
                                        var TYPEEXPEDITEUR = y.DocumentsSenders.Type == 1 ? "Fournisseur" : "Interne";

                                        var fournisseur = "";
                                        if (y.DocumentsSenders.Type == 1)
                                        {
                                            if (ged.Suppliers.Any(a => a.Id == y.DocumentsSenders.Id))//PARTIE PROJET : rattachement d'un doc à un projet : à modifier après affectation doc à des projets//
                                                fournisseur = ged.Suppliers.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id).Name;
                                        }
                                        else
                                        {
                                            var userInfo = ged.Users.FirstOrDefault(a => a.Id == y.DocumentsSenders.Id);
                                            fournisseur = (!String.IsNullOrEmpty(userInfo.Fonction) ? userInfo.Fonction.ToString() : "SANS FONCTION") + " : " + userInfo.Username.ToString() + " : " + userInfo.LastName.ToString() + " " + userInfo.FirstName.ToString();
                                        }

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
                                                if (ged.UsersSteps.Any(a => a.DocumentStepId == s.Id && a.DeletionDate == null && a.IsValidator == true))// != NULL suite requête de GED : GetTotalNumberOfCanceledDocuments
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
                                            COMM = comm,
                                            SITE = siteIntitule,
                                            TYPEEXPEDITEUR = TYPEEXPEDITEUR,
                                            PROJET = projetIntitule,
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
    }
}
