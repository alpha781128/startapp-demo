using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Components;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles.Components
{
    public partial class TopViewed
    {
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        [Inject] public IArticleService ArticleService { get; set; }

        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
        public MetaData MetaData { get; set; } = new MetaData();
        private PagingParameters PagingParameters = new PagingParameters();

        public bool Loading { get; set; } = true;

        protected async override Task OnInitializedAsync()
        {
            await GetArticlesAsync();
        }

        protected async Task SelectedPage(int page)
        {
            PagingParameters.PageNumber = page;
            await GetArticlesAsync();
        }

        private async Task GetArticlesAsync()
        {
            string url = $"api/article/articles/topviewed";
            Loading = true;
            Articles.Clear();
            StateHasChanged();
            var pagingResponse = await ArticleService.GetVMPageAsync(PagingParameters, url);
            Articles = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
            Loading = false;
            StateHasChanged();
        }
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

    }
}
