namespace PastorNub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Confessions", "UserConfession_Id", "dbo.UserConfessions");
            DropForeignKey("dbo.UserConfessionApplicationUsers", "UserConfession_Id", "dbo.UserConfessions");
            DropForeignKey("dbo.UserConfessionApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Confessions", new[] { "UserConfession_Id" });
            DropIndex("dbo.UserConfessionApplicationUsers", new[] { "UserConfession_Id" });
            DropIndex("dbo.UserConfessionApplicationUsers", new[] { "ApplicationUser_Id" });
            AddColumn("dbo.UserConfessions", "Confession_Id", c => c.Int());
            AddColumn("dbo.UserConfessions", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserConfessions", "Confession_Id");
            CreateIndex("dbo.UserConfessions", "User_Id");
            AddForeignKey("dbo.UserConfessions", "Confession_Id", "dbo.Confessions", "Id");
            AddForeignKey("dbo.UserConfessions", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Confessions", "UserConfession_Id");
            DropTable("dbo.UserConfessionApplicationUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserConfessionApplicationUsers",
                c => new
                    {
                        UserConfession_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserConfession_Id, t.ApplicationUser_Id });
            
            AddColumn("dbo.Confessions", "UserConfession_Id", c => c.Int());
            DropForeignKey("dbo.UserConfessions", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserConfessions", "Confession_Id", "dbo.Confessions");
            DropIndex("dbo.UserConfessions", new[] { "User_Id" });
            DropIndex("dbo.UserConfessions", new[] { "Confession_Id" });
            DropColumn("dbo.UserConfessions", "User_Id");
            DropColumn("dbo.UserConfessions", "Confession_Id");
            CreateIndex("dbo.UserConfessionApplicationUsers", "ApplicationUser_Id");
            CreateIndex("dbo.UserConfessionApplicationUsers", "UserConfession_Id");
            CreateIndex("dbo.Confessions", "UserConfession_Id");
            AddForeignKey("dbo.UserConfessionApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserConfessionApplicationUsers", "UserConfession_Id", "dbo.UserConfessions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Confessions", "UserConfession_Id", "dbo.UserConfessions", "Id");
        }
    }
}
