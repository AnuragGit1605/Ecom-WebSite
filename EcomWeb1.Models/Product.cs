using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EcomWeb1.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public double Price { get; set; }
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }
        public int TotalCopiesSold { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category category { get; set; }
        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }
        public CoverType coverType { get; set; }

    }
}
