namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LatLongAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AddColumn("dbo.Addresses", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 9));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "Longitude");
            DropColumn("dbo.Addresses", "Latitude");
        }
    }
}
