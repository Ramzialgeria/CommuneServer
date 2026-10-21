namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.Activations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivationId = c.String(),
                        ActivationVersion = c.Int(nullable: false),
                        CodeCommune = c.String(),
                        ActivatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "public.Commune",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    CodeCommune = c.String(),
                    NameCommune = c.String(),
                    CodeWilaya = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "public.Bureau",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Type = c.String(),
                        NbInscrit = c.Int(nullable: false),
                        CentreId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Centre", t => t.CentreId, cascadeDelete: true)
                .Index(t => t.CentreId);
            
            CreateTable(
                "public.Centre",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Lib = c.String(),
                        Type = c.String(),
                        NbInscrit = c.Int(nullable: false),
                        CommuneId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Commune", t => t.CommuneId, cascadeDelete: true)
                .Index(t => t.CommuneId);
            
            CreateTable(
             "public.ListeElectoral",
             c => new
             {
                 Id = c.Long(nullable: false, identity: true),
                 Lib = c.String(),
                 NumIdentifiant = c.Int(nullable: false),
             })
             .PrimaryKey(t => t.Id);

            CreateTable(
                    "public.Candidat",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FullNameAr = c.String(),
                        FullNameFr = c.String(),
                        Ordre = c.Int(nullable: false),
                        ListeElectoralId = c.Long(nullable: false),
                    })
                    .PrimaryKey(t => t.Id)
                    .ForeignKey("public.ListeElectoral", t => t.ListeElectoralId, cascadeDelete: true)
                    .Index(t => t.ListeElectoralId);
            
            CreateTable(
                "public.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Role = c.String(),
                        CodeStructure = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }

        public override void Down()
        {
            DropForeignKey("public.Candidat", "ListeElectoralId", "public.ListeElectoral");
            DropForeignKey("public.Centre", "CommuneId", "public.Commune");
            DropForeignKey("public.Bureau", "CentreId", "public.Centre");

            DropIndex("public.Candidat", new[] { "ListeElectoralId" });
            DropIndex("public.Centre", new[] { "CommuneId" });
            DropIndex("public.Bureau", new[] { "CentreId" });

            DropTable("public.Users");
            DropTable("public.Candidat");
            DropTable("public.ListeElectoral");
            DropTable("public.Commune");
            DropTable("public.Centre");
            DropTable("public.Bureau");
            DropTable("public.Activations");
        }
    }
}
