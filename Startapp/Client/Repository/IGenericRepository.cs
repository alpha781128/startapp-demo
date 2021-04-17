using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Startapp.Client.Repository
{
    public interface IGenericRepository
    {
        void SetResponse(LoginResponse LoginResponse);
        Task<T> GetAsync<T>(string uri);
        Task<R> GetStreamAsync<T, R>(string uri);
        Task<T> PostAsync<T>(string uri, T data);
        Task<T> PutAsync<T>(string uri, T data);
        Task<R> PostAsync<T, R>(string uri, T data);
        Task<T> DeleteAsync<T>(string uri);
        Task<R> GetPageAsync<T, R>(string uri, PagingParameters pagingParameters, string userId = null);
        Task<string> UploadImage(MultipartFormDataContent content, string id = null, string table = null);
    }
}
