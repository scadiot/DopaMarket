namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Name");
        }
    }
}
