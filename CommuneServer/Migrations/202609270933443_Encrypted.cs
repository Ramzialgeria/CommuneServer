namespace CommuneServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Encrypted : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.Bureau", "CentreId", "public.Centre");
            DropForeignKey("public.Centre", "CommuneId", "public.Commune");
            DropForeignKey("public.Candidat", "ListeElectoralId", "public.ListeElectoral");
            DropForeignKey("public.ResultBureau", "BureauId", "public.Bureau");
            DropForeignKey("public.ResultCandidat", "CandidatId", "public.Candidat");
            DropForeignKey("public.ResultList", "ListeElectoralId", "public.ListeElectoral");
            DropIndex("public.Bureau", new[] { "CentreId" });
            DropIndex("public.Centre", new[] { "CommuneId" });
            DropIndex("public.Candidat", new[] { "ListeElectoralId" });
            DropIndex("public.ResultBureau", new[] { "BureauId" });
            DropIndex("public.ResultCandidat", new[] { "CandidatId" });
            DropIndex("public.ResultList", new[] { "ListeElectoralId" });
            CreateTable(
                "public.Bu",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        C = c.String(),
                        T = c.String(),
                        NI = c.Int(nullable: false),
                        Cnd = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Cn", t => t.Cnd, cascadeDelete: true)
                .Index(t => t.Cnd);
            
            CreateTable(
                "public.Cn",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        C = c.String(),
                        LEncrypted = c.String(),
                        T = c.String(),
                        NI = c.Int(nullable: false),
                        Cmd = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Cm", t => t.Cmd, cascadeDelete: true)
                .Index(t => t.Cmd);
            
            CreateTable(
                "public.Cm",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        CC = c.String(),
                        CW = c.String(),
                        NCEncrypted = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Cd",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        NAEncrypted = c.String(),
                        NFEncrypted = c.String(),
                        O = c.Int(nullable: false),
                        LId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.LE", t => t.LId, cascadeDelete: true)
                .Index(t => t.LId);
            
            CreateTable(
                "public.LE",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        LEncrypted = c.String(),
                        NI = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.RB",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Bid = c.Long(nullable: false),
                        CB = c.String(),
                        CA = c.DateTime(nullable: false),
                        UBy = c.String(),
                        UA = c.DateTime(),
                        VB = c.String(),
                        VA = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Bu", t => t.Bid)
                .Index(t => t.Bid, unique: true);
            
            CreateTable(
                "public.RC",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Cid = c.Long(nullable: false),
                        CB = c.String(),
                        CA = c.DateTime(nullable: false),
                        UBy = c.String(),
                        UA = c.DateTime(),
                        VB = c.String(),
                        VA = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Cd", t => t.Cid)
                .Index(t => t.Cid, unique: true);
            
            CreateTable(
                "public.RL",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Lid = c.Long(nullable: false),
                        CB = c.String(),
                        CA = c.DateTime(nullable: false),
                        UBy = c.String(),
                        UA = c.DateTime(),
                        VB = c.String(),
                        VA = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.LE", t => t.Lid)
                .Index(t => t.Lid, unique: true);
            
            CreateTable(
                "public.U",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Un = c.String(),
                        PH = c.String(),
                        IA = c.Boolean(nullable: false),
                        Rl = c.String(),
                        CS = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("public.Bureau");
            DropTable("public.Centre");
            DropTable("public.Commune");
            DropTable("public.Candidat");
            DropTable("public.ListeElectoral");
            DropTable("public.ResultBureau");
            DropTable("public.ResultCandidat");
            DropTable("public.ResultList");
            DropTable("public.Users");
        }
        
        public override void Down()
        {
            CreateTable(
                "public.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Role = c.String(),
                        CodeStructure = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ResultList",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ListeElectoralId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedAt = c.DateTime(),
                        ValidatedBy = c.String(),
                        ValidatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ResultCandidat",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CandidatId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedAt = c.DateTime(),
                        ValidatedBy = c.String(),
                        ValidatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ResultBureau",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BureauId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedAt = c.DateTime(),
                        ValidatedBy = c.String(),
                        ValidatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("public.RL", "Lid", "public.LE");
            DropForeignKey("public.RC", "Cid", "public.Cd");
            DropForeignKey("public.RB", "Bid", "public.Bu");
            DropForeignKey("public.Cd", "LId", "public.LE");
            DropForeignKey("public.Bu", "Cnd", "public.Cn");
            DropForeignKey("public.Cn", "Cmd", "public.Cm");
            DropIndex("public.RL", new[] { "Lid" });
            DropIndex("public.RC", new[] { "Cid" });
            DropIndex("public.RB", new[] { "Bid" });
            DropIndex("public.Cd", new[] { "LId" });
            DropIndex("public.Cn", new[] { "Cmd" });
            DropIndex("public.Bu", new[] { "Cnd" });
            DropTable("public.U");
            DropTable("public.RL");
            DropTable("public.RC");
            DropTable("public.RB");
            DropTable("public.LE");
            DropTable("public.Cd");
            DropTable("public.Cm");
            DropTable("public.Cn");
            DropTable("public.Bu");
            CreateIndex("public.ResultList", "ListeElectoralId");
            CreateIndex("public.ResultCandidat", "CandidatId");
            CreateIndex("public.ResultBureau", "BureauId");
            CreateIndex("public.Candidat", "ListeElectoralId");
            CreateIndex("public.Centre", "CommuneId");
            CreateIndex("public.Bureau", "CentreId");
            AddForeignKey("public.ResultList", "ListeElectoralId", "public.ListeElectoral", "Id");
            AddForeignKey("public.ResultCandidat", "CandidatId", "public.Candidat", "Id");
            AddForeignKey("public.ResultBureau", "BureauId", "public.Bureau", "Id");
            AddForeignKey("public.Candidat", "ListeElectoralId", "public.ListeElectoral", "Id", cascadeDelete: true);
            AddForeignKey("public.Centre", "CommuneId", "public.Commune", "Id", cascadeDelete: true);
            AddForeignKey("public.Bureau", "CentreId", "public.Centre", "Id", cascadeDelete: true);
        }
    }
}
