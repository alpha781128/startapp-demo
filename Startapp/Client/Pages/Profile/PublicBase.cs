using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Profile
{
    public class PublicBase : ComponentBase
    {
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ICustomerService CustomerService { get; set; }
        [Inject] public CurrentUserInfo CurrentUserInfo { get; set; }

        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        public List<string> Selections { get; set; } = new List<string>();
        public string ProfilePic { get; set; }
        public bool FormValid { get; set; } = false;
        public bool IsBusy { get; set; } = false;

        public EditContext ProfileContext { get; set; }

        protected UserEditViewModel Profile { get; set; } = new UserEditViewModel();
        protected AppUser User { get; set; } = new AppUser();

        protected override async Task OnInitializedAsync()
        {
            ProfilePic = "images/no-photo.jpg";         

            User = await CustomerService.GetUserAsync(CurrentUserInfo.CurrentUser.Id);
            Mapper.Map<AppUser, UserEditViewModel>(User, Profile);

            Selections = await CustomerService.GetUserRolesAsync(CurrentUserInfo.CurrentUser.Id);
            Profile.Roles = Selections.ToArray();
            if (!string.IsNullOrEmpty(CurrentUserInfo.CurrentUser.Photo))
            {
                ProfilePic = PictureExtensions.ImagePath($"{CurrentUserInfo.CurrentUser.Photo}", "i", DateTime.Now);
            }
            ProfileContext = new EditContext(Profile);
            ProfileContext.OnFieldChanged += HandleFieldChanged;         

        }

        protected async Task UpdateUser()
        {
            IsBusy = true; FormValid = false;
            if (!Profile.HasPassword)
            {
                Profile.CurrentPassword = null;
            }
            if (!Profile.Editibale)
            {
                Profile.NewPassword = null;
            }
            Task<UserEditViewModel> result = CustomerService.UpdateUserAsync(Profile);
            UserEditViewModel user = await result;
            if (result.IsCompletedSuccessfully)
            {
                Profile = user;
                IsBusy = false;
            }
        }       

        public void ChangePassword()
        {
            if (!string.IsNullOrEmpty(User.PasswordHash))
            {
                Profile.HasPassword = true;
            }
            Profile.Editibale = true;
            FormValid = false;
        }

        public void SavePicture(string imgUrl)
        {
            ProfilePic = imgUrl;
        }
        
        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            FormValid = ProfileContext.Validate();
            StateHasChanged();
        }
             
        public void Dispose()
        {
            ProfileContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}
