using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmailNotify.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public Receiver Receiver { get; set; }

        public string Subject { get; set; }

        public NotificationBody Body { get; set; }

        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }
    }
}
