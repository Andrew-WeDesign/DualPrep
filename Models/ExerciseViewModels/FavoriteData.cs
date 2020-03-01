using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DualPrep.Models.ExerciseViewModels
{
    public class FavoriteData
    {
        public IEnumerable<Meal> Exercises { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public IEnumerable<ExerciseFavorite> ExerciseFavorites { get; set; }
    }
}
