using License_Server.Services.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static License_Server.Services.Licensing.License;

namespace License_Server
{
    public class LicenseDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public LicenseDbContext(IConfiguration configuration,
                                DbContextOptions<LicenseDbContext> options) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<License> Licenses => Set<License>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("MicrosoftSQL"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Convert ENUM to STRING
            modelBuilder.Entity<License>()
                .Property(d => d.Status)
                .HasConversion(new EnumToStringConverter<LICENSE_STATUS>());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken); ;
        }
    }
}
