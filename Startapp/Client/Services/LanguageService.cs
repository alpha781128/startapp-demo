using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface ILanguageService
    {
        Task<IEnumerable<Language>> GetAsync();
        Task<PagingResponse<Language>> GetPageAsync(PagingParameters pagingParameters);
        Task<Language> GetAsync(int id);
        Task<Language> AddAsync(Language language);
        Task<Language> UpdateAsync(Language language);
        Task<Language> DeleteAsync(int id);
    }

    public class LanguageService : ILanguageService
    {
        private readonly IGenericRepository _genericRepository;
        public LanguageService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<Language>> GetAsync()
        {
            var response = await _genericRepository.GetStreamAsync<Language, IEnumerable<Language>>($"api/language/languages");
            return response;
        }

        public async Task<PagingResponse<Language>> GetPageAsync(PagingParameters pagingParameters)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<Language, PagingResponse<Language>>($"api/language/languages", pagingParameters);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<Language>();
            }
        }

        public async Task<Language> GetAsync(int id)
        {
            try
            {
                return await _genericRepository.GetAsync<Language>($"api/language/languages/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new Language();
            }
        }
        public async Task<Language> AddAsync(Language language)
        {
            try
            {
                var response = await _genericRepository.PostAsync<Language>($"api/language/languages", language);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return language;
            }
        }

        public async Task<Language> UpdateAsync(Language language)
        {
            try
            {
                var response = await _genericRepository.PutAsync<Language>($"api/language/languages", language);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return language;
            }
        }
        public async Task<Language> DeleteAsync(int id)
        {
            try
            {
                return await _genericRepository.DeleteAsync<Language>($"api/language/languages/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }
    }


}
