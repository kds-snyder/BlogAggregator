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
                        Guid = c.String(),
                        Link = c.String(),
                        PublicationDate = c.DateTime(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.PostID)
                .ForeignKey("dbo.Blogs", t => t.BlogID, cascadeDelete: true)
                .Index(t => t.BlogID);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        Authorized = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExternalLogins", "UserID", "dbo.Users");
            DropForeignKey("dbo.Posts", "BlogID", "dbo.Blogs");
            DropIndex("dbo.ExternalLogins", new[] { "UserID" });
            DropIndex("dbo.Posts", new[] { "BlogID" });
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Users");
            DropTable("dbo.ExternalLogins");
            DropTable("dbo.Clients");
            DropTable("dbo.Posts");
            DropTable("dbo.Blogs");
        }
    }
}
