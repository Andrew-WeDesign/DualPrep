using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DualPrep.Models
{
    public class Meal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public string Author { get; set; }        //optional place to put the author
        public string CreatedByUser { get; set; }        //add user ID/string to be bound to the meal
        //add rating system either upvotes/downvotes or star ratings

        public ICollection<MealFavorite> MealFavorites { get; set; }
    }
}
