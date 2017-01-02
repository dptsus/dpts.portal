namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateAddressMappingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        AddressId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.AddressMappings");
        }
    }
}
