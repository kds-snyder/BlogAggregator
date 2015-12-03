namespace BlogAggregator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBlogType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "BlogType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blogs", "BlogType");
        }
    }
}
