using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IDGFAuth.Data.Configs;
using IDGFAuth.Data.Entities;
using IDGF.AuthDB.Data.Entities;

namespace IDGFAuth.Data
{
    public class IDGFAuthDbContextSQL : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        private readonly IServiceProvider _serviceProvider;
        public IDGFAuthDbContextSQL(DbContextOptions<IDGFAuthDbContextSQL> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public IDGFAuthDbContextSQL(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<ApplicationUser> ApplicationUsers{ get; set; }
        public DbSet<ClaimDefinition> ClaimDefinitions { get; set; }

        private static string getConnectionStringSQLServer()
        {
            var environmentName =
              Environment.GetEnvironmentVariable(
                  "ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder().AddJsonFile("appsettings" + (String.IsNullOrWhiteSpace(environmentName) ? "" : "." + environmentName) + ".json", false).Build();
            
            return config.GetConnectionString("DefaultConnectionSQLServer");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = getConnectionStringSQLServer();

                using (var scope = _serviceProvider.CreateScope())
                {
                    var connectionStringConfig = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionStringConfig>>().Value;
                    if (connectionStringConfig.SQLServerActivaityStatus == "true")
                    {
                        optionsBuilder.UseSqlServer(connectionString);/*, options =>*/
                        //options.MigrationsAssembly("Migrations.SQL"));
                    }
                }   
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ClaimDefinition>().HasData(
        new ClaimDefinition { Id = 1, Type = "Permission", Value = "CanViewInvoice", Description = "View invoices" },
        new ClaimDefinition { Id = 2, Type = "Permission", Value = "CanCreateInvoice", Description = "Create invoices" },
        new ClaimDefinition { Id = 3, Type = "Permission", Value = "DeleteUser", Description = "Delete invoices" },
        new ClaimDefinition { Id = 4, Type = "Permission", Value = "RegisterUser", Description = "Manage application users" }
    );
        }
    }
}
