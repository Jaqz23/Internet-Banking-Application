using IB.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IB.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            #region Tables
            modelBuilder.Entity<Beneficiary>().ToTable("Beneficiaries");
            modelBuilder.Entity<SavingsAccount>().ToTable("SavingsAccounts");
            modelBuilder.Entity<Loan>().ToTable("Loans");
            modelBuilder.Entity<CreditCard>().ToTable("CreditCards");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            #endregion

            #region Primary Keys
            modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
            modelBuilder.Entity<SavingsAccount>().HasKey(sa => sa.Id);
            modelBuilder.Entity<Loan>().HasKey(l => l.Id);
            modelBuilder.Entity<CreditCard>().HasKey(cc => cc.Id);
            modelBuilder.Entity<Beneficiary>().HasKey(b => b.Id);
            #endregion

            #region RelationShip

            // Un usuario puede tener muchas cuentas de ahorro
            modelBuilder.Entity<SavingsAccount>()
                .HasMany(sa => sa.Transactions)
                .WithOne(t => t.SavingsAccount)
                .HasForeignKey(t => t.SavingsAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un usuario puede tener varias tarjetas de credito
            modelBuilder.Entity<CreditCard>()
                .HasMany(cc => cc.Transactions)
                .WithOne(t => t.CreditCard)
                .HasForeignKey(t => t.CreditCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un usuario puede tener varios prestamos
            modelBuilder.Entity<Loan>()
                .HasMany(l => l.Transactions)
                .WithOne(t => t.Loan)
                .HasForeignKey(t => t.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Una cuenta de ahorro puede tener varios beneficiarios
            modelBuilder.Entity<SavingsAccount>()
                .HasMany(sa => sa.Beneficiaries)
                .WithOne(b => b.SavingsAccount)
                .HasForeignKey(b => b.SavingsAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un beneficiario puede estar asociado a varias transacciones
            modelBuilder.Entity<Beneficiary>()
                .HasMany(b => b.Transactions)
                .WithOne(t => t.Beneficiary)
                .HasForeignKey(t => t.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Property Configuration

            // Configuración de UserId en todas las entidades
            modelBuilder.Entity<SavingsAccount>()
                .Property(sa => sa.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<CreditCard>()
                .Property(cc => cc.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<Loan>()
                .Property(l => l.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<Beneficiary>()
                .Property(b => b.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            // Configuracion de AccountNumber y CardNumber como unicos
            modelBuilder.Entity<SavingsAccount>()
                .Property(sa => sa.AccountNumber)
                .IsRequired();

            modelBuilder.Entity<CreditCard>()
                .Property(cc => cc.CardNumber)
                .IsRequired();

            // Configuración de precision para montos de dinero
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            modelBuilder.Entity<SavingsAccount>()
                .Property(sa => sa.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            modelBuilder.Entity<Loan>()
                .Property(l => l.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            modelBuilder.Entity<Loan>()
                .Property(l => l.RemainingBalance)
                .HasPrecision(18, 2)
                .IsRequired();

            modelBuilder.Entity<CreditCard>()
                .Property(cc => cc.CreditLimit)
                .HasPrecision(18, 2)
                .IsRequired();

            modelBuilder.Entity<CreditCard>()
                .Property(cc => cc.Debt)
                .HasPrecision(18, 2)
                .IsRequired();

            #endregion

            #region Indexes
            modelBuilder.Entity<SavingsAccount>().HasIndex(sa => sa.AccountNumber).IsUnique();
            modelBuilder.Entity<CreditCard>().HasIndex(cc => cc.CardNumber).IsUnique();
            #endregion
        }
    }
}
