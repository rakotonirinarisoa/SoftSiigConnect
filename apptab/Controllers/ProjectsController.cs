using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using apptab.Data.Entities;
using System.Data.Entity;
using System;
using apptab;

namespace apptab.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly SOFTCONNECTSIIG _db;
        private readonly JsonSerializerSettings _settings;

        public ProjectsController()
        {
            _db = new SOFTCONNECTSIIG();
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public async Task<ActionResult> Details(int id)
        {
            var project = await _db.SI_PROJETS.Where(x => x.ID == id && x.DELETIONDATE == null).FirstOrDefaultAsync();

            if (project == null)
            {
                throw new Exception("404");
            }

            ViewData["Id"] = id;
            ViewData["Title"] = project.PROJET;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Update(ProjectToUpdate projectToUpdate)
        {
            var user = await _db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == projectToUpdate.Login && a.PWD == projectToUpdate.Password && a.DELETIONDATE == null);

            if (user == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion!" }, _settings));
            }

            var res = await _db.SI_PROJETS.FirstOrDefaultAsync(project => project.ID == projectToUpdate.Id && project.DELETIONDATE == null);

            if (res != null)
            {
                res.DELETIONDATE = DateTime.Now;

                _db.SI_PROJETS.Add(new SI_PROJETS
                {
                    PROJET = projectToUpdate.Title
                });

                await _db.SaveChangesAsync();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Enregistrement avec succès." }, _settings));
            }

            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Projet non trouvé!" }, _settings));
        }

        [HttpPost]
        public async Task<JsonResult> Delete(ProjectToDelete projectToDelete)
        {
            var user = await _db.SI_USERS.FirstOrDefaultAsync(a => a.LOGIN == projectToDelete.Login && a.PWD == projectToDelete.Password && a.DELETIONDATE == null);

            if (user == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion!" }, _settings));
            }

            var project = await _db.SI_PROJETS.FirstOrDefaultAsync(x => x.ID == projectToDelete.Id && x.DELETIONDATE == null);

            if (project != null)
            {
                var prosoa = await _db.SI_PROSOA.FirstOrDefaultAsync(x => x.IDPROJET == projectToDelete.Id && x.DELETIONDATE == null);
                var now = DateTime.Now;

                if (prosoa != null)
                {
                    prosoa.DELETIONDATE = now;
                }

                project.DELETIONDATE = now;

                await _db.SaveChangesAsync();

                return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Suppression avec succès." }, _settings));
            }

            return Json(JsonConvert.SerializeObject(new { type = "error", msg = "Projet non trouvé!" }, _settings));
        }
    }
}
