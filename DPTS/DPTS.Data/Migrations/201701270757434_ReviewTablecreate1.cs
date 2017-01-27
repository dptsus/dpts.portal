namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewTablecreate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReviewComments", "Rating", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReviewComments", "Rating", c => c.Boolean(nullable: false));
        }
    }
}
