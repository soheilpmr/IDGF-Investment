using BackEndInfrastructure.Infrastructure.UnitOfWork;
using BackEndInfrsastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure
{
    public interface IDynamicTestableUnitOfWorkAsync : IUnitOfWorkAsync
    {
        ILDRCompatibleRepositoryAsync<T, PrimKey> GetRepo<T, PrimKey>() where T : Model<PrimKey> where PrimKey : struct;
    }
}
