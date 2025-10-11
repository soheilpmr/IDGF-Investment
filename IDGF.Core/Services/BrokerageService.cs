using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Domain;

namespace IDGF.Core.Services
{
    public class BrokerageService : StorageBusinessService<Brokerage, int>
    {
        public override Task<int> AddAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        public override Task<LinqDataResult<Brokerage>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task ModifyAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public override Task<Brokerage> RetrieveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnAddAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(Brokerage recievedItem, Brokerage storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
