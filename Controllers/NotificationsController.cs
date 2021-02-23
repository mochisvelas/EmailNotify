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

                //Image if any
                string fileName = Path.GetFileNameWithoutExtension(notification.Image.FileName);
                string extension = Path.GetExtension(notification.Image.FileName);
                fileName += DateTime.Now.ToString("yymmssfff") + extension;
                string imagePath = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await notification.Image.CopyToAsync(fileStream);
                }

                //Video if any
                string videoPath = "";
                //_context.Add(notification);
                //await _context.SaveChangesAsync();

                SendEmail(notification.Receiver, notification.Subject, notification.Text,
                    imagePath, fileName, videoPath, notification.Link).Wait();

                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        static async Task SendEmail(string Receiver, string Subject, string Text, string Image, string imageName, string Video, string Link)
        {
            var apiKey = System.IO.File.ReadAllText("C:\\Users\\Brenner\\vsprojects\\EmailNotify/.key.txt");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("velasquezmochis@hotmail.com", "Brenner");
            var to = new EmailAddress(Receiver);
            var subject = Subject;
            var text = Text;
            var html = "<a href="+ Link +">Click this!</a>";
            var message = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                text,
                html);
            var bytes = System.IO.File.ReadAllBytes(Image);
            var file = Convert.ToBase64String(bytes);
            message.AddAttachment(imageName, file);
            var response = await client.SendEmailAsync(message);
        }
    }
}
