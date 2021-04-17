using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class Option
    {
        public int Id { get; set; }
        public string Property { get; set; }
        [Required]
        public string Value { get; set; }
        public Article Article { get; set; }
    }
}
