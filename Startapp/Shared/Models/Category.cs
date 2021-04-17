using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class Category : AuditableEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
