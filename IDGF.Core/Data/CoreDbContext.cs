using IDGF.Core.Data.Configurations;
using IDGF.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Data
{
    public class CoreDbContext:DbContext
    {
        //private readonly IHostEnvironment _configuration;

        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }
        public CoreDbContext() : base()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            //_configuration = configuration;
        }
        public DbSet<MandehTransactionsEntity> MndehTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new MandehConfiguration());
        }

        private static string getConnectionString()
        {
            var environmentName =
              Environment.GetEnvironmentVariable(
                  "ASPNETCORE_ENVIRONMENT");

            //Console.WriteLine("2");
            var config = new ConfigurationBuilder().AddJsonFile("appsettings" + (String.IsNullOrWhiteSpace(environmentName) ? "" : "." + environmentName) + ".json", false).Build();


            return config.GetConnectionString("DefaultConnectionSQLServer");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Console.WriteLine("4");
            optionsBuilder.UseSqlServer(getConnectionString());

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentName == "Development")
            {
                optionsBuilder
                  .UseSqlServer(getConnectionString())
                  .LogTo(Console.WriteLine, LogLevel.Information) // 👈 logs queries to console
                  .EnableSensitiveDataLogging() // 👈 shows parameter values
                  .EnableDetailedErrors();      // 👈 more detail on errors
            }
           
        }
    }
}
