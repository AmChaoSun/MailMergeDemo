using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailMergeDemo.Models
{
    public class EmailRequest
    {
        public EmailTemplate Template { get; set; }
        public IFormFile UploadFile { get; set; } 
    }
}
