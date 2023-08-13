﻿using License_Server.Services.LicenseService;
using Microsoft.EntityFrameworkCore;

namespace Licensing_System
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken); ;
        }
    }
}