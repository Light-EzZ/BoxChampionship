using BoxChampionship.Services;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Web.Mvc;

namespace BoxChampionship.Controllers
{
    public class BattlesListViewController : Controller
    {


        public ActionResult Index()
        {
            var vm = BattlesMetods.GetContainer();
            return View(vm);
        }

        public JsonResult GetData(string sidx, string sord, int page, int rows, string winLossFilter, string boxerNameFilter)
        {
            object dataFromService = Services.BattlesMetods.GetRankingsData(sidx, sord, page, rows, winLossFilter, boxerNameFilter);
            return Json(dataFromService, JsonRequestBehavior.AllowGet);
        }


    }
}
