using AutoMapper;
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
    public class ResetPasswordBase : ComponentBase
    {
        public bool FormValid { get; set; }
        public bool IsBusy { get; set; } = false;

        [Inject] public IMapper Mapper { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] IAuthService AuthService { get; set; }
        [Parameter] public string ResetCode { get; set; } = "";

        public EditContext EditContext { get; set; }

        protected UserPasswordReset PasswordReset { get; set; } = new UserPasswordReset();


        public async Task ResetPassword()
        {
            IsBusy = true; FormValid = false;
            string ModalParams;
            var result = AuthService.ResetPasswordAsync(PasswordReset);
            UserPasswordReset lr = await result;
            if (!string.IsNullOrEmpty(lr.UsernameOrEmail))
            {
                var m = new ModalSuccess
                {
                    Title = Translate.Keys["Reset"],
                    Message = $"{ Translate.Keys["SuccessPasswordUpdate"] }"
                };
                ModalParams = JsonSerializer.Serialize(m);
            }
            else
            {
                var m = new ModalError
                {
                    Title = Translate.Keys["Reset"],
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
            await JsRuntime.InvokeAsync<object>("HideModal", "resetModal", collapse);
            //await JsRuntime.InvokeAsync<object>("ShowModal", "loginModal");
        }

        protected override void OnInitialized()
        {
            EditContext = new EditContext(PasswordReset);
            EditContext.OnFieldChanged += HandleFieldChanged;
        }
      
        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = EditContext.Validate();
            PasswordReset.ResetCode = ResetCode;
            StateHasChanged();
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }
    }

}
