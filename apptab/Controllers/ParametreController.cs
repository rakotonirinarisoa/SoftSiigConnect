using apptab.Data;
using apptab;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace apptab.Controllers
{
    public class ParametreController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //Financement//
        public ActionResult FinanCreate()
        {
            ViewBag.Controller = "Paramétrage Financement";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsFinan(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_FINANCEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer un nouveau financement. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateFinan(SI_USERS suser, SI_FINANCEMENT param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_FINANCEMENT.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.FINANCEMENT != param.FINANCEMENT)
                    {
                        SExist.FINANCEMENT = param.FINANCEMENT;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_FINANCEMENT.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_FINANCEMENT()
                        {
                            FINANCEMENT = param.FINANCEMENT,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_FINANCEMENT.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_FINANCEMENT()
                    {
                        FINANCEMENT = param.FINANCEMENT,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_FINANCEMENT.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_FINANCEMENT.FirstOrDefault(a => a.IDPROJET == IdS && a.FINANCEMENT == param.FINANCEMENT && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_FINANCEMENT()
                    {
                        FINANCEMENT = isElemH.FINANCEMENT,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_FINANCEMENT.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Convention//
        public ActionResult ConvCreate()
        {
            ViewBag.Controller = "Paramétrage Convention";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsConv(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_CONVENTION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle convention. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateConv(SI_USERS suser, SI_CONVENTION param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_CONVENTION.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.CONVENTION != param.CONVENTION)
                    {
                        SExist.CONVENTION = param.CONVENTION;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_CONVENTION.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_CONVENTION()
                        {
                            CONVENTION = param.CONVENTION,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_CONVENTION.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_CONVENTION()
                    {
                        CONVENTION = param.CONVENTION,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_CONVENTION.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_CONVENTION.FirstOrDefault(a => a.IDPROJET == IdS && a.CONVENTION == param.CONVENTION && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_CONVENTION()
                    {
                        CONVENTION = isElemH.CONVENTION,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_CONVENTION.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Categorie//
        public ActionResult CatCreate()
        {
            ViewBag.Controller = "Paramétrage Catégorie";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsCat(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_CATEGORIE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle catégorie. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateCat(SI_USERS suser, SI_CATEGORIE param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_CATEGORIE.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.CATEGORIE != param.CATEGORIE)
                    {
                        SExist.CATEGORIE = param.CATEGORIE;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_CATEGORIE.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_CATEGORIE()
                        {
                            CATEGORIE = param.CATEGORIE,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_CATEGORIE.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_CATEGORIE()
                    {
                        CATEGORIE = param.CATEGORIE,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_CATEGORIE.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_CATEGORIE.FirstOrDefault(a => a.IDPROJET == IdS && a.CATEGORIE == param.CATEGORIE && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_CATEGORIE()
                    {
                        CATEGORIE = isElemH.CATEGORIE,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_CATEGORIE.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Engagement//
        public ActionResult EngaCreate()
        {
            ViewBag.Controller = "Paramétrage Engagement";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsEnga(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_ENGAGEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer un nouvel engagement. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateEnga(SI_USERS suser, SI_ENGAGEMENT param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_ENGAGEMENT.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.ENGAGEMENT != param.ENGAGEMENT)
                    {
                        SExist.ENGAGEMENT = param.ENGAGEMENT;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_ENGAGEMENT.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_ENGAGEMENT()
                        {
                            ENGAGEMENT = param.ENGAGEMENT,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_ENGAGEMENT.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_ENGAGEMENT()
                    {
                        ENGAGEMENT = param.ENGAGEMENT,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_ENGAGEMENT.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_ENGAGEMENT.FirstOrDefault(a => a.IDPROJET == IdS && a.ENGAGEMENT == param.ENGAGEMENT && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_ENGAGEMENT()
                    {
                        ENGAGEMENT = isElemH.ENGAGEMENT,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_ENGAGEMENT.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Procédure//
        public ActionResult ProcCreate()
        {
            ViewBag.Controller = "Paramétrage Procédure";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsProc(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_PROCEDURE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer un nouveau procédure. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateProc(SI_USERS suser, SI_PROCEDURE param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_PROCEDURE.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.PROCEDURE != param.PROCEDURE || SExist.CODEDEG != param.CODEDEG || SExist.PROCEDUREDEG != param.PROCEDUREDEG)
                    {
                        SExist.PROCEDURE = param.PROCEDURE;
                        SExist.CODE = param.CODE;
                        SExist.PROCEDUREDEG = param.PROCEDUREDEG;
                        SExist.CODEDEG = param.CODEDEG;
                        db.SaveChanges();

                        var H = db.HSI_PROCEDURE.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_PROCEDURE()
                        {
                            PROCEDURE = param.PROCEDURE,
                            CODE = param.CODE,
                            PROCEDUREDEG = param.PROCEDUREDEG,
                            CODEDEG = param.CODEDEG,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_PROCEDURE.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_PROCEDURE()
                    {
                        PROCEDURE = param.PROCEDURE,
                        CODE = param.CODE,
                        PROCEDUREDEG = param.PROCEDUREDEG,
                        CODEDEG = param.CODEDEG,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_PROCEDURE.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_PROCEDURE.FirstOrDefault(a => a.IDPROJET == IdS && a.PROCEDURE == param.PROCEDURE && a.CODE == param.CODE && a.PROCEDUREDEG == param.PROCEDUREDEG && a.CODEDEG == param.CODEDEG && a.DELETIONDATE == null);
                    var newElemH = new HSI_PROCEDURE()
                    {
                        PROCEDURE = isElemH.PROCEDURE,
                        CODE = isElemH.CODE,
                        PROCEDUREDEG = param.PROCEDUREDEG,
                        CODEDEG = param.CODEDEG,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_PROCEDURE.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Ministère//
        public ActionResult MinCreate()
        {
            ViewBag.Controller = "Paramétrage Ministère";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsMin(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_MINISTERE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer un nouveau ministère. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateMin(SI_USERS suser, SI_MINISTERE param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_MINISTERE.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.MINISTERE != param.MINISTERE)
                    {
                        SExist.MINISTERE = param.MINISTERE;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_MINISTERE.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_MINISTERE()
                        {
                            MINISTERE = param.MINISTERE,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_MINISTERE.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_MINISTERE()
                    {
                        MINISTERE = param.MINISTERE,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_MINISTERE.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_MINISTERE.FirstOrDefault(a => a.IDPROJET == IdS && a.MINISTERE == param.MINISTERE && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_MINISTERE()
                    {
                        MINISTERE = isElemH.MINISTERE,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_MINISTERE.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Mission//
        public ActionResult MisCreate()
        {
            ViewBag.Controller = "Paramétrage Mission";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsMis(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_MISSION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle mission. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateMis(SI_USERS suser, SI_MISSION param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_MISSION.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.MISSION != param.MISSION)
                    {
                        SExist.MISSION = param.MISSION;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_MISSION.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_MISSION()
                        {
                            MISSION = param.MISSION,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_MISSION.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_MISSION()
                    {
                        MISSION = param.MISSION,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_MISSION.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_MISSION.FirstOrDefault(a => a.IDPROJET == IdS && a.MISSION == param.MISSION && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_MISSION()
                    {
                        MISSION = isElemH.MISSION,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_MISSION.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Programme//
        public ActionResult ProgCreate()
        {
            ViewBag.Controller = "Paramétrage Programme";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsProg(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_PROGRAMME.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer un nouveau programme. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateProg(SI_USERS suser, SI_PROGRAMME param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_PROGRAMME.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.PROGRAMME != param.PROGRAMME)
                    {
                        SExist.PROGRAMME = param.PROGRAMME;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_PROGRAMME.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_PROGRAMME()
                        {
                            PROGRAMME = param.PROGRAMME,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_PROGRAMME.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_PROGRAMME()
                    {
                        PROGRAMME = param.PROGRAMME,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_PROGRAMME.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_PROGRAMME.FirstOrDefault(a => a.IDPROJET == IdS && a.PROGRAMME == param.PROGRAMME && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_PROGRAMME()
                    {
                        PROGRAMME = isElemH.PROGRAMME,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_PROGRAMME.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Activité//
        public ActionResult ActCreate()
        {
            ViewBag.Controller = "Paramétrage Activité";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsAct(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_ACTIVITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle activité. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateAct(SI_USERS suser, SI_ACTIVITE param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_ACTIVITE.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.CODE != param.CODE || SExist.ACTIVITE != param.ACTIVITE)
                    {
                        SExist.ACTIVITE = param.ACTIVITE;
                        SExist.CODE = param.CODE;
                        db.SaveChanges();

                        var H = db.HSI_ACTIVITE.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_ACTIVITE()
                        {
                            ACTIVITE = param.ACTIVITE,
                            CODE = param.CODE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_ACTIVITE.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_ACTIVITE()
                    {
                        ACTIVITE = param.ACTIVITE,
                        CODE = param.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_ACTIVITE.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_ACTIVITE.FirstOrDefault(a => a.IDPROJET == IdS && a.ACTIVITE == param.ACTIVITE && a.CODE == param.CODE && a.DELETIONDATE == null);
                    var newElemH = new HSI_ACTIVITE()
                    {
                        ACTIVITE = isElemH.ACTIVITE,
                        CODE = isElemH.CODE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_ACTIVITE.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //Correspondance ETAT//
        public ActionResult CorrEtatCreate()
        {
            ViewBag.Controller = "Correspondance des états";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsCorrEtat(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer une nouvelle correspondance des états. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateCorrEtat(SI_USERS suser, SI_PARAMETAT param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            int IdS = exist.IDPROJET.Value;

            if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == IdS) == null)
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(IdS);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == param.DEF) == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'état DEF n'est pas présent sur TOM²PRO. " }, settings));
            if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == param.TEF) == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'état TEF n'est pas présent sur TOM²PRO. " }, settings));
            if (tom.CPTADMIN_CHAINETRAITEMENT.FirstOrDefault(a => a.NUM == param.BE) == null)
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'état BE n'est pas présent sur TOM²PRO. " }, settings));

            try
            {
                var SExist = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == IdS && a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.DEF != param.DEF || SExist.TEF != param.TEF || SExist.BE != param.BE)
                    {
                        SExist.DEF = param.DEF;
                        SExist.TEF = param.TEF;
                        SExist.BE = param.BE;

                        db.SaveChanges();

                        var H = db.HSI_PARAMETAT.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_PARAMETAT()
                        {
                            DEF = param.DEF,
                            TEF = param.TEF,
                            BE = param.BE,
                            IDPROJET = IdS,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_PARAMETAT.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_PARAMETAT()
                    {
                        DEF = param.DEF,
                        TEF = param.TEF,
                        BE = param.BE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_PARAMETAT.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_PARAMETAT.FirstOrDefault(a => a.IDPROJET == IdS && a.DEF == param.DEF && a.TEF == param.TEF && a.BE == param.BE && a.DELETIONDATE == null);
                    var newElemH = new HSI_PARAMETAT()
                    {
                        DEF = isElemH.DEF,
                        TEF = isElemH.TEF,
                        BE = isElemH.BE,
                        IDPROJET = IdS,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_PARAMETAT.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement des informations. " }, settings));
            }
        }

        //MOTIF//
        public ActionResult MotifCreate()
        {
            ViewBag.Controller = "Paramétrage Motifs de rejet";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsMotif(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_MOTIF.FirstOrDefault(a => a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez créer les nouvelles listes des motifs de rejet. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateMotif(SI_USERS suser, SI_MOTIF param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_MOTIF.FirstOrDefault(a => a.DELETIONDATE == null);

                if (SExist != null)
                {
                    if (SExist.MOTIFTRAIT != param.MOTIFTRAIT || SExist.MOTIFPAI != param.MOTIFPAI)
                    {
                        SExist.MOTIFTRAIT = param.MOTIFTRAIT;
                        SExist.MOTIFPAI = param.MOTIFPAI;
                        db.SaveChanges();

                        var H = db.HSI_MOTIF.FirstOrDefault(a => a.IDPARENT == SExist.ID && a.DELETIONDATE == null);
                        if (H != null)
                        {
                            H.DELETIONDATE = DateTime.Now;
                            db.SaveChanges();
                        }

                        var newElemH = new HSI_MOTIF()
                        {
                            MOTIFTRAIT = param.MOTIFTRAIT,
                            MOTIFPAI = param.MOTIFPAI,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDPARENT = SExist.ID
                        };
                        db.HSI_MOTIF.Add(newElemH);
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_MOTIF()
                    {
                        MOTIFTRAIT = param.MOTIFTRAIT,
                        MOTIFPAI = param.MOTIFPAI,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_MOTIF.Add(newPara);
                    db.SaveChanges();

                    var isElemH = db.SI_MOTIF.FirstOrDefault(a => a.MOTIFTRAIT == param.MOTIFTRAIT && a.MOTIFPAI == param.MOTIFPAI && a.DELETIONDATE == null);
                    var newElemH = new HSI_MOTIF()
                    {
                        MOTIFTRAIT = isElemH.MOTIFTRAIT,
                        MOTIFPAI = isElemH.MOTIFPAI,
                        CREATIONDATE = isElemH.CREATIONDATE,
                        IDUSER = isElemH.IDUSER,
                        IDPARENT = isElemH.ID
                    };
                    db.HSI_MOTIF.Add(newElemH);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }

        //TYPE ECRITURE//
        public ActionResult TypeECreate()
        {
            ViewBag.Controller = "Paramétrage type d'écriture";

            return View();
        }

        [HttpPost]
        public ActionResult DetailsTypeE(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;
                var crpto = db.SI_TYPECRITURE.FirstOrDefault(a => a.DELETIONDATE == null && a.IDPROJET == crpt);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Veuillez paramétrer le type d'écriture. " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateTypeE(SI_USERS suser, SI_TYPECRITURE param)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int IdS = exist.IDPROJET.Value;
                var SExist = db.SI_TYPECRITURE.FirstOrDefault(a => a.DELETIONDATE == null && a.IDPROJET == IdS);

                if (SExist != null)
                {
                    if (SExist.TYPE != param.TYPE)
                    {
                        SExist.TYPE = param.TYPE;
                        SExist.IDUSER = exist.ID;
                        SExist.CREATIONDATE = DateTime.Now;
                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
                else
                {
                    var newPara = new SI_TYPECRITURE()
                    {
                        TYPE = param.TYPE,
                        IDPROJET = IdS,
                        CREATIONDATE = DateTime.Now,
                        IDUSER = exist.ID
                    };

                    db.SI_TYPECRITURE.Add(newPara);
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = param }, settings));
                }
            }
            catch (Exception)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Erreur d'enregistrement de l'information. " }, settings));
            }
        }
    }
}
