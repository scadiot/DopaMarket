namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectCartToSessionId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemCarts", "CustomerId", "dbo.Customers");
            DropIndex("dbo.ItemCarts", new[] { "CustomerId" });
            AddColumn("dbo.ItemCarts", "SessionId", c => c.String());
            DropColumn("dbo.ItemCarts", "CustomerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemCarts", "CustomerId", c => c.Int(nullable: false));
            DropColumn("dbo.ItemCarts", "SessionId");
            CreateIndex("dbo.ItemCarts", "CustomerId");
            AddForeignKey("dbo.ItemCarts", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
        }
    }
}
