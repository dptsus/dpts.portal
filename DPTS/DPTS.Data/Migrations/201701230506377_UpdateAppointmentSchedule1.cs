namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppointmentSchedule1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppointmentSchedules", "AppointmentDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppointmentSchedules", "AppointmentDate");
        }
    }
}
