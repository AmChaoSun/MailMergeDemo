using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MailMergeDemo.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [Display(Name = "Upload Date")]
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; }
        public string Body { get; set; }
    }
}
