using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using apptab.Data.Entities;
using System.Data.Entity;
using System;
using System.Runtime;
using System.Collections.Generic;

namespace apptab.Controllers
{
    public class PdfController : Controller
    {
        private readonly SOFTCONNECTSIIG _db;
        private readonly JsonSerializerSettings _settings;

        private static string tableId = "";
        private static string columnsIndexesToHide = "";
        private static string domElement = "";
        private static string pdfFilename = "";

        public PdfController()
        {
            _db = new SOFTCONNECTSIIG();
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ExportToPdf(SI_USERS suser, string Id, string columnsIndexes, string element, string filename)
        {
            var currentUser = await _db.SI_USERS.FirstOrDefaultAsync(u => u.LOGIN == suser.LOGIN && u.PWD == suser.PWD && u.DELETIONDATE == null);

            if (currentUser == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion!" }, _settings));
            }

            tableId = Id;
            columnsIndexesToHide = columnsIndexes;
            domElement = element;
            pdfFilename = filename;

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "Suppression avec succès." }, _settings));
        }

        public ActionResult Index()
        {
            ViewData["TableId"] = tableId;
            ViewData["ColumnsIndexesToHide"] = columnsIndexesToHide;
            ViewData["DomElement"] = domElement;
            ViewData["Filename"] = pdfFilename;

            return View();
        }
    }
}
