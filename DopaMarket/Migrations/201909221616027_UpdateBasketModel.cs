namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBasketModel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemBaskets", "ItemId");
            CreateIndex("dbo.ItemBaskets", "ClientId");
            AddForeignKey("dbo.ItemBaskets", "ClientId", "dbo.Clients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemBaskets", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemBaskets", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemBaskets", "ClientId", "dbo.Clients");
            DropIndex("dbo.ItemBaskets", new[] { "ClientId" });
            DropIndex("dbo.ItemBaskets", new[] { "ItemId" });
        }
    }
}
