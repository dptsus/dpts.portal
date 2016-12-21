namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDoctor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specialities", "Doctor_Id", c => c.Int());
            CreateIndex("dbo.Specialities", "Doctor_Id");
            AddForeignKey("dbo.Specialities", "Doctor_Id", "dbo.Doctors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Specialities", "Doctor_Id", "dbo.Doctors");
            DropIndex("dbo.Specialities", new[] { "Doctor_Id" });
            DropColumn("dbo.Specialities", "Doctor_Id");
        }
    }
}
