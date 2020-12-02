using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Likes
    {
        public string UserId { get; set; }
        public User User { get; set; } = new User() { Username = "Username" };
        public string IdComment { get; set; }
        public Comment Comment { get; set; } = new Comment() { Title = "Title", Body = "Body", Author = "Author" };
    }
}
