using BoxChampionship.Models;
using BoxChampionship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BoxChampionship.Services
{
    public class ChampionshipMetods
    {
        public static object GetContainer()
        {
            var vm = new ChampionshipTable();

   
            using (var db = new BoxChampionshipDBEntities())
            {
                var champsFromDb = db.Championship.ToList();

                var tempList = champsFromDb.Select(c => new SelectListItem
                {
                    Text = c.ChampionshipName,
                    Value = c.ChampionshipId.ToString()
                }).ToList();

                tempList.Insert(0, new SelectListItem { Text = "--- Choose Championship ---", Value = "" });

                vm.ChampionshipList = tempList;
            }
            return vm;
        }
        [HttpGet]
        public static object GetRankingsData(string sidx, string sord, int page, int rows, int championshipId, string boxerFilter)
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                var rankingsQuery = db.Boxers
                    .Where(b => b.Battles1.Any(bt => bt.ChampionshipId == championshipId) ||
                                b.Battles.Any(bt => bt.ChampionshipId == championshipId))

                    
                    .Select(b => new
                    {
                        BoxerId = b.BoxerId,
                        BoxerName = b.BoxerName + " " + b.BoxerSurname,
                        Wins = b.Battles1.Count(bt => bt.ChampionshipId == championshipId),
                        Losses = b.Battles.Count(bt => bt.ChampionshipId == championshipId)
                    })

                
                    .Select(r => new BoxerRanking
                    {
                        BoxerId = r.BoxerId,
                        BoxerName = r.BoxerName,
                        Wins = r.Wins,
                        Losses = r.Losses,
                        TotalGames = r.Wins + r.Losses,
                        WinRatio = (r.Wins + r.Losses) > 0 ? (double)r.Wins / (r.Wins + r.Losses) : 0
                    });

                //  Фільтрація за іменем 
                if (!string.IsNullOrEmpty(boxerFilter))
                {
                   
                    rankingsQuery = rankingsQuery.Where(r =>
                        r.BoxerName.IndexOf(boxerFilter, StringComparison.OrdinalIgnoreCase) >= 0
                    );
                }

                
                int totalRecords = rankingsQuery.Count(); 
                bool isDescending = (sord ?? "").Equals("desc", StringComparison.OrdinalIgnoreCase);

                //Сортування 
                if (string.IsNullOrEmpty(sidx))
                {
                    rankingsQuery = rankingsQuery
                       .OrderByDescending(r => r.WinRatio)
                       .ThenBy(r => r.Losses);
                }
                else
                {
                    switch (sidx)
                    {
                        case "BoxerName":
                            rankingsQuery = isDescending ? rankingsQuery.OrderByDescending(r => r.BoxerName) : rankingsQuery.OrderBy(r => r.BoxerName);
                            break;
                        case "Wins":
                            rankingsQuery = isDescending ? rankingsQuery.OrderByDescending(r => r.Wins) : rankingsQuery.OrderBy(r => r.Wins);
                            break;
                        case "Losses":
                            rankingsQuery = isDescending ? rankingsQuery.OrderByDescending(r => r.Losses) : rankingsQuery.OrderBy(r => r.Losses);
                            break;
                        case "TotalGames":
                            rankingsQuery = isDescending ? rankingsQuery.OrderByDescending(r => r.TotalGames) : rankingsQuery.OrderBy(r => r.TotalGames);
                            break;
                        case "WinRatio":
                        default:
                            rankingsQuery = isDescending ? rankingsQuery.OrderByDescending(r => r.WinRatio) : rankingsQuery.OrderBy(r => r.WinRatio);
                            break;
                    }
                }

                //  Пагінація
                var pagedData = rankingsQuery
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToList();

                //  Формування JSON
                var jsonData = new
                {
                    total = (int)Math.Ceiling((double)totalRecords / rows),
                    page = page,
                    records = totalRecords,
                    rows = pagedData
                };

                return jsonData;
            }
        }


    }
}