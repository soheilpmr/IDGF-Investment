using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class ReportRepository : RepositoryAsync<ReportEntity, Report, int>, IReportRepository
    {
        private readonly WorkFlowDbContext _coreDbContext;
        public ReportRepository(WorkFlowDbContext coreDbContext) : base(coreDbContext)
        {
            _coreDbContext = coreDbContext;
        }
    }
}
