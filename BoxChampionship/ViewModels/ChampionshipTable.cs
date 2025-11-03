using BoxChampionship.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoxChampionship.ViewModels
{
    public class ChampionshipTable
    {
        public IEnumerable<SelectListItem> ChampionshipList { get; set; }
        public int BoxerId { get; set; }
        public string BoxerName { get; set; }
        public string BoxerSurname { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalGames { get; set; }
        public double WinRatio { get; set; }

    }
}