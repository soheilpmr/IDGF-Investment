using IDGF.Core.Data.Configurations;
using IDGF.Core.Data.Entities;
using IDGF.Core.Data.Views;
using IDGF.Core.Domain;
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
        public DbSet<BondsTypeEntity> BondTypes { get; set; }
        public DbSet<BondsEntity> Bonds { get; set; }
        public DbSet<BrokerageEntity> Brokerages { get; set; }
        public DbSet<TransactionsEntity> Transactions { get; set; }
        public DbSet<CouponPaymentsEntity> CouponPayments { get; set; }
        public DbSet<MeetingEntity> Meetings { get; set; }
        public DbSet<MeetingFileEntity> MeetingFiles { get; set; }

        #region Query-Sets
        public DbSet<TransactionBasicViewEntity> TransactionBasicViews { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new MandehConfiguration());
            modelBuilder.ApplyConfiguration(new BondTypesConfiguration());
            modelBuilder.ApplyConfiguration(new BondsConfiguration());
            modelBuilder.ApplyConfiguration(new BrokerageConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionsConfiguration());
            modelBuilder.ApplyConfiguration(new CouponPaymentsConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingsConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingFilesConfiguration());

            modelBuilder.Entity<TransactionBasicViewEntity>().ToView("vw_TransactionBasic");
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
