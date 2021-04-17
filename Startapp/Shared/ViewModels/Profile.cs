using FluentValidation;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Startapp.Shared.ViewModels
{
    public class ProfileVM
    {
        public ProfileVM()
        {
            BirthDate = new DateTime(01/01/2000);
            Gender = Gender.Male;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
        public string WebSite { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }

    }

    public class ProfileVMValidator : AbstractValidator<ProfileVM>
    {
        public ProfileVMValidator()
        {
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Email).Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").NotEmpty();
            RuleFor(user => user.FirstName).NotEmpty().Length(4, 50);
            RuleFor(user => user.LastName).NotEmpty().Length(4, 50);
            RuleFor(user => user.Address).NotEmpty().Length(4, 50);
            RuleFor(user => user.City).NotEmpty().Length(4, 50);
            RuleFor(user => user.State).NotEmpty().Length(4, 50);
            RuleFor(user => user.Country).NotEmpty().Length(4, 50);
        }
    }
}
