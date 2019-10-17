namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateOrderAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Street = c.String(nullable: false),
                        Street2 = c.String(),
                        City = c.String(nullable: false),
                        State = c.String(),
                        PostalCode = c.String(nullable: false),
                        Country = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "BillingAddressId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "ShippingAddressId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "BillingAddressId");
            CreateIndex("dbo.Orders", "ShippingAddressId");
            AddForeignKey("dbo.Orders", "BillingAddressId", "dbo.OrderAddresses", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Orders", "ShippingAddressId", "dbo.OrderAddresses", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ShippingAddressId", "dbo.OrderAddresses");
            DropForeignKey("dbo.Orders", "BillingAddressId", "dbo.OrderAddresses");
            DropIndex("dbo.Orders", new[] { "ShippingAddressId" });
            DropIndex("dbo.Orders", new[] { "BillingAddressId" });
            DropColumn("dbo.Orders", "ShippingAddressId");
            DropColumn("dbo.Orders", "BillingAddressId");
            DropTable("dbo.OrderAddresses");
        }
    }
}
