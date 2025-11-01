using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public interface IWorkflowUnitOfWork : IDynamicTestableUnitOfWorkAsync
    {
        IReportRepository ReportRP { get; }
    }
}
