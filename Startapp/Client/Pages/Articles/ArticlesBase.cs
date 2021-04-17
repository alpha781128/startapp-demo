using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MultiLanguages.Translator;
using Startapp.Client.Pages.Articles.Components;
using Startapp.Client.Services;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles
{
    public class ArticlesBase : ComponentBase
    {
        [CascadingParameter] Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IArticleService ArticleService { get; set; }

        public string Message { get; set; }
        public List<Article> Articles { get; set; } = new List<Article>();
        public MetaData MetaData { get; set; } = new MetaData();
        private PagingParameters PagingParameters = new PagingParameters();

        public string SearchTerm { get; set; } = "";

        public bool Filtred { get; set; } = false;
        public bool Loading { get; set; } = true;
        public bool DisableSearch { get; set; } = true;

        protected AddArticleDialog AddArticleDialog { get; set; }

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
            string url = $"api/article/articles";
            Loading = true;
            Articles.Clear();
            StateHasChanged();
            var pagingResponse = await ArticleService.GetPageAsync(PagingParameters, url);
            Articles = pagingResponse.Items;
            if (Articles.Count == 0)
            {
                Message = $"{Translate.Keys["SorryNo"]} {Translate.Keys["Articles"]} {Translate.Keys["Found"]} ! ";
            }
            MetaData = pagingResponse.MetaData;
            Loading = false;
            StateHasChanged();
           
        }      

        public async void AddUpdateArticle_OnDialogClose()
        {
            await GetArticlesAsync();
        }

        protected async Task AddUpdateArticle(Article article)
        {
            var authenticationState = await AuthenticationStateTask;
            if (authenticationState.User.Identity.Name != "Kevin")
            {
                AddArticleDialog.Show(article);
            }
        }

        protected async Task DeleteArticle(Article article)
        {
            var authenticationState = await AuthenticationStateTask;
            if (authenticationState.User.Identity.Name != "Kevin")
            {
               await ArticleService.DeleteAsync(article.Id);
               Articles.Remove(article);
               StateHasChanged();
            }
        }
    }
}
