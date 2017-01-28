namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppointmentSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppointmentSchedules", "Subject", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppointmentSchedules", "Subject");
        }
    }
}
