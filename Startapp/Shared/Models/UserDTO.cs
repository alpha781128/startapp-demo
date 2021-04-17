using FluentValidation;

namespace Startapp.Shared.Models
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Configuration { get; set; }
        public string FullName { get; set; }
        public string Photo { get; set; }
    }
    public class ClientInfo
    {
        public virtual string FullName
        {
            get
            {
                string fullName = null;
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    fullName += FirstName;
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    fullName += " " + LastName;
                }
                return fullName;
            }
        }

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AdminArea1 { get; set; } 
        public string AdminArea2 { get; set; }
        public string PostalCode { get; set; } 
        public string CountryCode { get; set; } 
    }
    public class ClientInfoValidator : AbstractValidator<ClientInfo>
    {
        public ClientInfoValidator()
        {
            RuleFor(customer => customer.FirstName).NotEmpty();
            RuleFor(customer => customer.LastName).NotEmpty();
            RuleFor(customer => customer.AddressLine1).NotEmpty().Length(10, 50);
            RuleFor(customer => customer.AdminArea1).NotEmpty();
            RuleFor(customer => customer.AddressLine2).NotEmpty();
            RuleFor(customer => customer.AdminArea2).NotEmpty();
            RuleFor(customer => customer.PostalCode).NotEmpty();
            RuleFor(customer => customer.CountryCode).NotEmpty();
        }
    }
}
