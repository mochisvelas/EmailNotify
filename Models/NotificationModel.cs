using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailNotify.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }

        public ReceiverModel Receiver { get; set; }

        public string Subject { get; set; }

        public NotificationBodyModel Body { get; set; }
    }
}
