using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "User name is required!")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class LoginVMValidator : AbstractValidator<LoginVM>
    {
        public LoginVMValidator()
        {
            //for password complixity u can add this line 
            //.Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$")
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Password).NotEmpty().Length(6, 50);
        }
    }
}
