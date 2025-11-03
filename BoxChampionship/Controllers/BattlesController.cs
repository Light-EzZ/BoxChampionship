

using BoxChampionship.Services;
using BoxChampionship.ViewModels;
using System.Web.Mvc;

namespace BoxChampionship.Controllers
{
    public class BattlesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BattlesAdd());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BattlesAdd vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            else if (vm.Boxer1_Score == vm.Boxer2_Score)
            {
                return View(vm);
            }
            else
            {

                Services.TryCreateBattle.CreatingBattle(vm);
                TempData["SuccessMessage"] = "The battle was successfully added!";
                return RedirectToAction("Index");
            }
        }
           
    }
}
