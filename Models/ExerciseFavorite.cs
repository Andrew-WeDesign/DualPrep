using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DualPrep.Models
{
    public class ExerciseFavorite
    {
        [Key]
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string ApplicationUserId { get; set; }

        public Exercise Exercise { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
