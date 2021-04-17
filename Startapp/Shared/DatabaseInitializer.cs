using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Startapp.Shared.Core;
using Startapp.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Startapp.Shared
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }


    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;
        private readonly string _defaultRoleName;

        public DatabaseInitializer(AppDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
        }

        public DatabaseInitializer(AppDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger, string defaultRoleName)
            : this(context, accountManager, logger)
        {
            _defaultRoleName = defaultRoleName;
        }


        virtual public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            if (!await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Generating inbuilt accounts");

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                if (!string.IsNullOrWhiteSpace(_defaultRoleName))
                    await EnsureRoleAsync(_defaultRoleName, "Default public role", new string[] { });

                await CreateUserAsync("admin", "start@31Trwz", "Inbuilt", "Administrator", "admin@startapppro.com", "+1 (123) 000-0000", new string[] { adminRoleName });
                await CreateUserAsync("user", "start@31Trwz", "Inbuilt", "Standard User", "user@startapppro.com", "+1 (123) 000-0001", new string[] { userRoleName });

                _logger.LogInformation("Inbuilt account generation completed");
            }

            if (!await _context.Languages.AnyAsync())
            {
                Language lng_1 = new Language { LocalName = "Arabic - الـعـربـيـة", LanguageCode = "ar", CountryCode = "DZ", SettingsLanguage = SettingsLanguage.Arabic };
                Language lng_2 = new Language { LocalName = "English - US English (Default)", LanguageCode = "en", CountryCode = "US", SettingsLanguage = SettingsLanguage.English };
                Language lng_3 = new Language { LocalName = "Chinese - 中文", LanguageCode = "zh", CountryCode = "CN", SettingsLanguage = SettingsLanguage.Chinese };
                Language lng_4 = new Language { LocalName = "Indian - हिन्दी", LanguageCode = "hi", CountryCode = "IN", SettingsLanguage = SettingsLanguage.Hindi };
                Language lng_5 = new Language { LocalName = "Spanish - Español", LanguageCode = "es", CountryCode = "ES", SettingsLanguage = SettingsLanguage.Spanish };
                Language lng_6 = new Language { LocalName = "Frensh - Français", LanguageCode = "fr", CountryCode = "FR", SettingsLanguage = SettingsLanguage.French };
                Language lng_7 = new Language { LocalName = "Bengali - বাংলা", LanguageCode = "bn", CountryCode = "BD", SettingsLanguage = SettingsLanguage.Bengali };
                Language lng_8 = new Language { LocalName = "Russian - Русский", LanguageCode = "ru", CountryCode = "RU", SettingsLanguage = SettingsLanguage.Russian };
                Language lng_9 = new Language { LocalName = "Portuguese - Português", LanguageCode = "pt", CountryCode = "PT", SettingsLanguage = SettingsLanguage.Portuguese };
                Language lng_10 = new Language { LocalName = "Indonisan - Bahasa Indonesia", LanguageCode = "id", CountryCode = "ID", SettingsLanguage = SettingsLanguage.Indonesian };
                Language lng_11 = new Language { LocalName = "German - Deutsch", LanguageCode = "de", CountryCode = "DE", SettingsLanguage = SettingsLanguage.German };
                Language lng_12 = new Language { LocalName = "Japanese - 日本語", LanguageCode = "ja", CountryCode = "JP", SettingsLanguage = SettingsLanguage.Japanese };
                Language lng_13 = new Language { LocalName = "Corian - 한국어", LanguageCode = "ko", CountryCode = "KR", SettingsLanguage = SettingsLanguage.Corian };
                Language lng_14 = new Language { LocalName = "Turkish - Türkçe", LanguageCode = "tr", CountryCode = "TR", SettingsLanguage = SettingsLanguage.Turkish };
                Language lng_15 = new Language { LocalName = "Italian - Italiano", LanguageCode = "it", CountryCode = "IT", SettingsLanguage = SettingsLanguage.Italian };
                _context.Languages.Add(lng_1);
                _context.Languages.Add(lng_2);
                _context.Languages.Add(lng_3);
                _context.Languages.Add(lng_4);
                _context.Languages.Add(lng_5);
                _context.Languages.Add(lng_6);
                _context.Languages.Add(lng_7);
                _context.Languages.Add(lng_8);
                _context.Languages.Add(lng_9);
                _context.Languages.Add(lng_10);
                _context.Languages.Add(lng_11);
                _context.Languages.Add(lng_12);
                _context.Languages.Add(lng_13);
                _context.Languages.Add(lng_14);
                _context.Languages.Add(lng_15);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeding initial data completed");
            };

            if (!await _context.Articles.AnyAsync() && !await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Seeding initial data");
                

                Category prodCat_1 = new Category
                {
                    Name = "All",
                    Description = "Default category. Products that have not been assigned a category"                   
                };
                Category prodCat_2 = new Category
                {
                    Name = "Tablet",
                    Description = "Tablet"           
                };
                Category prodCat_3 = new Category
                {
                    Name = "Laptop",
                    Description = "Laptop"                   
                };
                Category prodCat_4 = new Category
                {
                    Name = "Headphones",
                    Description = "Headphones"                  
                };
                Category prodCat_5 = new Category
                {
                    Name = "Smartphones",
                    Description = "Smartphones"                   
                };

                Article prod_1 = new Article { Title = "iPhone 7", Description = "iPhone 7", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_5, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_2 = new Article { Title = "iPad Pro", Description = "iPad Pro", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_2, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_3 = new Article { Title = "Sony TV RT56", Description = "Sony TV RT56", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_4 = new Article { Title = "Philips V56", Description = "Philips V56", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_5 = new Article { Title = "Dell V756", Description = "Dell V756", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_3, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_6 = new Article { Title = "Canon D-67i", Description = "Canon D-67i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_7 = new Article { Title = "Samsung V54", Description = "Samsung V54", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_8 = new Article { Title = "Dell 786i", Description = "Dell 786i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_3, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_9 = new Article { Title = "iMac 27inch", Description = "iMac 27inch", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_10 = new Article { Title = "Headphones", Description = "Headphones", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_4, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_11 = new Article { Title = "Dell V-964i", Description = "Dell V-964i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_3, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_12 = new Article { Title = "Dell 786i", Description = "Dell 786i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_3, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_13 = new Article { Title = "Asus GT67i", Description = "Asus GT67i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_3, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_14 = new Article { Title = "Canon 675-D", Description = "Canon 675-D", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_15 = new Article { Title = "iMac", Description = "iMac", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_16 = new Article { Title = "Sony TV-675i", Description = "Sony TV-675i", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_17 = new Article { Title = "Samsung Y78", Description = "Samsung Y78", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                Article prod_18 = new Article { Title = "Nikon", Description = "Nikon", BuyingPrice = 550, SellingPrice = 650, UnitsInStock = 10, IsActive = true, Category = prodCat_1, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };

                _context.Articles.Add(prod_1);
                _context.Articles.Add(prod_2);
                _context.Articles.Add(prod_3);
                _context.Articles.Add(prod_4);
                _context.Articles.Add(prod_5);
                _context.Articles.Add(prod_6);
                _context.Articles.Add(prod_7);
                _context.Articles.Add(prod_8);
                _context.Articles.Add(prod_9);
                _context.Articles.Add(prod_10);
                _context.Articles.Add(prod_11);
                _context.Articles.Add(prod_12);
                _context.Articles.Add(prod_13);
                _context.Articles.Add(prod_14);
                _context.Articles.Add(prod_15);
                _context.Articles.Add(prod_16);
                _context.Articles.Add(prod_17);
                _context.Articles.Add(prod_18);


                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeding initial data completed");
            }

        }



        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                AppRole applicationRole = new AppRole(roleName, description);

                var result = await this._accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private async Task<AppUser> CreateUserAsync(string userName, string password, string firstName, string lastName, string email, string phoneNumber, string[] roles)
        {
            AppUser appUser = new AppUser
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await _accountManager.CreateUserAsync(appUser, roles, password);

            if (!result.Succeeded)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");


            return appUser;
        }
    }
}
