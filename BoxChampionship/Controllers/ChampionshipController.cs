using BoxChampionship.Models;
using BoxChampionship.Services;
using BoxChampionship.ViewModels; 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;

using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;

namespace BoxChampionship.Controllers
{
    public class ChampionshipController : Controller
    {

        public ActionResult Index()
        {
           var vm= ChampionshipMetods.GetContainer();
            return View(vm);
        }

        public JsonResult GetData(string sidx, string sord, int page, int rows, int championshipId, string boxerFilter)
        {
            object dataFromService = Services.ChampionshipMetods.GetRankingsData(sidx, sord, page, rows, championshipId, boxerFilter);
            return Json(dataFromService, JsonRequestBehavior.AllowGet);
        }



    }
}

