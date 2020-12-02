using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Image
    {
        [Key]
        public string ImageId { get; set; } = Guid.NewGuid().ToString("N");
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public string ImageText { get; set; }
    }
}