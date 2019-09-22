namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKey : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Clients", "AddressId");
            CreateIndex("dbo.ItemImages", "ItemId");
            CreateIndex("dbo.ItemInfoes", "ItemId");
            CreateIndex("dbo.ItemKeywords", "ItemId");
            CreateIndex("dbo.ItemKeywords", "KeywordId");
            CreateIndex("dbo.ItemPrices", "ItemId");
            AddForeignKey("dbo.Clients", "AddressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.ItemImages", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemInfoes", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemKeywords", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemKeywords", "KeywordId", "dbo.Keywords", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemPrices", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPrices", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemKeywords", "KeywordId", "dbo.Keywords");
            DropForeignKey("dbo.ItemKeywords", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemInfoes", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemImages", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Clients", "AddressId", "dbo.Addresses");
            DropIndex("dbo.ItemPrices", new[] { "ItemId" });
            DropIndex("dbo.ItemKeywords", new[] { "KeywordId" });
            DropIndex("dbo.ItemKeywords", new[] { "ItemId" });
            DropIndex("dbo.ItemInfoes", new[] { "ItemId" });
            DropIndex("dbo.ItemImages", new[] { "ItemId" });
            DropIndex("dbo.Clients", new[] { "AddressId" });
        }
    }
}
