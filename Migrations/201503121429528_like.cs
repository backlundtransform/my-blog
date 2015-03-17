namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class like : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Like", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "Dislike", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Dislike");
            DropColumn("dbo.Comments", "Like");
        }
    }
}
