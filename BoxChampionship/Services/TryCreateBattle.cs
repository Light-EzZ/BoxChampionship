using BoxChampionship.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;

namespace BoxChampionship.Services
{
    public class TryCreateBattle
    {
        public static void CreatingBattle(BattlesAdd vm)
        {
            int championshipID;
            int id1;
            int id2;
            championshipID = BattlesMetods.GetchmID(vm.Championship);
            id1 = BattlesMetods.AddOrCreate(vm.Boxer1_Name, vm.Boxer1_Surname);
            id2 = BattlesMetods.AddOrCreate(vm.Boxer2_Name, vm.Boxer2_Surname);
            if (vm.Boxer1_Score > vm.Boxer2_Score)
            {
                BattlesMetods.WinAdd(id1);
                BattlesMetods.LoseAdd(id2);
                BattlesMetods.CreateBattle(id1, id2, championshipID, vm.Rounds, vm.DateTime, vm.Boxer1_Score, vm.Boxer2_Score);
            }
            else
            {
                BattlesMetods.WinAdd(id2);
                BattlesMetods.LoseAdd(id1);
                BattlesMetods.CreateBattle(id2, id1, championshipID, vm.Rounds, vm.DateTime, vm.Boxer2_Score, vm.Boxer1_Score);
            }
            
        }
    }
}