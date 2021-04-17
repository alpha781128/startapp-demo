using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Threading.Tasks;

namespace Startapp.Client.Shared.Auth
{
    public class LoginDialogBase : ComponentBase
    {
        public bool FormValid { get; set; }
        public bool IsBusy { get; set; } = false;

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] IAuthService AuthService { get; set; }

        public EditContext EditContext { get; set; }
        string message = string.Empty;

        protected LoginVM User { get; set; } = new LoginVM();

        public async void GoToRegister()
        {
            await CloseLoginModal(false);
            await JsRuntime.InvokeAsync<object>("ShowModal", "registerModal");
        }
        public async void GoToRecover()
        {
            await CloseLoginModal(false);
            await JsRuntime.InvokeAsync<object>("ShowModal", "recoverModal");
        }

        public async Task LoginUser() 
        {
            IsBusy = true; FormValid = false;
            Task<LoginResponse> result = AuthService.LogWithPasswordAsync(User);
            LoginResponse lr = await result;
            if (string.IsNullOrEmpty(lr.AccessToken))
            {
                message = lr.Message;
                IsBusy = false;
                await JsRuntime.InvokeAsync<object>("AlertShow", "LoginAlert", message);
                AuthService.SetTimer(Functions.RemainingTime(lr.ExpiresIn));
            }
            else
            {
                message = lr.Message;
                await CloseLoginModal(true);
            }
        }

        public async Task CloseLoginModal(bool collapse)
        {
            message = string.Empty;
            IsBusy = false;
            await JsRuntime.InvokeAsync<object>("HideModal", "loginModal", collapse);
        }

        protected override void OnInitialized()
        {
            EditContext = new EditContext(User);
            EditContext.OnFieldChanged += HandleFieldChanged;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = EditContext.Validate();
            StateHasChanged();
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}
