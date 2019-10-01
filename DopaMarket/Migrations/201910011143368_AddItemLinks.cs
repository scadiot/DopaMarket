namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemLinks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.ItemLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MainItemId = c.Int(nullable: false),
                        OtherItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.MainItemId, cascadeDelete: false)
                .ForeignKey("dbo.Items", t => t.OtherItemId, cascadeDelete: false)
                .Index(t => t.MainItemId)
                .Index(t => t.OtherItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLinks", "OtherItemId", "dbo.Items");
            DropForeignKey("dbo.ItemLinks", "MainItemId", "dbo.Items");
            DropForeignKey("dbo.ItemFeatures", "ItemId", "dbo.Items");
            DropIndex("dbo.ItemLinks", new[] { "OtherItemId" });
            DropIndex("dbo.ItemLinks", new[] { "MainItemId" });
            DropIndex("dbo.ItemFeatures", new[] { "ItemId" });
            DropTable("dbo.ItemLinks");
            DropTable("dbo.ItemFeatures");
        }
    }
}
