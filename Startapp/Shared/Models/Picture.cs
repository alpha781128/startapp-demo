using System;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class Picture
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public string Extension { get; set; }
        public Article Article { get; set; }
    }
}
