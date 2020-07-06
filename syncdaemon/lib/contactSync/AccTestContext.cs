using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace syncdaemon
{
    //TODO: Get access to production DB and set constring, new dbContext
    ///<summary>
    ///This is the db context for the TEST SQL database. This was scaffolded using Entity Framework - dbcontext scaffold
    ///</summary>
    public partial class AccTestContext : DbContext
    {
        private IConfigurationRoot _configuration { get; }
        public AccTestContext(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public AccTestContext(DbContextOptions<AccTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Patient> Patient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["AccTestDB"],
                builder => builder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AccountId).HasColumnName("Account ID");

                entity.Property(e => e.Address1).HasMaxLength(255);

                entity.Property(e => e.Address2).HasMaxLength(255);

                entity.Property(e => e.CellPhone)
                    .HasColumnName("Cell Phone")
                    .HasMaxLength(255);

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .HasColumnName("First Name")
                    .HasMaxLength(255);

                entity.Property(e => e.HomePhone)
                    .HasColumnName("Home Phone")
                    .HasMaxLength(255);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LastName)
                    .HasColumnName("Last Name")
                    .HasMaxLength(255);

                entity.Property(e => e.MiddleName)
                    .HasColumnName("Middle Name")
                    .HasMaxLength(255);

                entity.Property(e => e.Nickname).HasMaxLength(255);

                entity.Property(e => e.OtherPhone)
                    .HasColumnName("Other Phone")
                    .HasMaxLength(255);

                entity.Property(e => e.PreferredContact)
                    .HasColumnName("Preferred Contact")
                    .HasMaxLength(255);

                entity.Property(e => e.PreferredContactType)
                    .HasColumnName("Preferred Contact Type")
                    .HasMaxLength(255);

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.Suffix).HasMaxLength(255);

                entity.Property(e => e.TodaysDate)
                    .HasColumnName("Todays Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.WorkPhone)
                    .HasColumnName("Work Phone")
                    .HasMaxLength(255);

                entity.Property(e => e.Zipcode).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
