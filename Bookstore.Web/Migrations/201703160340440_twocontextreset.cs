namespace BabyStore.Web.StoreMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class twocontextreset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductImageMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageNumber = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProductImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.ProductImages", t => t.ProductImageId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ProductImageId);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FileName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImageMappings", "ProductImageId", "dbo.ProductImages");
            DropForeignKey("dbo.ProductImageMappings", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.ProductImages", new[] { "FileName" });
            DropIndex("dbo.ProductImageMappings", new[] { "ProductImageId" });
            DropIndex("dbo.ProductImageMappings", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.ProductImages");
            DropTable("dbo.ProductImageMappings");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
