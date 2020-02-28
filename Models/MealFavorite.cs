using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DualPrep.Models
{
    public class MealFavorite
    {
        [Key]
        public int Id { get; set; }
        public int MealId { get; set; }
        public string ApplicationUserId { get; set; }

        public Meal Meal { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
