namespace Startapp.Shared.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        //public int ProductId { get; set; }
        public Article Article { get; set; }
        public Order Order { get; set; }
    }
}
