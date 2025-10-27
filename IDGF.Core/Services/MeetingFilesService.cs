using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class MeetingFilesService : StorageBusinessService<MeetingFile, int>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 800;
        public MeetingFilesService(ILogger<MeetingFile> logger, ICoreUnitOfWork coreUnitOfWork) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }

        public async override Task<int> AddAsync(MeetingFile item)
        {
            if (item == null)
            {
                var ex = new ServiceArgumentNullException("Input parameter was null:" + nameof(item));
                LogAdd(null, null, ex);
                throw ex;
            }

            await ValidateOnAddAsync(item);

            MeetingFileEntity fileEntity = new MeetingFileEntity(item);
            var result = await _coreUnitOfWork.MeetingFilesRP.InsertAsync(fileEntity);

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogAdd(item, "Successfully added MeetingFile with ID:" + result.ID.ToString());
                return result.ID;
            }
            catch (Exception ex)
            {
                LogAdd(item, $"Error adding MeetingFile with exception + {ex.Message}", ex);
                throw new ServiceStorageException("Error adding MeetingFile", ex, _serviceLogNumber);
            }
        }

        public async override Task<LinqDataResult<MeetingFile>> ItemsAsync(LinqDataRequest request)
        {
            try
            {
                var f = await _coreUnitOfWork.MeetingFilesRP.AllItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the MeetingFile list", ex, _serviceLogNumber);
            }
        }

        public async override Task ModifyAsync(MeetingFile item)
        {
            if (item == null)
            {
                var exception = new ServiceArgumentNullException(typeof(MeetingFile).Name);
                LogModify(item, null, exception);
                throw exception;
            }
            var currentItem = await _coreUnitOfWork.MeetingFilesRP.GetByIdAsync(item.ID);

            if (currentItem == null)
            {
                var noObj = new ServiceObjectNotFoundException(typeof(MeetingFile).Name + " Not Found");
                LogModify(item, null, noObj);
                throw noObj;
            }

            await ValidateOnModifyAsync(item, currentItem);
            currentItem.FileType = item.FileType;
            currentItem.OriginalFileName = item.OriginalFileName;

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogModify(item, "Successfully Modified item with ID:" + item.ID.ToString());
            }
            catch (Exception ex)
            {
                LogModify(item, "ID :" + currentItem.ID, ex);
                throw new ServiceStorageException($"Error modifing MeetingFile with message : {ex.Message}", ex.InnerException, _serviceLogNumber);
            }
        }

        public async override Task RemoveByIdAsync(int ID)
        {
            var itemToDelete = await _coreUnitOfWork.MeetingFilesRP.GetByIdAsync(ID);

            if (itemToDelete == null)
            {
                var f = new ServiceObjectNotFoundException(typeof(MeetingFile).Name + " not found");
                LogRemove(ID, "MeetingFile With This Id Not Found", f);
                throw f;
            }
            //itemToDelete.IsDeleted = true; //TODO: Soft delete if needed

            try
            {
                await _coreUnitOfWork.CommitAsync();
                LogRemove(ID, "MeetingFile Deleted Successfully", null);
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error deleting MeetingFile with name : " + itemToDelete.OriginalFileName, ex, _serviceLogNumber);
                LogRemove(ID, "Error deleting MeetingFile with ID : " + itemToDelete.ID.ToString(), ex);
                throw innerEx;
            }
        }

        public async override Task<MeetingFile> RetrieveByIdAsync(int ID)
        {
            MeetingFile? item;
            try
            {
                item = await _coreUnitOfWork.MeetingFilesRP.FirstOrDefaultAsync(ss => ss.ID.Equals(ID));
            }
            catch (Exception ex)
            {
                LogRetrieveSingle(ID, ex);
                throw new ServiceStorageException("Error loading MeetingFile", ex, _serviceLogNumber);
            }

            if (item == null)
            {
                var ex = new ServiceObjectNotFoundException(typeof(MeetingFile).Name + $" not found by ID : {ID}");
                LogRetrieveSingle(ID, ex);
                throw ex;
            }

            LogRetrieveSingle(ID);
            return item;
        }

        protected override async Task ValidateOnAddAsync(MeetingFile item)
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

        protected override async Task ValidateOnModifyAsync(MeetingFile recievedItem, MeetingFile storageItem)
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

        public async Task CommonValidationResult(List<ModelFieldValidationResult> validationErrors, MeetingFile file, int id = 0)
        {
            //if (file.MeetingID <= 0)
            //{
            //    validationErrors.Add(new ModelFieldValidationResult()
            //    {
            //        Code = _logBaseID + 1,
            //        FieldName = nameof(file.MeetingID),
            //        ValidationMessage = "A valid MeetingID is required."
            //    });
            //}
            //else
            //{
            //    var meetingExists = await _coreUnitOfWork.MeetingsRP.(m => m.ID == file.MeetingID);
            //    if (!meetingExists)
            //    {
            //        validationErrors.Add(new ModelFieldValidationResult()
            //        {
            //            Code = _logBaseID + 2,
            //            FieldName = nameof(file.MeetingID),
            //            ValidationMessage = $"Meeting with ID {file.MeetingID} does not exist."
            //        });
            //    }
            //}
        }
    }
}
