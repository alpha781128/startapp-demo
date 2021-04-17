using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Models;
using System;

namespace Startapp.Client.Shared
{
    public class NavigationBase: ComponentBase
    {
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        public int Number { get; set; } = 0;

        public UserProfile Profile { get; set; }

        protected LoginDisplay LoginDisplay { get; set; }

        protected override void OnInitialized()
        {
            Profile = new UserProfile();
            CurrentUserInfo.PropertyChanged += CurrentUserInfoChanged;
        }

        void CurrentUserInfoChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentUserInfo.CurrentUser.FullName))
            {
                Profile.FullName = "No name"; 
            }
            else
            {
                Profile.FullName = CurrentUserInfo.CurrentUser.FullName;
            }
            if (!string.IsNullOrEmpty(CurrentUserInfo.CurrentUser.Photo))
            {
                Profile.Pic = PictureExtensions.ImagePath($"{CurrentUserInfo.CurrentUser.Photo}", "i", DateTime.Now);
            }
            StateHasChanged();
        }
        public void OnLogOutAsync()
        {
            Profile = new UserProfile();
            StateHasChanged();
        }

        public void Dispose()
        {
            CurrentUserInfo.PropertyChanged -= CurrentUserInfoChanged;
        }

       
    }
}
