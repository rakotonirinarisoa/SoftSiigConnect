using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using apptab.Data.Entities;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class GetCountTDBController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        [HttpPost]
        public ActionResult GetCountTDB(SI_USERS suser)
        {
            var exist = db.SI_USERS.FirstOrDefault(a => a.LOGIN == suser.LOGIN && a.PWD == suser.PWD && a.DELETIONDATE == null);
            if (exist == null) return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion. " }, settings));

            try
            {
                var newElemH = new COUNTTDB()
                {
                    MandatT = 0,
                    MandatV = 0,
                    MandatA = 0,
                    AvanceT = 0,
                    AvanceV = 0,
                    AvanceA = 0,
                    PaieR = 0,
                    PaieT = 0,
                    PaieV = 0,
                    PaieF = 0,
                    PaieA = 0
                };

                var test = db.SI_USERS.Where(x => x.LOGIN == exist.LOGIN && x.PWD == exist.PWD && x.DELETIONDATE == null).FirstOrDefault();

                var mandat = db.SI_TRAITPROJET.ToList();

                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    newElemH = new COUNTTDB()
                    {
                        MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                        MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                        MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                        AvanceT = 0,
                        AvanceV = 0,
                        AvanceA = 0,
                        PaieR = 0,
                        PaieT = 0,
                        PaieV = 0,
                        PaieF = 0,
                        PaieA = 0
                    };

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = newElemH }, settings));
                }
                else
                {
                    if (test.IDPROJET != 0)
                    {
                        mandat = mandat.Where(a => a.IDPROJET == test.IDPROJET).ToList();

                        newElemH = new COUNTTDB()
                        {
                            MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                            MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                            MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                            AvanceT = 0,
                            AvanceV = 0,
                            AvanceA = 0,
                            PaieR = 0,
                            PaieT = 0,
                            PaieV = 0,
                            PaieF = 0,
                            PaieA = 0
                        };

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = newElemH }, settings));
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

                        foreach (var x in user)
                        {
                            mandat = mandat.Where(a => a.IDPROJET == x.ID).ToList();

                            newElemH = new COUNTTDB()
                            {
                                MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                                MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                                MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                                AvanceT = 0,
                                AvanceV = 0,
                                AvanceA = 0,
                                PaieR = 0,
                                PaieT = 0,
                                PaieV = 0,
                                PaieF = 0,
                                PaieA = 0
                            };
                        }

                        return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = newElemH }, settings));
                    }
                }
            }
            catch (Exception e)
            {
                return Json(JsonConvert.SerializeObject(new { type = "error", msg = e.Message }, settings));
            }
        }
    }
}
