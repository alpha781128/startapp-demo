using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface IPictureService
    {
        Task<Picture> GetAsync(string picId);
        Task<Picture> AddAsync(Picture picture);
        Task<Picture> UpdateAsync(Picture picture);
        Task<Picture> DeleteAsync(string picId);
    }

    public class PictureService : IPictureService
    {
        private readonly IGenericRepository _genericRepository;
        public PictureService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<Picture> GetAsync(string picId)
        {        
            try
            {
                return await _genericRepository.GetAsync<Picture>($"api/picture/pictures/{picId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new Picture();
            }
        }
        public async Task<Picture> AddAsync(Picture picture)
        {
            try
            {
                var response = await _genericRepository.PostAsync<Picture>($"api/picture/pictures", picture);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return picture;
            }
        }

        public async Task<Picture> UpdateAsync(Picture picture)
        {
            try
            {
                var response = await _genericRepository.PutAsync<Picture>($"api/picture/pictures", picture);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return picture;
            }
        }
        public async Task<Picture> DeleteAsync(string picId)
        {
            try
            {
                return await _genericRepository.DeleteAsync<Picture>($"api/picture/pictures/{picId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }
    }

   
}
