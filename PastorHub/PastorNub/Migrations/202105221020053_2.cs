namespace PastorNub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserConfessionApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserConfessionApplicationUsers", "UserConfession_Id", "dbo.UserConfessions");
            DropForeignKey("dbo.Confessions", "UserConfession_Id", "dbo.UserConfessions");
            DropIndex("dbo.UserConfessionApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserConfessionApplicationUsers", new[] { "UserConfession_Id" });
            DropIndex("dbo.Confessions", new[] { "UserConfession_Id" });
            DropColumn("dbo.Confessions", "UserConfession_Id");
            DropTable("dbo.UserConfessionApplicationUsers");
            DropTable("dbo.UserConfessions");         
        }
        
        public override void Down()
        {
            CreateTable(
              "dbo.UserConfessions",
              c => new
              {
                  Id = c.Int(nullable: false, identity: true),
              })
              .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UserConfessionApplicationUsers",
                c => new
                {
                    UserConfession_Id = c.Int(nullable: false),
                    ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserConfession_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.UserConfessions", t => t.UserConfession_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.UserConfession_Id)
                .Index(t => t.ApplicationUser_Id);

            AddColumn("dbo.Confessions", "UserConfession_Id", c => c.Int());
            CreateIndex("dbo.Confessions", "UserConfession_Id");
            AddForeignKey("dbo.Confessions", "UserConfession_Id", "dbo.UserConfessions", "Id");
        }
    }
}
