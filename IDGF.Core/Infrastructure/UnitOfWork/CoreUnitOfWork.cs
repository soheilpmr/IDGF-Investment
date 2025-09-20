using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.UnitOfWork;
using BackEndInfrsastructure.Domain;
using IDGF.Core.Data;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public class CoreUnitOfWork : UnitOfWorkAsync<CoreDbContext>, ICoreUnitOfWork
    {
        public CoreUnitOfWork() : base(new CoreDbContext())
        {
            MandehTransactionsRP = new MandehTransactionsRepositories(base._dbContext);
        }

        public IMandehTransactionsRepositories MandehTransactionsRP { get; private set; }

        public ILDRCompatibleRepositoryAsync<T, PrimKey> GetRepo<T, PrimKey>()
            where T : Model<PrimKey>
            where PrimKey : struct
        {
            ILDRCompatibleRepositoryAsync<T, PrimKey> ff = null;

            if (typeof(T) == typeof(MandehTransactions))
            {
                ff = MandehTransactionsRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            return ff;
        }
    }
}
