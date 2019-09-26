namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTickets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ticketId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: false)
                .ForeignKey("dbo.Tickets", t => t.ticketId, cascadeDelete: true)
                .Index(t => t.ticketId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        PersonInChargeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: false)
                .ForeignKey("dbo.Customers", t => t.PersonInChargeId, cascadeDelete: false)
                .Index(t => t.CustomerId)
                .Index(t => t.PersonInChargeId);
            
            AddColumn("dbo.Orders", "VisibleId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "Status", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "VisibleId", unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketMessages", "ticketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "PersonInChargeId", "dbo.Customers");
            DropForeignKey("dbo.Tickets", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.TicketMessages", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Tickets", new[] { "PersonInChargeId" });
            DropIndex("dbo.Tickets", new[] { "CustomerId" });
            DropIndex("dbo.TicketMessages", new[] { "CustomerId" });
            DropIndex("dbo.TicketMessages", new[] { "ticketId" });
            DropIndex("dbo.Orders", new[] { "VisibleId" });
            DropColumn("dbo.Orders", "Status");
            DropColumn("dbo.Orders", "VisibleId");
            DropTable("dbo.Tickets");
            DropTable("dbo.TicketMessages");
        }
    }
}
