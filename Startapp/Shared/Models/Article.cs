
using FluentValidation;
using System;
using System.Collections.Generic;

namespace Startapp.Shared.Models
{
    public class Article : AuditableEntity
    {
        public Article()
        {
            Negotiable = Negotiable.Negotiable;
            Duration = Duration.OneMonths;          
            Validated = true;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public Boolean Validated { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfSelled { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Handling { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsDiscontinued { get; set; }
        public Negotiable Negotiable { get; set; }
        public Duration Duration { get; set; }
        public AppUser AppUser { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Picture> Pictures { get; set; }
        public ICollection<Option> Options { get; set; }

    }

    public class EditArticleValidator : AbstractValidator<Article>
    {
        public EditArticleValidator()
        {
            RuleFor(article => article.Title).NotEmpty();
            RuleFor(article => article.Description).NotEmpty();
        }
    }

}
