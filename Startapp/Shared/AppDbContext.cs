using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Startapp.Shared.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Startapp.Shared
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public string CurrentUserId { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Language> Languages { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string priceDecimalType = "decimal(18,2)";

            builder.Entity<AppUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Customer>().Property(c => c.UserId).IsRequired().HasMaxLength(100);
            builder.Entity<Customer>().HasIndex(c => c.UserId);
            builder.Entity<Customer>().Property(c => c.AdminArea1).HasMaxLength(50);
            builder.Entity<Customer>().ToTable($"App{nameof(this.Customers)}");

            builder.Entity<Category>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Entity<Category>().Property(p => p.Description).HasMaxLength(500);
            builder.Entity<Category>().ToTable($"App{nameof(this.Categories)}");

            builder.Entity<Article>().Property(p => p.Title).IsRequired().HasMaxLength(100);
            builder.Entity<Article>().HasIndex(p => p.Title);
            builder.Entity<Article>().ToTable($"App{nameof(this.Articles)}");
            builder.Entity<Article>().Property(p => p.BuyingPrice).HasColumnType(priceDecimalType);
            builder.Entity<Article>().Property(p => p.SellingPrice).HasColumnType(priceDecimalType);
            //builder.Entity<Picture>().HasOne(p => p.Article).WithMany(a => a.Pictures).IsRequired().OnDelete(DeleteBehavior.Cascade);
            //builder.Entity<Option>().HasOne(o => o.Article).WithMany(a => a.Options).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Order>().Property(o => o.Comments).HasMaxLength(500);
            builder.Entity<Order>().ToTable($"App{nameof(this.Orders)}");
            builder.Entity<Order>().Property(p => p.Discount).HasColumnType(priceDecimalType);

            builder.Entity<OrderItem>().ToTable($"App{nameof(this.OrderItems)}");
            builder.Entity<OrderItem>().Property(p => p.UnitPrice).HasColumnType(priceDecimalType);
            builder.Entity<OrderItem>().Property(p => p.Discount).HasColumnType(priceDecimalType);

            builder.Entity<Language>().ToTable($"App{nameof(this.Languages)}");

            builder.Entity<Option>().ToTable($"App{nameof(this.Options)}");

            builder.Entity<Picture>().ToTable($"App{nameof(this.Pictures)}");

        }




        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.Created = now;
                    entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.Created).IsModified = false;
                }

                entity.Updated = now;
                entity.UpdatedBy = CurrentUserId;
            }
        }
    }
}
