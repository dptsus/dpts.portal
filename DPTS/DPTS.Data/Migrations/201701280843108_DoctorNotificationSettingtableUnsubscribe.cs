namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoctorNotificationSettingtableUnsubscribe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorNotificationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(),
                        Message = c.String(),
                        DoctorId = c.String(maxLength: 128),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        EmailCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)
                .ForeignKey("dbo.EmailCategories", t => t.EmailCategory_Id)
                .Index(t => t.DoctorId)
                .Index(t => t.EmailCategory_Id);
            
            AddColumn("dbo.AspNetUsers", "IsEmailUnsubscribed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsPhoneNumberUnsubscribed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorNotificationSettings", "EmailCategory_Id", "dbo.EmailCategories");
            DropForeignKey("dbo.DoctorNotificationSettings", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorNotificationSettings", new[] { "EmailCategory_Id" });
            DropIndex("dbo.DoctorNotificationSettings", new[] { "DoctorId" });
            DropColumn("dbo.AspNetUsers", "IsPhoneNumberUnsubscribed");
            DropColumn("dbo.AspNetUsers", "IsEmailUnsubscribed");
            DropTable("dbo.DoctorNotificationSettings");
        }
    }
}
