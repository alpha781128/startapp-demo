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
    public class RegisterDialogBase : ComponentBase 
    {      
        public bool FormValid { get; set; }
        public bool IsBusy { get; set; } = false;
        public EditContext EditContext { get; set; }
        public string message = string.Empty;

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] IAuthService AuthService { get; set; }
                   
        protected RegisterViewModel User { get; set; } = new RegisterViewModel();

        public async void GoToLogin()
        {
            //LoginModel.UserName = "me@gmail.com";
            await CloseRegisterModal(false);
            await JsRuntime.InvokeAsync<object>("ShowModal", "loginModal");
        }

        public async Task RegisterUser()
        {
            IsBusy = true; FormValid = false;

            Task<LoginResponse> result = AuthService.RegisterUserAsync(User);
            LoginResponse lr = await result;
            if (string.IsNullOrEmpty(lr.AccessToken))
            {
                message = lr.Message;
                IsBusy = false;
                await JsRuntime.InvokeAsync<object>("AlertShow", "RegisterAlert", message);
                AuthService.SetTimer(Functions.RemainingTime(lr.ExpiresIn));
            }
            else
            {
                message = lr.Message;
                await CloseRegisterModal(true);
            }
        }

        public async Task CloseRegisterModal(bool collapse)
        {
            message = string.Empty;
            IsBusy = false;
            await JsRuntime.InvokeAsync<object>("HideModal", "registerModal", collapse);
        }

        protected override void OnInitialized()
        {
             EditContext = new EditContext(User);
            EditContext.OnFieldChanged += HandleFieldChanged;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = EditContext.Validate();
            if (!string.IsNullOrEmpty(User.Email))
            {
                User.Email = User.Email.ToLower();
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }

    }
}
