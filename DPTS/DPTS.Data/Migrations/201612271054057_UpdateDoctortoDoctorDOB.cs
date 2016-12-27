namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateDoctortoDoctorDOB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "DateOfBirth", c => c.DateTime(nullable: true));
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String());
            DropColumn("dbo.Doctors", "DateOfBirth");
        }
    }
}
