using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MailMergeDemo.Models;

namespace MailMergeDemo.Data
{
    public class MailMergeDemoContext : DbContext
    {
        public MailMergeDemoContext (DbContextOptions<MailMergeDemoContext> options)
            : base(options)
        {
        }

        public DbSet<MailMergeDemo.Models.EmailTemplate> EmailTemplate { get; set; }
    }
}
