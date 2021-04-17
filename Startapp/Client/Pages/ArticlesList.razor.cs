using Microsoft.AspNetCore.Components;
using MultiLanguages.Translator;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;

namespace Startapp.Client.Pages
{
    public partial class ArticlesList
    {
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Parameter] public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();

        [Parameter] public string Message { get; set; }

        [Parameter] public bool Loading { get; set; } = true;       

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
