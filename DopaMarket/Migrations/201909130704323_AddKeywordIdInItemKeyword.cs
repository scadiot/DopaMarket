namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeywordIdInItemKeyword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemKeywords", "KeywordId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemKeywords", "KeywordId");
        }
    }
}
