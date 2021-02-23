using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EmailNotify.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Receiver { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        public string VideoName { get; set; }
        [NotMapped]
        public IFormFile Video { get; set; }

        public string Link { get; set; }

        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }
    }
}
