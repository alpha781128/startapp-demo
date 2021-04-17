using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using System;
using System.Text.Json;

namespace Startapp.Client.Components
{
    public class ModalDialogBase : ComponentBase
    {       
        private static Action action;
        protected static bool ShowModal { get; set; }
        public static ModalParams ModalParams { get; set; }     

        protected override void OnInitialized()
        {
            action = ShowDialog;

            var m = new ModalSuccess();

            ModalParams = JsonSerializer.Deserialize<ModalParams>(JsonSerializer.Serialize(m));
        }

        [JSInvokable]
        public static void ShowModalDialog(string parameters)
        {
            ModalParams = JsonSerializer.Deserialize<ModalParams>(parameters);
            action.Invoke();
        }

        private void ShowDialog()
        {
            ShowModal = true;
            StateHasChanged();
        }

        protected static void OnClose()
        {
            ShowModal = false;
        }
    }
}
