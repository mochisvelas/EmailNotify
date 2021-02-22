using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmailNotify.Data;
using EmailNotify.Models;

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
            return View(receiver);
        }
    }
}
