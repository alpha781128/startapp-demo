

using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Startapp.Shared.ViewModels
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string Detail { get; set; }
        public Boolean Validated { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfSelled { get; set; }
        //public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsDiscontinued { get; set; }
        public Negotiable Negotiable { get; set; }
        public Duration Duration { get; set; }
        public Customer Customer { get; set; }


        public Category Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Picture> Pictures { get; set; }
        public ICollection<Option> Options { get; set; }
    }
}
