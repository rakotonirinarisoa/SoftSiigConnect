using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using apptab;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using apptab.Data.Entities;
using apptab.Data;
using System.Data.Entity;
using static System.Net.Mime.MediaTypeNames;
using static apptab.Controllers.RSFController;
using System.Web.Helpers;

namespace apptab.Controllers
{
    public class UserController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTGED ged = new SOFTCONNECTGED();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //// GET: User
        //[Route("UserList")]
        //[HttpGet]
        public ActionResult List()
        {
            ViewBag.Controller = "Liste des Utilisateurs";
            return View();
        }

        public class ListeUser
        {
            public string LOGIN { get; set; }
            public string PWD { get; set; }
            public string ROLE { get; set; }
            public string USERGED { get; set; }
            public int ID { get; set; }
            public string PROJET { get; set; }
            public DateTime? DELETONDATE { get; set; }
            public DateTime? CREAT { get; set; }
        }

        [HttpPost]
        public JsonResult FillTable(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            ViewBag.Role = exist.ROLE;
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            List<ListeUser> users = new List<ListeUser>();

            try
            {
                var test = db.SI_USERS.Where(x => x.ROLE == exist.ROLE && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).FirstOrDefault();
                //var test = db.SI_USERS.Where(x => x.ROLE == suser.ROLE && x.IDPROJET == suser.IDPROJET).FirstOrDefault();
                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var adminUser = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur && x.DELETIONDATE == null).ToList();
                    foreach (var a in adminUser)
                    {
                        var tt = ged.Users.FirstOrDefault(z => z.Id == a.IDUSERGED && z.DeletionDate == null);
                        if (a.IDUSERGED != null)
                        {
                            users.Add(new ListeUser()
                            {
                                LOGIN = a.LOGIN,
                                PWD = a.PWD,
                                ROLE = a.ROLE.ToString(),
                                ID = a.ID,
                                PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                                DELETONDATE = a.DELETIONDATE != null ? a.DELETIONDATE : null,
                                CREAT = a.CREATIONDATE != null ? a.CREATIONDATE : null,
                                USERGED = tt != null ? tt.Username : ""
                            });

                        }
                        else
                        {
                            users.Add(new ListeUser()
                            {
                                LOGIN = a.LOGIN,
                                PWD = a.PWD,
                                ROLE = a.ROLE.ToString(),
                                ID = a.ID,
                                PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                                DELETONDATE = a.DELETIONDATE != null ? a.DELETIONDATE : null,
                                CREAT = a.CREATIONDATE != null ? a.CREATIONDATE : null,
                                USERGED = ""
                            });
                        }
                    }
                    //OrderBy(a => a.PROJET).OrderBy(a => a.LOGIN).ToList();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = users }, settings));
                }
                else
                {
                    var testss = db.SI_USERS.Where(x => x.ROLE != Role.SAdministrateur && x.ROLE != Role.Organe_de_Suivi && x.ROLE != Role.Validateur_paiements && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).ToList();
                    foreach (var a in testss)
                    {
                        if (a.IDUSERGED != null)
                        {
                            var tt = ged.Users.Where(z => z.Id == a.IDUSERGED && z.DeletionDate == null).FirstOrDefault();
                            if (tt != null) {
                                users.Add(new ListeUser()
                                {
                                    LOGIN = a.LOGIN,
                                    PWD = a.PWD,
                                    ROLE = a.ROLE.ToString(),
                                    ID = a.ID,
                                    PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                                    DELETONDATE = a.DELETIONDATE != null ? a.DELETIONDATE : null,
                                    //STAT = a.DELETIONDATE == null ? "ACTIF" : "INACTIF",
                                    CREAT = a.CREATIONDATE != null ? a.CREATIONDATE : null,
                                    USERGED = a.IDUSERGED != null ? tt.Username : ""
                                });
                            }
                          
                        }
                        else
                        {
                            users.Add(new ListeUser()
                            {
                                LOGIN = a.LOGIN,
                                PWD = a.PWD,
                                ROLE = a.ROLE.ToString(),
                                ID = a.ID,
                                PROJET = a.IDPROJET == 0 ? "MULTIPLES" : db.SI_PROJETS.Where(z => z.ID == a.IDPROJET && z.DELETIONDATE == null).FirstOrDefault().PROJET,
                                DELETONDATE = a.DELETIONDATE != null ? a.DELETIONDATE : null,
                                //STAT = a.DELETIONDATE == null ? "ACTIF" : "INACTIF",
                                CREAT = a.CREATIONDATE != null ? a.CREATIONDATE : null,
                                USERGED = "",
                            });
                        }
                    }
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = users }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public async Task<JsonResult> Password(UserPassword userPassword)
        {
            var connectedUser = await db.SI_USERS.FirstOrDefaultAsync(
                a => a.LOGIN == userPassword.LoginName && a.PWD == userPassword.Password && a.DELETIONDATE == null && (a.ROLE == Role.SAdministrateur || a.ROLE == Role.Administrateur)
            );

            if (connectedUser == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "" }, settings));
            }

            var res = await db.SI_USERS.FirstOrDefaultAsync(user => user.ID == userPassword.UserId);

            return Json(JsonConvert.SerializeObject(new
            {
                type = "success",
                msg = "Connexion avec succès. ",
                data = new
                {
                    login = res.LOGIN,
                    password = res.PWD
                }
            }, settings));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAllRole(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var enumlist = Enum.GetValues(typeof(Role));

            var roles = new Dictionary<int, string>();

            if (exist.ROLE != Role.SAdministrateur)
            {
                foreach (var item in enumlist)
                {
                    if (item.ToString() != "SAdministrateur" && item.ToString() != "Organe_de_Suivi" && item.ToString() != "Validateur_paiements")
                        roles.Add((int)item, Enum.GetName(typeof(Role), item));
                }
            }
            else
            {
                foreach (var item in enumlist)
                {
                    //roles.Add((int)item, Enum.GetName(typeof(Role), item));
                    if (item.ToString() != "SAdministrateur")
                        roles.Add((int)item, Enum.GetName(typeof(Role), item));
                }
            }
            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = roles }, settings));
        }

        //GET ALL PROJET//
        [HttpPost]
        public ActionResult GetAllPROJET(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            var user = db.SI_PROJETS.Select(a => new
            {
                PROJET = a.PROJET,
                ID = a.ID,
                DELETIONDATE = a.DELETIONDATE,
            }).Where(a => a.DELETIONDATE == null).ToList();

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = user }, settings));
        }

        [HttpPost]
        public JsonResult AddUser(SI_USERS suser, SI_USERS user, string listProjet, Guid? userGED)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var userExist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == user.LOGIN && a.DELETIONDATE == null/* && a.IDPROJET == exist.IDPROJET*/);
                var test = db.SI_USERS.Where(x => x.ROLE == exist.ROLE && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).FirstOrDefault();
                if (userExist == null)
                {
                    if (db.SI_USERS.Any(a => a.LOGIN == user.LOGIN && a.DELETIONDATE == null))
                        return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'utilisateur existe déjà. " }, settings));

                    if (test.ROLE == Role.SAdministrateur)
                    {
                        if (listProjet == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Veuillez sélectionner au moins un projet. " }, settings));

                        if (user.ROLE == Role.Administrateur || user.ROLE == Role.Autre)
                        {
                            int TestProjetRole = 0;
                            if (!int.TryParse(listProjet, out TestProjetRole))
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous ne pouvez pas affecter plusieurs projets à ce type d'utilisateur. " }, settings));

                            var newUser = new SI_USERS()
                            {
                                LOGIN = user.LOGIN,
                                PWD = user.PWD,
                                IDPROJET = int.Parse(listProjet),
                                ROLE = user.ROLE,
                                CREATIONDATE = DateTime.Now,
                                IDUSER = exist.ID,
                                IDUSERGED = userGED
                            };
                            db.SI_USERS.Add(newUser);
                            db.SaveChanges();
                        }
                        else
                        {
                            var newUser = new SI_USERS()
                            {
                                LOGIN = user.LOGIN,
                                PWD = user.PWD,
                                IDPROJET = 0,
                                ROLE = user.ROLE,
                                CREATIONDATE = DateTime.Now,
                                IDUSER = exist.ID,
                                IDUSERGED = userGED
                            };
                            db.SI_USERS.Add(newUser);
                            db.SaveChanges();

                            var userExistTest = db.SI_USERS.FirstOrDefault(a => a.LOGIN == user.LOGIN && a.PWD == user.PWD && a.IDPROJET == 0 && a.ROLE == user.ROLE && a.DELETIONDATE == null/* && a.IDPROJET == exist.IDPROJET*/);

                            string[] separators = { "," };
                            var pro = listProjet;
                            if (pro != null)
                            {
                                string listUser = pro.ToString();
                                string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var a in lst)
                                {
                                    var newRel = new SI_MAPUSERPROJET()
                                    {
                                        IDUS = userExistTest.ID,
                                        IDPROJET = int.Parse(a),
                                        CREATIONDATE = DateTime.Now,
                                        IDUSER = exist.ID
                                    };
                                    db.SI_MAPUSERPROJET.Add(newRel);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        var newUser = new SI_USERS()
                        {
                            LOGIN = user.LOGIN,
                            PWD = user.PWD,
                            IDPROJET = exist.IDPROJET,
                            ROLE = user.ROLE,
                            CREATIONDATE = DateTime.Now,
                            IDUSER = exist.ID,
                            IDUSERGED = userGED
                        };
                        db.SI_USERS.Add(newUser);

                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = user }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'utilisateur existe déjà (pour ce projet ou un autre). " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult UpdateUser(SI_USERS suser, SI_USERS user, string oldPassword, string UserId, string listProjet, Guid? userGED)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int userId = int.Parse(UserId);
                var userExist = db.SI_USERS.FirstOrDefault(a => a.ID == userId && a.DELETIONDATE == null);
                var test = db.SI_USERS.Where(x => x.ROLE == exist.ROLE && x.IDPROJET == exist.IDPROJET && x.DELETIONDATE == null).FirstOrDefault();
                if (userExist != null)
                {
                    if (userExist.PWD != oldPassword) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Ancien mot de passe non valide. ", data = user }, settings));

                    if (test.ROLE == Role.SAdministrateur)
                    {
                        if (user.ROLE == Role.Administrateur || user.ROLE == Role.Autre)
                        {
                            int TestProjetRole = 0;
                            if (!int.TryParse(listProjet, out TestProjetRole))
                                return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous ne pouvez pas affecter plusieurs projets à ce type d'utilisateur. " }, settings));

                            //userExist.LOGIN = user.LOGIN;
                            userExist.PWD = user.PWD;
                            userExist.IDPROJET = int.Parse(listProjet);
                            userExist.ROLE = user.ROLE;
                            userExist.IDUSER = exist.ID;

                            db.SaveChanges();
                        }
                        else
                        {
                            //userExist.LOGIN = user.LOGIN;
                            userExist.PWD = user.PWD;
                            userExist.IDPROJET = 0;
                            userExist.ROLE = user.ROLE;
                            userExist.IDUSER = exist.ID;

                            db.SaveChanges();

                            if (db.SI_MAPUSERPROJET.Any(a => a.IDUS == userId))
                            {
                                foreach (var x in db.SI_MAPUSERPROJET.Where(a => a.IDUS == userId).ToList())
                                {
                                    db.SI_MAPUSERPROJET.Remove(x);
                                    db.SaveChanges();
                                }
                            }

                            string[] separators = { "," };
                            var pro = listProjet;
                            if (pro != null)
                            {
                                string listUser = pro.ToString();
                                string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var a in lst)
                                {
                                    var newRel = new SI_MAPUSERPROJET()
                                    {
                                        IDUS = userId,
                                        IDPROJET = int.Parse(a),
                                        CREATIONDATE = DateTime.Now,
                                        IDUSER = exist.ID
                                    };
                                    db.SI_MAPUSERPROJET.Add(newRel);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        //userExist.LOGIN = user.LOGIN;
                        userExist.PWD = user.PWD;
                        userExist.IDPROJET = exist.IDPROJET;
                        userExist.ROLE = user.ROLE;
                        userExist.IDUSER = exist.ID;

                        db.SaveChanges();
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = user }, settings));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "error", msg = "L'utilisateur existe déjà (pour ce projet ou un autre). " }, settings));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
        public ActionResult Param()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Param(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.ID;
                var crpto = db.SI_USERS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null);
                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto }, settings));
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

        [HttpPost]
        public JsonResult UpdateMDP(SI_USERS suser, SI_USERS user/*, string MDPA*/)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.ID;
                var SExist = db.SI_USERS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null);
                if (SExist != null)
                {
                    if (SExist.PWD != user.LOGIN)
                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Veuillez vérifier votre ancien mot de passe. ", data = user }, settings));

                    SExist.PWD = user.PWD;
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès. ", data = user }, settings));
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
        public ActionResult DetailsUser(string UserId)
        {
            return View();
        }
        [HttpPost]
        public ActionResult DetailsUser(SI_USERS suser, string UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
            SOFTCONNECTGED ged = new SOFTCONNECTGED();

            try
            {
                int useID = int.Parse(UserId);
                var user = db.SI_USERS.FirstOrDefault(a => a.ID == useID && a.DELETIONDATE == null);

                var proj = new List<int>();
                if (user.IDPROJET == 0)
                {
                    if (db.SI_MAPUSERPROJET.Any(a => a.IDUS == useID))
                    {
                        foreach (var x in db.SI_MAPUSERPROJET.Where(a => a.IDUS == useID).ToList())
                        {
                            proj.Add(x.IDPROJET.Value);
                        }
                    }
                }
                else
                {
                    proj.Add(user.IDPROJET.Value);
                }

                if (user != null)
                {
                    var usergename = ged.Users.FirstOrDefault(a => a.Id == user.IDUSERGED && a.DeletionDate == null);
                    if (usergename != null)
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { LOGIN = user.LOGIN, ROLE = user.ROLE, PROJET = proj, USERGEDid = user.IDUSERGED, USERGEDname = usergename.Username } }, settings));
                    }
                    else
                    {
                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { LOGIN = user.LOGIN, ROLE = user.ROLE, PROJET = proj, USERGEDid = user.IDUSERGED, USERGEDname = "" } }, settings));
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
        public ActionResult Login()
        {
            db.SI_USERS.Any();
            return View();
        }

        [HttpPost]
        public ActionResult Login(SI_USERS Users)
        {
            try
            {
                var test = db.SI_USERS.FirstOrDefault(x => x.LOGIN == Users.LOGIN && x.PWD == Users.PWD && x.DELETIONDATE == null);
                if (test == null) return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vérifiez vos identifiants. " }, settings));

                Session["PROCESDEPS"] = 2;//1 NON et 2 APPLICABLE
                Session["PROCESPAIE"] = 2;//1 NON et 2 APPLICABLE

                if (test.ROLE != Role.SAdministrateur && test.ROLE != Role.Organe_de_Suivi && test.ROLE != Role.Validateur_paiements)
                {
                    if (test.IDPROJET != 0)
                    {
                        if (db.SI_TYPEPROCESSUS.Any(a => a.DELETIONDATE == null && a.IDPROJET == test.IDPROJET))
                        {
                            var isProcess = db.SI_TYPEPROCESSUS.FirstOrDefault(a => a.DELETIONDATE == null && a.IDPROJET == test.IDPROJET);

                            Session["PROCESDEPS"] = isProcess.VALDEPENSES;//1 NON et 2 APPLICABLE
                            Session["PROCESPAIE"] = isProcess.VALPAIEMENTS;//1 NON et 2 APPLICABLE
                        }

                        if (String.IsNullOrEmpty(test.IDPROJET.ToString()) || !db.SI_PROJETS.Any(a => a.ID == test.IDPROJET && a.DELETIONDATE == null))
                        {
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Vous n'êtes pas rattaché à un projet actif. " }, settings));
                        }
                    }
                }

                //PCOP//
                Session["PCOP"] = "PCOP";
                if (db.SI_TYPEPROCESSUS.Any(a => a.IDPROJET == test.IDPROJET))
                    Session["PCOP"] = db.SI_TYPEPROCESSUS.FirstOrDefault(a => a.IDPROJET == test.IDPROJET && a.DELETIONDATE == null).INTITULE;

                //MENU//
                if (db.SI_MENU.Any())
                {
                    var isMenu = db.SI_MENU.FirstOrDefault();
                    Session["MTNON"] = isMenu.MTNON;
                    Session["MT0"] = isMenu.MT0;
                    Session["MT1"] = isMenu.MT1;
                    Session["MT2"] = isMenu.MT2;
                    Session["MP1"] = isMenu.MP1;
                    Session["MP2"] = isMenu.MP2;
                    Session["MP3"] = isMenu.MP3;
                    Session["MP4"] = isMenu.MP4;

                    Session["MD0"] = isMenu.MD0;
                    Session["MD1"] = isMenu.MD1;
                    Session["MD2"] = isMenu.MD2;
                    Session["MD3"] = isMenu.MD3;

                    Session["MOP0"] = isMenu.MOP0;
                    Session["MOP1"] = isMenu.MOP1;
                    Session["MOP2"] = isMenu.MOP2;

                    Session["TDB0"] = isMenu.TDB0;
                    Session["TDB1"] = isMenu.TDB1;
                    Session["TDB2"] = isMenu.TDB2;
                    Session["TDB3"] = isMenu.TDB3;
                    Session["TDB4"] = isMenu.TDB4;
                    Session["TDB5"] = isMenu.TDB5;
                    Session["TDB6"] = isMenu.TDB6;
                    Session["TDB7"] = isMenu.TDB7;
                    Session["TDB8"] = isMenu.TDB8;

                    Session["J0"] = isMenu.J0;
                    Session["J1"] = isMenu.J1;
                    Session["J2"] = isMenu.J2;
                    Session["J3"] = isMenu.J3;

                    Session["JR"] = isMenu.JR;
                    Session["JRA"] = isMenu.JRA;

                    Session["RSF"] = isMenu.RSF;
                    Session["RSFT"] = isMenu.RSFT;
                    Session["TDB9"] = isMenu.TDB9;

                    Session["TDB11"] = isMenu.TDB11;
                    Session["TDB12"] = isMenu.TDB12;
                    Session["TDB13"] = isMenu.TDB13;
                    Session["TDB14"] = isMenu.TDB14;

                    Session["TDB9i"] = isMenu.TDB9i == true ? 1 : 0;
                    Session["TDB0i"] = isMenu.TDB0i == true ? 1 : 0;
                    Session["JRi"] = isMenu.JRi == true ? 1 : 0;
                    Session["JRAi"] = isMenu.JRAi == true ? 1 : 0;
                    Session["TDB1i"] = isMenu.TDB1i == true ? 1 : 0;
                    Session["TDB2i"] = isMenu.TDB2i == true ? 1 : 0;
                    Session["TDB3i"] = isMenu.TDB3i == true ? 1 : 0;
                    Session["TDB4i"] = isMenu.TDB4i == true ? 1 : 0;
                    Session["TDB5i"] = isMenu.TDB5i == true ? 1 : 0;
                    Session["TDB6i"] = isMenu.TDB6i == true ? 1 : 0;
                    Session["TDB7i"] = isMenu.TDB7i == true ? 1 : 0;
                    Session["TDB8i"] = isMenu.TDB8i == true ? 1 : 0;
                    Session["TDB11i"] = isMenu.TDB11i == true ? 1 : 0;
                    Session["TDB12i"] = isMenu.TDB12i == true ? 1 : 0;
                    Session["TDB13i"] = isMenu.TDB13i == true ? 1 : 0;
                    Session["TDB14i"] = isMenu.TDB14i == true ? 1 : 0;
                }

                //PRIVILEGES//
                Session["RMENUPAR1"] = 0;
                Session["RMENUPAR2"] = 0;
                Session["RMENUPAR3"] = 0;
                Session["RMENUPAR4"] = 0;
                Session["RMENUPAR5"] = 0;
                Session["RMENUPAR6"] = 0;
                Session["RMENUPAR7"] = 0;
                Session["RMENUPAR8"] = 0;
                Session["RMENUPAR9"] = 0;
                Session["RMENUPAR10"] = 0;

                Session["RMTNON"] = 0;
                Session["RMT0"] = 0;
                Session["RMT1"] = 0;
                //Session["RMT2"] = 0;
                Session["RMP1"] = 0;
                Session["RMP2"] = 0;
                Session["RMP3"] = 0;
                Session["RMP4"] = 0;

                Session["RMD0"] = 0;
                Session["RMD1"] = 0;
                Session["RMD2"] = 0;
                //Session["RMD3"] = 0;

                Session["RMOP0"] = 0;
                Session["RMOP1"] = 0;
                Session["RMOP2"] = 0;

                Session["RTDB0"] = 0;
                Session["RTDB1"] = 0;
                Session["RTDB2"] = 0;
                Session["RTDB3"] = 0;
                Session["RTDB4"] = 0;
                Session["RTDB5"] = 0;
                Session["RTDB6"] = 0;
                Session["RTDB7"] = 0;
                Session["RTDB8"] = 0;

                Session["RJ0"] = 0;
                Session["RJ1"] = 0;
                Session["RJ2"] = 0;
                Session["RJ3"] = 0;

                Session["RJR"] = 0;
                Session["RJRA"] = 0;

                Session["RGED"] = 0;

                Session["RRSF"] = 0;
                Session["RRSFT"] = 0;
                Session["RTDB9"] = 0;

                Session["RTDB11"] = 0;
                Session["RTDB12"] = 0;
                Session["RTDB13"] = 0;
                Session["RTDB14"] = 0;

                if (db.SI_PRIVILEGE.Any(a => a.IDUSERPRIV == test.ID))
                {
                    var isMenu = db.SI_PRIVILEGE.FirstOrDefault(a => a.IDUSERPRIV == test.ID);

                    Session["RMENUPAR1"] = isMenu.MENUPAR1;
                    Session["RMENUPAR2"] = isMenu.MENUPAR2;
                    Session["RMENUPAR3"] = isMenu.MENUPAR3;
                    Session["RMENUPAR4"] = isMenu.MENUPAR4;
                    Session["RMENUPAR5"] = isMenu.MENUPAR5;
                    Session["RMENUPAR6"] = isMenu.MENUPAR6;
                    Session["RMENUPAR7"] = isMenu.MENUPAR7;
                    Session["RMENUPAR8"] = isMenu.MENUPAR8;
                    Session["RMENUPAR9"] = isMenu.MENUPAR9;
                    Session["RMENUPAR10"] = isMenu.MENUPAR10;

                    Session["RMTNON"] = isMenu.MTNON;
                    Session["RMT0"] = isMenu.MT0;
                    Session["RMT1"] = isMenu.MT1;
                    //Session["RMT2"] = isMenu.MT2;
                    Session["RMP1"] = isMenu.MP1;
                    Session["RMP2"] = isMenu.MP2;
                    Session["RMP3"] = isMenu.MP3;
                    Session["RMP4"] = isMenu.MP4;

                    Session["RMD0"] = isMenu.MD0;
                    Session["RMD1"] = isMenu.MD1;
                    Session["RMD2"] = isMenu.MD2;
                    //Session["RMD3"] = isMenu.MD3;

                    Session["RMOP0"] = isMenu.MOP0;
                    Session["RMOP1"] = isMenu.MOP1;
                    Session["RMOP2"] = isMenu.MOP2;

                    Session["RTDB0"] = isMenu.TDB0;
                    Session["RTDB1"] = isMenu.TDB1;
                    Session["RTDB2"] = isMenu.TDB2;
                    Session["RTDB3"] = isMenu.TDB3;
                    Session["RTDB4"] = isMenu.TDB4;
                    Session["RTDB5"] = isMenu.TDB5;
                    Session["RTDB6"] = isMenu.TDB6;
                    Session["RTDB7"] = isMenu.TDB7;
                    Session["RTDB8"] = isMenu.TDB8;

                    Session["RJ0"] = isMenu.J0;
                    Session["RJ1"] = isMenu.J1;
                    Session["RJ2"] = isMenu.J2;
                    Session["RJ3"] = isMenu.J3;

                    Session["RJR"] = isMenu.JR;
                    Session["RJRA"] = isMenu.JRA;

                    Session["RGED"] = isMenu.GED;

                    Session["RRSF"] = isMenu.RSF;
                    Session["RRSFT"] = isMenu.RSFT;
                    Session["RTDB9"] = isMenu.TDB9;

                    Session["RTDB11"] = isMenu.TDB11;
                    Session["RTDB12"] = isMenu.TDB12;
                    Session["RTDB13"] = isMenu.TDB13;
                    Session["RTDB14"] = isMenu.TDB14;
                }

                if (db.SI_GEDLIEN.Any())
                {
                    var isMenu = db.SI_GEDLIEN.FirstOrDefault();
                    Session["GED"] = isMenu.LIEN;
                }

                foreach (var x in db.SI_USERSHISTO.Where(a => a.IDUSER == test.ID).ToList())
                {
                    var isusr = db.SI_USERSHISTO.Where(a => a.ID == x.ID).FirstOrDefault();
                    isusr.DISCONNEX = DateTime.Now;
                    db.SaveChanges();
                }

                test.LASTCONNEXTION = DateTime.Now;
                db.SaveChanges();

                var elem = new SI_USERSHISTO()
                {
                    IDUSER = test.ID,
                    IDPROJET = test.IDPROJET,
                    CONNEX = DateTime.Now
                };
                db.SI_USERSHISTO.Add(elem);
                db.SaveChanges();

                Session["userSession"] = test;
                Session["UserName"] = test.LOGIN;

                Session["VERSIONCONNNECT"] = "1.0.0";
                Session["VERSION"] = "1.30.0";

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", Data = new { test.ROLE, test.IDPROJET } }, settings));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult DeleteUser(SI_USERS suser, string UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int useID = int.Parse(UserId);
                var user = db.SI_USERS.FirstOrDefault(a => a.ID == useID && a.DELETIONDATE == null);
                if (user != null)
                {
                    user.DELETIONDATE = DateTime.Now;
                    db.SaveChanges();
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Suppression avec succès. " }, settings));
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

        [HttpPost]
        public JsonResult GetUR(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            return Json(JsonConvert.SerializeObject(new { type = "login", msg = "", data = exist.ROLE != (int)Role.SAdministrateur }, settings));
        }

        public class ListeUserGEd
        {
            public string Username { get; set; }
            public Guid Id { get; set; }
        }

        [HttpPost]
        public ActionResult GETALLUSER(SI_USERS suser, string iProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            List<ListeUserGEd> crpto = new List<ListeUserGEd>();

            try
            {
                SOFTCONNECTGED.connex = new Data.Extension().GetConGED();
                SOFTCONNECTGED ged = new SOFTCONNECTGED();

                if (!String.IsNullOrEmpty(iProjet))
                {
                    string[] separators = { "," };
                    var pro = iProjet;
                    if (pro != null)
                    {
                        string listUser = pro.ToString();
                        string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var a in lst)
                        {
                            var idP = int.Parse(a);
                            var projet = db.SI_PROGED.FirstOrDefault(b => b.IDPROJET == idP && b.DELETIONDATE == null);

                            if (projet != null)
                            {
                                var projetGED = ged.Projects.FirstOrDefault(b => b.Id == projet.IDGED && b.DeletionDate == null);

                                if (projetGED != null)
                                {
                                    foreach (var y in ged.Users.Where(b => b.ProjectId == projetGED.Id && b.DeletionDate == null).ToList())
                                    {
                                        if (!db.SI_USERS.Any(z => z.IDUSERGED == y.Id && z.DELETIONDATE == null))
                                        {
                                            crpto.Add(new ListeUserGEd()
                                            {
                                                Username = y.Username,
                                                Id = y.Id
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
                    var idP = exist.IDPROJET;

                    var projet = db.SI_PROGED.FirstOrDefault(b => b.IDPROJET == idP && b.DELETIONDATE == null);

                    if (projet != null)
                    {
                        var projetGED = ged.Projects.FirstOrDefault(b => b.Id == projet.IDGED && b.DeletionDate == null);

                        if (projetGED != null)
                        {
                            foreach (var y in ged.Users.Where(b => b.ProjectId == projetGED.Id && b.DeletionDate == null).ToList())
                            {
                                if (!db.SI_USERS.Any(z => z.IDUSERGED == y.Id && z.DELETIONDATE == null))
                                {
                                    crpto.Add(new ListeUserGEd()
                                    {
                                        Username = y.Username,
                                        Id = y.Id
                                    });
                                }
                            }
                        }
                    }
                }

                if (crpto != null)
                {
                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = crpto, settings }));
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { type = "notYet", msg = "Veuillez créer des utilisateurs. ", data = crpto, settings }));
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
