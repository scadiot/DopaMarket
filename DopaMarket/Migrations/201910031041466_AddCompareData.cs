namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompareData : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ItemInfoes", newName: "ItemSpecifications");
            RenameTable(name: "dbo.ItemInfoTypes", newName: "Specifications");
            CreateTable(
                "dbo.CompareGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LinkName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompareGroupSpecifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompareGroupId = c.Int(nullable: false),
                        SpecificationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompareGroups", t => t.CompareGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Specifications", t => t.SpecificationId, cascadeDelete: true)
                .Index(t => t.CompareGroupId)
                .Index(t => t.SpecificationId);
            
            AddColumn("dbo.Items", "CompareGroupId", c => c.Int());
            AddColumn("dbo.ItemReviews", "Title", c => c.String());
            CreateIndex("dbo.Items", "CompareGroupId");
            AddForeignKey("dbo.Items", "CompareGroupId", "dbo.CompareGroups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "CompareGroupId", "dbo.CompareGroups");
            DropForeignKey("dbo.CompareGroupSpecifications", "SpecificationId", "dbo.Specifications");
            DropForeignKey("dbo.CompareGroupSpecifications", "CompareGroupId", "dbo.CompareGroups");
            DropIndex("dbo.Items", new[] { "CompareGroupId" });
            DropIndex("dbo.CompareGroupSpecifications", new[] { "SpecificationId" });
            DropIndex("dbo.CompareGroupSpecifications", new[] { "CompareGroupId" });
            DropColumn("dbo.ItemReviews", "Title");
            DropColumn("dbo.Items", "CompareGroupId");
            DropTable("dbo.CompareGroupSpecifications");
            DropTable("dbo.CompareGroups");
            RenameTable(name: "dbo.Specifications", newName: "ItemInfoTypes");
            RenameTable(name: "dbo.ItemSpecifications", newName: "ItemInfoes");
        }
    }
}
