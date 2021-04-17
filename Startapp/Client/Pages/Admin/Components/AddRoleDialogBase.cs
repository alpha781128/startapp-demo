using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Client.Services;
using Startapp.Shared.Core;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin.Components
{   

    public class AddRoleDialogBase : ComponentBase
    {

        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IRoleService RoleService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Parameter] public EventCallback<bool> CloseEventCallback { get; set; }

        protected static ReadOnlyCollection<ApplicationPermission> Permissions = ApplicationPermissions.AllPermissions;


        public string DialogTitle { get; set; }
        public bool IsNew { get; set; } = false;

        public List<PermissionVM> PermissionVMs { get; set; } = new List<PermissionVM>();
        public RoleViewModel Role { get; set; } = new RoleViewModel();

        public async void Show(RoleViewModel role)
        {
            IsNew = false;
            if (string.IsNullOrEmpty(role.Name))
            {
                IsNew = true;
            }
            Role = role;
            MarkSelectedPermission();
            DialogTitle = IsNew ? $"{Translate.Keys["Add"]} {Translate.Keys["Role"]}" : $"{Translate.Keys["Update"]}: {role.Name}";        

            StateHasChanged();
            await JsRuntime.InvokeAsync<object>("ShowModal", "roleModal");
        }

        void MarkSelectedPermission()
        {
            PermissionVMs.Clear();
            foreach (var p in Permissions)
            {
                var pvm = new PermissionVM();
                Mapper.Map<ApplicationPermission, PermissionVM>(p, pvm);
                PermissionVMs.Add(pvm);
            }
            if (Role.Permissions != null)
            {
                foreach (var permission in Role.Permissions)
                {
                    if (PermissionVMs.Find(p => p.Name == permission.Name) != null)
                    {
                        PermissionVMs.Find(p => p.Name == permission.Name).Checked = true;
                    }
                }
            }
            GetAllSelections();
        }

        void GetAllSelections()
        {
            var permissions = new List<PermissionViewModel>();
            foreach (var permission in PermissionVMs)
            {
                if (permission.Checked)
                {
                    var pvm = new PermissionViewModel();
                    Mapper.Map<PermissionVM, PermissionViewModel>(permission, pvm);
                    permissions.Add(pvm);
                }               
            }

            Role.Permissions = permissions.ToArray();
        }

        protected void CheckboxClicked(ChangeEventArgs e, string name)
        {
            if (PermissionVMs.Find(p => p.Name == name) != null)
            {
                bool Checked = PermissionVMs.Find(p => p.Name == name).Checked;
                PermissionVMs.Find(p => p.Name == name).Checked = !Checked;
                GetAllSelections();                
            }
        }

        public async void Close()
        {
            await JsRuntime.InvokeAsync<object>("HideModal", "roleModal");
            StateHasChanged();
        }       

        protected async Task HandleValidSubmit()
        {
           
            Role = IsNew ? await RoleService.AddAsync(Role) : await RoleService.UpdateAsync(Role.Id, Role);
            await JsRuntime.InvokeAsync<object>("HideModal", "roleModal");
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();

        }
    }
}
