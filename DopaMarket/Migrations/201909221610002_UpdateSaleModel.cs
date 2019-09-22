namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSaleModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleItems", "SaleId", c => c.Int(nullable: false));
            AddColumn("dbo.SaleItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Sales", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sales", "ItemsSumPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Sales", "ExpeditionPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Sales", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.SaleItems", "SaleId");
            CreateIndex("dbo.SaleItems", "ItemId");
            CreateIndex("dbo.Sales", "ClientId");
            AddForeignKey("dbo.SaleItems", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Sales", "ClientId", "dbo.Clients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SaleItems", "SaleId", "dbo.Sales", "Id", cascadeDelete: true);
            DropColumn("dbo.SaleItems", "CurrentPrice");
            DropColumn("dbo.Sales", "InsertDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sales", "InsertDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SaleItems", "CurrentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.SaleItems", "SaleId", "dbo.Sales");
            DropForeignKey("dbo.Sales", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.SaleItems", "ItemId", "dbo.Items");
            DropIndex("dbo.Sales", new[] { "ClientId" });
            DropIndex("dbo.SaleItems", new[] { "ItemId" });
            DropIndex("dbo.SaleItems", new[] { "SaleId" });
            DropColumn("dbo.Sales", "TotalPrice");
            DropColumn("dbo.Sales", "ExpeditionPrice");
            DropColumn("dbo.Sales", "ItemsSumPrice");
            DropColumn("dbo.Sales", "Date");
            DropColumn("dbo.SaleItems", "Price");
            DropColumn("dbo.SaleItems", "SaleId");
        }
    }
}
