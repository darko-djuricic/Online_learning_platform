using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Categories
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        public virtual List<Categories> Subcategories { get; set; }
    }
}