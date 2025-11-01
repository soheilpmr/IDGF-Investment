using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.UnitOfWork;
using BackEndInfrsastructure.Domain;
using IDGF.Core.Data;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Implemention;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.UnitOfWork
{
    public class CoreUnitOfWork : UnitOfWorkAsync<CoreDbContext>, ICoreUnitOfWork
    {
        public CoreUnitOfWork() : base(new CoreDbContext())
        {
            MandehTransactionsRP = new MandehTransactionsRepositories(base._dbContext);
            BondsRP = new BondsRepository(base._dbContext);
            BondsTypeRP = new BondsTypeRepository(base._dbContext);
            BrokerageRP = new BrokerageRepository(base._dbContext);
            TransactionRP = new TransactionsRepository(base._dbContext);
            MeetingsRP = new MeetingsRepository(base._dbContext);
            MeetingFilesRP = new MeetingsFileRepository(base._dbContext);
            //ReportRP = new ReportRepository(_workflowDbContext);
        }

        public IMandehTransactionsRepositories MandehTransactionsRP { get; private set; }
        public IBondsRepository BondsRP { get; private set; }
        public IBondsTypeRepository BondsTypeRP { get; private set; }
        public IBrokerageRepository BrokerageRP { get; private set; }
        public ITransactionsRepository TransactionRP { get; private set; }
        public IMeetingsRepository MeetingsRP { get; private set; }
        public IMeetingFileRepository MeetingFilesRP { get; private set; }
        //public IReportRepository ReportRP { get; private set; }

        public ILDRCompatibleRepositoryAsync<T, PrimKey> GetRepo<T, PrimKey>()
            where T : Model<PrimKey>
            where PrimKey : struct
        {
            ILDRCompatibleRepositoryAsync<T, PrimKey> ff = null;

            if (typeof(T) == typeof(MandehTransactions))
            {
                ff = MandehTransactionsRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(Bonds))
            {
                ff = BondsRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(BondsType))
            {
                ff = BondsTypeRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(Brokerage))
            {
                ff = BrokerageRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(Transactions))
            {
                ff = TransactionRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(Meeting))
            {
                ff = MeetingsRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }
            if (typeof(T) == typeof(MeetingFile))
            {
                ff = MeetingFilesRP as ILDRCompatibleRepositoryAsync<T, PrimKey>;

            }

            return ff;
        }
    }
}
