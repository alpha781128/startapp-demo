using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages
{
    public class IndexBase: ComponentBase
    {
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IArticleService ArticleService { get; set; }

        public string Message { get; set; }
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
        public MetaData MetaData { get; set; } = new MetaData();
        private PagingParameters PagingParameters = new PagingParameters();

        public string SearchTerm { get; set; } = "";

        public bool Filtred { get; set; } = false;
        public bool Loading { get; set; } = true;
        public bool DisableSearch { get; set; } = true;

        protected async override Task OnInitializedAsync()
        {
            Message = string.Empty;
            await GetArticlesAsync();
        }
        protected async Task SelectedPage(int page)
        {
            PagingParameters.PageNumber = page;
            await GetArticlesAsync();
        }
        protected async void Search()
        {
            PagingParameters.SearchTerm = SearchTerm;
            Filtred = true; DisableSearch = true;
            await GetArticlesAsync();
        }
        protected void SearchKeyup(ChangeEventArgs e)
        {
            SearchTerm = e.Value.ToString();

            DisableSearch = string.IsNullOrWhiteSpace(SearchTerm.Trim()) ? true : false;
        }

        protected void EnterKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                Search();
            }
        }

        protected async void Filter()
        {
            SearchTerm = "";
            PagingParameters.SearchTerm = SearchTerm;
            Filtred = false; DisableSearch = true;
            await GetArticlesAsync();
        }

        private async Task GetArticlesAsync()
        {
            string url = $"api/article/articles/newest";
            Loading = true;
            Articles.Clear();
            StateHasChanged();
            var pagingResponse = await ArticleService.GetVMPageAsync(PagingParameters, url);
            Articles = pagingResponse.Items;
            if (Articles.Count == 0)
            {
                Message = $"{Translate.Keys["SorryNo"]} {Translate.Keys["Articles"]} {Translate.Keys["Found"]} ! ";
            }
            MetaData = pagingResponse.MetaData;
            Loading = false;
            StateHasChanged();

        }


    }
}
