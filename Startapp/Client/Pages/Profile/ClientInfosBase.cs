using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared.Models;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Profile
{
    public class ClientInfosBase : ComponentBase
    {
        [Inject] public ICustomerService CustomerService { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        public bool FormValid { get; set; } = false;
        public bool IsBusy { get; set; } = false;

        public EditContext ClientInfoContext { get; set; }


        protected ClientInfo ClientInfo { get; set; } = new ClientInfo();

        protected override async Task OnInitializedAsync()
        {
            ClientInfo = await CustomerService.GetClientInfoAsync(CurrentUserInfo.CurrentUser.Id);
            ClientInfoContext = new EditContext(ClientInfo);
            ClientInfoContext.OnFieldChanged += HandleFieldChanged;
        }              

        protected async Task UpdateClient()
        {
            IsBusy = true; FormValid = false;
            ClientInfo.UserId = CurrentUserInfo.CurrentUser.Id;
            Task<ClientInfo> result = CustomerService.UpdateClientInfoAsync(ClientInfo);
            ClientInfo clientInfo = await result;
            if (result.IsCompletedSuccessfully)
            {
                ClientInfo = clientInfo;
                IsBusy = false;
            }
        }            


        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = ClientInfoContext.Validate();
            StateHasChanged();
        }

        public void Dispose()
        {
            ClientInfoContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}
