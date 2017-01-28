namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ReviewTablecreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReviewComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommentForId = c.Int(nullable: false),
                        CommentOwnerId = c.String(),
                        Comment = c.String(),
                        Rating = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
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
            DropForeignKey("dbo.ReviewComments", "Doctor_DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.ReviewComments", "AspNetUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ReviewComments", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.ReviewComments", new[] { "AspNetUser_Id" });
            DropTable("dbo.ReviewComments");
        }
    }
}
