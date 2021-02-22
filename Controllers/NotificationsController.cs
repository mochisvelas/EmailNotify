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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace EmailNotify.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ReceiverContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public NotificationsController(ReceiverContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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
        public async Task<IActionResult> Notify([Bind("Receiver, Subject, Text, Image, Video, Link")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(notification.Image);
                string extension = Path.GetExtension(notification.Image);
                notification.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                //using (var fileStream = new FileStream(path, FileMode.Create))
                //{
                //    await notification.Image(fileStream);
                //}

                SendEmail(notification.Receiver, notification.Subject, notification.Text).Wait();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        static async Task SendEmail(string Receiver, string Subject, string Text)
        {
            var apiKey = "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";
            //var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
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
