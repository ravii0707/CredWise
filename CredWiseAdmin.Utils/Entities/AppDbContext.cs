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

    public virtual DbSet<GoldLoanDetail> GoldLoanDetails { get; set; }

    public virtual DbSet<HomeLoanDetail> HomeLoanDetails { get; set; }

    public virtual DbSet<LoanApplication> LoanApplications { get; set; }

    public virtual DbSet<LoanBankStatement> LoanBankStatements { get; set; }

    public virtual DbSet<LoanEnquiry> LoanEnquiries { get; set; }

    public virtual DbSet<LoanProduct> LoanProducts { get; set; }

    public virtual DbSet<LoanProductDocument> LoanProductDocuments { get; set; }

    public virtual DbSet<LoanRepaymentSchedule> LoanRepaymentSchedules { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<PersonalLoanDetail> PersonalLoanDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DecisionAppLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Decision__5E548648FC39C733");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProcessedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.DecisionAppLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DecisionA__LoanA__1DB06A4F");
        });

        modelBuilder.Entity<Fdapplication>(entity =>
        {
            entity.HasKey(e => e.FdapplicationId).HasName("PK__FDApplic__5A2486C095E84D9E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Fdtype).WithMany(p => p.Fdapplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDApplica__FDTyp__10566F31");

            entity.HasOne(d => d.User).WithMany(p => p.Fdapplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDApplica__UserI__0F624AF8");
        });

        modelBuilder.Entity<Fdtransaction>(entity =>
        {
            entity.HasKey(e => e.FdtransactionId).HasName("PK__FDTransa__76CF381FEA20E48E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Fdapplication).WithMany(p => p.Fdtransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FDTransac__FDApp__17F790F9");
        });

        modelBuilder.Entity<Fdtype>(entity =>
        {
            entity.HasKey(e => e.FdtypeId).HasName("PK__FDTypes__FFD0291BDED02890");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<GoldLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__GoldLoan__0D22CCC2A858BDD2");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.GoldLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoldLoanD__LoanP__628FA481");
        });

        modelBuilder.Entity<HomeLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__HomeLoan__0D22CCC21811B8E9");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.HomeLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeLoanD__LoanP__59063A47");
        });

        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.LoanApplicationId).HasName("PK__LoanAppl__F60027BD8B8A4CF5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.LoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanAppli__LoanP__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.LoanApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanAppli__UserI__6EF57B66");
        });

        modelBuilder.Entity<LoanBankStatement>(entity =>
        {
            entity.HasKey(e => e.BankStatementId).HasName("PK__LoanBank__D4AD9FA43C3BC6BF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.LoanBankStatements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanBankS__LoanA__76969D2E");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.LoanBankStatements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanBankS__Verif__778AC167");
        });

        modelBuilder.Entity<LoanEnquiry>(entity =>
        {
            entity.HasKey(e => e.EnquiryId).HasName("PK__LoanEnqu__0A019B7D417F16A5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<LoanProduct>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__LoanProd__0D22CCC2C0E64D8F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<LoanProductDocument>(entity =>
        {
            entity.HasKey(e => e.LoanProductDocumentId).HasName("PK__LoanProd__D85B694F0D5F49BF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.LoanProductDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanProdu__LoanP__6754599E");
        });

        modelBuilder.Entity<LoanRepaymentSchedule>(entity =>
        {
            entity.HasKey(e => e.RepaymentId).HasName("PK__LoanRepa__10AD21F27C6F407A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.LoanRepaymentSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoanRepay__LoanA__7E37BEF6");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BF345DF0E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentTr__LoanA__04E4BC85");

            entity.HasOne(d => d.Repayment).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentTr__Repay__05D8E0BE");
        });

        modelBuilder.Entity<PersonalLoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanProductId).HasName("PK__Personal__0D22CCC2566C80F8");

            entity.Property(e => e.LoanProductId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.LoanProduct).WithOne(p => p.PersonalLoanDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonalL__LoanP__5DCAEF64");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6495B77B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
