namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class updateDoctorToYrsofExpr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "YearsOfExperience", c => c.Int(nullable: true));
        }

        public override void Down()
        {
            AlterColumn("dbo.Doctors", "YearsOfExperience", c => c.String());
        }
    }
}
