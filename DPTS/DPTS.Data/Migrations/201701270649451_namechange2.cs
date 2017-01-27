namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class namechange2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DefaultNotificationSettings", "CategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.DefaultNotificationSettings", "Message", c => c.String());
            DropColumn("dbo.DefaultNotificationSettings", "CountryId");
            DropColumn("dbo.DefaultNotificationSettings", "Abbreviation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DefaultNotificationSettings", "Abbreviation", c => c.String());
            AddColumn("dbo.DefaultNotificationSettings", "CountryId", c => c.Int(nullable: false));
            DropColumn("dbo.DefaultNotificationSettings", "Message");
            DropColumn("dbo.DefaultNotificationSettings", "CategoryId");
        }
    }
}
