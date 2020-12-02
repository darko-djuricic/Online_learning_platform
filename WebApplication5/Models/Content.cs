using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Content
    {
#nullable enable
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        [Required, Range(1,5)]
        public int Number { get; set; } = 1;
        [Required]
        public string Title { get; set; } = "Content Title";
        public string YtVideoId { get; set; } = "";
        public string? Text { get; set; }
        public int Duration { get; set; }
        public virtual List<Comment> QA { get; set; } = new List<Comment>();
#nullable disable
        public byte[] Document { get; set; }
    }
}