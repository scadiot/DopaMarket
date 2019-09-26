namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableMainCategoryId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Items", "MainCategoryId", "dbo.Categories");
            DropIndex("dbo.Items", new[] { "MainCategoryId" });
            AlterColumn("dbo.Items", "MainCategoryId", c => c.Int());
            CreateIndex("dbo.Items", "MainCategoryId");
            AddForeignKey("dbo.Items", "MainCategoryId", "dbo.Categories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "MainCategoryId", "dbo.Categories");
            DropIndex("dbo.Items", new[] { "MainCategoryId" });
            AlterColumn("dbo.Items", "MainCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Items", "MainCategoryId");
            AddForeignKey("dbo.Items", "MainCategoryId", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}
