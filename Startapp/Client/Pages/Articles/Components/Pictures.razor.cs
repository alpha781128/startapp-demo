using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Client.Services;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles.Components
{
    public partial class Pictures
    {

        [Parameter] public EventCallback<string> DeleteEventCallback { get; set; }
        [Parameter] public List<PictureVM> Pics { get; set; } = new List<PictureVM>();
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        public string PicId { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }

        protected void Delete(string picId)
        {
            PicId = picId;
            DeleteConfirmation.Show("picture with id: " + picId);
        }

        protected async Task ConfirmDelete(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await DeleteEventCallback.InvokeAsync(PicId);
                StateHasChanged();
            }
        }
    }
}
