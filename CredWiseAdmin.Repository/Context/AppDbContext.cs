using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CredWiseAdmin.Core.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DecisionAppLog> DecisionAppLogs { get; set; }

    public virtual DbSet<Fdapplication> Fdapplications { get; set; }

    public virtual DbSet<Fdtransaction> Fdtransactions { get; set; }

    public virtual DbSet<Fdtype> Fdtypes { get; set; }

    public virtual DbSet<GoldLoanApplication> GoldLoanApplications { get; set; }

    public virtual DbSet<GoldLoanDetail> GoldLoanDetails { get; set; }

    public virtual DbSet<HomeLoanApplication> HomeLoanApplications { get; set; }

    public virtual DbSet<HomeLoanDetail> HomeLoanDetails { get; set; }

    public virtual DbSet<LoanApplication> LoanApplications { get; set; }

    public virtual DbSet<LoanBankStatement> LoanBankStatements { get; set; }

    public virtual DbSet<LoanEnquiry> LoanEnquiries { get; set; }

    public virtual DbSet<LoanProduct> LoanProducts { get; set; }

    public virtual DbSet<LoanProductDocument> LoanProductDocuments { get; set; }

    public virtual DbSet<LoanRepaymentSchedule> LoanRepaymentSchedules { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<PersonalLoanDetail> PersonalLoanDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DecisionAppLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Decision__5E548648AC8B9A9E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProcessedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.DecisionAppLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DecisionA__LoanA__0C85DE4D");
        });

        modelBuilder.Entity<Fdapplication>(entity =>
        {
            entity.HasKey(e => e.FdapplicationId).HasName("PK__FDApplic__5A2486C043A982AE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Fdtype).WithMany(p => p.Fdapplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDApplica__FDTyp__0D7A0286");

            entity.HasOne(d => d.User).WithMany(p => p.Fdapplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDApplica__UserI__0E6E26BF");
        });

        modelBuilder.Entity<Fdtransaction>(entity =>
        {
            entity.HasKey(e => e.FdtransactionId).HasName("PK__FDTransa__76CF381F1C7EAE46");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Fdapplication).WithMany(p => p.Fdtransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDTransac__FDApp__0F624AF8");
        });

        modelBuilder.Entity<Fdtype>(entity =>
        {
            entity.HasKey(e => e.FdtypeId).HasName("PK__FDTypes__FFD0291BD069EBDD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<GoldLoanApplication>(entity =>
        {
            entity.HasKey(e => e.GoldLoanAppId).HasName("PK__GoldLoan__BD7F0043D33979CF");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.GoldLoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoldLoanA__LoanA__10566F31");
        });

        modelBuilder.Entity<GoldLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__GoldLoan__0D22CCC2361E90BE");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.GoldLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoldLoanD__LoanP__114A936A");
        });

        modelBuilder.Entity<HomeLoanApplication>(entity =>
        {
            entity.HasKey(e => e.HomeLoanAppId).HasName("PK__HomeLoan__08F023772BB46EE2");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.HomeLoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeLoanA__LoanA__123EB7A3");
        });

        modelBuilder.Entity<HomeLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__HomeLoan__0D22CCC2EC79413F");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.HomeLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeLoanD__LoanP__1332DBDC");
        });

        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.LoanApplicationId).HasName("PK__LoanAppl__F60027BD921E9060");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.LoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanAppli__LoanP__14270015");

            entity.HasOne(d => d.User).WithMany(p => p.LoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanAppli__UserI__151B244E");
        });

        modelBuilder.Entity<LoanBankStatement>(entity =>
        {
            entity.HasKey(e => e.BankStatementId).HasName("PK__LoanBank__D4AD9FA4D6AAB1D6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.LoanBankStatements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanBankS__LoanA__160F4887");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.LoanBankStatements).HasConstraintName("FK__LoanBankS__Verif__17036CC0");
        });

        modelBuilder.Entity<LoanEnquiry>(entity =>
        {
            entity.HasKey(e => e.EnquiryId).HasName("PK__LoanEnqu__0A019B7DBAF502DE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<LoanProduct>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__LoanProd__0D22CCC2F1619D3D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<LoanProductDocument>(entity =>
        {
            entity.HasKey(e => e.LoanProductDocumentId).HasName("PK__LoanProd__D85B694FD1A6229C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.LoanProductDocuments).HasConstraintName("FK__LoanProdu__LoanA__17F790F9");

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.LoanProductDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanProdu__LoanP__18EBB532");
        });

        modelBuilder.Entity<LoanRepaymentSchedule>(entity =>
        {
            entity.HasKey(e => e.RepaymentId).HasName("PK__LoanRepa__10AD21F2821F615C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.LoanRepaymentSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanRepay__LoanA__19DFD96B");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC078D6788CC");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BDDF2F2F0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentTr__LoanA__1AD3FDA4");

            entity.HasOne(d => d.Repayment).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentTr__Repay__1BC821DD");
        });

        modelBuilder.Entity<PersonalLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__Personal__0D22CCC2E53A9142");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.PersonalLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonalL__LoanP__1CBC4616");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C225C0E65");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
