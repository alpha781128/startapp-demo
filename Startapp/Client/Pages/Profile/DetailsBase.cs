using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared.Models;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Profile
{
    public class DetailsBase : ComponentBase
    {
        [Inject] public ICustomerService CustomerService { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        public bool FormValid { get; set; } = false;
        public bool IsBusy { get; set; } = false;

        public EditContext CustomerContext { get; set; }


        protected Customer Customer { get; set; } = new Customer();

        protected override async Task OnInitializedAsync()
        {
            Customer = await CustomerService.GetAsync(CurrentUserInfo.CurrentUser.Id);
            CustomerContext = new EditContext(Customer);
            CustomerContext.OnFieldChanged += HandleFieldChanged;
        }              

        protected async Task UpdateCustomer()
        {
            IsBusy = true; FormValid = false;
            Customer.UserId = CurrentUserInfo.CurrentUser.Id;
            Task<Customer> result = CustomerService.UpdateAsync(Customer);
            Customer customer = await result;
            if (result.IsCompletedSuccessfully)
            {
                Customer = customer;
                IsBusy = false;
            }
        }            


        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = CustomerContext.Validate();
            StateHasChanged();
        }

        public void Dispose()
        {
            CustomerContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}
