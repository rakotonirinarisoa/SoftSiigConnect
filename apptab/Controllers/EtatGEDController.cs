using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace apptab.Controllers
{
    public class EtatGEDController : Controller
    {
        private readonly SOFTCONNECTSIIG db = new SOFTCONNECTSIIG();
        private readonly SOFTCONNECTGED ged = new SOFTCONNECTGED();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        //TB2: Situation des étapes par type de document (état d'avancement)//
        public ActionResult EtapTypeDocs()
        {
            ViewBag.Controller = "Situation des étapes par type de document";

            return View();
        }
    }
}
