using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmailNotify.Models;

namespace EmailNotify.Data
{
    public class ReceiverContext : DbContext
    {
        public ReceiverContext(DbContextOptions<ReceiverContext> options) : base (options)
        {
        }

        public DbSet<Receiver> Receiver { get; set; }

        public DbSet<Notification> Notification { get; set; }
    }
}
