
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class Customer : AuditableEntity
    {
        public Customer()
        {
            BirthDate =  new DateTime(01/01/2000);
            Gender = Gender.Male;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AdminArea1 { get; set; }
        public string AdminArea2 { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
        public string WebSite { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
   
        public Gender Gender { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    public class EditCustomerValidator : AbstractValidator<Customer>
    {
        public EditCustomerValidator()
        {
            RuleFor(customer => customer.AddressLine1).NotEmpty().Length(6, 50);
            RuleFor(customer => customer.AdminArea1).NotEmpty();
            RuleFor(customer => customer.PostalCode).NotEmpty();
            RuleFor(customer => customer.CountryCode).NotEmpty();
        }
    }
}
