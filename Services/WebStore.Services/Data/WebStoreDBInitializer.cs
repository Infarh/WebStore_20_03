using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Services.Data
{
    public class WebStoreDBInitializer
    {
        private readonly WebStoreDB _db;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<Role> _RoleManager;

        public WebStoreDBInitializer(WebStoreDB db, UserManager<User> UserManager, RoleManager<Role> RoleManager)
        {
            _db = db;
            _UserManager = UserManager;
            _RoleManager = RoleManager;
        }

        public void Initialize() => InitializeAsync().Wait();

        public async Task InitializeAsync()
        {
            var db = _db.Database;

            await db.MigrateAsync().ConfigureAwait(false);

            await InitializeIdentityAsync().ConfigureAwait(false);

            await InitializeProductsAsync().ConfigureAwait(false);
        }

        private async Task InitializeProductsAsync()
        {
            // Если в БД есть товары, то БД считается проинициализированной и делать ничего не надо
            if (await _db.Products.AnyAsync()) return;
            var db = _db.Database;

            // Если надо инициализировать БД товарами, то надо её почистить от того, что там уже может быть (хлам)
            await using (await db.BeginTransactionAsync())
            {
                // Находим все секции
                var exists_sections = await _db.Sections.ToArrayAsync();
                if (exists_sections.Length > 0) // Если они есть, то надо их удалить
                {
                   _db.Sections.RemoveRange(exists_sections);
                    await _db.SaveChangesAsync();
                }

                // Находим все бренды
                var exists_brands = await _db.Brands.ToArrayAsync();
                if (exists_brands.Length > 0) // Если они есть, то их тоже надо удалить
                {
                    _db.Brands.RemoveRange(exists_brands);
                    await _db.SaveChangesAsync();
                }

                db.CommitTransaction();
            }

            // Создаём словарь по идентификаторам секций
            var sections = TestData.Sections.ToDictionary(s => s.Id);
            await using (await db.BeginTransactionAsync())
            {
                // Группируем все секции по идентификатору родительской секции
                foreach (var child_sections in TestData.Sections.Where(s => s.ParentId != null).GroupBy(s => sections[(int)s.ParentId]))
                    foreach (var child_section in child_sections) // В каждой группе мы получаем дочерние секции, а ключом группы будет родительская секция
                        child_section.ParentSection = child_sections.Key; // Устанавливаем значение родительской секции для каждой дочерней

                // Чтобы EF съела и не подавилась нашими секциями, у них надо почистить значения первичного и внешнего ключей
                foreach (var section in sections.Values)
                {
                    section.Id = 0;
                    section.ParentId = null;
                }

                // Просто добавляем все секции в контекст БД
                await _db.Sections.AddRangeAsync(sections.Values);
                await _db.SaveChangesAsync();

                db.CommitTransaction();
            }

            // Аналогично для брендов - все бренды в словарь по их первичному ключу
            // Словарь нам понадобится ниже при добавлении товаров.
            var brands = TestData.Brands.ToDictionary(b => b.Id);
            foreach (var brand in brands.Values) // И очищаем значение первичного ключа для брендов чтобы EF не подавилась.
                brand.Id = 0;

            await using (await db.BeginTransactionAsync())
            {
                await _db.Brands.AddRangeAsync(TestData.Brands); // Просто скармливаем все бренды контексту БД
                await _db.SaveChangesAsync();
                db.CommitTransaction();
            }

            // Надо предварительно обработать все наши товары
            foreach (var product in TestData.Products)
            {
                // 1. Удалить первичный ключ
                product.Id = 0;
                if (product.BrandId != null) // Если задан внешний ключ в таблицу брендов, то
                {
                    product.Brand = brands[(int)product.BrandId]; // Извлекаем бренд из словаря брендов
                    product.BrandId = null;                       // И обязательно удаляем значение внешнего ключа
                }

                product.Section = sections[product.SectionId];    // Извлекаем секцию из ловаря секций
                product.SectionId = 0;                            // И обязательно удаляем значение внешнего ключа
            }

            await using (await db.BeginTransactionAsync())
            {
                await _db.Products.AddRangeAsync(TestData.Products); // После чистки товары просто попадают в контекст БД!
                // В принципе, все обращения к контексту для отдельно для добавления брендов и секций можно было не проводить
                // Бренды и секции всё равно попали бы в БД при добавлении товаров!

                await _db.SaveChangesAsync();
                db.CommitTransaction();
            }
        }

        private async Task InitializeIdentityAsync()
        {
            if (!await _RoleManager.RoleExistsAsync(Role.Administrator))
                await _RoleManager.CreateAsync(new Role { Name = Role.Administrator });

            if (!await _RoleManager.RoleExistsAsync(Role.User))
                await _RoleManager.CreateAsync(new Role { Name = Role.User });

            if (await _UserManager.FindByNameAsync(User.Administrator) is null)
            {
                var admin = new User
                {
                    UserName = User.Administrator,
                    //Email = "amin@server.com"
                };

                var create_result = await _UserManager.CreateAsync(admin, User.AdminDefaultPassword);
                if (create_result.Succeeded)
                    await _UserManager.AddToRoleAsync(admin, Role.Administrator);
                else
                {
                    var errors = create_result.Errors.Select(error => error.Description);
                    throw new InvalidOperationException($"Ошибка при создании пользователя - Администратора: {string.Join(", ", errors)}");
                }
            }
        }
    }
}
