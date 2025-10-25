using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.DynamicLinqCore.Helper;
using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;
using System.Linq.Dynamic.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IDGF.Core.Services
{
    public class MandehtransactionService : StorageBusinessService<MandehTransactions, long>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 100;
        private readonly ILDRCompatibleRepositoryAsync<MandehTransactions, long> _baseRepo;
        
        public MandehtransactionService(ICoreUnitOfWork coreUnitOfWork, ILogger<MandehTransactions> logger) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _baseRepo = _coreUnitOfWork.GetRepo<MandehTransactions, long>();
        }
        public override Task<long> AddAsync(MandehTransactions item)
        {
            throw new NotImplementedException();
        }

        public async Task<long> AddMutipleAsync(List<MandehTransactions> items)
        {
            try
            {
                foreach (var item in items)
                {
                    MandehTransactionsEntity entity = new MandehTransactionsEntity(item);
                    await _baseRepo.InsertAsync(entity);
                    //await _coreUnitOfWork.MandehTransactionsRP.InsertAsync(entity);
                    LogAdd(item, $"Item Added Successfully with transactionDate {item.TransactionDateTime}");
                }
                await _coreUnitOfWork.CommitAsync();
                return items.Count;
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error adding items", ex, _serviceLogNumber);
                LogAdd(null, "Error adding items", innerEx);
                throw innerEx;
            }
        }

        public override async Task<LinqDataResult<MandehTransactions>> ItemsAsync(LinqDataRequest request)
        {
            try
            {
                if (request.Sort == null || !request.Sort.Any())
                {
                    request.Sort = new List<Sort>
                    {
                        new Sort { Field = "TransactionDate", Dir = "desc" }
                    };
                }
                var f = await _coreUnitOfWork.MandehTransactionsRP.AllItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the Mandeh list ", ex, _serviceLogNumber);
            }
        }

        public async Task<List<MandehTransactions>> ItemsAsync()
        {
            try
            {
                var f = await _coreUnitOfWork.MandehTransactionsRP.AllItemsAsync();
                LogRetrieveMultiple(null);
                return f.OrderByDescending(t => t.TransactionDate).ToList();
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error retrieving the Mandeh list ", ex, _serviceLogNumber);
            }
        }


        public override async Task ModifyAsync(MandehTransactions item)
        {
            var itemToUpdate = await _baseRepo.GetByIdAsync(item.ID);

            if (itemToUpdate == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(MandehTransactions).Name + " not found", _serviceLogNumber);
                LogModify(item, "Item With This Id Not Found", f);
                throw f;
            }

            // Update properties
            itemToUpdate.DarRah = item.DarRah;
            itemToUpdate.Taeed = 0;

            await _baseRepo.UpdateAsync(itemToUpdate);

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogModify(item, "Item Updated Successfully", null);
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error updating item with ID: " + itemToUpdate.ID, ex, _serviceLogNumber);
                LogModify(item, "Error updating item with ID: " + itemToUpdate.ID, ex);
                throw innerEx;
            }
        }

        public override async Task RemoveByIdAsync(long Id)
        {
            var itemToDelete = await _baseRepo.GetByIdAsync(Id);

            if (itemToDelete == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(MandehTransactions).Name + " not found");
                LogRemove(Id, "Item With This Id Not Found", f);
                throw f;
            }


            await _baseRepo.DeleteAsync(itemToDelete);
            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogRemove(Id, "Item Deleted Successfully", null);
            }

            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error deleting item with name : " + itemToDelete.ID, ex, _serviceLogNumber);
                LogRemove(Id, "Error deleting item with ID : " + itemToDelete.ID.ToString(), ex);
                throw innerEx;
            }
        }

        public override async Task<MandehTransactions> RetrieveByIdAsync(long ID)
        {
            MandehTransactions? item;
            try
            {
                item = await _baseRepo.FirstOrDefaultAsync(ss => ss.ID == ID);
            }
            catch (Exception ex)
            {
                LogRetrieveSingle(ID, ex);
                throw new ServiceStorageException("Error loading company", ex, _serviceLogNumber);
            }
            if (item == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(MandehTransactions).Name + " not found by id");
                LogRetrieveSingle(ID, f);
                throw f;
            }
            LogRetrieveSingle(ID);
            return item;
        }

        protected override Task ValidateOnAddAsync(MandehTransactions item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(MandehTransactions recievedItem, MandehTransactions storageItem)
        {
            throw new NotImplementedException();
        }

    }
}
