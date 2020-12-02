using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
        public class Progress
        {
            [Key]
            public string ProgressId { get; set; } = Guid.NewGuid().ToString("N");
            public CourseUser CourseUser { get; set; }
            public Content Content { get; set; }
            public static int ProgressCounter(List<Progress> progress)
            {
                if (progress != null && progress.Count > 0 && progress[0].CourseUser!=null)
                {
                    return (int)((100 * progress.Count) / progress[0].CourseUser.Course.Sections.SelectMany(el => el.Contents).Count());
                }

                return 0;
            }
        }
}
