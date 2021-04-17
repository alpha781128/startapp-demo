
using FluentValidation;
using Startapp.Shared.Helpers;
using System.ComponentModel.DataAnnotations;


namespace Startapp.Shared.ViewModels
{
    public class UserEditViewModel : UserBaseViewModel
    {
        public bool HasPassword { get; set; } = false;
        public bool Editibale { get; set; } = false;

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [MinimumCount(1, ErrorMessage = "Roles cannot be empty")]
        public string[] Roles { get; set; }
    }

    public class EditUserValidator : AbstractValidator<UserEditViewModel>
    {
        public EditUserValidator()
        {
            //for password complixity u can add this line 
            //.Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$")
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Email).Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").NotEmpty();
            RuleFor(user => user.CurrentPassword).NotEmpty().Length(8, 50).When(user => user.HasPassword);
            RuleFor(user => user.NewPassword).NotEmpty().Length(8, 50).When(user => user.Editibale);
            RuleFor(user => user.ConfirmPassword).NotEmpty().Equal(u => u.NewPassword)
                   .When(user => user.Editibale);
            RuleFor(user => user.Roles).NotNull();
        }
    }


}
