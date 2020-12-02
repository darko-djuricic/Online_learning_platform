using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Section
    {
        [Key]
        public string SectionId { get; set; } = Guid.NewGuid().ToString("N");
        [Required]
        public string Title { get; set; } = "Section Title";
        [Range(1,5)]
        public int Number { get; set; } = 1;
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0);
        [Required]
        public virtual List<Content> Contents { get; set; } = new List<Content>();
    }
}