using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DualPrep.Models
{
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser()
        {

        }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string UserRole { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string LastName { get; set; }

        public ICollection<MealFavorite> MealFavorites { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }

}
