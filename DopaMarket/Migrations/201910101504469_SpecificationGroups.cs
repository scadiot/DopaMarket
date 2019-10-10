namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecificationGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpecificationGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LongName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Specifications", "SpecificationGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Specifications", "SpecificationGroupId");
            AddForeignKey("dbo.Specifications", "SpecificationGroupId", "dbo.SpecificationGroups", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Specifications", "SpecificationGroupId", "dbo.SpecificationGroups");
            DropIndex("dbo.Specifications", new[] { "SpecificationGroupId" });
            DropColumn("dbo.Specifications", "SpecificationGroupId");
            DropTable("dbo.SpecificationGroups");
        }
    }
}
