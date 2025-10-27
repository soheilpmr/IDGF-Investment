using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;
using IDGF.Core.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace IDGF.Core.Services
{
    public class MeetingBusinessService : IMeetingBusinessService
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private readonly ILogger<MeetingBusinessService> _logger;
        private readonly IConfiguration _configuration;
        private const int _serviceLogNumber = 750;

        public MeetingBusinessService(
            ICoreUnitOfWork coreUnitOfWork,
            ILogger<MeetingBusinessService> logger,
            IConfiguration configuration)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<int> CreateMeetingWithFilesAsync(CreateMeetingRequestDto dto)
        {
            var meetingDomain = new Meeting
            {
                MeetingDate = dto.MeetingDate,
                Description = dto.Description,
                CreatedAt = DateTime.Now
            };


            var meetingEntity = new MeetingEntity(meetingDomain);

            await _coreUnitOfWork.MeetingsRP.InsertAsync(meetingEntity);

            var t1 = SaveFileAndCreateRecordAsync(dto.Minutes, meetingEntity, MeetingFileTypes.Minutes);
            var t2 = SaveFileAndCreateRecordAsync(dto.MaturedBondsReport, meetingEntity, MeetingFileTypes.MaturedBondsReport);
            var t3 = SaveFileAndCreateRecordAsync(dto.CashFlowReport, meetingEntity, MeetingFileTypes.CashFlowReport);
            var t4 = SaveFileAndCreateRecordAsync(dto.PurchasedBondsReport, meetingEntity, MeetingFileTypes.PurchasedBondsReport);
            var t5 = SaveFileAndCreateRecordAsync(dto.Report30To70, meetingEntity, MeetingFileTypes.Report30To70);

            Task.WaitAll(t1, t2, t3, t4, t5);

            try
            {
                await _coreUnitOfWork.CommitAsync();
                _logger.LogInformation("Successfully created meeting {MeetingID} with files.", meetingEntity.ID);
                return meetingEntity.ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to commit new meeting and files.");
                throw new ServiceStorageException("Error creating meeting", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<MeetingResponseDto>> GetMeetingListAsync(LinqDataRequest request)
        {
            try
            {
                var meetings = await _coreUnitOfWork.MeetingsRP.AllItemsAsync();
                var files = await _coreUnitOfWork.MeetingFilesRP.AllItemsAsync();
                var filesLookup = files.ToLookup(f => f.MeetingID);

                var dtoList = meetings.Select(m => new MeetingResponseDto
                {
                    ID = m.ID,
                    MeetingDate = m.MeetingDate.ToString("yyyy-MM-dd"),
                    Description = m.Description,
                    HasMinutes = filesLookup[m.ID].Any(f => f.FileType == MeetingFileTypes.Minutes),
                    HasMaturedBondsReport = filesLookup[m.ID].Any(f => f.FileType == MeetingFileTypes.MaturedBondsReport),
                    HasCashFlowReport = filesLookup[m.ID].Any(f => f.FileType == MeetingFileTypes.CashFlowReport),
                    HasPurchasedBondsReport = filesLookup[m.ID].Any(f => f.FileType == MeetingFileTypes.PurchasedBondsReport),
                    HasReport30To70 = filesLookup[m.ID].Any(f => f.FileType == MeetingFileTypes.Report30To70)
                })
                .AsQueryable();


                if (request.Sort != null && request.Sort.Any())
                {

                    var sortString = string.Join(",", request.Sort.Select(s => $"{s.Field} {s.Dir}"));
                    dtoList = dtoList.OrderBy(sortString);
                }
                else
                {
                    dtoList = dtoList.OrderByDescending(x => x.MeetingDate);
                }
                var totalCount = dtoList.Count();

                if (request.Skip > 0)
                {
                    dtoList = dtoList.Skip(request.Skip);
                }
                if (request.Take > 0)
                {
                    dtoList = dtoList.Take(request.Take);
                }
                var pagedData = dtoList.ToList();

                var pagedResult = new LinqDataResult<MeetingResponseDto>
                {
                    Data = pagedData,
                    RecordsTotal = totalCount
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving meeting list.");
                throw new ServiceStorageException("Error retrieving meeting list", ex, _serviceLogNumber);
            }
        }

        public async Task<Meeting> GetMeetingByIdAsync(int id)
        {
            var meeting = await _coreUnitOfWork.MeetingsRP.GetByIdAsync(id);
            if (meeting == null)
            {
                throw new ServiceObjectNotFoundException(typeof(Meeting).Name + " not found");
            }
            return meeting;
        }

        public async Task RemoveMeetingAndFilesAsync(int id)
        {
            var allFiles = await _coreUnitOfWork.MeetingFilesRP.AllItemsAsync();

            var filesToDelete = allFiles.Where(f => f.MeetingID == id).ToList();

            var meetingToDelete = await _coreUnitOfWork.MeetingsRP.GetByIdAsync(id);
            if (meetingToDelete == null)
            {
                throw new ServiceObjectNotFoundException(typeof(Meeting).Name + " not found");
            }

            await _coreUnitOfWork.MeetingsRP.DeleteAsync(meetingToDelete);

            try
            {
                await _coreUnitOfWork.CommitAsync();
                _logger.LogInformation("Successfully deleted meeting {MeetingID} from database.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting meeting {MeetingID} from database.", id);
                throw new ServiceStorageException("Error deleting meeting", ex, _serviceLogNumber);
            }

            foreach (var file in filesToDelete)
            {
                try
                {
                    if (File.Exists(file.StoredFilePath))
                    {
                        File.Delete(file.StoredFilePath);
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogWarning(ex, "Failed to delete physical file {FilePath} for deleted meeting {MeetingID}", file.StoredFilePath, id);
                }
            }
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)> GetFileForDownloadAsync(int meetingId, string fileType)
        {
            var allFiles = await _coreUnitOfWork.MeetingFilesRP.AllItemsAsync();

            var file = allFiles.FirstOrDefault(f => f.MeetingID == meetingId && f.FileType == fileType);

            if (file == null)
            {
                throw new ServiceObjectNotFoundException("File not found");
            }

            if (!File.Exists(file.StoredFilePath))
            {
                _logger.LogError("File record exists but physical file is missing: {FilePath}", file.StoredFilePath);
                throw new ServiceStorageException("File is missing from storage", null, _serviceLogNumber);
            }

            var stream = File.OpenRead(file.StoredFilePath);
            var contentType = GetMimeType(file.OriginalFileName);

            return (stream, contentType, file.OriginalFileName);
        }



        private async Task SaveFileAndCreateRecordAsync(IFormFile? file, MeetingEntity meetingEntity, string fileType)
        {
            if (file == null || file.Length == 0)
            {
                return;
            }

            var baseUploadPath = _configuration["FileStorageSettings:BaseUploadPath"];

            if (string.IsNullOrWhiteSpace(baseUploadPath))
            {
                _logger.LogError("FileStorageSettings:BaseUploadPath is not configured in appsettings.json.");
                throw new ServiceStorageException("File storage path is not configured.", null, _serviceLogNumber);
            }

            var customFolderName = "MeetingUploads";
            var targetDirectory = Path.Combine(baseUploadPath, customFolderName);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var storedPath = Path.Combine(targetDirectory, uniqueFileName);

            try
            {
                await using (var stream = new FileStream(storedPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save physical file {FileName}", file.FileName);
                throw new ServiceStorageException("Error saving file to disk", ex, _serviceLogNumber);
            }

            var fileDomain = new MeetingFile
            {
                FileType = fileType,
                OriginalFileName = file.FileName,
                StoredFilePath = storedPath,
                FileSizeKB = (int)(file.Length / 1024),
                UploadedAt = DateTime.Now
            };

            var fileEntity = new MeetingFileEntity(fileDomain);
            fileEntity.Meeting = meetingEntity;

            meetingEntity.Files.Add(fileEntity);
        }

        private string GetMimeType(string fileName)
        {

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}