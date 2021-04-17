using Startapp.Shared.Core;

namespace Startapp.Shared.ViewModels
{
    public class PermissionVM
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool Checked { get; set; } = false;
    }

    public class PermissionViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }


        public static explicit operator PermissionViewModel(ApplicationPermission permission)
        {
            return new PermissionViewModel
            {
                Name = permission.Name,
                Value = permission.Value,
                GroupName = permission.GroupName,
                Description = permission.Description
            };
        }
    }
}
