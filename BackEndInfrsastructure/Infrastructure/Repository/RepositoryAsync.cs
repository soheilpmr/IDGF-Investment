using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Enums;
using BackEndInfrsastructure.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BackEndInfrastructure.Infrastructure.Repository
{
    public abstract class RepositoryAsync<DBModelEntity, DomainModelEntity, PrimaryKeyType> : IRepositoryAsync<DomainModelEntity, PrimaryKeyType>
       where DomainModelEntity : Model<PrimaryKeyType>
       where DBModelEntity : DomainModelEntity
       where PrimaryKeyType : struct

    {
        protected readonly DbSet<DBModelEntity> _entity;
        private readonly DataBase _dataBaseKind;
        protected readonly DbContext _dbContext;
        public RepositoryAsync(DbContext dbContext, DataBase dataBaseKind = DataBase.SQLServer)
        {
            _dbContext = dbContext;
            _entity = _dbContext.Set<DBModelEntity>();
            _dataBaseKind = dataBaseKind;
        }
        /// <summary>
        /// Get All ITems Of DomainModelEntity IN IReadOnlyList Type 
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<DomainModelEntity>> AllItemsAsync()
        {
            return await _entity.ToListAsync();
        }

        /// <summary>
        /// Get All ITems Of DomainModelEntity IN IEnumerable Type With Handling Paging - Sorting - Filter
        /// </summary>
        /// <returns></returns>
        public virtual async Task<LinqDataResult<DomainModelEntity>> AllItemsAsync(LinqDataRequest request)
        {
            var t = _entity.AsEnumerable();
            var rtn = await ((IQueryable<DomainModelEntity>)_entity).ToLinqDataResultAsync(request.Take, request.Skip, request.Sort, request.Filter, _dataBaseKind);
            return rtn;
        }

        public virtual async Task DeleteAsync(DomainModelEntity item)
        {
            if (item is DBModelEntity)
            {
                _entity.Remove(item as DBModelEntity);
            }
        }

        public virtual Task<DomainModelEntity?> FirstOrDefaultAsync(Expression<Func<DomainModelEntity, bool>> predicate)
        {
            return ((IQueryable<DomainModelEntity>)_entity).FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<DomainModelEntity?> GetByIdAsync(PrimaryKeyType id)
        {
            return (DomainModelEntity)(Model<PrimaryKeyType>)(await _entity.FindAsync(id));
        }

        public virtual async Task<DomainModelEntity> InsertAsync(DomainModelEntity item)
        {
            if (item is DBModelEntity)
            {
                //return _entity.Add(item as DBModelEntity).Entity;
                return (DomainModelEntity)(Model<PrimaryKeyType>)_entity.Add(item as DBModelEntity).Entity;
            }
            return null;
        }

        public virtual async Task<IEnumerable<DomainModelEntity>> InsertMultipleAsync(IEnumerable<DomainModelEntity> items)
        {
            if (items == null || !items.Any())
                return Enumerable.Empty<DomainModelEntity>();

            // Convert domain models to database entities
            var dbEntities = items.Select(i => (DBModelEntity)Activator.CreateInstance(typeof(DBModelEntity), i))
                                  .ToList();

            await _entity.AddRangeAsync(dbEntities);
            return items;
        }


        public virtual async Task UpdateAsync(DomainModelEntity item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }
    }
}
