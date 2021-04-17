using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using System.Threading.Tasks;

namespace Startapp.Client.Components
{
    public class ConfirmBase : ComponentBase
    {
        protected bool ShowConfirmation { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Parameter]
        public string ConfirmationTitle { get; set; } = "Confirm Delete" ;
        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        public string ConfirmationMessage { get; set; } 

        public void Show(string msg)
        {
            ConfirmationTitle = Translate.Keys["ConfirmDelete"];
            ConfirmationMessage = Translate.Keys["AreYouSureToDel"] + " " + msg;
            ShowConfirmation = true;
            StateHasChanged();
        }


        protected async Task OnConfirmationChange(bool value)
        {
            ShowConfirmation = false;
            await ConfirmationChanged.InvokeAsync(value);
        }
    }
}