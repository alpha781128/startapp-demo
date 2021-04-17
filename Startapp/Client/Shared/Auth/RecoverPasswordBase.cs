using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace Startapp.Client.Shared.Auth
{
    public class RecoverPasswordBase: ComponentBase
    {
        public bool FormValid { get; set; }
        public bool IsBusy { get; set; } = false;

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] IAuthService AuthService { get; set; }

        public EditContext EditContext { get; set; }

        protected UserPasswordRecovery UserRecovery { get; set; } = new UserPasswordRecovery();
      

        public async Task RecoverPassword()
        {
            IsBusy = true; FormValid = false;
            string ModalParams;
            var result = AuthService.RecoverPasswordAsync(UserRecovery);
            UserPasswordRecovery lr = await result;
            if (!string.IsNullOrEmpty(lr.UsernameOrEmail))
            {
                var m = new ModalSuccess
                {
                    Title = Translate.Keys["Recover"],
                    Message = $"{ Translate.Keys["CheckYourEmail"] }" 
                };
                ModalParams = JsonSerializer.Serialize(m);
            }
            else
            {
                var m = new ModalError
                {
                    Title = Translate.Keys["Recover"],
                    Message = $"{ Translate.Keys["UNotFoundOrMNotConfirmed"] }"
                };
                ModalParams = JsonSerializer.Serialize(m);
            }
            await CloseLoginModal(true);
            await JsRuntime.InvokeVoidAsync("showModalDialog", ModalParams);
        }

        public async Task CloseLoginModal(bool collapse)
        {
            IsBusy = false;
            await JsRuntime.InvokeAsync<object>("HideModal", "recoverModal", collapse);
            //await JsRuntime.InvokeAsync<object>("ShowModal", "loginModal");
        }

        protected override void OnInitialized()
        {
            EditContext = new EditContext(UserRecovery);
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
