using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoxChampionship.ViewModels
{
    public class BattlesList
    {
        public IEnumerable<SelectListItem> BoxersList { get; set; }
        public int BoxerId { get; set; }
        public int Id { get; set; }
        public string WinnerName { get; set; }


        public string LoserName { get; set; }
   
        public int Winner { get; set; }
        public int Loser { get; set; }
        public System.DateTime DateTime { get; set; }
        public int RoundsCount { get; set; }
        public string Score { get; set; }
    }
}