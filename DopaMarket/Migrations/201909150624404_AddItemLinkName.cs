namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemLinkName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "LinkName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "LinkName");
        }
    }
}
