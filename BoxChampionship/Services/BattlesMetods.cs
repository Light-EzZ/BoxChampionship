using Antlr.Runtime;
using Antlr.Runtime.Tree;
using BoxChampionship.Models;
using BoxChampionship.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BoxChampionship.Services
{
    public class BattlesMetods
    {
        private readonly string _dbContextName = "BoxChampionshipModelEntities";
        public static int AddOrCreate(string name,string surname) { //пошук або додавання нового боксера
           
            using (var db = new BoxChampionshipDBEntities())
            {
                Boxers boxer = db.Boxers.FirstOrDefault(c => c.BoxerName == name && c.BoxerSurname == surname);

                if (boxer != null)
                {

                    return boxer.BoxerId;
                }
                else
                {
                    Boxers newBoxer = new Boxers
                    {
                        BoxerName = name,
                        BoxerSurname = surname
                    };

                    db.Boxers.Add(newBoxer);
                    db.SaveChanges();
                    return newBoxer.BoxerId;
                }
            }
        }

        public static void CreateBattle(int id1, int id2, int championshipID, int rounds, DateTime dateTime, int boxer1_Score, int boxer2_Score) //Додавання нового матчу
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                Battles newBattle = new Battles
                {
                    Winner = id1,
                    Looser = id2,
                    ChampionshipId = championshipID,
                    Rounds = rounds,
                    DateTime = dateTime,
                    WinnerPoints = boxer1_Score,
                    LooserPoints = boxer2_Score
                };

                db.Battles.Add(newBattle);

                db.SaveChanges();
            }
        }

        public static int GetchmID(string championshipname)  //Отримання айді чемпіонату по назві
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                Championship champ = db.Championship.FirstOrDefault(c => c.ChampionshipName == championshipname);
                
                if (champ != null)
                {
                    return champ.ChampionshipId;
                }
                else
                {
                    Championship newChamp = new Championship
                    {
                        ChampionshipName = championshipname
                        
                    };
                    
                    db.Championship.Add(newChamp);              
                    db.SaveChanges();
                    return newChamp.ChampionshipId;
                }
                                      
            }
        }



        public static object GetContainer()
        {
            var vm = new BattlesList();
            using (var db = new BoxChampionshipDBEntities())
            {
             
                var tempList = db.Boxers
                    .OrderBy(b => b.BoxerName)
                    .ThenBy(b => b.BoxerSurname)
                    .Select(c => new
                    {
                        FullName = c.BoxerName + " " + c.BoxerSurname
                    })
                    .ToList() 
                    .Select(c => new SelectListItem
                    {
                        Text = c.FullName,
                        Value = c.FullName
                    }).ToList();
                tempList.Insert(0, new SelectListItem { Text = "--- Choose Boxer ---", Value = "" });
                vm.BoxersList = tempList;
            }
            return vm;
        }
            
        [HttpGet]
        
        public static object GetRankingsData(string sidx, string sord, int page, int rows, string winLossFilter, string boxerNameFilter) //Дані для таблиці всіх матчів
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                
                var battlesQuery = db.Battles
                        .Include("Boxers")
                        .Include("Boxers1")  
                        .AsQueryable();

          

                // Фільтр по імені
                if (!string.IsNullOrEmpty(boxerNameFilter))
                {
                    battlesQuery = battlesQuery.Where(b =>
                        (b.Boxers1 != null && (b.Boxers1.BoxerName + " " + b.Boxers1.BoxerSurname).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase)) || 
                        (b.Boxers != null && (b.Boxers.BoxerName + " " + b.Boxers.BoxerSurname).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))    
                    );
                }

                // Фільтр по поразкам та перемогам
                if (!string.IsNullOrEmpty(boxerNameFilter) && !string.IsNullOrEmpty(winLossFilter) && winLossFilter != "all")
                {
                    if (winLossFilter == "winner")
                    {
                        battlesQuery = battlesQuery.Where(b =>
                            (b.Boxers1 != null && (b.Boxers1.BoxerName + " " + b.Boxers1.BoxerSurname).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))
                        );
                    }
                    else if (winLossFilter == "loser")
                    {
                        battlesQuery = battlesQuery.Where(b =>
                            (b.Boxers != null && (b.Boxers.BoxerName + " " + b.Boxers.BoxerSurname).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))
                        );
                    }
                }

                var rowQuery = battlesQuery.Select(g => new BattlesList
                {
                    Id = g.BattleId,
                    WinnerName = (g.Boxers1 != null) ? (g.Boxers1.BoxerName + " " + g.Boxers1.BoxerSurname) : "N/A",
                    LoserName = (g.Boxers != null) ? (g.Boxers.BoxerName + " " + g.Boxers.BoxerSurname) : "N/A",
                    DateTime = g.DateTime
                });

   
                int totalRecords = rowQuery.Count();

                //  сортування
                bool isDescending = (sord ?? "").Equals("desc", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(sidx))
                {
                    rowQuery = rowQuery.OrderByDescending(r => r.DateTime);
                }
                else
                {
                    switch (sidx)
                    {
                        case "WinnerName":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.WinnerName) : rowQuery.OrderBy(r => r.WinnerName);
                            break;
                        case "LoserName":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.LoserName) : rowQuery.OrderBy(r => r.LoserName);
                            break;
                        case "DateTime":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.DateTime) : rowQuery.OrderBy(r => r.DateTime);
                            break;
                        default:
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.DateTime) : rowQuery.OrderBy(r => r.DateTime);
                            break;
                    }
                }

                // Пагінація
                var pagedData = rowQuery
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToList(); 

                //  JSON
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
        public static object GetSingleBattleDetails(int id)
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                var battle = db.Battles
                               .Include("Boxers")  
                               .Include("Boxers1")
                               .Include("Championship")
                               .FirstOrDefault(b => b.BattleId == id);

                if (battle == null)
                {
                    return null;
                }

              
                var result = new
                {
                    Id = battle.BattleId,
                    WinnerName = (battle.Boxers1 != null) ? (battle.Boxers1.BoxerName + " " + battle.Boxers1.BoxerSurname) : "N/A",
                    LoserName = (battle.Boxers != null) ? (battle.Boxers.BoxerName + " " + battle.Boxers.BoxerSurname) : "N/A",
                    RoundsCount = battle.Rounds,
                    Score = battle.WinnerPoints + ":" + battle.LooserPoints,
                    DateTime = battle.DateTime,
                    ChampionshipName = (battle.Championship != null) ? battle.Championship.ChampionshipName : "N/A"
                };
                return result;
            }
        }
    }
}