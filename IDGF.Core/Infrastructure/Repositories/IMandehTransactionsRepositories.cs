using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories
{
    public interface IMandehTransactionsRepositories : ILDRCompatibleRepositoryAsync<MandehTransactions, long>
    {
    }
}
