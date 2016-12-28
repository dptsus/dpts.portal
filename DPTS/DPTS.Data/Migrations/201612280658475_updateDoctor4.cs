namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDoctor4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String());
        }
    }
}
