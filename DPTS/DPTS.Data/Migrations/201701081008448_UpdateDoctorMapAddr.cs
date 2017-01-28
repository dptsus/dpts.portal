namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDoctorMapAddr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AddressMappings", "Doctor_DoctorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.AddressMappings", "Doctor_DoctorId");
            AddForeignKey("dbo.AddressMappings", "Doctor_DoctorId", "dbo.Doctors", "DoctorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AddressMappings", "Doctor_DoctorId", "dbo.Doctors");
            DropIndex("dbo.AddressMappings", new[] { "Doctor_DoctorId" });
            DropColumn("dbo.AddressMappings", "Doctor_DoctorId");
        }
    }
}
