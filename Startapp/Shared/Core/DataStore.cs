using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Shared.Core
{
    public class DataStore : IDataStore
    {
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public DataStore(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetTotalArticlesForCustomerAsync(string userId)
        {
            var model = await _context.Articles.Where(a => a.CreatedBy == userId && a.Validated == true).CountAsync();
            return model;
        }
        
        public async Task<PagedList<Article>> GetNewestArticlesAsync(PagingParameters pg)
        {
            var model = await _context.Articles.Where(a => (pg.SearchTerm == null || a.Title.Contains(pg.SearchTerm) || a.Description.Contains(pg.SearchTerm)) && a.Validated == true)
                 .OrderByDescending(a => a.Created).Include(a => a.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }
        public async Task<PagedList<Article>> GetRandomArticlesAsync(PagingParameters pg)
        {
            var model = await _context.Articles.Where(a => a.Validated == true)
                .Include(a => a.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);

            return articles;
        }
        public async Task<PagedList<Article>> GetRelatedArticlesAsync(PagingParameters pg, string userId = null)
        {
            var model = await _context.Articles.Where(a => (pg.SearchTerm == null || a.Title.Contains(pg.SearchTerm) || a.Description.Contains(pg.SearchTerm)) && a.CreatedBy.Trim() == userId.Trim() && a.Validated == true)
                 .OrderByDescending(a => a.Updated).Include(a => a.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }
        public async Task<PagedList<Article>> GetRelatedCategoryAsync(PagingParameters pg)
        {
            var model = await _context.Articles.AsNoTracking().Where(a => pg.Cat == 0 || a.Category.Id == pg.Cat && a.Validated == true)
                 .OrderByDescending(a => a.Updated).Include(a => a.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }
        public async Task<PagedList<Article>> GetTopSelledAsync(PagingParameters pg)
        {
            var model = await _context.Articles.Where(a => pg.Cat == 0 || a.Category.Id == pg.Cat && (DateTime.Now - a.Updated).TotalDays <= 60 && a.Validated == true)
                 .OrderByDescending(a => a.Updated).OrderByDescending(a => a.NumberOfSelled)
                 .Include(p => p.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }
        public async Task<PagedList<Article>> GetTopViewedAsync(PagingParameters pg)
        {
            var model = await _context.Articles.Where(a => pg.Cat == 0 || a.Category.Id == pg.Cat
                                                        && (DateTime.Now - a.Updated).TotalDays <= 60 && a.Validated == true)
                 .OrderByDescending(a => a.Updated).OrderByDescending(a => a.NumberOfViews)
                 .Include(p => p.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }
        public async Task<PagedList<Article>> AutocompleteAsync(PagingParameters pg)
        {
            var model = await _context.Articles.Where(a => (pg.SearchTerm == null || a.Title.Contains(pg.SearchTerm) || a.Description.Contains(pg.SearchTerm)) && a.Validated == true)
                .OrderByDescending(a => a.Title).Include(a => a.Pictures).ToListAsync();

            var articles = PagedList<Article>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return articles;
        }

        public async Task<(bool Succeeded, Article Article, string Message)> GetArticleDetailsAsync(int id)
        {
            string message; bool succeeded; Article newArticle = new Article();
            try
            {
                newArticle = await _context.Articles.Include(a => a.Options).Include(a => a.Category)
              .Include(a => a.Pictures).FirstOrDefaultAsync(a => a.Id == id);
                if (newArticle != null)
                {
                    newArticle.NumberOfViews++;
                    await _context.SaveChangesAsync();
                    message = newArticle.Title ;
                    succeeded = true;
                }
                else
                {
                    message = newArticle.Title;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = id.ToString();
                succeeded = false;
            }
            return (succeeded, newArticle, message);
            // return _context.Ads.Include(a => a.Options).FirstOrDefault(a => a.Id == id);
        }
        public async Task<(bool Succeeded, Article Article, string Message)> GetArticleAsync(int id)
        {
            string message; bool succeeded; Article newArticle = new Article();
            try
            {
                newArticle = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
                if (newArticle != null)
                {                    
                    message = newArticle.Title;
                    succeeded = true;
                }
                else
                {
                    message = newArticle.Title;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = id.ToString();
                succeeded = false;
            }
            return (succeeded, newArticle, message);
        }

        public async Task<(bool Succeeded, Article Article, string Message)> AddArticleAsync(Article article)
        {
            string message; bool succeeded;
            try
            {
                await _context.Articles.AddAsync(article);
                message = article.Title;
                succeeded = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                message = article.Title;
                succeeded = false;
            }
            return (succeeded, article, message);
        }
        public async Task<(bool Succeeded, Article Article, string Message)> UpdateArticleAsync(Article article)
        {
            string message; bool succeeded;
            try
            {
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
                message = article.Title;
                succeeded = true;
            }
            catch (Exception)
            {
                message = article.Title;
                succeeded = false;
            }
            return (succeeded, article, message);
        }

        public async Task<(bool Succeeded, Article Article, string Message)> DeleteArticleAsync(int id)
        {
            string message; bool succeeded; Article article = new Article();
            try
            {
                article = await _context.Articles.FirstOrDefaultAsync(p => p.Id == id);
                if (article != null)
                {
                    _context.Articles.Remove(article);
                    await _context.SaveChangesAsync();
                    message = article.Title;
                    succeeded = true;
                }
                else
                {
                    message = article.Title;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = id.ToString();
                succeeded = false;
            }
            return (succeeded, article, message);
        }

        public async Task<PagedList<Customer>> GetCustomersAsync(PagingParameters pg)
        {
            var model = await _context.Customers.Where(p => pg.SearchTerm == null || p.Mobile.Contains(pg.SearchTerm) || p.AddressLine1.Contains(pg.SearchTerm))
                .OrderByDescending(a => a.Created).ToListAsync();

            var customers = PagedList<Customer>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return customers;
        }
        public async Task<(bool Succeeded, Customer Customer, string Message)> GetCustomerAsync()
        {
            string userId = _context.CurrentUserId;
            string message; bool succeeded; Customer customer = new Customer();
            try
            {
                customer = await _context.Customers.FirstOrDefaultAsync(u => u.UserId == userId);
                if (customer != null)
                {
                    message = customer.UserId;
                    succeeded = true;
                }
                else
                {
                    message = userId;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = userId;
                succeeded = false;
            }
            return (succeeded, customer, message);
            // return _context.Ads.Include(a => a.Options).FirstOrDefault(a => a.Id == id);
        }        
        public async Task<(bool Succeeded, Customer Customer, string Message)> AddCustomerAsync(Customer customer)
        {
            string message; bool succeeded;
            try
            {
                await _context.Customers.AddAsync(customer);
                message = "Customer";
                succeeded = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                message = "Customer";
                succeeded = false;
            }
            return (succeeded, customer, message);
        }
        public async Task<(bool Succeeded, Customer Customer, string Message)> UpdateCustomerAsync(Customer customer)
        {
            string message; bool succeeded; Customer oldCustomer;
            try
            {
                oldCustomer = await _context.Customers.FindAsync(customer.Id);
                if (oldCustomer != null)
                {
                  
                    _context.Customers.Update(customer);
                    message = "Customer";
                    succeeded = true;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var result = await AddCustomerAsync(customer);
                    succeeded = result.Succeeded;
                    message = result.Message;
                }
            }
            catch (Exception)
            {
                message = "Customer";
                succeeded = false;
            }
            return (succeeded, customer, message);
        }
        public async Task<(bool Succeeded, Customer Customer, string Message)> DeleteCustomerAsync(int id)
        {
            string message; bool succeeded; Customer customer = new Customer();
            try
            {
                customer = await _context.Customers.FirstOrDefaultAsync(p => p.Id == id);
                if (customer != null)
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                    message = "Customer";
                    succeeded = true;
                }
                else
                {
                    message = "Customer";
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = "Customer";
                succeeded = false;
            }
            return (succeeded, customer, message);
        }

        public IEnumerable<Language> GetLanguagesAsync()
        {
            var languages = _context.Languages;
            return languages;
        }
        public async Task<PagedList<Language>> GetLanguagesAsync(PagingParameters pg)
        {
            var model = await _context.Languages.Where(p => pg.SearchTerm == null || p.LocalName.Contains(pg.SearchTerm) || p.CountryCode.Contains(pg.SearchTerm) || p.LanguageCode.Contains(pg.SearchTerm))
                .OrderByDescending(a => a.LocalName).ToListAsync();

            var languages = PagedList<Language>.ToPagedList(model, pg.PageNumber, pg.PageSize);
            return languages;
        }
        public async Task<(bool Succeeded, Language Language, string Message)> GetLanguageAsync(int id)
        {
            string message; bool succeeded; Language language = new Language();
            try
            {
                 await _context.Languages.FirstOrDefaultAsync(p => p.Id == id);
                if (language != null)
                {                    
                    message = language.LocalName;
                    succeeded = true;
                }
                else
                {
                    message = language.LocalName;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = id.ToString();
                succeeded = false;
            }
            return (succeeded, language, message);
        }
        public async Task<(bool Succeeded, Language Language, string Message)> AddLanguageAsync(Language language)
        {
            string message; bool succeeded;
            try
            {
               var result = await _context.Languages.AddAsync(language);
                if (result.Entity.Id > 0)
                {
                    message = language.LocalName;
                    succeeded = true;
                }
                else
                {
                    message = language.LocalName;
                    succeeded = false;
                }
            
            }
            catch (Exception)
            {
                message = language.LocalName;
                succeeded = false;
            }
            return (succeeded, language, message);
        }
        public async Task<(bool Succeeded, Language Language, string Message)> UpdateLanguageAsync(Language language)
        {
            string message; bool succeeded;
            try
            {
                _context.Languages.Update(language);
                await _context.SaveChangesAsync();
                message = language.LocalName;
                succeeded = true;
            }
            catch (Exception)
            {
                message = language.LocalName;
                succeeded = false;
            }
            return (succeeded, language, message);
        }
        public async Task<(bool Succeeded, Language Language, string Message)> DeleteLanguageAsync(int id)
        {
            string message; bool succeeded; Language language = new Language();
            try
            {
                language = await _context.Languages.FirstOrDefaultAsync(p => p.Id == id);
                if (language != null)
                {
                    _context.Languages.Remove(language);
                    await _context.SaveChangesAsync();
                    message = language.LocalName ;
                    succeeded = true;
                }
                else
                {
                    message = language.LocalName;
                    succeeded = false;
                }
            }
            catch (Exception)
            {
                message = id.ToString();
                succeeded = false;
            }
            return (succeeded, language, message);
        }

        public async Task<Option> GetOptionAsync(int id)
        {
            return await _context.Options.Include(p => p.Article).Include(p => p.Article.AppUser).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<string> AddOptionAsync(Option Option)
        {
            string r = "L'option: " + Option.Value;

            await _context.Options.AddAsync(Option);
            r += " à été ajouter avec success";

            await _context.SaveChangesAsync();
            return r;
        }
        public async Task<string> UpdateOptionAsync(Option Option)
        {
            string r = "L'option: " + Option.Value;

            var rub = await _context.Options.FindAsync(Option.Id);
            rub.Value = Option.Value;
            rub.Property = Option.Property;
            _context.Entry(rub).State = EntityState.Modified;
            r += " à été modifier avec success";

            await _context.SaveChangesAsync();
            return r;
        }
        public async Task<string> DeleteOptionAsync(int id)
        {
            Option Option = await _context.Options.FirstOrDefaultAsync(a => a.Id == id);
            if (Option == null) return "l'option n'existe pas";

            _context.Options.Remove(Option);
            await _context.SaveChangesAsync();
            return "L'option: " + Option.Value + " à été supprimer avec success";
        }

        public async Task<Picture> GetPictureAsync(Guid id)
        {
            Picture Picture = await _context.Pictures.Include(p => p.Article).FirstOrDefaultAsync(pic => pic.Id == id);
            return Picture; //System.Drawing.Picture.FromStream(ms);
        }
        public async Task<Picture> GetArticlePictureAsync(int id)
        {
            Picture Pic = await _context.Pictures.FirstOrDefaultAsync(i => i.Article.Id == id);
            return Pic;
        }
        public async Task<Guid> AddPictureAsync(Picture picture)
        {
            await _context.Pictures.AddAsync(picture);
            await _context.SaveChangesAsync();
            return picture.Id;
        }
        public async Task<Guid> UpdatePictureAsync(Picture picture)
        {
            //var pic = await _context.UserProfiles.FindAsync(picture.Id);               
            _context.Entry(picture).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return picture.Id;
        }
        public async Task<Picture> DeletePictureAsync(Guid id)
        {
            Picture pic = await _context.Pictures.FirstOrDefaultAsync(a => a.Id == id);
            if (pic == null) return new Picture();
            _context.Pictures.Remove(pic);
             await _context.SaveChangesAsync();
            return pic;
        }

        public async Task<(bool Succeeded, Order Order, string Message)> AddOrderAsync(Order order)
        {
            string message; bool succeeded;
            try
            {
                await _context.Orders.AddAsync(order);
                message = "Order saved successfully";
                succeeded = true;
                await _context.SaveChangesAsync();               
            }
            catch (Exception)
            {
                message = "Failed to save order";
                succeeded = false;
            }
            return (succeeded, order, message);
        }

    }
}
