using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace ImageSharingWithUpload.Models
{
    public class Image
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]+", ErrorMessage = "Only alphanumeric allowed")]
        public String Id { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Max 40 Characters")]
        public String Caption { get; set; }
        [StringLength(200, ErrorMessage = "Max 200 Characters")]
        public String Description { get; set; }
        [Required]
        [DataType(DataType.Date, ErrorMessage ="Please Enter Valid Date")]
        public DateTime DateTaken { get; set; }
        public String Userid { get; set; }

        public Image()
        {
        }
    }
}