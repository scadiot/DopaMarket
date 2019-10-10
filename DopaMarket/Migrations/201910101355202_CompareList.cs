namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompareList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemCompares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        SessionId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemCompares", "ItemId", "dbo.Items");
            DropIndex("dbo.ItemCompares", new[] { "ItemId" });
            DropTable("dbo.ItemCompares");
        }
    }
}
