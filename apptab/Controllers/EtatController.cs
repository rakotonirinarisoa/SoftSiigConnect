using apptab.Data.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace apptab.Controllers
{
    public class EtatController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //Financement//
        public ActionResult InfoProCreate()
        {
            ViewBag.Controller = "Informations PROJET";

            return View();
        }

        [HttpPost]
        //public async DetailsInfoPro(SI_USERS suser)
        public async Task<ActionResult> DetailsInfoPro(SI_USERS suser, int iProjet)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                //int crpt = exist.IDPROJET.Value;
                int crpt = iProjet;

                var fina = "";
                if (db.SI_FINANCEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    fina = db.SI_FINANCEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).FINANCEMENT;
                }
                var convention = "";
                if (db.SI_CONVENTION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    convention = db.SI_CONVENTION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).CONVENTION;
                }
                var catego = "";
                if (db.SI_CATEGORIE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    catego = db.SI_CATEGORIE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).CATEGORIE;
                }
                var enga = "";
                if (db.SI_ENGAGEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    enga = db.SI_ENGAGEMENT.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).ENGAGEMENT;
                }
                var proc = "";
                if (db.SI_PROCEDURE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    proc = db.SI_PROCEDURE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).PROCEDURE;
                }
                var min = "";
                if (db.SI_MINISTERE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    min = db.SI_MINISTERE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MINISTERE;
                }
                var mis = "";
                if (db.SI_MISSION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    mis = db.SI_MISSION.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).MISSION;
                }
                var prog = "";
                if (db.SI_PROGRAMME.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    prog = db.SI_PROGRAMME.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).PROGRAMME;
                }
                var act = "";
                if (db.SI_ACTIVITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) != null)
                {
                    act = db.SI_ACTIVITE.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null).ACTIVITE;
                }
                int proj = 0;
                if (db.SI_PROJETS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null) != null)
                {
                    proj = db.SI_PROJETS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null).ID;
                }
                var soaA = "";
                if ((from soa in db.SI_SOAS
                     join pro in db.SI_PROSOA on soa.ID equals pro.IDSOA
                     where pro.IDPROJET == crpt && pro.DELETIONDATE == null && soa.DELETIONDATE == null
                     select soa.SOA).FirstOrDefault() != null)
                {
                    soaA = (from soa in db.SI_SOAS
                            join pro in db.SI_PROSOA on soa.ID equals pro.IDSOA
                            where pro.IDPROJET == crpt && pro.DELETIONDATE == null && soa.DELETIONDATE == null
                            select soa.SOA).FirstOrDefault();
                }

                return Json(JsonConvert.SerializeObject(new
                {
                    type = "success",
                    msg = "message",
                    data = new
                    {
                        FIN = fina,
                        CONV = convention,
                        CAT = catego,
                        ENG = enga,
                        PROC = proc,
                        MIN = min,
                        MIS = mis,
                        PROG = prog,
                        ACT = act,
                        PROJ = proj,
                        SOA = soaA
                    }
                }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public ActionResult DetailsInfoProMANDAT(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = suser.IDPROJET.Value;

                int proj = 0;
                if (db.SI_PROJETS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null) != null)
                {
                    proj = db.SI_PROJETS.FirstOrDefault(a => a.ID == crpt && a.DELETIONDATE == null).ID;
                }

                if (proj != 0)
                {
                    return Json(JsonConvert.SerializeObject(new
                    {
                        type = "success",
                        msg = "message",
                        data = new
                        {
                            PROJ = proj
                        }
                    }, settings));
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

        //GET ALL PROJET//
        [HttpPost]
        public ActionResult GetAllPROJET(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();
                var proj = new List<int>();

                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    var user = db.SI_PROJETS.Select(a => new
                    {
                        PROJET = a.PROJET,
                        ID = a.ID,
                        DELETIONDATE = a.DELETIONDATE,
                    }).Where(a => a.DELETIONDATE == null).ToList();

                    foreach (var x in user)
                    {
                        proj.Add(x.ID);
                    }

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
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

                        foreach (var x in user)
                        {
                            proj.Add(x.ID);
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
                    }
                    else
                    {
                        var user = (from usr in db.SI_PROJETS
                                    join prj in db.SI_MAPUSERPROJET on usr.ID equals prj.IDPROJET
                                    where prj.IDUS == test.ID && usr.DELETIONDATE == null
                                    select new SI_PROJETS
                                    {
                                        PROJET = usr.PROJET,
                                        ID = usr.ID,
                                        DELETIONDATE = usr.DELETIONDATE,
                                    }).ToList();

                        foreach (var x in user)
                        {
                            proj.Add(x.ID);
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = new { List = user, PROJET = proj } }, settings));
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        //ETAT MANDAT PROJET//
        public ActionResult EtatMandatP()
        {
            ViewBag.Controller = "Etat MANDATS";

            return View();
        }

        private async Task<List<DATATRPROJET>> GetM(Guid idLiquidation, int crpt)
        {
            var res = new List<DATATRPROJET>();

            SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

            SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
            SOFTCONNECTOM tom = new SOFTCONNECTOM();

            var ms = await tom.CPTADMIN_MLIQUIDATION.Where(a => a.IDLIQUIDATION == idLiquidation).ToListAsync();

            for (int i = 0; i < ms.Count; i += 1)
            {
                res.Add(new DATATRPROJET
                {
                    REF = ms[i].ID.ToString(),
                    OBJ = ms[i].LIBELLE,
                    TITUL = ms[i].LIBELLE,
                    MONT = Math.Round((double)ms[i].MONTANTLOCAL, 2).ToString(),
                    COMPTE = ms[i].COGE,
                    PCOP = ms[i].POSTE
                });
            }

            return res;
        }

        [HttpPost]
        public async Task<JsonResult> EtatMandatProjet(SI_USERS suser)
        {
            var exist = await db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = exist.IDPROJET.Value;

                var db = new SOFTCONNECTSIIG();
                var connex = new Data.Extension().GetCon(crpt);

                var list = new List<DATATRPROJET>();

                if (await db.SI_TRAITPROJET.FirstOrDefaultAsync(a => a.IDPROJET == crpt) != null)
                {
                    foreach (var x in await db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt).ToListAsync())
                    {
                        var sta = "Attente validation";
                        if (x.ETAT == 1)
                            sta = "Validée";
                        else if (x.ETAT == 2)
                            sta = "Annulée";
                        else if (x.ETAT == 3)
                            sta = "Traitée SIIGFP";

                        list.Add(new X
                        {
                            No = x.No,
                            REF = x.REF,
                            OBJ = x.OBJ,
                            TITUL = x.TITUL,
                            MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                            COMPTE = x.COMPTE,
                            DATE = x.DATEMANDAT.Value.Date,
                            PCOP = x.PCOP,
                            DATEDEF = x.DATEDEF.Value.Date,
                            DATETEF = x.DATETEF.Value.Date,
                            DATEBE = x.DATEBE.Value.Date,
                            STAT = sta,
                            M = await GetM((Guid)x.No, crpt)
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult EtatMandatChange(SI_USERS suser, int IdPROJET)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = IdPROJET;

                SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.ETAT == 0) != null)
                {
                    foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.ETAT == 0).ToList())
                    {
                        var sta = "Attente validation";
                        if (x.ETAT == 1)
                            sta = "Validée";
                        else if (x.ETAT == 2)
                            sta = "Annulée";
                        else if (x.ETAT == 3)
                            sta = "Traitée SIIGFP";

                        list.Add(new DATATRPROJET
                        {
                            No = x.No,
                            REF = x.REF,
                            OBJ = x.OBJ,
                            TITUL = x.TITUL,
                            MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                            COMPTE = x.COMPTE,
                            DATE = x.DATEMANDAT.Value.Date,
                            PCOP = x.PCOP,
                            DATEDEF = x.DATEDEF.Value.Date,
                            DATETEF = x.DATETEF.Value.Date,
                            DATEBE = x.DATEBE.Value.Date,
                            STAT = sta
                        });
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult EtatMandatProjetSEARCH(SI_USERS suser, DateTime DateDebut, DateTime DateFin, int STAT)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = 0;
                if (suser.IDPROJET == null)
                    crpt = exist.IDPROJET.Value;
                else
                    crpt = suser.IDPROJET.Value;

                SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
                SOFTCONNECTOM.connex = new Data.Extension().GetCon(crpt);
                SOFTCONNECTOM tom = new SOFTCONNECTOM();

                List<DATATRPROJET> list = new List<DATATRPROJET>();

                if (STAT == 0)
                {
                    if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin) != null)
                    {
                        foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin).ToList())
                        {
                            var sta = "Attente validation";
                            if (x.ETAT == 1)
                                sta = "Validée";
                            else if (x.ETAT == 2)
                                sta = "Annulée";
                            else if (x.ETAT == 3)
                                sta = "Traitée SIIGFP";

                            list.Add(new DATATRPROJET
                            {
                                No = x.No,
                                REF = x.REF,
                                OBJ = x.OBJ,
                                TITUL = x.TITUL,
                                MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                COMPTE = x.COMPTE,
                                DATE = x.DATEMANDAT.Value.Date,
                                PCOP = x.PCOP,
                                DATEDEF = x.DATEDEF.Value.Date,
                                DATETEF = x.DATETEF.Value.Date,
                                DATEBE = x.DATEBE.Value.Date,
                                STAT = sta
                            });
                        }
                    }
                }
                else
                {
                    var ett = 0;
                    /*if (STAT == 1) ett = 0;*/
                    if (STAT == 2) ett = 1;
                    if (STAT == 3) ett = 2;
                    if (STAT == 4) ett = 3;

                    if (db.SI_TRAITPROJET.FirstOrDefault(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == ett) != null)
                    {
                        foreach (var x in db.SI_TRAITPROJET.Where(a => a.IDPROJET == crpt && a.DATEMANDAT >= DateDebut && a.DATEMANDAT <= DateFin && a.ETAT == ett).ToList())
                        {
                            var sta = "Attente validation";

                            list.Add(new DATATRPROJET
                            {
                                No = x.No,
                                REF = x.REF,
                                OBJ = x.OBJ,
                                TITUL = x.TITUL,
                                MONT = Data.Cipher.Decrypt(x.MONT, "Oppenheimer").ToString(),
                                COMPTE = x.COMPTE,
                                DATE = x.DATEMANDAT.Value.Date,
                                PCOP = x.PCOP,
                                DATEDEF = x.DATEDEF.Value.Date,
                                DATETEF = x.DATETEF.Value.Date,
                                DATEBE = x.DATEBE.Value.Date,
                                STAT = sta
                            });
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult DeleteUser(SI_USERS suser, string UserId)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDPROJET == suser.IDPROJET*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                int crpt = 0;
                if (suser.IDPROJET == null)
                    crpt = exist.IDPROJET.Value;
                else
                    crpt = suser.IDPROJET.Value;

                Guid useID = Guid.Parse(UserId);
                var user = db.SI_TRAITPROJET.FirstOrDefault(a => a.No == useID && a.IDPROJET == crpt);
                if (user != null)
                {
                    user.ETAT = 2;
                    user.DATEANNUL = DateTime.Now;
                    user.IDUSERANNUL = exist.ID;
                    db.SaveChanges();

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Annulation avec succès. " }, settings));
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

        //Historique de connexion des users//
        public ActionResult HistoUsers()
        {
            ViewBag.Controller = "Historique de connexion des utilisateurs";

            return View();
        }

        //GENERATION SIIGLOADOTHER//
        [HttpPost]
        public JsonResult LOADHistoUsers(SI_USERS suser, string listProjet)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                List<DATATRPROJET> list = new List<DATATRPROJET>();

                string[] separators = { "," };
                var pro = listProjet;
                if (pro != null)
                {
                    string listUser = pro.ToString();
                    string[] lst = listUser.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var idP in lst)
                    {
                        int crpt = int.Parse(idP);

                        if (db.SI_MAPPAGES.FirstOrDefault(a => a.IDPROJET == crpt && a.DELETIONDATE == null) == null)
                            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Le projet n'est pas mappé à une base de données TOM²PRO. " }, settings));
                    }

                    var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();

                    if (test.ROLE == (int)Role.SAdministrateur)
                    {
                        if (db.SI_USERSHISTO.Any())
                        {
                            foreach (var x in db.SI_USERSHISTO.OrderByDescending(a => a.CONNEX).ToList())
                            {
                                var soa = "MULTIPLE";
                                if (x.IDPROJET != 0)
                                {
                                    soa = (from soas in db.SI_SOAS
                                           join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                           where prj.IDPROJET == x.IDPROJET && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                           select new
                                           {
                                               soas.SOA
                                           }).FirstOrDefault().SOA;
                                }

                                var isUser = db.SI_USERS.FirstOrDefault(a => a.DELETIONDATE == null && a.ID == x.IDUSER);

                                list.Add(new DATATRPROJET
                                {
                                    IDUSERHISTO = x.ID,
                                    SOA = soa,
                                    PROJET = x.IDPROJET != 0 ? db.SI_PROJETS.Where(a => a.ID == x.IDPROJET && a.DELETIONDATE == null).FirstOrDefault().PROJET : "MULTIPLE",
                                    REF = isUser != null ? isUser.LOGIN : "",
                                    DATEDEF = x.CONNEX != null ? x.CONNEX : null,
                                    DATETEF = x.DISCONNEX != null ? x.DISCONNEX : null,

                                    isLATE = x.DISCONNEX != null ? false : true
                                });
                            }
                        }
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

                            foreach (var us in user)
                            {
                                if (db.SI_USERSHISTO.Any(a => a.IDPROJET == us.ID))
                                {
                                    foreach (var x in db.SI_USERSHISTO.Where(a => a.IDPROJET == us.ID).OrderByDescending(a => a.CONNEX).ToList())
                                    {
                                        var soa = "MULTIPLE";
                                        if (x.IDPROJET != 0)
                                        {
                                            soa = (from soas in db.SI_SOAS
                                                   join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                   where prj.IDPROJET == us.ID && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                   select new
                                                   {
                                                       soas.SOA
                                                   }).FirstOrDefault().SOA;
                                        }

                                        var isUser = db.SI_USERS.FirstOrDefault(a => a.DELETIONDATE == null && a.ID == x.IDUSER);

                                        list.Add(new DATATRPROJET
                                        {
                                            IDUSERHISTO = x.ID,
                                            SOA = soa,
                                            PROJET = x.IDPROJET != 0 ? db.SI_PROJETS.Where(a => a.ID == us.ID && a.DELETIONDATE == null).FirstOrDefault().PROJET : "MULTIPLE",
                                            REF = isUser != null ? isUser.LOGIN : "",
                                            DATEDEF = x.CONNEX != null ? x.CONNEX : null,
                                            DATETEF = x.DISCONNEX != null ? x.DISCONNEX : null,

                                            isLATE = x.DISCONNEX != null ? false : true
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            var user = (from usr in db.SI_PROJETS
                                        join prj in db.SI_MAPUSERPROJET on usr.ID equals prj.IDPROJET
                                        where prj.IDUS == test.ID && usr.DELETIONDATE == null
                                        select new SI_PROJETS
                                        {
                                            PROJET = usr.PROJET,
                                            ID = usr.ID,
                                            DELETIONDATE = usr.DELETIONDATE,
                                        }).ToList();

                            foreach (var us in user)
                            {
                                if (db.SI_USERSHISTO.Any(a => a.IDPROJET == us.ID))
                                {
                                    foreach (var x in db.SI_USERSHISTO.Where(a => a.IDPROJET == us.ID).OrderByDescending(a => a.CONNEX).ToList())
                                    {
                                        var soa = "MULTIPLE";
                                        if (x.IDPROJET != 0)
                                        {
                                            soa = (from soas in db.SI_SOAS
                                                   join prj in db.SI_PROSOA on soas.ID equals prj.IDSOA
                                                   where prj.IDPROJET == us.ID && prj.DELETIONDATE == null && soas.DELETIONDATE == null
                                                   select new
                                                   {
                                                       soas.SOA
                                                   }).FirstOrDefault().SOA;
                                        }

                                        var isUser = db.SI_USERS.FirstOrDefault(a => a.DELETIONDATE == null && a.ID == x.IDUSER);

                                        list.Add(new DATATRPROJET
                                        {
                                            IDUSERHISTO = x.ID,
                                            SOA = soa,
                                            PROJET = x.IDPROJET != 0 ? db.SI_PROJETS.Where(a => a.ID == us.ID && a.DELETIONDATE == null).FirstOrDefault().PROJET : "MULTIPLE",
                                            REF = isUser != null ? isUser.LOGIN : "",
                                            DATEDEF = x.CONNEX != null ? x.CONNEX : null,
                                            DATETEF = x.DISCONNEX != null ? x.DISCONNEX : null,

                                            isLATE = x.DISCONNEX != null ? false : true
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. ", data = list.OrderByDescending(a => a.isLATE) }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }

        [HttpPost]
        public JsonResult DisconHisto(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null/* && a.IDSOCIETE == suser.IDSOCIETE*/);

            try
            {
                if (db.SI_USERSHISTO.Any(x => x.IDUSER == exist.ID))
                {
                    var histo = db.SI_USERSHISTO.Where(x => x.IDUSER == exist.ID).OrderByDescending(a => a.ID).FirstOrDefault();
                    histo.DISCONNEX = DateTime.Now;
                    db.SaveChanges();
                }

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Connexion avec succès. " }, settings));
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
