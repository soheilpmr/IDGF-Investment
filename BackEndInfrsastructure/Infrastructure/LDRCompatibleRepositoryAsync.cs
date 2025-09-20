using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Repository;
using BackEndInfrsastructure.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure
{
    public class LDRCompatibleRepositoryAsync<DBModelEntity, DomainModelEntity, PrimaryKeyType> : RepositoryAsync<DBModelEntity, DomainModelEntity, PrimaryKeyType>, ILDRCompatibleRepositoryAsync<DomainModelEntity, PrimaryKeyType>, IRepositoryAsync<DomainModelEntity, PrimaryKeyType> where DBModelEntity : DomainModelEntity where DomainModelEntity : Model<PrimaryKeyType> where PrimaryKeyType : struct
    {
        public LDRCompatibleRepositoryAsync(DbContext context)
            : base(context)
        {
        }

        public virtual async Task<LinqDataResult<DomainModelEntity>> AllItemsAsync(LinqDataRequest request)
        {
            return await ((IQueryable<DomainModelEntity>)_dbContext.Set<DBModelEntity>()).ToLinqDataResultAsync(request.Take, request.Skip, request.Sort, request.Filter);
        }
    }
}
