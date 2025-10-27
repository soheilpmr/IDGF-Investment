using BackEndInfrastructure.Enums;
using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class MeetingsRepository : RepositoryAsync<MeetingEntity, Meeting, int>, IMeetingsRepository
    {
        private readonly CoreDbContext _context;
        public MeetingsRepository(CoreDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
