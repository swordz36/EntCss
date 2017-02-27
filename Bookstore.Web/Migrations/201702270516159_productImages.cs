namespace BabyStore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductImages");
        }
    }
}
