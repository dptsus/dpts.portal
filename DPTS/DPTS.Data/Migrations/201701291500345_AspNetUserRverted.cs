namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspNetUserRverted : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "IsEmailUnsubscribed");
            DropColumn("dbo.AspNetUsers", "IsPhoneNumberUnsubscribed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsPhoneNumberUnsubscribed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsEmailUnsubscribed", c => c.Boolean(nullable: false));
        }
    }
}
