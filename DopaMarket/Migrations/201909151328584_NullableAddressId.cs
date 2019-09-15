namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableAddressId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "AddressId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "AddressId", c => c.Int(nullable: false));
        }
    }
}
