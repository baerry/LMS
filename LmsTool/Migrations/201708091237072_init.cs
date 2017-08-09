namespace LmsTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityModels", "Document", c => c.String());
            AddColumn("dbo.ModulModels", "Document", c => c.String());
            AddColumn("dbo.CourseModels", "Document", c => c.String());
            AddColumn("dbo.ViewModuls", "Document", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ViewModuls", "Document");
            DropColumn("dbo.CourseModels", "Document");
            DropColumn("dbo.ModulModels", "Document");
            DropColumn("dbo.ActivityModels", "Document");
        }
    }
}
