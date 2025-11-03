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

        public static void LoseAdd(int id2)  //Калькулятор поразок
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                Boxers boxer = db.Boxers.FirstOrDefault(c => c.BoxerId == id2);
             
                {
                    boxer.LoseCount++;
                };

                db.SaveChanges();
            }
        }

        public static void WinAdd(int id1)//Калькулятор перемог
        {
            using (var db = new BoxChampionshipDBEntities())
            {
                Boxers boxer = db.Boxers.FirstOrDefault(c => c.BoxerId == id1);

                {
                    boxer.WinCount++;
                }
                ;

                db.SaveChanges();
            }
        }

        public static object GetContainer() //Список боксерів
        {
            var vm = new BattlesList();

            // Готуємо список для випадаючого меню
            using (var db = new BoxChampionshipDBEntities())
            {
                var champsFromDb = db.Boxers.ToList();

                var tempList = champsFromDb.Select(c => new SelectListItem
                {
                    Text = c.BoxerName + " " + c.BoxerSurname,
                    Value = c.BoxerName + " " + c.BoxerSurname 
                }).ToList();

                tempList.Insert(0, new SelectListItem { Text = "--- Choose Boxer ---", Value = "" });
                vm.BoxersList = tempList;
            }
            return vm;
        }
        [HttpGet]
        public static object GetRankingsData(string sidx, string sord, int page, int rows, string winLossFilter,string boxerNameFilter) //Дані для таблиці всіх матчів
        {

            using (var db = new BoxChampionshipDBEntities())
            {
                // Підтягуємо бої 
                var battlesQuery = db.Battles
                    .Include("Boxers")      
                    .Include("Boxers1")    
                    .AsQueryable(); 
       
                var allBattlesList = battlesQuery.ToList();

                IEnumerable<Battles> filteredBattles = allBattlesList;

                // Фільтр по імені
                if (!string.IsNullOrEmpty(boxerNameFilter))
                {

                    filteredBattles = filteredBattles.Where(b =>
                        (b.Boxers1 != null && (b.Boxers1.BoxerName.Trim() + " " + b.Boxers1.BoxerSurname.Trim()).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase)) || // Переможець
                        (b.Boxers != null && (b.Boxers.BoxerName.Trim() + " " + b.Boxers.BoxerSurname.Trim()).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))    // Програвший
                    );
                }

                // Фільтр по поразкам та перемогам
                if (!string.IsNullOrEmpty(boxerNameFilter) && !string.IsNullOrEmpty(winLossFilter) && winLossFilter != "all")
                {
                    if (winLossFilter == "winner")
                    {
                        filteredBattles = filteredBattles.Where(b =>
                            (b.Boxers1 != null && (b.Boxers1.BoxerName.Trim() + " " + b.Boxers1.BoxerSurname.Trim()).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))
                        );
                    }
                    else if (winLossFilter == "loser")
                    {
                        filteredBattles = filteredBattles.Where(b =>
                            (b.Boxers != null && (b.Boxers.BoxerName.Trim() + " " + b.Boxers.BoxerSurname.Trim()).Equals(boxerNameFilter, StringComparison.OrdinalIgnoreCase))
                        );
                    }
                }

                // Відфільтрований список ==> ViewModel
                var rowQuery = filteredBattles.Select(g => new BattlesList
                {
                    Id = g.BattleId,
                    WinnerName = (g.Boxers1 != null) ? (g.Boxers1.BoxerName + " " + g.Boxers1.BoxerSurname) : "N/A",
                    LoserName = (g.Boxers != null) ? (g.Boxers.BoxerName + " " + g.Boxers.BoxerSurname) : "N/A",
                    RoundsCount = g.Rounds,
                    Score = g.WinnerPoints + ":" + g.LooserPoints,
                    DateTime = g.DateTime
                });

                // Динамічне Сортування, Пагінація, JSON 
                int totalRecords = rowQuery.Count();
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
                        case "RoundsCount":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.RoundsCount) : rowQuery.OrderBy(r => r.RoundsCount);
                            break;
                        case "Score":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.Score) : rowQuery.OrderBy(r => r.Score);
                            break;
                        case "DateTime":
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.DateTime) : rowQuery.OrderBy(r => r.DateTime);
                            break;
                        default:
                            rowQuery = isDescending ? rowQuery.OrderByDescending(r => r.DateTime) : rowQuery.OrderBy(r => r.DateTime);
                            break;
                    }
                }

                var pagedData = rowQuery
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToList();

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