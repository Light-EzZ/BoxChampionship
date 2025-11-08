using BoxChampionship.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BoxChampionship.ViewModels
{
    public class BattlesAdd
    {
        [Required(ErrorMessage = "Please specify the date and time of the fight")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [PastOrCurrentDate(ErrorMessage = "Battles Date cant be in future!")]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Please specify the number of rounds")]
        [Range(1, 12, ErrorMessage = "The number of rounds must be between 1 and 12")]
        public int Rounds { get; set; }

        [Display(Name = "ChampionshipName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The championship name cannot be empty")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 50 characters long")]
        public string Championship { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The first name cannot be empty")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 50 characters long")]
        public string Boxer1_Name { get; set; }

        [Display(Name = "Surname")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The surname cannot be empty")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The surname must be between 2 and 50 characters long")]
        public string Boxer1_Surname { get; set; }

        [Display(Name = "Points")]
        [Required(ErrorMessage = "Please specify the number of points")]
        [Range(10, 120, ErrorMessage = "The number of points must be between 10 and 120")]
        public int Boxer1_Score { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The first name cannot be empty")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 50 characters long")]
        public string Boxer2_Name { get; set; }

        [Display(Name = "Surname")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The surname cannot be empty")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The surname must be between 2 and 50 characters long")]
        public string Boxer2_Surname { get; set; }

        [Display(Name = "Points")]
        [Required(ErrorMessage = "Please specify the number of points")]
        [Range(10, 120, ErrorMessage = "The number of points must be between 10 and 120")]
        public int Boxer2_Score { get; set; }

        
    }
}