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
                // Отримання даних
                var battlesQuery = db.Battles
                    .Include("Boxers")      
                    .Include("Boxers1")     
                    .Where(b => b.ChampionshipId == championshipId);

                var allBattlesList = battlesQuery.ToList();

                //  Збираємо учасників для рахування статистики
                var winners = allBattlesList.Select(b => new {
                    BoxerObject = b.Boxers1, // Переможець
                    IsWinner = true
                });
                var losers = allBattlesList.Select(b => new {
                    BoxerObject = b.Boxers, // Програвший
                    IsWinner = false
                });
                var allParticipants = winners.Concat(losers);

                // Групуємо та Рахуємо Статистику 

                var rankingsQuery = allParticipants
                    .GroupBy(p => p.BoxerObject.BoxerId) 
                    .Select(g => new BoxerRanking 
                    {
                        BoxerId = g.Key,
                        BoxerName = g.First().BoxerObject.BoxerName + " " + g.First().BoxerObject.BoxerSurname,
                        TotalGames = g.Count(),
                        Wins = g.Count(match => match.IsWinner == true),
                        Losses = g.Count(match => match.IsWinner == false),
                        WinRatio = (g.Count() > 0)
                            ? (double)g.Count(match => match.IsWinner == true) / g.Count()
                            : 0
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

                if (string.IsNullOrEmpty(sidx))
                {
                    // сортування за замовчуванням
                    rankingsQuery = rankingsQuery
                        .OrderByDescending(r => r.WinRatio)
                        .ThenBy(r => r.Losses);
                }
                else
                {
                    // Динамічне сортування котре по кнопкам в таблиці
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