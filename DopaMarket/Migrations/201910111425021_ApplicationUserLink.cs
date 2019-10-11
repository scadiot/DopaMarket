namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUserLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Customers", "ApplicationUserId");
            AddForeignKey("dbo.Customers", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.Customers", "IdentityUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "IdentityUserId", c => c.String(nullable: false));
            DropForeignKey("dbo.Customers", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Customers", new[] { "ApplicationUserId" });
            DropColumn("dbo.Customers", "ApplicationUserId");
        }
    }
}
