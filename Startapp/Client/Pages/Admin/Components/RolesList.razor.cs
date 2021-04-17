using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin.Components
{
    public partial class RolesList
    {
        [Parameter] public string Message { get; set; }
        [Parameter] public List<RoleViewModel> Roles { get; set; }
        [Parameter] public EventCallback<RoleViewModel> UpdateEventCallback { get; set; }
        [Parameter] public EventCallback<RoleViewModel> DeleteEventCallback { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }

        protected RoleViewModel Role { get; set; }       


        public async Task Update(RoleViewModel role)
        {
            await UpdateEventCallback.InvokeAsync(role);

            StateHasChanged();
        }

        protected void Delete(RoleViewModel role)
        {
            Role = role;
            DeleteConfirmation.Show(Role.Name);
        }

        protected async Task ConfirmDelete(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await DeleteEventCallback.InvokeAsync(Role);
                StateHasChanged();
            }
        }
    }
}
