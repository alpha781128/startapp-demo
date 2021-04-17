using System.Collections.Generic;

namespace Startapp.Shared.Models
{
    public class Order : AuditableEntity
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }
        public string Comments { get; set; }
        //public string CashierId { get; set; }
        public AppUser AppUser { get; set; }
        //public int CustomerId { get; set; }
        //public Customer Customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
