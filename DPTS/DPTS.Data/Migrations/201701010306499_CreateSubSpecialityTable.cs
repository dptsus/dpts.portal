namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSubSpecialityTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubSpecialities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SpecialityId = c.Int(nullable: false),
                        Name = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Specialities", t => t.SpecialityId, cascadeDelete: true)
                .Index(t => t.SpecialityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubSpecialities", "SpecialityId", "dbo.Specialities");
            DropIndex("dbo.SubSpecialities", new[] { "SpecialityId" });
            DropTable("dbo.SubSpecialities");
        }
    }
}
