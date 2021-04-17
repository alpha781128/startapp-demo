using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Client.Services;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin.Components
{
   

    public class AddUserDialogBase : ComponentBase
    {

        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public IPictureService PictureService { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Parameter] public EventCallback<bool> CloseEventCallback { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        public string DialogTitle { get; set; }
        public bool IsNew { get; set; } = false;
        public string Collapse { get; set; } = "collapse";
        public string Hidden { get; set; } = "";

        [Parameter] public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public List<RoleVM> RoleVMs { get; set; } = new List<RoleVM>();
        public List<string> Selections { get; set; } = new List<string>();
        public AppUser User { get; set; } = new AppUser();
        public UserEditViewModel UserVM { get; set; } = new UserEditViewModel();

        public async void Show(AppUser user)
        {
            IsNew = false;
            Hidden = "";
            Collapse = "collapse";
            UserVM = new UserEditViewModel();
            if (string.IsNullOrEmpty(user.UserName))
            {
                UserVM.Editibale = true;
                IsNew = true;
                user.IsEnabled = true;
                Hidden = "hidden";
                Collapse = "";
            }
            User = user;
            Mapper.Map<AppUser, UserEditViewModel>(User, UserVM);
            MarkSelectedRoles();
            DialogTitle = IsNew ? $"{Translate.Keys["Add"]} {Translate.Keys["User"]}" : $"{Translate.Keys["Update"]}: {user.FullName}";
         
            StateHasChanged();
            await JsRuntime.InvokeAsync<object>("ShowModal", "userModal");
        }

        void MarkSelectedRoles()
        {
            RoleVMs.Clear();
            foreach (var role in Roles)
            {
                var rolevm = new RoleVM { Id = role.Id, Name = role.Name };
                RoleVMs.Add(rolevm);
            }
            if (User.Roles != null)
            {
                foreach (var role in User.Roles)
                {
                    if (RoleVMs.Find(r => r.Id == role.RoleId) != null)
                    {
                        RoleVMs.Find(r => r.Id == role.RoleId).Checked = true;
                    }
                }
            }
            if (string.IsNullOrEmpty(User.UserName))
            {
                if (RoleVMs.Find(r => r.Name.ToLower() == "user")!=null)
                {
                    RoleVMs.Find(r => r.Name.ToLower() == "user").Checked = true;
                }
            }
            GetAllSelections();
        }

        void GetAllSelections()
        {
            Selections.Clear();
            foreach (var role in RoleVMs)
            {
                if (role.Checked)
                {
                    Selections.Add(role.Name);
                }
            }
            UserVM.Roles = Selections.ToArray();
        }

        protected void CheckboxClicked(ChangeEventArgs e, string id)
        {
            if (RoleVMs.Find(r => r.Id == id) != null)
            {
                bool Checked = RoleVMs.Find(r => r.Id == id).Checked;
                RoleVMs.Find(r => r.Id == id).Checked = !Checked;
                GetAllSelections();
            }
        }

        public async void Close()
        {
            await JsRuntime.InvokeAsync<object>("HideModal", "userModal");
            StateHasChanged();
        }

        public void ChangePassword()
        {
            if (!string.IsNullOrEmpty(User.PasswordHash))
            {
                UserVM.HasPassword = true;
            }
            UserVM.Editibale = true;
        }

        protected async Task HandleValidSubmit()
        {
            if (!UserVM.HasPassword)
            {
                UserVM.CurrentPassword = null;
            }
            if (!UserVM.Editibale)
            {
                UserVM.NewPassword = null;
            }
            UserVM = IsNew ? await UserService.AddAsync(UserVM) : await UserService.UpdateAsync(UserVM.Id, UserVM);
            await JsRuntime.InvokeAsync<object>("HideModal", "userModal");
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();

        }
    }
}
