using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;
using Microsoft.VisualBasic.FileIO;

namespace IDGF.Core.Services
{
    public class ReportService : BaseBusinessService<Report>
    {
        private readonly ILogger<Report> _logger;
        private readonly IConfiguration _configuration;
        private const int _serviceLogNumber = 900;
        private readonly IWorkflowUnitOfWork _workflowUnitOfWork;
        public ReportService(ILogger<Report> logger, IConfiguration configuration, IWorkflowUnitOfWork workflowUnitOfWork) : base(logger)
        {
            _logger = logger;
            _configuration = configuration;
            _workflowUnitOfWork = workflowUnitOfWork;
        }

        public async Task<int> SubmitReportToWorkFlow(IFormFile file, string uploadBy)
        {
            if (file == null || file.Length == 0)
            {
                throw new FileLoadException(file.FileName);
            }

            var baseUploadPath = _configuration["FileStorageSettings:BaseUploadPath"];

            if (string.IsNullOrWhiteSpace(baseUploadPath))
            {
                _logger.LogError("FileStorageSettings:BaseUploadPath is not configured in appsettings.json.");
                throw new ServiceStorageException("File storage path is not configured.", null, _serviceLogNumber);
            }

            var customFolderName = "ReportFiles";
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

                var fileDomain = new Report
                {
                    FileType = file.ContentType,
                    OriginalFileName = file.FileName,
                    StoredFilePath = storedPath,
                    FileSizeKB = (int)(file.Length / 1024),
                    UploadedAt = DateTime.Now,
                    UploadedBy = uploadBy
                };

                ReportEntity reportEntity = new ReportEntity(fileDomain);
                var report = await _workflowUnitOfWork.ReportRP.InsertAsync(reportEntity);
                await _workflowUnitOfWork.CommitAsync();
                return report.ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save physical file : {FileName}", file.FileName);
                throw new ServiceStorageException("Error saving file to disk", ex, _serviceLogNumber);
            }
        }
    }
}
