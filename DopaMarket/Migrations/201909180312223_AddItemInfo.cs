namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        ItemInfoTypeId = c.Int(nullable: false),
                        IntegerValue = c.Int(nullable: false),
                        DecimalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BooleanValue = c.Boolean(nullable: false),
                        StringValue = c.String(),
                        Unity = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemInfoTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemInfoTypes");
            DropTable("dbo.ItemInfoes");
        }
    }
}
