using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Shared.Core
{
    public interface IDataStore
    {
        Task<int> GetTotalArticlesForCustomerAsync(string userId);
        Task<PagedList<Article>> GetRandomArticlesAsync(PagingParameters pg);
        Task<PagedList<Article>> GetRelatedArticlesAsync(PagingParameters pg, string userId = null);
        Task<PagedList<Article>> GetRelatedCategoryAsync(PagingParameters pg);
        Task<PagedList<Article>> GetNewestArticlesAsync(PagingParameters pg);
        Task<PagedList<Article>> GetTopSelledAsync(PagingParameters pg);
        Task<PagedList<Article>> GetTopViewedAsync(PagingParameters pg);
        Task<PagedList<Article>> AutocompleteAsync(PagingParameters pg);

        Task<(bool Succeeded, Article Article, string Message)> GetArticleDetailsAsync(int id);
        Task<(bool Succeeded, Article Article, string Message)> GetArticleAsync(int id);
        Task<(bool Succeeded, Article Article, string Message)> AddArticleAsync(Article article);
        Task<(bool Succeeded, Article Article, string Message)> UpdateArticleAsync(Article article);
        Task<(bool Succeeded, Article Article, string Message)> DeleteArticleAsync(int id);

        Task<PagedList<Customer>> GetCustomersAsync(PagingParameters pagingParameters);
        Task<(bool Succeeded, Customer Customer, string Message)> GetCustomerAsync();
        Task<(bool Succeeded, Customer Customer, string Message)> AddCustomerAsync(Customer customer);
        Task<(bool Succeeded, Customer Customer, string Message)> UpdateCustomerAsync(Customer customer);
        Task<(bool Succeeded, Customer Customer, string Message)> DeleteCustomerAsync(int id);

        IEnumerable<Language> GetLanguagesAsync();
        Task<PagedList<Language>> GetLanguagesAsync(PagingParameters pagingParameters);
        Task<(bool Succeeded, Language Language, string Message)> GetLanguageAsync(int id);
        Task<(bool Succeeded, Language Language, string Message)> AddLanguageAsync(Language language);
        Task<(bool Succeeded, Language Language, string Message)> UpdateLanguageAsync(Language language);
        Task<(bool Succeeded, Language Language, string Message)> DeleteLanguageAsync(int id);

        Task<Option> GetOptionAsync(int id);
        Task<string> AddOptionAsync(Option Option);
        Task<string> UpdateOptionAsync(Option Option);
        Task<string> DeleteOptionAsync(int id);

        Task<Picture> GetPictureAsync(Guid id);
        Task<Picture> GetArticlePictureAsync(int id);
        Task<Guid> AddPictureAsync(Picture picture);
        Task<Guid> UpdatePictureAsync(Picture picture);
        Task<Picture> DeletePictureAsync(Guid id);

        Task<(bool Succeeded, Order Order, string Message)> AddOrderAsync(Order order);

        //Task<(bool Succeeded, Option Option, string Message)> GetOptionAsync(int id);
        //Task<(bool Succeeded, Option Option, string Message)> AddOptionAsync(Option option);
        //Task<(bool Succeeded, Option Option, string Message)> UpdateOptionAsync(Option option);
        //Task<(bool Succeeded, Option Option, string Message)> DeleteOptionAsync(int id);

        //Task<(bool Succeeded, Picture Picture, string Message)> GetPictureAsync(Guid id);
        //Task<(bool Succeeded, Picture Picture, string Message)> GetArticlePictureAsync(int id);
        //Task<(bool Succeeded, Picture Picture, string Message)> AddPictureAsync(Picture picture);
        //Task<(bool Succeeded, Picture Picture, string Message)> UpdatePictureAsync(Picture picture);
        //Task<(bool Succeeded, Picture Picture, string Message)> DeletePictureAsync(Guid id);


    }
}