

using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Startapp.Shared.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        //[Required(ErrorMessage = "Role name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int UsersCount { get; set; }

        public PermissionViewModel[] Permissions { get; set; }

        public class EditRoleValidator : AbstractValidator<RoleViewModel>
        {
            public EditRoleValidator()
            {
                RuleFor(role => role.Name).NotEmpty().Length(4, 50);
                RuleFor(role => role.Description).NotEmpty().Length(4, 50);
            }
        }
    }
}
