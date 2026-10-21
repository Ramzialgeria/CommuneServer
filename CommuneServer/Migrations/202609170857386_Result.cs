namespace CommuneServer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Result : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.ResultBureau",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    BureauId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Bureau", t => t.BureauId)
                .Index(t => t.BureauId, unique: true);

            CreateTable(
                "public.ResultCandidat",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    CandidatId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Candidat", t => t.CandidatId)
                .Index(t => t.CandidatId, unique: true);

            CreateTable(
                "public.ResultList",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    ListeElectoralId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.ListeElectoral", t => t.ListeElectoralId)
                .Index(t => t.ListeElectoralId, unique: true);
        }

        public override void Down()
        {
            DropForeignKey(
                "public.ResultList",
                "ListeElectoralId",
                "public.ListeElectoral");

            DropForeignKey(
                "public.ResultCandidat",
                "CandidatId",
                "public.Candidat");

            DropForeignKey(
                "public.ResultBureau",
                "BureauId",
                "public.Bureau");

            DropIndex(
                "public.ResultList",
                new[] { "ListeElectoralId" });

            DropIndex(
                "public.ResultCandidat",
                new[] { "CandidatId" });

            DropIndex(
                "public.ResultBureau",
                new[] { "BureauId" });

            DropTable("public.ResultList");
            DropTable("public.ResultCandidat");
            DropTable("public.ResultBureau");
        }
    }
}