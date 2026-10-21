namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createdAt : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("public.Users");
            AddColumn("public.ResultBureau", "CreatedBy", c => c.String());
            AddColumn("public.ResultBureau", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultBureau", "UpdatedBy", c => c.String());
            AddColumn("public.ResultBureau", "UpdatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultBureau", "ValidatedBy", c => c.String());
            AddColumn("public.ResultBureau", "ValidatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultCandidat", "CreatedBy", c => c.String());
            AddColumn("public.ResultCandidat", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultCandidat", "UpdatedBy", c => c.String());
            AddColumn("public.ResultCandidat", "UpdatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultCandidat", "ValidatedBy", c => c.String());
            AddColumn("public.ResultCandidat", "ValidatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultList", "CreatedBy", c => c.String());
            AddColumn("public.ResultList", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultList", "UpdatedBy", c => c.String());
            AddColumn("public.ResultList", "UpdatedAt", c => c.DateTime(nullable: false));
            AddColumn("public.ResultList", "ValidatedBy", c => c.String());
            AddColumn("public.ResultList", "ValidatedAt", c => c.DateTime(nullable: false));
            AlterColumn("public.Users", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("public.Users", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("public.Users");
            AlterColumn("public.Users", "Id", c => c.Long(nullable: false, identity: true));
            DropColumn("public.ResultList", "ValidatedAt");
            DropColumn("public.ResultList", "ValidatedBy");
            DropColumn("public.ResultList", "UpdatedAt");
            DropColumn("public.ResultList", "UpdatedBy");
            DropColumn("public.ResultList", "CreatedAt");
            DropColumn("public.ResultList", "CreatedBy");
            DropColumn("public.ResultCandidat", "ValidatedAt");
            DropColumn("public.ResultCandidat", "ValidatedBy");
            DropColumn("public.ResultCandidat", "UpdatedAt");
            DropColumn("public.ResultCandidat", "UpdatedBy");
            DropColumn("public.ResultCandidat", "CreatedAt");
            DropColumn("public.ResultCandidat", "CreatedBy");
            DropColumn("public.ResultBureau", "ValidatedAt");
            DropColumn("public.ResultBureau", "ValidatedBy");
            DropColumn("public.ResultBureau", "UpdatedAt");
            DropColumn("public.ResultBureau", "UpdatedBy");
            DropColumn("public.ResultBureau", "CreatedAt");
            DropColumn("public.ResultBureau", "CreatedBy");
            AddPrimaryKey("public.Users", "Id");
        }
    }
}
