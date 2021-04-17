using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Startapp.Shared;
using System;

namespace Startapp.Server.Helpers
{
    public class PicExtensions
    {       
        public static float Ratio(Image image, int desiredWidth)
        {
            float longer = image.Width > image.Height ? image.Width : image.Height;
            float dif;
            float ratio;
            if (desiredWidth > longer)
            {
                dif = desiredWidth - longer;
                ratio = dif * 100 / desiredWidth;
                ratio += 100;
            }
            else
            {
                dif = longer - desiredWidth;
                ratio = dif * 100 / longer;
                ratio = 100 - ratio;
            }
            ratio /= 100;
            return ratio;
        }
        public static string Compress(IFormFile picture, string type, int desiredWidth, string fileName)
        {
            string fullPath;          
            fullPath = PictureExtensions.CreatePath(type, fileName);
            
            using (var image = Image.Load(picture.OpenReadStream()))
            {
                float ratio = Ratio(image, desiredWidth);
                image.Mutate(x => x.Resize(PictureExtensions.RoundOff((int)(image.Width * ratio), 10), PictureExtensions.RoundOff((int)(image.Height * ratio), 10)));
                image.Save(fullPath);
            }
            return PictureExtensions.ImagePath(fileName, type, DateTime.Now);
        }

    }
}
