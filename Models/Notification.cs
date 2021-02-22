using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailNotify.Models
{
    public class Notification
    {

        public string Receiver { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }

        public string Video { get; set; }

        public string Link { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime SentDate { get; set; }
    }
}
