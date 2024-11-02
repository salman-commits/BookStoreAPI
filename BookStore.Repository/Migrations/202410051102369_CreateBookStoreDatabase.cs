namespace BookStore.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBookStoreDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        AddressLine1 = c.String(nullable: false, maxLength: 200),
                        AddressLine2 = c.String(maxLength: 200),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(nullable: false, maxLength: 50),
                        Country = c.String(nullable: false, maxLength: 50),
                        ZipCode = c.Long(nullable: false),
                        PhoneNumber = c.Long(nullable: false),
                        AddressTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.AddressType", t => t.AddressTypeId, cascadeDelete: true)
                .Index(t => t.AddressTypeId);
            
            CreateTable(
                "dbo.AddressType",
                c => new
                    {
                        AddressTypeId = c.Int(nullable: false, identity: true),
                        AddressTypeName = c.String(),
                    })
                .PrimaryKey(t => t.AddressTypeId);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ISBN = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 200),
                        AuthorName = c.String(nullable: false, maxLength: 50),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PublishedDate = c.DateTime(nullable: false),
                        Rating = c.Int(),
                        ImageURL = c.String(nullable: false),
                        ImageThumbnailURL = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        BookCategoryId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookCategory", t => t.BookCategoryId, cascadeDelete: true)
                .Index(t => t.BookCategoryId);
            
            CreateTable(
                "dbo.BookCategory",
                c => new
                    {
                        BookCategoryId = c.Int(nullable: false, identity: true),
                        BookCategoryName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.BookCategoryId);
            
            CreateTable(
                "dbo.DeliveryMethod",
                c => new
                    {
                        DeliveryMethodId = c.Int(nullable: false, identity: true),
                        Days = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DeliveryMethodId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AddressId = c.Int(nullable: false),
                        DeliveryMethodId = c.Int(nullable: false),
                        OrderStatus = c.Int(),
                        OrderDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Address", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.DeliveryMethod", t => t.DeliveryMethodId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.AddressId)
                .Index(t => t.DeliveryMethodId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.OrderItem",
                c => new
                    {
                        OrderItemId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.ShoppingCartItem",
                c => new
                    {
                        ShoppingCartItemId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShoppingCartItemId)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCartItem", "BookId", "dbo.Book");
            DropForeignKey("dbo.OrderItem", "BookId", "dbo.Book");
            DropForeignKey("dbo.Order", "UserId", "dbo.User");
            DropForeignKey("dbo.User", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Order", "DeliveryMethodId", "dbo.DeliveryMethod");
            DropForeignKey("dbo.Order", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Book", "BookCategoryId", "dbo.BookCategory");
            DropForeignKey("dbo.Address", "AddressTypeId", "dbo.AddressType");
            DropIndex("dbo.ShoppingCartItem", new[] { "BookId" });
            DropIndex("dbo.OrderItem", new[] { "BookId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "RoleId" });
            DropIndex("dbo.Order", new[] { "DeliveryMethodId" });
            DropIndex("dbo.Order", new[] { "AddressId" });
            DropIndex("dbo.Book", new[] { "BookCategoryId" });
            DropIndex("dbo.Address", new[] { "AddressTypeId" });
            DropTable("dbo.ShoppingCartItem");
            DropTable("dbo.OrderItem");
            DropTable("dbo.Role");
            DropTable("dbo.User");
            DropTable("dbo.Order");
            DropTable("dbo.DeliveryMethod");
            DropTable("dbo.BookCategory");
            DropTable("dbo.Book");
            DropTable("dbo.AddressType");
            DropTable("dbo.Address");
        }
    }
}
