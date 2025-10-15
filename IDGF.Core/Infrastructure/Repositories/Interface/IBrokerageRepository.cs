using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories.Interface
{
    public interface IBrokerageRepository : IRepositoryAsync<Brokerage, int>
    {
        Task<IReadOnlyList<Brokerage>> GetAllForDropDown();
    }
}
