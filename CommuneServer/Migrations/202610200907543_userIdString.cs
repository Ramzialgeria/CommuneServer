namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userIdString : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("public.U");
            AlterColumn("public.U", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("public.U", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("public.U");
            AlterColumn("public.U", "Id", c => c.Long(nullable: false));
            AddPrimaryKey("public.U", "Id");
        }
    }
}
