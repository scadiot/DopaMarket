namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderNotification : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Orders", new[] { "VisibleId" });
            CreateTable(
                "dbo.OrderNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Text = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            AddColumn("dbo.Orders", "Key", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "VisibleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "VisibleId", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderNotifications", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderNotifications", new[] { "OrderId" });
            DropColumn("dbo.Orders", "Key");
            DropTable("dbo.OrderNotifications");
            CreateIndex("dbo.Orders", "VisibleId", unique: true);
        }
    }
}
