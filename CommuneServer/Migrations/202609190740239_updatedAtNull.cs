namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAtNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("public.ResultBureau", "UpdatedAt", c => c.DateTime());
            AlterColumn("public.ResultBureau", "ValidatedAt", c => c.DateTime());
            AlterColumn("public.ResultCandidat", "UpdatedAt", c => c.DateTime());
            AlterColumn("public.ResultCandidat", "ValidatedAt", c => c.DateTime());
            AlterColumn("public.ResultList", "UpdatedAt", c => c.DateTime());
            AlterColumn("public.ResultList", "ValidatedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("public.ResultList", "ValidatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.ResultList", "UpdatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.ResultCandidat", "ValidatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.ResultCandidat", "UpdatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.ResultBureau", "ValidatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.ResultBureau", "UpdatedAt", c => c.DateTime(nullable: false));
        }
    }
}
