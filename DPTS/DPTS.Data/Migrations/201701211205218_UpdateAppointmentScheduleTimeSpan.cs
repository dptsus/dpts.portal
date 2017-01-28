namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppointmentScheduleTimeSpan : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AppointmentSchedules", "AppointmentTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AppointmentSchedules", "AppointmentTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
