namespace BookStore.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookModelUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Book", "ImageThumbnailURL", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Book", "ImageThumbnailURL", c => c.String(nullable: false));
        }
    }
}
