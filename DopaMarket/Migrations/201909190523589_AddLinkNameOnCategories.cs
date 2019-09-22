namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkNameOnCategories : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "LinkName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "LinkName");
        }
    }
}
