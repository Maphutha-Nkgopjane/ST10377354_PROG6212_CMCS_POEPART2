using CMCS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClaimDocument>().ToTable("ClaimDocuments"); // ✅ pluralized to match DB

            builder.Entity<ClaimDocument>()
                .Property(c => c.HourlyRate)
                .HasPrecision(18, 2);

            builder.Entity<ClaimDocument>()
                .Property(c => c.Notes)
                .HasMaxLength(400);
        }
    }
}
