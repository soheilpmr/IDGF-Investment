using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Infrastructure.Repositories.Implemention;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public interface ICoreUnitOfWork : IDynamicTestableUnitOfWorkAsync
    {
        IMandehTransactionsRepositories MandehTransactionsRP { get; }
        IBondsRepository BondsRP { get; }
        IBondsTypeRepository BondsTypeRP { get; }
        IBrokerageRepository BrokerageRP { get; }
        ITransactionsRepository TransactionRP { get; }
    }
}
