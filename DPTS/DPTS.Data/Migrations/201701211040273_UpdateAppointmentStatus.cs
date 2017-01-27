namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppointmentStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppointmentStatus", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppointmentStatus", "Description");
        }
    }
}
