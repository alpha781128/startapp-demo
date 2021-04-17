using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Client.Shared.Auth;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Startapp.Client.Shared
{
    public class MainLayoutBase: LayoutComponentBase
    {
        class AppState
        {            
            public string ConfirmEmail { get; set; }
            public string RecoverCode { get; set; }
        }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }
        [Inject] public UserSettings UserSettings { get; set; }
        [Inject] public ILanguageContainerService Translate { get; set; }

        protected RecoverPassword RecoverPassword { get; set; }
        protected ResetPassword ResetPassword { get; set; }
        public string ResetCode { get; set; } = "";

        protected string direction = "auto";
        public UserConfiguration UserConfiguration { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserConfiguration = new UserConfiguration();
            CurrentUserInfo.PropertyChanged += CurrentUserInfoChanged;
            UserSettings.PropertyChanged += SettingsChanged;
            await GetAppStateAsync();
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        var conf = await JsRuntime.InvokeAsync<string>("GetFromLocalStorege", "user-conf");
        //        //await JsRuntime.InvokeAsync<object>("ConsoleLog", conf);
        //        if (!string.IsNullOrEmpty(conf.ToString()))
        //        {
        //            ApplyUserPreferences(conf.ToString());
        //        }
        //    }
        //}

        private async Task GetAppStateAsync()
        {
            string appState = await JsRuntime.InvokeAsync<string>("GetAppStateFromLocalStorege");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Deserialize<AppState>(appState, options);

            if (!string.IsNullOrEmpty(json.ConfirmEmail)) await ConfirmEmailState(json.ConfirmEmail);

            if (!string.IsNullOrEmpty(json.RecoverCode)) await RecoverPasswordState(json.RecoverCode);
          
        }

        private async Task ConfirmEmailState(string type)
        {
            string ModalParams;
            if (type == "success")
            {
                var m = new ModalSuccess
                {
                    Title = Translate.Keys["ConfirmEmail"],
                    Message = Translate.Keys["SuccessEmailConfirmation"]
                };
                ModalParams = JsonSerializer.Serialize(m);
            }
            else
            {
                var m = new ModalError
                {
                    Title = Translate.Keys["ConfirmEmail"],
                    Message = Translate.Keys["FailEmailConfirmation"]
                };
                ModalParams = JsonSerializer.Serialize(m);
            }
            await JsRuntime.InvokeVoidAsync("showModalDialog", ModalParams);
            await JsRuntime.InvokeVoidAsync("RemoveFromLocalStorege", "ConfirmEmail");
        }

        private async Task RecoverPasswordState(string resetCode)
        {
            ResetCode = resetCode;
            await JsRuntime.InvokeVoidAsync("ShowModal", "resetModal");
            await JsRuntime.InvokeVoidAsync("RemoveFromLocalStorege", "RecoverCode");
        }

        void CurrentUserInfoChanged(object sender, EventArgs e)
        {
            string conf = CurrentUserInfo.CurrentUser.Configuration;
            if (!string.IsNullOrEmpty(conf))
            {
                ApplyUserPreferences(conf);
            }
        }

        void SettingsChanged(object sender, EventArgs e)
        {
            string conf = JsonSerializer.Serialize(UserSettings.UserConfiguration);
            if (!string.IsNullOrEmpty(conf))
            {
                ApplyUserPreferences(conf);
            }
        }

        async void ApplyUserPreferences(string conf)
        {
            await JsRuntime.InvokeAsync<string>("SetToLocalStorege", "user-conf", conf);
            UserConfiguration = JsonSerializer.Deserialize<UserConfiguration>(conf);
            if (!string.IsNullOrEmpty(UserConfiguration.Direction))
            {
                direction = UserConfiguration.Direction;
                SetLanguage(UserConfiguration.Language);
            }
            StateHasChanged();
        }

        void SetLanguage(string cultureCode = "en-US")
        {
            //LanguageTrans.SetLanguage(System.Globalization.CultureInfo.GetCultureInfo(cultureCode));
            Translate.CultureCode = cultureCode;
            string[] subs = cultureCode.Split('-');

            FluentValidation.ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo(subs[0]);
        }

        public void Dispose()
        {
            CurrentUserInfo.PropertyChanged -= CurrentUserInfoChanged;
            UserSettings.PropertyChanged -= SettingsChanged;
        }


       


    }
}
