using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DualPrep.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Equipment { get; set; }
        public string Directions { get; set; }
        public MuscleGroups Muscle { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedByUser { get; set; }        //add user ID/string to be bound to the meal
        //add some way to add images/gifs/videos to better explain/demonstrate form/range of motion
    }

    public enum MuscleGroups
    {
        Biceps,
        Triceps,
        Forearms,
        Shoulders,
        Chest,
        Abdominals,
        Trapezius,
        UpperBack,
        LowerBack,
        Quadriceps,
        Hamstrings,
        Calves
    }
}
