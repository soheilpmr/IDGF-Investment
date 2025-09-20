using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Infrastructure.Repositories;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public interface ICoreUnitOfWork : IDynamicTestableUnitOfWorkAsync
    {
        IMandehTransactionsRepositories MandehTransactionsRP { get; }
    }
}
