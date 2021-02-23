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
using Newtonsoft.Json;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<Receiver> selection)
        {
            if (selection == null)
            {
                return NotFound();
            }

            foreach (var selected in selection)
            {
                var receiver = await _context.Receiver.FindAsync(selected.Id);

                if (receiver == null)
                {
                    return NotFound();
                }
            }

            //TempData["checkedReceivers"] = selection.Where(x => x.Checked).ToList();
            TempData["checkedReceivers"] = JsonConvert.SerializeObject(selection.Where(x => x.Checked).ToList());

            return RedirectToAction("NotifySelected");
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

        public IActionResult NotifySelected()
        {
            ViewData["checkedReceivers"] = JsonConvert.DeserializeObject<List<Receiver>>((string)TempData["checkedReceivers"]);

            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notify([Bind("Receiver, Subject, Text, Image, Video, Link, SentDate")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string imagePath = "";
                string videoPath = "";
                notification.SentDate = DateTime.Now;

                //Image if any
                if(notification.Image != null)
                {
                    string imageName = Path.GetFileNameWithoutExtension(notification.Image.FileName);
                    string imageExtension = Path.GetExtension(notification.Image.FileName);
                    notification.ImageName = imageName += DateTime.Now.ToString("yymmssfff") + imageExtension;
                    imagePath = Path.Combine(wwwRootPath + "/Image/", imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await notification.Image.CopyToAsync(fileStream);
                    }
                }

                //Video if any
                if (notification.Video != null)
                {
                    string videoName = Path.GetFileNameWithoutExtension(notification.Video.FileName);
                    string videoExtension = Path.GetExtension(notification.Video.FileName);
                    notification.VideoName = videoName += DateTime.Now.ToString("yymmssfff") + videoExtension;
                    videoPath = Path.Combine(wwwRootPath + "/Video/", videoName);
                    using (var fileStream = new FileStream(videoPath, FileMode.Create))
                    {
                        await notification.Video.CopyToAsync(fileStream);
                    }
                }

                _context.Add(notification);
                await _context.SaveChangesAsync();

                SendEmail(notification.Receiver, notification.Subject, notification.Text,
                    imagePath, videoPath, notification.Link).Wait();

                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifySelected([Bind("Receiver, Subject, Text, Image, Video, Link, SentDate")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string imagePath = "";
                string videoPath = "";
                notification.SentDate = DateTime.Now;

                //Image if any
                if (notification.Image != null)
                {
                    string imageName = Path.GetFileNameWithoutExtension(notification.Image.FileName);
                    string imageExtension = Path.GetExtension(notification.Image.FileName);
                    notification.ImageName = imageName += DateTime.Now.ToString("yymmssfff") + imageExtension;
                    imagePath = Path.Combine(wwwRootPath + "/Image/", imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await notification.Image.CopyToAsync(fileStream);
                    }
                }

                //Video if any
                if (notification.Video != null)
                {
                    string videoName = Path.GetFileNameWithoutExtension(notification.Video.FileName);
                    string videoExtension = Path.GetExtension(notification.Video.FileName);
                    notification.VideoName = videoName += DateTime.Now.ToString("yymmssfff") + videoExtension;
                    videoPath = Path.Combine(wwwRootPath + "/Video/", videoName);
                    using (var fileStream = new FileStream(videoPath, FileMode.Create))
                    {
                        await notification.Video.CopyToAsync(fileStream);
                    }
                }

                _context.Add(notification);
                await _context.SaveChangesAsync();

                SendEmail(notification.Receiver, notification.Subject, notification.Text,
                    imagePath, videoPath, notification.Link).Wait();

                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }


        static async Task SendEmail(string Receiver, string Subject, string Text, string Image, string Video, string Link)
        {
            var apiKey = System.IO.File.ReadAllText("C:\\Users\\Brenner\\vsprojects\\EmailNotify/.key.txt");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("velasquezmochis@hotmail.com", "Brenner");
            var to = new EmailAddress(Receiver);
            var subject = Subject;
            var body = Text;
            var html = "<a href="+ Link +">Click this!</a>";
            var message = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                body,
                html);

            byte[] bytes;
            string file;
            string name;

            //Attach image if any
            if(Image != "")
            {
                bytes = System.IO.File.ReadAllBytes(Image);
                file = Convert.ToBase64String(bytes);
                name = Path.GetFileName(Image);
                message.AddAttachment(name, file);
            }

            //Attach video if any
            if (Video != "")
            {
                bytes = System.IO.File.ReadAllBytes(Video);
                file = Convert.ToBase64String(bytes);
                name = Path.GetFileName(Video);
                message.AddAttachment(name, file);
            }
            var response = await client.SendEmailAsync(message);
        }
    }
}
