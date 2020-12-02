
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models;

namespace WebApplication5.Models
{
    
    public class Course
    {
        [Key]
        public string CourseId { get; set; }
        [Required, StringLength(70)]
        public string Title { get; set; }
        [Required, StringLength(200)]
        public string TitleDescription { get; set; }
        [StringLength(4000)]
        public string Description{ get; set; }
        public string Keywords { get; set; }
        public Image Image { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public Categories Category { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Published { get; set; } = false;
        public virtual List<Section> Sections{ get; set; } = new List<Section>();
        public virtual List<Comment> Comments{ get; set; }
        public virtual ICollection<CourseUser> Participants { get; set; }

        // return how much time passed since date object
        public string GetTimeSince()
        {
            // here we are going to subtract the passed in DateTime from the current time converted to UTC
            TimeSpan ts = DateTime.Now.ToUniversalTime().Subtract(this.UpdateDate.ToUniversalTime());
            int intDays = ts.Days;
            int intHours = ts.Hours;
            int intMinutes = ts.Minutes;
            int intSeconds = ts.Seconds;

            if (intDays > 0)
                return string.Format("{0}", this.UpdateDate.ToShortDateString());
            if (intHours > 0)
                return string.Format("{0} hours ago", intHours);
            if (intMinutes > 0)
                return string.Format("{0} minutes ago", intMinutes);
            if (intSeconds > 0)
                return string.Format("{0} seconds ago", intSeconds);

            //future times
            if (intDays < 0)
                return string.Format("in {0} days", Math.Abs(intDays));
            if (intHours < 0)
                return string.Format("in {0} hours", Math.Abs(intHours));
            if (intMinutes < 0)
                return string.Format("in {0} minutes", Math.Abs(intMinutes));
            if (intSeconds < 0)
                return string.Format("in {0} seconds", Math.Abs(intSeconds));
            return "a bit ";
        }
        public List<string> GetKeywords() => Keywords.Split(";").Select(el => el).ToList();
    }
    public enum CourseInfo
    {
        Start,
        Continue,
        Enroll,
        Edit
    }
}
