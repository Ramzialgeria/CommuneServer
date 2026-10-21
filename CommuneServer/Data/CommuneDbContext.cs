using CommuneServer.Data.Entities;
using CommuneServer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace CommuneServer.Data
{
    public class CommuneDbContext : DbContext
    {
        public CommuneDbContext()
            : base("name=CommuneDb")
        {
        }

        public DbSet<ActivationInfo> Activations { get; set; }

        public DbSet<Cm> Cm { get; set; }   // Commune
        public DbSet<Cn> Cn { get; set; }   // Centre
        public DbSet<Bu> Bu { get; set; }   // Bureau
        public DbSet<LE> LE { get; set; }   // ListeElectoral
        public DbSet<Cd> Cd { get; set; }   // Candidat
        public DbSet<U> U { get; set; }     // User

        public DbSet<RB> RB { get; set; }   // ResultBureau
        public DbSet<RL> RL { get; set; }   // ResultList
        public DbSet<RC> RC { get; set; }   // ResultCandidat

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // TABLES
            // =====================================================

            modelBuilder.Entity<Cm>()
                .ToTable("Cm", "public");

            modelBuilder.Entity<Cn>()
                .ToTable("Cn", "public");

            modelBuilder.Entity<Bu>()
                .ToTable("Bu", "public");

            modelBuilder.Entity<LE>()
                .ToTable("LE", "public");

            modelBuilder.Entity<Cd>()
                .ToTable("Cd", "public");

            modelBuilder.Entity<U>()
                .ToTable("U", "public");

            modelBuilder.Entity<ActivationInfo>()
                .ToTable("Activations", "public");

            modelBuilder.Entity<RB>()
                .ToTable("RB", "public");

            modelBuilder.Entity<RL>()
                .ToTable("RL", "public");

            modelBuilder.Entity<RC>()
                .ToTable("RC", "public");


            // =====================================================
            // Cm -> Cn
            // Commune 1 ---- * Centre
            // =====================================================

            modelBuilder.Entity<Cn>()
                .HasRequired(x => x.Cm)
                .WithMany(x => x.Cns)
                .HasForeignKey(x => x.Cmd)
                .WillCascadeOnDelete(true);


            // =====================================================
            // Cn -> Bu
            // Centre 1 ---- * Bureau
            // =====================================================

            modelBuilder.Entity<Bu>()
                .HasRequired(x => x.Cn)
                .WithMany(x => x.Bux)
                .HasForeignKey(x => x.Cnd)
                .WillCascadeOnDelete(true);


            // =====================================================
            // LE -> Cd
            // ListeElectoral 1 ---- * Candidat
            // =====================================================

            modelBuilder.Entity<Cd>()
                .HasRequired(x => x.LE)
                .WithMany(x => x.Cds)
                .HasForeignKey(x => x.LId)
                .WillCascadeOnDelete(true);


            // =====================================================
            // RB -> Bu
            // ResultBureau -> Bureau
            // =====================================================

            modelBuilder.Entity<RB>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<RB>()
                .HasRequired(x => x.B)
                .WithMany()
                .HasForeignKey(x => x.Bid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RB>()
                .HasIndex(x => x.Bid)
                .IsUnique();

            // =====================================================
            // RL -> LE
            // ResultList -> ListeElectoral
            // =====================================================

            modelBuilder.Entity<RL>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<RL>()
                .HasRequired(x => x.L)
                .WithMany()
                .HasForeignKey(x => x.Lid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RL>()
                .HasIndex(x => x.Lid)
                .IsUnique();

            // =====================================================
            // RC -> Cd
            // ResultCandidat -> Candidat
            // =====================================================

            modelBuilder.Entity<RC>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<RC>()
                .HasRequired(x => x.C)
                .WithMany()
                .HasForeignKey(x => x.Cid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RC>()
                .HasIndex(x => x.Cid)
                .IsUnique();

            // =====================================================
            // IDs are supplied by the activation package
            // =====================================================

            modelBuilder.Entity<Cm>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Cm>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Cn>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Cn>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Bu>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Bu>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<LE>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<LE>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Cd>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Cd>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<U>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<U>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Commune>().ToTable("Commune", "public");
        //    modelBuilder.Entity<Centre>().ToTable("Centre", "public");
        //    modelBuilder.Entity<Bureau>().ToTable("Bureau", "public");
        //    modelBuilder.Entity<ListeElectoral>().ToTable("ListeElectoral", "public");
        //    modelBuilder.Entity<Candidat>().ToTable("Candidat", "public");

        //    modelBuilder.Entity<User>().ToTable("Users", "public");
        //    modelBuilder.Entity<ActivationInfo>().ToTable("Activations", "public");

        //    modelBuilder.Entity<ResultBureau>().ToTable("ResultBureau", "public");
        //    modelBuilder.Entity<ResultList>().ToTable("ResultList", "public");
        //    modelBuilder.Entity<ResultCandidat>().ToTable("ResultCandidat", "public");

        //    // Candidat -> ListeElectoral
        //    modelBuilder.Entity<Candidat>()
        //        .HasRequired(x => x.ListeElectoral)
        //        .WithMany(x => x.Candidats)
        //        .HasForeignKey(x => x.ListeElectoralId)
        //        .WillCascadeOnDelete(true);


        //    // ResultBureau.Id is the primary key
        //    modelBuilder.Entity<ResultBureau>()
        //        .HasKey(x => x.Id);

        //    // ResultBureau.BureauId -> Bureau.Id
        //    modelBuilder.Entity<ResultBureau>()
        //        .HasRequired(x => x.Bureau)
        //        .WithMany()
        //        .HasForeignKey(x => x.BureauId)
        //        .WillCascadeOnDelete(false);

        //    // ResultList.Id is the primary key
        //    modelBuilder.Entity<ResultList>()
        //        .HasKey(x => x.Id);

        //    // ResultList.ListeElectoralId -> ListeElectoral.Id
        //    modelBuilder.Entity<ResultList>()
        //        .HasRequired(x => x.ListeElectoral)
        //        .WithMany()
        //        .HasForeignKey(x => x.ListeElectoralId)
        //        .WillCascadeOnDelete(false);

        //    // ResultCandidat.Id is the primary key
        //    modelBuilder.Entity<ResultCandidat>()
        //        .HasKey(x => x.Id);

        //    // ResultCandidat.CandidatId -> Candidat.Id
        //    modelBuilder.Entity<ResultCandidat>()
        //        .HasRequired(x => x.Candidat)
        //        .WithMany()
        //        .HasForeignKey(x => x.CandidatId)
        //        .WillCascadeOnDelete(false);
        //}
    }
}
