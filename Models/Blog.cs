using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DualPrep.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public string TLDR { get; set; }
        public  DateTime Date { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedByUser { get; set; }
    }
}
