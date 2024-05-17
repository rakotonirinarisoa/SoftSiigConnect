﻿using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data.Entity;

namespace apptab.Controllers
{
    public class PdfController : Controller
    {
        private readonly SOFTCONNECTSIIG _db;
        private readonly JsonSerializerSettings _settings;

        private static string s_tableId = "";
        private static string s_domElement = "";
        private static string s_pdfFilename = "";
        private static string s_header = "";
        private static string s_footer = "";

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
        public async Task<ActionResult> ExportToPdf(SI_USERS suser, string Id, string element, string filename, string header, string footer)
        {
            var currentUser = await _db.SI_USERS.FirstOrDefaultAsync(u => u.LOGIN == suser.LOGIN && u.PWD == suser.PWD && u.DELETIONDATE == null);

            if (currentUser == null)
            {
                return Json(JsonConvert.SerializeObject(new { type = "login", msg = "Problème de connexion!" }, _settings));
            }

            s_tableId = Id;
            s_domElement = element;
            s_pdfFilename = filename;
            s_header = header;
            s_footer = footer;

            return Json(JsonConvert.SerializeObject(new { type = "success", msg = "PDF en cours de génération..." }, _settings));
        }

        public ActionResult Index()
        {
            ViewData["TableId"] = s_tableId;
            ViewData["DomElement"] = s_domElement;
            ViewData["Filename"] = s_pdfFilename;
            ViewData["Header"] = s_header;
            ViewData["Footer"] = s_footer;

            return View();
        }
    }
}
