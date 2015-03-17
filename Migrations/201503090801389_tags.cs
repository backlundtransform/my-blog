namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PostId_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId_Id)
                .Index(t => t.PostId_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "PostId_Id", "dbo.Posts");
            DropIndex("dbo.Tags", new[] { "PostId_Id" });
            DropTable("dbo.Tags");
        }
    }
}
