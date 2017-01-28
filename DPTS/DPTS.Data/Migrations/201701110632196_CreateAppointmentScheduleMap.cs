namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateAppointmentScheduleMap : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppointmentSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.String(nullable: false, maxLength: 128),
                        PatientId = c.String(nullable: false, maxLength: 128),
                        DiseasesDescription = c.String(),
                        StatusId = c.Int(nullable: false),
                        AppointmentTime = c.Time(nullable: false, precision: 7),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppointmentStatus", t => t.StatusId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)
                .ForeignKey("dbo.AspNetUsers", t => t.PatientId)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId)
                .Index(t => t.StatusId);

            CreateTable(
                "dbo.AppointmentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.String(nullable: false, maxLength: 128),
                        Day = c.String(nullable: false, maxLength: 10),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)
                .Index(t => t.DoctorId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AppointmentSchedules", "PatientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Schedules", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.AppointmentSchedules", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.AppointmentSchedules", "StatusId", "dbo.AppointmentStatus");
            DropIndex("dbo.Schedules", new[] { "DoctorId" });
            DropIndex("dbo.AppointmentSchedules", new[] { "StatusId" });
            DropIndex("dbo.AppointmentSchedules", new[] { "PatientId" });
            DropIndex("dbo.AppointmentSchedules", new[] { "DoctorId" });
            DropTable("dbo.Schedules");
            DropTable("dbo.AppointmentStatus");
            DropTable("dbo.AppointmentSchedules");
        }
    }
}
