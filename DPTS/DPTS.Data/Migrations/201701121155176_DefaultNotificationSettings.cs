namespace DPTS.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DefaultNotificationSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DefaultNotificationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryId = c.Int(nullable: false),
                        Name = c.String(),
                        Abbreviation = c.String(),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        EmailCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailCategories", t => t.EmailCategory_Id)
                .Index(t => t.EmailCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DefaultNotificationSettings", "EmailCategory_Id", "dbo.EmailCategories");
            DropIndex("dbo.DefaultNotificationSettings", new[] { "EmailCategory_Id" });
            DropTable("dbo.DefaultNotificationSettings");
        }
    }
}
