namespace PastorNub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "FK_dbo.AspNetUsers_dbo.Confessions_Confession_Id1");
            DropIndex("dbo.AspNetUsers", new[] { "Confession_Id1" });
            DropColumn("dbo.AspNetUsers", "Confession_Id1");
        }
        
        public override void Down()
        {
            
        }
    }
}
