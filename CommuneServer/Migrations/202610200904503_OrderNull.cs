namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("public.Cd", "O", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("public.Cd", "O", c => c.Int(nullable: false));
        }
    }
}
