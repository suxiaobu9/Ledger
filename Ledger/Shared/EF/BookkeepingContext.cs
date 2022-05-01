using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Ledger.Server
{
    public partial class BookkeepingContext : DbContext
    {
        public BookkeepingContext()
        {
        }

        public BookkeepingContext(DbContextOptions<BookkeepingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accounting> Accountings { get; set; } = null!;
        public virtual DbSet<DeleteAccount> DeleteAccounts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Accounting>(entity =>
            {
                entity.ToTable("Accounting");

                entity.HasIndex(e => new { e.AccountDate, e.UserId }, "IX_Accounting");

                entity.Property(e => e.AccountDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Event).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Accountings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accounting_User");
            });

            modelBuilder.Entity<DeleteAccount>(entity =>
            {
                entity.ToTable("DeleteAccount");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.DeleteAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DeleteAccount_Accounting");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Name).HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
