namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class updateScheduleTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Schedules", "StartTime", c => c.String());
            AlterColumn("dbo.Schedules", "EndTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Schedules", "EndTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.Schedules", "StartTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
