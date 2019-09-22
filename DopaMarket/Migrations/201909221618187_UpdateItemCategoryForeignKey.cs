namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemCategoryForeignKey : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemCategories", "ItemId");
            CreateIndex("dbo.ItemCategories", "CategoryId");
            AddForeignKey("dbo.ItemCategories", "CategoryId", "dbo.Categories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemCategories", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemCategories", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemCategories", "CategoryId", "dbo.Categories");
            DropIndex("dbo.ItemCategories", new[] { "CategoryId" });
            DropIndex("dbo.ItemCategories", new[] { "ItemId" });
        }
    }
}
