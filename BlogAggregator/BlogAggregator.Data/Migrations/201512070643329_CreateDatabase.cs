namespace BlogAggregator.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        BlogID = c.Int(nullable: false, identity: true),
                        BlogType = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        AuthorEmail = c.String(),
                        AuthorName = c.String(),
                        Description = c.String(),
                        Link = c.String(),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.BlogID);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostID = c.Int(nullable: false, identity: true),
                        BlogID = c.Int(nullable: false),
                        Content = c.String(),
                        Description = c.String(),
                        Link = c.String(),
                        PublicationDate = c.DateTime(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.PostID)
                .ForeignKey("dbo.Blogs", t => t.BlogID, cascadeDelete: true)
                .Index(t => t.BlogID);
            
            CreateTable(
                "dbo.ExternalLogins",
                c => new
                    {
                        ExternalLoginID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                    })
                .PrimaryKey(t => t.ExternalLoginID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExternalLogins", "UserID", "dbo.Users");
            DropForeignKey("dbo.Posts", "BlogID", "dbo.Blogs");
            DropIndex("dbo.ExternalLogins", new[] { "UserID" });
            DropIndex("dbo.Posts", new[] { "BlogID" });
            DropTable("dbo.Users");
            DropTable("dbo.ExternalLogins");
            DropTable("dbo.Posts");
            DropTable("dbo.Blogs");
        }
    }
}
