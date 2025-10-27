using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories.Interface
{
    public interface IMeetingsRepository : ILDRCompatibleRepositoryAsync<Meeting, int>
    {
    }
}
