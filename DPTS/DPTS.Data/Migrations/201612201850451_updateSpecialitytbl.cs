namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSpecialitytbl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Specialities", "Title", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Specialities", "Title", c => c.String());
        }
    }
}
