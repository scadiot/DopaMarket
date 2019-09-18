namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelatedItemInfoType : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemInfoes", "ItemInfoTypeId");
            AddForeignKey("dbo.ItemInfoes", "ItemInfoTypeId", "dbo.ItemInfoTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemInfoes", "ItemInfoTypeId", "dbo.ItemInfoTypes");
            DropIndex("dbo.ItemInfoes", new[] { "ItemInfoTypeId" });
        }
    }
}
