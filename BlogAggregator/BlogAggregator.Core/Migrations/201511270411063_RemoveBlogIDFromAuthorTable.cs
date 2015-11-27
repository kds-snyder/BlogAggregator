namespace BlogAggregator.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBlogIDFromAuthorTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Authors", "BlogID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Authors", "BlogID", c => c.Int(nullable: false));
        }
    }
}
