using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Shared;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles.Components
{
    public partial class ArticleTable
    {
        [Inject] public NavigationManager Navigation { get; set; }
        [Parameter] public string Message { get; set; }
        [Parameter] public List<Article> Articles { get; set; }
        [Parameter] public EventCallback<Article> UpdateEventCallback { get; set; }
        [Parameter] public EventCallback<Article> DeleteEventCallback { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }

        protected Article Article { get; set; }

        public string Picture(Picture picture)
        {
            var src = PictureExtensions.NoImage;
            if (picture == null)
            {
                return src;
            }
            if (!string.IsNullOrEmpty(picture.Id.ToString()))
            {
                src = PictureExtensions.ImagePath(picture.Id + picture.Extension, "m", picture.Created);
            }
            return src;
        }

        public void Detail(int id)
        {
            Navigation.NavigateTo($"/articledetails/{id}");
        }

        public async Task Update(Article article)
        {
            await UpdateEventCallback.InvokeAsync(article);

            StateHasChanged();
        }
             
        protected void Delete(Article article)
        {
            Article = article;
            DeleteConfirmation.Show(Article.Title);
        }

        protected async Task ConfirmDelete(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await DeleteEventCallback.InvokeAsync(Article);
                StateHasChanged();
            }
        }
    }
}
