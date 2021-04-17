using System;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }

    public class AuditableEntity : IAuditableEntity
    {
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
    }


}
