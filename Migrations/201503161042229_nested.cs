namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nested : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ParentId", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "Comment_Id", c => c.Int());
            CreateIndex("dbo.Comments", "Comment_Id");
            AddForeignKey("dbo.Comments", "Comment_Id", "dbo.Comments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Comment_Id", "dbo.Comments");
            DropIndex("dbo.Comments", new[] { "Comment_Id" });
            DropColumn("dbo.Comments", "Comment_Id");
            DropColumn("dbo.Comments", "ParentId");
        }
    }
}
