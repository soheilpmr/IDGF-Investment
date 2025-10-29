using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Data
{
    public class WorkFlowDbContext : DbContext
    {
        public WorkFlowDbContext(DbContextOptions<WorkFlowDbContext> options) : base(options)
        {
                
        }

        public WorkFlowDbContext() : base()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            //_configuration = configuration;
        }

        #region WorkFlow
        public DbSet<WorkflowDefinitionEntity> WorkflowDefinitions => Set<WorkflowDefinitionEntity>();
        public DbSet<WorkflowStepEntity> WorkflowSteps => Set<WorkflowStepEntity>();
        public DbSet<WorkflowTransitionEntity> WorkflowTransitions => Set<WorkflowTransitionEntity>();
        public DbSet<WorkflowInstanceEntity> WorkflowInstances => Set<WorkflowInstanceEntity>();
        public DbSet<WorkflowHistoryEntity> WorkflowHistories => Set<WorkflowHistoryEntity>();
        public DbSet<ReportEntity> Reports => Set<ReportEntity>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkflowDefinitionEntity>()
             .HasMany(w => w.WorkflowStepEntities)
             .WithOne(s => s.WorkflowDefinitionEntity)
             .HasForeignKey(s => s.WorkflowDefinitionId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkflowDefinitionEntity>()
                .HasMany(w => w.WorkflowTransitionEntities)
                .WithOne(t => t.WorkflowDefinitionEntity)
                .HasForeignKey(t => t.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkflowInstanceEntity>()
                .HasMany(i => i.WorkflowHistoryEntities)
                .WithOne(h => h.WorkflowInstanceEntity)
                .HasForeignKey(h => h.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkflowStepEntity>()
                .HasIndex(s => new { s.WorkflowDefinitionId, s.StepOrder });

            modelBuilder.Entity<WorkflowStepEntity>()
                .HasIndex(s => new { s.WorkflowDefinitionId, s.StepKey })
                .IsUnique();
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
