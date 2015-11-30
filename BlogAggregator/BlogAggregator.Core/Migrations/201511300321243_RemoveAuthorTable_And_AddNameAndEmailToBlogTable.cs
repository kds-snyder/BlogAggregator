namespace BlogAggregator.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAuthorTable_And_AddNameAndEmailToBlogTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Blogs", "AuthorID", "dbo.Authors");
            DropIndex("dbo.Blogs", new[] { "AuthorID" });
            AddColumn("dbo.Blogs", "AuthorEmail", c => c.String());
            AddColumn("dbo.Blogs", "AuthorName", c => c.String());
            DropColumn("dbo.Blogs", "AuthorID");
            DropTable("dbo.Authors");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorID = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        Email = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AuthorID);
            
            AddColumn("dbo.Blogs", "AuthorID", c => c.Int(nullable: false));
            DropColumn("dbo.Blogs", "AuthorName");
            DropColumn("dbo.Blogs", "AuthorEmail");
            CreateIndex("dbo.Blogs", "AuthorID");
            AddForeignKey("dbo.Blogs", "AuthorID", "dbo.Authors", "AuthorID", cascadeDelete: true);
        }
    }
}
