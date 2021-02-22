using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmailNotify.Data;
using EmailNotify.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailNotify.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ReceiverContext _context;

        public NotificationsController(ReceiverContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Receiver.ToListAsync());
        }

        //GET
        public async Task<IActionResult> Notify(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var receiver = await _context.Receiver.FindAsync(id);
            
            if (receiver == null)
            {
                return NotFound();
            }
            ViewData["Email"] = receiver.Email;
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Notify([Bind("Receiver, Subject, Text, Image, Video, Link")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                SendEmail(notification.Receiver, notification.Subject, notification.Text).Wait();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        static async Task SendEmail(string Receiver, string Subject, string Text)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("velasquezmochis@hotmail.com", "Brenner");
            var to = new EmailAddress(Receiver);
            var subject = Subject;
            var text = Text;
            var html = "";
            var message = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                text,
                html);

            var response = await client.SendEmailAsync(message);
        }
    }
}
