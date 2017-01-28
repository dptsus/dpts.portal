namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDoctorDOB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "DateOfBirth", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors", "DateOfBirth", c => c.DateTime());
        }
    }
}
