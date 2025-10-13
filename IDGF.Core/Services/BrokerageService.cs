using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class BrokerageService : StorageBusinessService<Brokerage, int>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 400;
        public BrokerageService(ILogger<Brokerage> logger, ICoreUnitOfWork coreUnitOfWork):base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }


        public async override Task<int> AddAsync(Brokerage item)
        {
            if (item == null)
            {
                var ex = new ServiceArgumentNullException("Input parameter was null:" + nameof(item));
                LogAdd(null, null, ex);
                throw ex;
            }
            await ValidateOnAddAsync(item);
            BrokerageEntity brokerageEntity = new BrokerageEntity(item);
            var result = await _coreUnitOfWork.BrokerageRP.InsertAsync(brokerageEntity);
            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogAdd(item, "Successfully add Brokerage with ,ID:" +
                   result.ID.ToString()
                 );
                return result.ID;
            }
            catch (Exception ex)
            {
                LogAdd(item, $"Error adding Brokerage with exception + {ex.Message}", ex);
                throw new ServiceStorageException("Error adding Brokerage", ex, _serviceLogNumber);
            }
        }

        public async override Task<LinqDataResult<Brokerage>> ItemsAsync(LinqDataRequest request)
        {
            try
            {
                var f = await _coreUnitOfWork.BrokerageRP.AllItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the Brokerage list ", ex, _serviceLogNumber);
            }
        }

        public async override Task ModifyAsync(Brokerage item)
        {
            if (item == null)
            {
                var exception = new ServiceArgumentNullException(typeof(Brokerage).Name);
                LogModify(item, null, exception);
                throw exception;
            }
            var currentItem = await _coreUnitOfWork.BrokerageRP.GetByIdAsync(item.ID);

            if (currentItem == null)
            {
                var noObj = new ServiceObjectNotFoundException(typeof(Brokerage).Name + " Not Found");
                LogModify(item, null, noObj);
                throw noObj;
            }

            await ValidateOnModifyAsync(item, currentItem);

            currentItem.Name = item.Name;


            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogModify(item, "Successfully Modified item with ,ID:" +
                   item.ID.ToString()
                  );
            }

            catch (Exception ex)
            {
                LogModify(item, "ID :" + currentItem.ID, ex);
                throw new ServiceStorageException($"Error modifing Brokerage with message : {ex.Message}" , ex.InnerException, _serviceLogNumber);
            }
        }

        public async override Task RemoveByIdAsync(int ID)
        {
            var itemToDelete = await _coreUnitOfWork.BrokerageRP.GetByIdAsync(ID);

            if (itemToDelete == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(Brokerage).Name + " not found");
                LogRemove(ID, "Brokerage With This Id Not Found", f);
                throw f;
            }
            itemToDelete.IsDeleted = true;
            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogRemove(ID, "Brokerage Deleted Successfully", null);
            }

            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error deleting Brokerage with name : " + itemToDelete.Name, ex, _serviceLogNumber);
                LogRemove(ID, "Error deleting Brokerage with ID : " + itemToDelete.ID.ToString(), ex);
                throw innerEx;
            }
        }

        public async override Task<Brokerage> RetrieveByIdAsync(int ID)
        {
            Brokerage? item;
            try
            {
                item = await _coreUnitOfWork.BrokerageRP.FirstOrDefaultAsync(ss => ss.ID.Equals(ID));
            }
            catch (Exception ex)
            {
                LogRetrieveSingle(ID, ex);
                throw new ServiceStorageException("Error loading Brokerage", ex, _serviceLogNumber);
            }
            if (item == null)
            {
                var ex = new ServiceObjectNotFoundException(typeof(Brokerage).Name + $" not found by ID : {ID}");
                LogRetrieveSingle(ID, ex);
                throw ex;
            }
            LogRetrieveSingle(ID);
            return item;
        }

        protected async override Task ValidateOnAddAsync(Brokerage item)
        {
            List<ModelFieldValidationResult> _validationErrorResult = new List<ModelFieldValidationResult>();
            await CommonValidationResult(_validationErrorResult, item);

            if (_validationErrorResult.Any())
            {
                var exp = new ServiceModelValidationException(_validationErrorResult, "Error validating the model");
                LogAdd(item, "Error in Adding item when validating:" + exp.JSONFormattedErrors, exp);
                throw exp;
            }
        }

        protected async override Task ValidateOnModifyAsync(Brokerage recievedItem, Brokerage storageItem)
        {
            List<ModelFieldValidationResult> _validationErrors = new List<ModelFieldValidationResult>();

            await CommonValidationResult(_validationErrors, recievedItem, recievedItem.ID);

            if (_validationErrors.Any())
            {
                var exp = new ServiceModelValidationException(_validationErrors, "Error validating the model on modify");
                LogModify(recievedItem, "Error in Modifying item when validating:" + exp.JSONFormattedErrors, exp);
                throw exp;
            }
        }

        public async Task CommonValidationResult(List<ModelFieldValidationResult> validationErrors, Brokerage brokerage, int id = 0)
        {
            var allDevicesQuery = await _coreUnitOfWork.BrokerageRP.AllItemsAsync();

            if (allDevicesQuery.Any(ss => ss.Name == brokerage.Name && ss.ID != id))
            {
                validationErrors.Add(new ModelFieldValidationResult()
                {
                    Code = _logBaseID + 1,
                    FieldName = nameof(brokerage.Name),
                    ValidationMessage = "The Name must be unique."
                });
            }
        }
    }
}
