namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDoctortoDoctorId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "DoctorId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors", "DoctorId");
        }
    }
}
