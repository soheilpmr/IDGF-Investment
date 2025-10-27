using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class MeetingsService : StorageBusinessService<Meeting, int>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 700;
        public MeetingsService(ILogger<Meeting> logger, ICoreUnitOfWork coreUnitOfWork) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }


        public async override Task<int> AddAsync(Meeting item)
        {
            if (item == null)
            {
                var ex = new ServiceArgumentNullException("Input parameter was null:" + nameof(item));
                LogAdd(null, null, ex);
                throw ex;
            }

            await ValidateOnAddAsync(item);
            MeetingEntity meetingEntity = new MeetingEntity(item);
            var result = await _coreUnitOfWork.MeetingsRP.InsertAsync(meetingEntity);

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogAdd(item, "Successfully added Meeting with ID:" + result.ID.ToString());
                return result.ID;
            }
            catch (Exception ex)
            {
                LogAdd(item, $"Error adding Meeting with exception + {ex.Message}", ex);
                throw new ServiceStorageException("Error adding Meeting", ex, _serviceLogNumber);
            }
        }

        public async override Task<LinqDataResult<Meeting>> ItemsAsync(LinqDataRequest request)
        {
            try
            {
                var f = await _coreUnitOfWork.MeetingsRP.AllItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the Meeting list", ex, _serviceLogNumber);
            }
        }

        public async override Task ModifyAsync(Meeting item)
        {
            if (item == null)
            {
                var exception = new ServiceArgumentNullException(typeof(Meeting).Name);
                LogModify(item, null, exception);
                throw exception;
            }

            var currentItem = await _coreUnitOfWork.MeetingsRP.GetByIdAsync(item.ID);

            if (currentItem == null)
            {
                var noObj = new ServiceObjectNotFoundException(typeof(Meeting).Name + " Not Found");
                LogModify(item, null, noObj);
                throw noObj;
            }

            await ValidateOnModifyAsync(item, currentItem);

            currentItem.MeetingDate = item.MeetingDate;
            currentItem.Description = item.Description;

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogModify(item, "Successfully Modified item with ID:" + item.ID.ToString());
            }
            catch (Exception ex)
            {
                LogModify(item, "ID :" + currentItem.ID, ex);
                throw new ServiceStorageException($"Error modifing Meeting with message : {ex.Message}", ex.InnerException, _serviceLogNumber);
            }
        }

        public async override Task RemoveByIdAsync(int ID)
        {
            var itemToDelete = await _coreUnitOfWork.MeetingsRP.GetByIdAsync(ID);

            if (itemToDelete == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(Meeting).Name + " not found");
                LogRemove(ID, "Meeting With This Id Not Found", f);
                throw f;
            }

            //itemToDelete.IsDeleted = true; //TODO: Soft delete if needed

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogRemove(ID, "Meeting Deleted Successfully", null);
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error deleting Meeting with date : " + itemToDelete.MeetingDate, ex, _serviceLogNumber);
                LogRemove(ID, "Error deleting Meeting with ID : " + itemToDelete.ID.ToString(), ex);
                throw innerEx;
            }
        }

        public async override Task<Meeting> RetrieveByIdAsync(int ID)
        {
            Meeting? item;
            try
            {
                item = await _coreUnitOfWork.MeetingsRP.FirstOrDefaultAsync(ss => ss.ID.Equals(ID));
            }
            catch (Exception ex)
            {
                LogRetrieveSingle(ID, ex);
                throw new ServiceStorageException("Error loading Meeting", ex, _serviceLogNumber);
            }

            if (item == null)
            {
                var ex = new ServiceObjectNotFoundException(typeof(Meeting).Name + $" not found by ID : {ID}");
                LogRetrieveSingle(ID, ex);
                throw ex;
            }

            LogRetrieveSingle(ID);
            return item;
        }

        protected override async Task ValidateOnAddAsync(Meeting item)
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

        protected override async Task ValidateOnModifyAsync(Meeting recievedItem, Meeting storageItem)
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

        public Task CommonValidationResult(List<ModelFieldValidationResult> validationErrors, Meeting meeting, int id = 0)
        {
            if (meeting.MeetingDate == default(DateTime))
            {
                validationErrors.Add(new ModelFieldValidationResult()
                {
                    Code = _logBaseID + 1,
                    FieldName = nameof(meeting.MeetingDate),
                    ValidationMessage = "Meeting Date is required."
                });
            }

            return Task.CompletedTask;
        }
    }
}
