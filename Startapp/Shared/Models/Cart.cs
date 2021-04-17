using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Startapp.Shared.Models
{
    public class CartVM
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItem
    {
        public CartItem()
        {
            Quantity = 0;
            SellingPrice = 0;
        }
        public virtual decimal Amount
        {
            get
            {               
                return Quantity * SellingPrice;
            }
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OldPrice { get; set; }     
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Handling { get; set; }
        public bool Checked { get; set; } = true;
        public ICollection<Picture> Pictures { get; set; }
    }
    public class CartDetailsVM
    {
        public string orderId { get; set; }

        public string cartDetail { get; set; }
    }

    public class CartItemVM
    {
        public CartItemVM()
        {
            Quantity = 0; UnitAmount = 0; Tax = 0;
        }
        public virtual decimal Amount
        {
            get
            {
                return Quantity * UnitAmount;
            }
        }
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Handling { get; set; }

    }

    public class ShoppingCartVM
    {         
        public virtual decimal ItemTotal
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total += item.Quantity * item.UnitAmount;
                }
                return Math.Round(total, 2);
            }
        }
        public virtual decimal TaxTotal
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total += item.Quantity * item.Tax;
                }
                return Math.Round(total, 2) ;
            }
        }
        public virtual decimal Shipping
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total += item.Quantity * item.Shipping;
                }
                return Math.Round(total, 2);
            }
        }
        public virtual decimal Handling
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total += item.Quantity * item.Handling;
                }
                return Math.Round(total, 2);
            }
        }
        public virtual decimal ShippingDiscount
        {
            get
            {               
                return Math.Round((Shipping * 30/100), 2);
            }
        }
        public virtual decimal Total
        {
            get
            {
                decimal  total = ItemTotal + Shipping + Handling + TaxTotal - ShippingDiscount;
                return Math.Round(total, 2);
            }
        }
        public bool SendDetails { get; set; } = true;
        public string CurrencyCode { get; set; } = "USD";       
        public List<CartItemVM> Items { get; set; }
        public ClientInfo ClientInfo { get; set; }
    }

}
