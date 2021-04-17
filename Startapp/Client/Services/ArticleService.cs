using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface IArticleService
    {
        Task<PagingResponse<Article>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null);
        Task<PagingResponse<ArticleViewModel>> GetVMPageAsync(PagingParameters pagingParameters, string url, string userId = null);
        Task<Article> GetAsync(int id);
        Task<Article> AddAsync(Article article);
        Task<Article> UpdateAsync(Article article);
        Task<Article> DeleteAsync(int id);
    }

    public class ArticleService : IArticleService
    {
        private readonly IGenericRepository _genericRepository;
        public ArticleService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<PagingResponse<Article>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<Article, PagingResponse<Article>>(url, pagingParameters, userId);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<Article>();
            }
        }

        public async Task<PagingResponse<ArticleViewModel>> GetVMPageAsync(PagingParameters pagingParameters, string url, string userId = null)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<ArticleViewModel, PagingResponse<ArticleViewModel>>(url, pagingParameters, userId);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<ArticleViewModel>();
            }
        }

        public async Task<Article> GetAsync(int id)
        {        
            try
            {
                return await _genericRepository.GetAsync<Article>($"api/article/articles/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new Article();
            }
        }
        public async Task<Article> AddAsync(Article article)
        {
            try
            {
                var response = await _genericRepository.PostAsync<Article>($"api/article/articles", article);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return article;
            }
        }

        public async Task<Article> UpdateAsync(Article article)
        {
            try
            {
                var response = await _genericRepository.PutAsync<Article>($"api/article/articles", article);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return article;
            }
        }
        public async Task<Article> DeleteAsync(int id)
        {
            try
            {
                return await _genericRepository.DeleteAsync<Article>($"api/article/articles/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }
    }

   
}
