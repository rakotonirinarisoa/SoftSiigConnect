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
        [HttpPost]
        public JsonResult GenereLISTE(SI_USERS suser, int PROJECTID, DateTime DateDebut, DateTime DateFin, string listSite, int status, string fournisseur,string reference)
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
                var suppliersname = ged.Suppliers.Where(x => x.Id == IDsup).FirstOrDefault();
                var RefDoc = ged.Documents.Where(x => x.DeletionDate == null && x.CreationDate >= DateDebut && x.CreationDate <= DateFin && x.Status == status).Join(ged.SuppliersDocumentsAcknowledgements, dcm => dcm.Id, sdal => sdal.Id, (dcm, sdal) => new
                {
                    ID = dcm.SenderId,
                    reference = sdal.ReferenceInterne,
                    Objet = dcm.Object,
                    Fournisseur = "",
                    Acusse = dcm.CreationDate,
                    Validateur = "",
                    Montant = 0,
                    Encours = dcm.Status,
                    ARCHIVES = "",
                    Lien = dcm.Url,
                    Site = dcm.Site,
                }).Join(ged.Suppliers, dcm => dcm.ID, sup => sup.Id, (dcm, sup) => new
                {
                    ID = dcm.ID,
                    reference = dcm.reference,
                    Objet = dcm.Objet,
                    Fournisseur = sup.Name,
                    Acusse = dcm.Acusse,
                    Validateur = "",
                    Montant = 0,
                    Encours = dcm.Encours,
                    ARCHIVES = dcm.Encours == 3 ? "ARCHIVER" : "NON ARCHIVER",
                    Lien = dcm.Lien,
                    Site = dcm.Site,

                }).Where(x => x.Fournisseur == suppliersname.Name).ToList();

                if (RefDoc != null)
                {
                    foreach (var typD in RefDoc)
                    {
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
                            Validations = user_validation != null ? user_validation.Username : "",
                        });
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
        public ActionResult EtapActuelDocs()
        {
            ViewBag.Controller = "Situation Actuel ded documents";

            return View();
        }
    }
}
