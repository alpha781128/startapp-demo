using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Client.Services;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Articles
{
    public class ArticleDetailsBase : ComponentBase
    {
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IPictureService PictureService { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Parameter] public string Id { get; set; }

        [Inject] public IArticleService ArticleService { get; set; }

        protected List<PictureVM> Pics { get; set; } = new List<PictureVM>();

        protected Article Article { get; set; } = new Article();


        //protected override async Task OnInitializedAsync()
        //{
           
        //}

        protected override async Task OnParametersSetAsync()
        {
            Id = Id ?? "1";
            Article = await ArticleService.GetAsync(int.Parse(Id));

            foreach (var picture in Article.Pictures)
            {
                var pic = new PictureVM
                {
                    Id = picture.Id,
                    MSource = PictureExtensions.ImagePath(picture.Id + picture.Extension, "m", picture.Created),
                    NSource = PictureExtensions.ImagePath(picture.Id + picture.Extension, "n", picture.Created),
                    Active = ""
                };
                Pics.Add(pic);
            }
            if (Pics.Count > 0)
            {
                Pics.First().Active = "active";
            }
        }


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

        public void GoBackToList()
        {
            Navigation.NavigateTo($"/articles");
        }

    }
}
