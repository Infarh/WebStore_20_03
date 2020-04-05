using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.Entities.Orders;

namespace WebStore.DAL.Context
{
    public class WebStoreDB : IdentityDbContext<User, Role, string>
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public WebStoreDB(DbContextOptions<WebStoreDB> Options) : base(Options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Описываем, что внутри сущности Секция есть
            builder.Entity<Section>()
               .HasMany(ParentSection => ParentSection.ChildSections)   // много других сущностей "Секция"
               .WithOne(ChildSection => ChildSection.ParentSection)     // а у каждый дочерней секции есть одна сущность "Секция" родительская
               .HasForeignKey(s => s.ParentId)                          // при этом внешним ключом будет служить указываемое здесь свойство
               .OnDelete(DeleteBehavior.Cascade);                       // и указываем, что при удалении надо удалять связанные сущности каскадно
        }
    }
}
