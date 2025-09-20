using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Repository;
using BackEndInfrsastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure
{

    public interface ILDRCompatibleRepositoryAsync<ModelItem, T> : IRepositoryAsync<ModelItem, T> where ModelItem : Model<T> where T : struct
    {
        Task<LinqDataResult<ModelItem>> AllItemsAsync(LinqDataRequest request);
    }
}
