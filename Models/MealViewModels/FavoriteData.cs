using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DualPrep.Models.MealViewModels
{
    public class FavoriteData
    {
        public IEnumerable<Meal> Meals { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public IEnumerable<MealFavorite> MealFavorites { get; set; }
    }
}
