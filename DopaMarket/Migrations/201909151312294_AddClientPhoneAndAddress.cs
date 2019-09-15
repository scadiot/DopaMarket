namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientPhoneAndAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Birthday", c => c.DateTime());
            AddColumn("dbo.Clients", "PhoneNumber", c => c.String());
            AddColumn("dbo.Clients", "AddressId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "AddressId");
            DropColumn("dbo.Clients", "PhoneNumber");
            DropColumn("dbo.Clients", "Birthday");
        }
    }
}
