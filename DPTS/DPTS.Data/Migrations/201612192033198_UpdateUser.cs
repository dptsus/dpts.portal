namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastIpAddress", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreatedOnUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "LastLoginDateUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastLoginDateUtc");
            DropColumn("dbo.AspNetUsers", "CreatedOnUtc");
            DropColumn("dbo.AspNetUsers", "LastIpAddress");
        }
    }
}
