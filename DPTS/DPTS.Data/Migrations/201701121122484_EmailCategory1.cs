namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EmailCategory1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailCategories", "TwoLetterIsoCode");
            DropColumn("dbo.EmailCategories", "ThreeLetterIsoCode");
            DropColumn("dbo.EmailCategories", "NumericIsoCode");
            DropColumn("dbo.EmailCategories", "SubjectToVat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailCategories", "SubjectToVat", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmailCategories", "NumericIsoCode", c => c.Int(nullable: false));
            AddColumn("dbo.EmailCategories", "ThreeLetterIsoCode", c => c.String());
            AddColumn("dbo.EmailCategories", "TwoLetterIsoCode", c => c.String());
        }
    }
}
