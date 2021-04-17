using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Shared;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin.Components
{
    public partial class UsersList
    {
        [Parameter] public string Message { get; set; }
        [Parameter] public List<AppUser> Users { get; set; }
        [Parameter] public EventCallback<AppUser> UpdateEventCallback { get; set; }
        [Parameter] public EventCallback<AppUser> DeleteEventCallback { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }

        protected AppUser User { get; set; }

        public string Picture(string photo)
        {
            var src = PictureExtensions.NoPhoto;
            if (string.IsNullOrEmpty(photo))
            {
                return src;
            }
            src = PictureExtensions.ImagePath($"{photo}", "i", DateTime.Now);

            return src;
        }
         

        public async Task Update(AppUser user)
        {
            await UpdateEventCallback.InvokeAsync(user);

            StateHasChanged();
        }

        protected void Delete(AppUser user)
        {
            User = user;
            DeleteConfirmation.Show(User.FullName);
        }

        protected async Task ConfirmDelete(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await DeleteEventCallback.InvokeAsync(User);
                StateHasChanged();
            }
        }
    }
}
