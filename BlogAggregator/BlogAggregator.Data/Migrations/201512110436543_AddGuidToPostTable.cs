namespace BlogAggregator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidToPostTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Guid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Guid");
        }
    }
}
