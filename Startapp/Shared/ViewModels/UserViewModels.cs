
using FluentValidation;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Startapp.Shared.ViewModels
{

    public class UserViewModel : UserBaseViewModel
    {
        public bool IsLockedOut { get; set; }
        public bool EmailConfirmed { get; set; }

        [MinimumCount(1, ErrorMessage = "Roles cannot be empty")]
        public string[] Roles { get; set; }
    }


    public class RegisterViewModel : UserBaseViewModel
    {
        public RegisterViewModel()
        {
            IsEnabled = true;
        }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterUserValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterUserValidator()
        {
            //for password complixity u can add this line 
            //.Matches("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$")
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Email).Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").NotEmpty();
            RuleFor(user => user.NewPassword).NotEmpty().Length(8, 50);
            RuleFor(user => user.ConfirmPassword).NotEmpty().Equal(u => u.NewPassword);
        }
    }

    public class RegisterVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Username is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 200 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }



    public class UserPasswordRecovery
    {
        [Required(ErrorMessage = "Username or email address is required")]
        public string UsernameOrEmail { get; set; }
    }
    public class PasswordRecoverValidator : AbstractValidator<UserPasswordRecovery>
    {
        public PasswordRecoverValidator()
        {
            RuleFor(user => user.UsernameOrEmail).NotEmpty().Length(5, 100);
        }
    }
   
    public class UserPasswordReset
    {
        [Required(ErrorMessage = "Username or email address is required")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required"), MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password reset code is required")]
        public string ResetCode { get; set; }

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
    public class UserPasswordResetValidator : AbstractValidator<UserPasswordReset>
    {
        public UserPasswordResetValidator()
        {
            RuleFor(user => user.UsernameOrEmail).NotEmpty().Length(5, 100);
            RuleFor(user => user.Password).NotEmpty().Length(8, 50);
            RuleFor(user => user.ResetCode).NotEmpty();
            RuleFor(user => user.ConfirmPassword).NotEmpty().Equal(u => u.Password);
        }
    }

    public class UserPatchViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }
    }



    public abstract class UserBaseViewModel
    {
        public UserBaseViewModel()
        {
            var uc = new UserConfiguration();
            Configuration = JsonSerializer.Serialize(uc);
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "User name is required!")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-mail is required!")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Valid email like:'me@gmail.com'")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }

        public bool IsEnabled { get; set; }

        public string ErrorMessage { get; set; }

    }



    public class LoginViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Userame or e-mail")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class LoginUserValidator : AbstractValidator<LoginViewModel>
    {
        public LoginUserValidator()
        {
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Password).NotEmpty().Length(8, 50);
        }
    }


}
