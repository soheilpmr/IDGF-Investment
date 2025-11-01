using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.UnitOfWork;
using BackEndInfrsastructure.Domain;
using IDGF.Core.Data;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Implemention;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public class WorkflowUnitOfWork : UnitOfWorkAsync<WorkFlowDbContext>, IWorkflowUnitOfWork
    {
        public WorkflowUnitOfWork() : base(new WorkFlowDbContext())
        {
            ReportRP = new ReportRepository(base._dbContext);
        }

        public IReportRepository ReportRP { get; private set; }

        public ILDRCompatibleRepositoryAsync<T, PrimKey> GetRepo<T, PrimKey>()
            where T : Model<PrimKey>
            where PrimKey : struct
        {
            ILDRCompatibleRepositoryAsync<T, PrimKey> ff = null;

            if (typeof(T) == typeof(Report))
            {
                ff = ReportRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;
            }

            return ff;
        }
    }
}
