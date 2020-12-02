using WebApplication5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class CourseUser
    {
        public string UserId { get; set; }
        public User User{ get; set; }
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollDate { get; set; } = DateTime.Now;
        public bool Listened { get; set; }
        [Range(1.0, 5.0)]
        public double Rating { get; set; }
    }
}
