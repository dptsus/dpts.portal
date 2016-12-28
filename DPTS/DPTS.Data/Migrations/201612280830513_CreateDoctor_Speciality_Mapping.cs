namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDoctor_Speciality_Mapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doctor_Speciality_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Doctor_Id = c.String(),
                        Speciality_Id = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Doctor_Speciality_Mapping");
        }
    }
}
