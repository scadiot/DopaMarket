namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSpecificationField : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemSpecifications", name: "ItemInfoTypeId", newName: "SpecificationId");
            RenameIndex(table: "dbo.ItemSpecifications", name: "IX_ItemInfoTypeId", newName: "IX_SpecificationId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemSpecifications", name: "IX_SpecificationId", newName: "IX_ItemInfoTypeId");
            RenameColumn(table: "dbo.ItemSpecifications", name: "SpecificationId", newName: "ItemInfoTypeId");
        }
    }
}
