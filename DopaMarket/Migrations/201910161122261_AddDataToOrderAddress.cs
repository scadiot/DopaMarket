namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataToOrderAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderAddresses", "Phone", c => c.String());
            AddColumn("dbo.OrderAddresses", "Company", c => c.String());
            AddColumn("dbo.OrderAddresses", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderAddresses", "Email");
            DropColumn("dbo.OrderAddresses", "Company");
            DropColumn("dbo.OrderAddresses", "Phone");
        }
    }
}
