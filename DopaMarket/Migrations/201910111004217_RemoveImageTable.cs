namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveImageTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemImages", "ItemId", "dbo.Items");
            DropIndex("dbo.ItemImages", new[] { "ItemId" });
            AddColumn("dbo.Items", "ImageCount", c => c.Int(nullable: false));
            DropTable("dbo.ItemImages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ItemImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        Path = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Items", "ImageCount");
            CreateIndex("dbo.ItemImages", "ItemId");
            AddForeignKey("dbo.ItemImages", "ItemId", "dbo.Items", "Id", cascadeDelete: true);
        }
    }
}
