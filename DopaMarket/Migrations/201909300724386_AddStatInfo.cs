namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brands", "ItemsCount", c => c.Int(nullable: false));
            AddColumn("dbo.Categories", "ItemsCount", c => c.Int(nullable: false));
            AddColumn("dbo.Items", "AverageRating", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Items", "Popularity", c => c.Int(nullable: false));
            DropColumn("dbo.Items", "AverageScore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "AverageScore", c => c.Int(nullable: false));
            DropColumn("dbo.Items", "Popularity");
            DropColumn("dbo.Items", "AverageRating");
            DropColumn("dbo.Categories", "ItemsCount");
            DropColumn("dbo.Brands", "ItemsCount");
        }
    }
}
