using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Comment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        [Required]
        public string Title { get; set; } = "Title";
        [Required]
        public string Author { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public bool IsQA { get; set; }
        public virtual List<Comment> Replies { get; set; }
        public virtual ICollection<Likes> UserWhoLiked { get; set; }
    }
}