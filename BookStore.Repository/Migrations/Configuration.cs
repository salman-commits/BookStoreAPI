namespace BookStore.Repository.Migrations
{
    using BookStore.Models.Account;
    using BookStore.Models.Book;
    using BookStore.Models.Order;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Context.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var roles = new List<Role>
            {
                new Role { RoleName = "Admin"},
                new Role { RoleName = "Manager"},
                new Role { RoleName = "User"}
            };
            roles.ForEach(s => context.Role.AddOrUpdate(p => p.RoleName, s));

            var addressType = new List<AddressType>
            {
                new AddressType { AddressTypeName = "Home"},
                new AddressType { AddressTypeName = "Work"},
                new AddressType { AddressTypeName = "Others"}
            };
            addressType.ForEach(s => context.AddressType.AddOrUpdate(p => p.AddressTypeName, s));

            context.DeliveryMethod.AddOrUpdate(h => h.Days,
                     new DeliveryMethod { Days = 2, Price = 4 },
                     new DeliveryMethod { Days = 4, Price = 2 },
                     new DeliveryMethod { Days = 6, Price = 1 }
            );

            var bookCategory = new List<BookCategory>
            {
                new BookCategory { BookCategoryName = "Action Adventure"},
                new BookCategory { BookCategoryName = "Fiction"},
                new BookCategory { BookCategoryName = "Fantasy"},
                new BookCategory { BookCategoryName = "Romance"},
                new BookCategory { BookCategoryName = "Horror"},
                new BookCategory { BookCategoryName = "Programming Languages"},
                new BookCategory { BookCategoryName = "Stock Market"},
                new BookCategory { BookCategoryName = "History"},
                new BookCategory { BookCategoryName = "Health & Fitness"},
                new BookCategory { BookCategoryName = "Politics"},
                new BookCategory { BookCategoryName = "Law"},
                new BookCategory { BookCategoryName = "Science"},
                new BookCategory { BookCategoryName = "Religion & Spirituality"},
                new BookCategory { BookCategoryName = "Sports"},
                new BookCategory { BookCategoryName = "Travel & Tourism"},
                new BookCategory { BookCategoryName = "Children"},
                new BookCategory { BookCategoryName = "Personal Development"}

            };
            bookCategory.ForEach(s => context.BookCategory.AddOrUpdate(p => p.BookCategoryName, s));

            context.SaveChanges();
        }
    }
}
