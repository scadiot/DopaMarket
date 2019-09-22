namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemReview : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                        Note = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemReviews", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemReviews", "ClientId", "dbo.Clients");
            DropIndex("dbo.ItemReviews", new[] { "ClientId" });
            DropIndex("dbo.ItemReviews", new[] { "ItemId" });
            DropTable("dbo.ItemReviews");
        }
    }
}
