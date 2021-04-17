using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Settings
{
    public class SettingsBase : ComponentBase
    {
        [Inject] public ICustomerService CustomerService { get; set; }
        [Inject] public ILanguageService LanguageService { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }
        [Inject] public UserSettings UserSettings { get; set; }

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        public List<string> Selections { get; set; } = new List<string>();

        public bool FormValid { get; set; }
        public bool IsBusy { get; set; } = false;
        public bool IsDefaultParams { get; set; } = true;

        public EditContext ProfileContext { get; set; }
        public EditContext ConfigurationContext { get; set; }

        protected UserEditViewModel Profile = new UserEditViewModel();
        protected UserConfiguration Configuration = new UserConfiguration();
        protected List<Language> Languages { get; set; } = new List<Language>();
       

        protected override async Task OnInitializedAsync()
        {
            Profile = await CustomerService.GetUserAsync();

            Selections = await CustomerService.GetUserRolesAsync(CurrentUserInfo.CurrentUser.Id);
            Profile.Roles = Selections.ToArray();

            Languages = (await LanguageService.GetAsync()).OrderBy(l => l.Id).ToList();
            if (!string.IsNullOrEmpty(Profile.Configuration))
            {
                Configuration = JsonSerializer.Deserialize<UserConfiguration>(Profile.Configuration);
                IsDefaultParams = ObjectComparer.Equals(new UserConfiguration(), JsonSerializer.Deserialize<UserConfiguration>(Profile.Configuration));
            }

            ProfileContext = new EditContext(Profile);
            ProfileContext.OnFieldChanged += ProfileHandleFieldChanged;
            ConfigurationContext = new EditContext(Configuration);
            ConfigurationContext.OnFieldChanged += HandleFieldChanged;
            await base.OnInitializedAsync();
        }

        protected async Task UpdateUser()
        {
            IsBusy = true; FormValid = false;
            Profile.Configuration = JsonSerializer.Serialize(Configuration).ToString();

            Task<UserEditViewModel> result = CustomerService.UpdateUserAsync(Profile);
            UserEditViewModel user = await result;
            if (result.IsCompletedSuccessfully)
            {
                Profile = user;
                IsBusy = false;
                await JsRuntime.InvokeAsync<string>("SetToLocalStorege", "user-conf", Profile.Configuration);
                IsDefaultParams = ObjectComparer.Equals(new UserConfiguration(), JsonSerializer.Deserialize<UserConfiguration>(Profile.Configuration));
                UserSettings.UserConfiguration = JsonSerializer.Deserialize<UserConfiguration>(Profile.Configuration);
            }
        }
      
        protected async Task ResetParams()
        {
            Configuration = new UserConfiguration();
            //ConfigurationContext = new EditContext(Configuration);
            await UpdateUser();
        }

        private void ProfileHandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = ProfileContext.Validate();
            StateHasChanged();
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = ConfigurationContext.Validate();
            StateHasChanged();
        }
      
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeAsync<string>("MdbSelectInitialization");
            }
        }

        public void Dispose()
        {
            ConfigurationContext.OnFieldChanged -= HandleFieldChanged;
            ProfileContext.OnFieldChanged -= ProfileHandleFieldChanged;
        }
    }
}
