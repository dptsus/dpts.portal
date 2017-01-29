namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableUpdated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SentEmailHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.String(),
                        ReceiverId = c.String(),
                        SenderType = c.String(),
                        ReceiverEmail = c.String(),
                        Email = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        AspNetUser_Id = c.String(maxLength: 128),
                        Doctor_DoctorId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUser_Id)
                .ForeignKey("dbo.Doctors", t => t.Doctor_DoctorId)
                .Index(t => t.AspNetUser_Id)
                .Index(t => t.Doctor_DoctorId);
            
            CreateTable(
                "dbo.SentSmsHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.String(),
                        ReceiverId = c.String(),
                        SenderType = c.String(),
                        ReceiverPhone = c.String(),
                        Text = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        AspNetUser_Id = c.String(maxLength: 128),
                        Doctor_DoctorId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUser_Id)
                .ForeignKey("dbo.Doctors", t => t.Doctor_DoctorId)
                .Index(t => t.AspNetUser_Id)
                .Index(t => t.Doctor_DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SentSmsHistories", "Doctor_DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.SentSmsHistories", "AspNetUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SentEmailHistories", "Doctor_DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.SentEmailHistories", "AspNetUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SentSmsHistories", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.SentSmsHistories", new[] { "AspNetUser_Id" });
            DropIndex("dbo.SentEmailHistories", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.SentEmailHistories", new[] { "AspNetUser_Id" });
            DropTable("dbo.SentSmsHistories");
            DropTable("dbo.SentEmailHistories");
        }
    }
}
