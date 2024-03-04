using System;
using System.Linq;
using System.Web.Mvc;
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
                    MandatI = 0,
                    MandatTR = 0,
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
                var payement = db.OPA_VALIDATIONS.ToList();

                if (test.ROLE == (int)Role.SAdministrateur)
                {
                    newElemH = new COUNTTDB()
                    {
                        MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                        MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                        MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                        //MandatI = mandat.Where(a => a.ETAT == 2).Count(),
                        MandatTR = mandat.Where(a => a.ETAT == 3).Count(),
                        AvanceT = 0,
                        AvanceV = 0,
                        AvanceA = 0,
                        PaieR = payement.Where(a => a.ETAT == 0).Count(),
                        PaieT = payement.Where(a => a.ETAT == 1).Count(),
                        PaieV = payement.Where(a => a.ETAT == 2).Count(),
                        PaieF = payement.Where(a => a.ETAT == 3).Count(),
                        PaieA = payement.Where(a => a.ETAT == 4).Count()
                    };

                    return Json(JsonConvert.SerializeObject(new { type = "success", msg = "message", data = newElemH }, settings));
                }
                else
                {
                    if (test.IDPROJET != 0)
                    {
                        mandat = mandat.Where(a => a.IDPROJET == test.IDPROJET).ToList();
                        payement = payement.Where(a => a.IDPROJET == test.IDPROJET).ToList() ;
                        newElemH = new COUNTTDB()
                        {
                            MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                            MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                            MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                            //MandatI = mandat.Where(a => a.ETAT == 2).Count(),
                            MandatTR = mandat.Where(a => a.ETAT == 3).Count(),
                            AvanceT = 0,
                            AvanceV = 0,
                            AvanceA = 0,
                            PaieR = payement.Where(a => a.ETAT == 0).Count(),
                            PaieT = payement.Where(a => a.ETAT == 1).Count(),
                            PaieV = payement.Where(a => a.ETAT == 2).Count(),
                            PaieF = payement.Where(a => a.ETAT == 3).Count(),
                            PaieA = payement.Where(a => a.ETAT == 4).Count()
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
                            payement = payement.Where(a => a.IDPROJET == x.ID).ToList();
                            newElemH = new COUNTTDB()
                            {
                                MandatT = mandat.Where(a => a.ETAT == 0).Count(),
                                MandatV = mandat.Where(a => a.ETAT == 1).Count(),
                                MandatA = mandat.Where(a => a.ETAT == 2).Count(),
                                //MandatI = mandat.Where(a => a.ETAT == 2).Count(),
                                MandatTR = mandat.Where(a => a.ETAT == 3).Count(),
                                AvanceT = 0,
                                AvanceV = 0,
                                AvanceA = 0,
                                PaieR = payement.Where(a => a.ETAT == 0).Count(),
                                PaieT = payement.Where(a => a.ETAT == 1).Count(),
                                PaieV = payement.Where(a => a.ETAT == 2).Count(),
                                PaieF = payement.Where(a => a.ETAT == 3).Count(),
                                PaieA = payement.Where(a => a.ETAT == 4).Count()
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
