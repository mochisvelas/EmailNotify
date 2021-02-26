using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmailNotify.Models
{
    public class Receiver
    {
        public int Id { get; set; }

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")] 
        [Required]
        public string Email { get; set; }

        [StringLength(30)]
        [Required]
        public string Name { get; set; }

        public bool Checked { get; set; }
    }
}
