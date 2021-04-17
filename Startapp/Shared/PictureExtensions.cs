using System;
using System.IO;

namespace Startapp.Shared
{
    public class PictureExtensions
    {      

        public static string NoImage = $"/images/no-image.jpg";
        public static string NoPhoto = $"/images/no-photo.jpg";
        public static string Domain = $"https://localhost:44385/";
        public static string ImagesPath = $"StaticFiles/Images";

        public static void DeleteFile(string type, string fileName)
        {
            string fullPath;

            fullPath = CreatePath(type, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        public static string ImagePath(string fileName, string type, DateTime date)
        {
            //string dmn = "https://startapp-pro.images.com"; //for a verry large project when you expose a big numbers of images you can,
            //host yours images as Independent project with a different host
            //string dmn = Domain + ImagesPath;
            string fullPath = Domain + ImagesPath + GetPath(type, date) + fileName;
            return fullPath;
        }
        public static string GetPath(string type, DateTime date)
        {
            string picPath;
            if (type == "i")
            {
                picPath = "/identity/";
            }
            else
            {
                picPath = $"/startapp/{date.Year}/{date.Month}/{type}/";
            }
            return picPath;
        }
        public static string CreatePath(string type, string fileName)
        {
            //string picPath = @"E:/images/"; // you can also use a external hard drive (Network hdd) to save your images,
            //if you consume a big data images
            string picPath = ImagesPath;
            picPath += GetPath(type, DateTime.Now);
            if (!Directory.Exists(picPath))
            { Directory.CreateDirectory(picPath); };
            return picPath + fileName;
        }
        public static int RoundOff(int number, int interval)
        {
            int remainder = number % interval;
            number += (remainder < interval / 2) ? -remainder : (interval - remainder);
            return number;
        }
    }

}
