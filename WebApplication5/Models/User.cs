using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models;

namespace WebApplication5.Models
{
    public class User
    {
        [Key]
        [StringLength(50)]
        public string UserId { get; set; }
        [Required]
        public string Username { get; set; } = "Username";
        public Image Image { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<CourseUser> Courses{ get; set; }
        public virtual ICollection<Likes> LikedComments { get; set; }
    }
}
