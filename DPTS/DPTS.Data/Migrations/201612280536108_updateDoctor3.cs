namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class updateDoctor3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String());
        }

        public override void Down()
        {
            AlterColumn("dbo.Doctors", "DoctorId", c => c.String(nullable: false));
        }
    }
}
