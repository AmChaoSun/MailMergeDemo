using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MailMergeDemo.Data;
using MailMergeDemo.Models;
using MailMergeDemo.Helper;

namespace MailMergeDemo.Controllers
{
    public class EmailTemplatesController : Controller
    {
        private readonly MailMergeDemoContext _context;

        public EmailTemplatesController(MailMergeDemoContext context)
        {
            _context = context;
        }

        // GET: EmailTemplates
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmailTemplate.ToListAsync());
        }

        // GET: EmailTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailTemplate = await _context.EmailTemplate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            return View(emailTemplate);
        }

        // GET: EmailTemplates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,UploadDate,Body")] EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailTemplate = await _context.EmailTemplate.FindAsync(id);
            if (emailTemplate == null)
            {
                return NotFound();
            }
            return View(emailTemplate);
        }

        // POST: EmailTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,UploadDate,Body")] EmailTemplate emailTemplate)
        {
            if (id != emailTemplate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailTemplateExists(emailTemplate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailTemplate = await _context.EmailTemplate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            return View(emailTemplate);
        }

        // POST: EmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailTemplate = await _context.EmailTemplate.FindAsync(id);
            _context.EmailTemplate.Remove(emailTemplate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: EmailTemplates/Send/5
        public async Task<IActionResult> Send(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailTemplate = await _context.EmailTemplate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            return View(new EmailRequest { Template = emailTemplate } );
        }

        // POST: EmailTemplates/Send/5
        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendConfirmed([FromForm]EmailRequest request)
        {
            var emailTemplate = await _context.EmailTemplate
                .FirstOrDefaultAsync(m => m.Id == request.Template.Id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            await MailerHelper.SendBulkEmailsAsync(emailTemplate, request.UploadFile);
            return RedirectToAction(nameof(Index));
        }

        private bool EmailTemplateExists(int id)
        {
            return _context.EmailTemplate.Any(e => e.Id == id);
        }
    }
}
