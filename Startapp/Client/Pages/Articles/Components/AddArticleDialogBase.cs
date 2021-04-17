using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Repository;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles.Components
{
    public class AddArticleDialogBase : ComponentBase
    {
        [Inject] public IArticleService ArticleService { get; set; }
        [Inject] public IPictureService PictureService { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }
        protected List<PictureVM> Pics { get; set; } = new List<PictureVM>();

        [Parameter] public EventCallback<bool> CloseEventCallback { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        public string DialogTitle { get; set; }

        public Article Article { get; set; } = new Article();

        public void SavePicture(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var pic = new PictureVM
                {
                    Id = Guid.Parse(Id),
                    MSource = PictureExtensions.ImagePath(Id + ".jpg", "m", DateTime.Now),
                    SSource = PictureExtensions.ImagePath(Id + ".jpg", "s", DateTime.Now),
                };
                Pics.Add(pic);
            }          
        }
        public void AddPic(Picture picture)
        {
            var pic = new PictureVM
            {
                Id = picture.Id,
                MSource = PictureExtensions.ImagePath(picture.Id + picture.Extension, "m", picture.Created),
                SSource = PictureExtensions.ImagePath(picture.Id + picture.Extension, "s", picture.Created),
            };
            Pics.Add(pic);
        }
        //public void RemovePic(Picture picture)
        //{
        //    var pic = Pics.FirstOrDefault(p => p.Id == picture.Id);
        //    if(pic != null)
        //    {
        //        Pics.Remove(pic);
        //    };
        //}

        protected async Task DeletePicture(string picId)
        {
            var result = await PictureService.DeleteAsync(picId);
            if (!string.IsNullOrEmpty(result.Id.ToString()))
            {
                var pic = Article.Pictures.FirstOrDefault(p => p.Id == Guid.Parse(picId));
                if (pic != null)
                {
                    Article.Pictures.Remove(pic);
                }
                Pics.Remove(Pics.FirstOrDefault(p => p.Id == Guid.Parse(picId)));
                StateHasChanged();
            }
        }

        public async void Show(Article article)
        {
            Article = article;
            Pics.Clear();
            if (Article.Pictures != null)
            {
                foreach (var pic in article.Pictures)
                {
                    AddPic(pic);
                }
            }
            DialogTitle = article.Id == 0 ? $"{Translate.Keys["Add"]} {Translate.Keys["Article"]}" : $"{Translate.Keys["Update"]}: {article.Title}";
            StateHasChanged();
            await JsRuntime.InvokeAsync<object>("ShowModal", "articleModal");
        }

        public async void Close()
        {
            await JsRuntime.InvokeAsync<object>("HideModal", "articleModal");
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Article = Article.Id == 0 ? await ArticleService.AddAsync(Article) : await ArticleService.UpdateAsync(Article);
            await JsRuntime.InvokeAsync<object>("HideModal", "articleModal");
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
