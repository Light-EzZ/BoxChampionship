using System.Collections.Generic;
using System.Web.Mvc;

namespace BoxChampionship.ViewModels
{

    public class BoxerRanking
    {
        public int BoxerId { get; set; }
        public string BoxerName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalGames { get; set; }
        public double WinRatio { get; set; }
    }

    
   
}

