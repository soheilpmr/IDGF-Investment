using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class MeetingsFileRepository : RepositoryAsync<MeetingFileEntity, MeetingFile, int>, IMeetingFileRepository
    {
        private readonly CoreDbContext _context;
        public MeetingsFileRepository(CoreDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
