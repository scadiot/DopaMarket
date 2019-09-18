namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAddItemInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemInfoTypes", "Unity", c => c.String());
            AddColumn("dbo.ItemInfoTypes", "LongName", c => c.String());
            DropColumn("dbo.ItemInfoes", "Unity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemInfoes", "Unity", c => c.String());
            DropColumn("dbo.ItemInfoTypes", "LongName");
            DropColumn("dbo.ItemInfoTypes", "Unity");
        }
    }
}
