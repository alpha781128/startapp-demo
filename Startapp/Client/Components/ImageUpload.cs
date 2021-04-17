using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiLanguages.Translator;
using Startapp.Client.Repository;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace Startapp.Client.Components
{
    public partial class ImageUpload
    {
        //private ElementReference _input;

        [Parameter] public string ImgUrl { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Table { get; set; }
        [Parameter] public EventCallback<string> OnChange { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IFileReaderService FileReaderService { get; set; }
        [Inject] public IGenericRepository Repository { get; set; }

        private async Task HandleSelected(InputFileChangeEventArgs e)
        {
            //foreach (var file in await FileReaderService.CreateReference(_input).EnumerateFilesAsync())
            //{
            //    if (file != null)
            //    {
            //        var fileInfo = await file.ReadFileInfoAsync();
            //        using (var ms = await file.CreateMemoryStreamAsync(4 * 1024))
            //        {
            //            var content = new MultipartFormDataContent();
            //            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            //            content.Add(new StreamContent(ms, Convert.ToInt32(ms.Length)), "image", fileInfo.Name);

            //            ImgUrl = await Repository.UploadImage(content, Id, Table);

            //            await OnChange.InvokeAsync(ImgUrl);
            //        }
            //    }
            //}

            var imageFiles = e.GetMultipleFiles();
            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null)
                {
                    //var resizedFile = await imageFile.RequestImageFileAsync("image/png", 300, 500);

                    using (var ms = imageFile.OpenReadStream(imageFile.Size))
                    {
                        var content = new MultipartFormDataContent();
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                        content.Add(new StreamContent(ms, Convert.ToInt32(imageFile.Size)), "image", imageFile.Name);
                        ImgUrl = await Repository.UploadImage(content, Id, Table);
                        await OnChange.InvokeAsync(ImgUrl);
                    }
                }
            }
        }

    }
}
