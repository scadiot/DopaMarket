namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubjectToTicket : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Subject", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "Subject");
        }
    }
}
