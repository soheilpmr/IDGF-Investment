using AutoMapper;
using Azure.Core;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using IDGF.Core.Domain.Views;
using IDGF.Core.Infrastructure;
using IDGF.Core.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IDGF.Core.Services
{
    public class TransactionService : StorageBusinessService<Transactions, decimal>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 600;
        private readonly IMapper _mapper;
        private readonly ILDRCompatibleRepositoryAsync<Transactions, decimal> _baseRepo;
        private readonly BondsService bondsService;

        public TransactionService(ILogger<Transactions> logger, ICoreUnitOfWork coreUnitOfWork, IMapper mapper, BondsService bondsService) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _baseRepo = _coreUnitOfWork.GetRepo<Transactions, decimal>();
            _mapper = mapper;
            this.bondsService = bondsService;
        }

        public override Task<decimal> AddAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Transactions>> UploadFileExcelMelli(IFormFile melliFile)
        {
            if (melliFile == null || melliFile.Length == 0)
                throw new UploadFileException("No file uploaded.");
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await melliFile.CopyToAsync(stream);


                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization"); using var package = new ExcelPackage(new FileInfo(tempPath));
                var ws = package.Workbook.Worksheets.First();

                DataTable dt = new();
                int startRow = 5; // ردیفی که نام ستون‌ها از آن شروع می‌شود

                // اضافه کردن نام ستون‌ها از ردیف پنجم
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    string colName = ws.Cells[startRow, col].Text.Trim();
                    if (string.IsNullOrEmpty(colName))
                        colName = $"Column{col}";
                    dt.Columns.Add(colName);
                }

                for (int row = 7; row <= ws.Dimension.End.Row; row++)
                {
                    var dataRow = dt.NewRow();
                    for (int col = 1; col <= ws.Dimension.End.Column; col++)
                        dataRow[col - 1] = ws.Cells[row, col].Text;
                    dt.Rows.Add(dataRow);
                }

                // Regex pattern برای پیدا کردن مقادیر شرح
                var regex = new Regex(@"تعداد\s+(\d+).*?\(اخزا(\d+)\).*?نرخ\s+([\d,]+)", RegexOptions.Compiled);

                List<Transactions> extractedRows = new List<Transactions>();

                foreach (DataRow row in dt.Rows)
                {
                    string desc = row["شرح"]?.ToString()?.Trim() ?? "";

                    string transactionDate = row?.ItemArray[0]?.ToString() ?? "";
                    transactionDate = transactionDate.Replace("/", "-");
                    var dateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(transactionDate);

                    string bedehkar = row.Table.Columns.Contains("بدهکار") ? row["بدهکار"]?.ToString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(desc))
                        continue;

                    // فقط سطرهایی که با "علی الحساب خرید" شروع می‌شوند
                    if (!desc.StartsWith("علی الحساب خرید"))
                        continue;

                    // رد کردن سطرهایی که با "تخفیف" شروع می‌شوند
                    if (desc.StartsWith("تخفیف"))
                        continue;

                    var match = regex.Match(desc);
                    if (!match.Success)
                        continue;

                    var match2 = Regex.Match(desc, @"تعداد\s+([\d,]+)");
                    string tedad = string.Empty;
                    if (match.Success)
                    {
                        tedad = match2.Groups[1].Value.Replace(",", "").Trim();
                    }
                    string akhza = match.Groups[2].Value;
                    if (akhza.Length > 3)
                        akhza = akhza[..3];
                    string price = match.Groups[3].Value.Replace(",", "");
                    var akhzaFinal = "اخزا " + akhza;
                    var bondID = await bondsService.GetBondIdWithName(akhzaFinal);
                    extractedRows.Add(new Transactions()
                    {
                        Quantity = int.Parse(tedad),
                        //Akhza = akhzaFinal,
                        PricePerUnit = decimal.Parse(price),
                        InvestmentPrice = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0,
                        TransactionDate = dateOnly,
                        TransactionType = "Buy",
                        BondId = bondID,
                        Commission = 0,
                        BrokerId = (int)BrokersEnum.Melli,
                        Status = (byte)TransactionStatusEnum.Unspecified
                    });
                }
                return extractedRows;
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error extracting Melli Excel file", ex, _serviceLogNumber);
                LogAdd(null, nameof(UploadFileExcelMelli), innerEx);
                throw innerEx;
            }   
        }

        public async Task<List<Transactions>> UploadFileExcelMellat(IFormFile mellatFile)
        {
            if (mellatFile == null || mellatFile.Length == 0)
                throw new UploadFileException("No file uploaded.");
            try
            {

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await mellatFile.CopyToAsync(stream);


                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization"); using var package = new ExcelPackage(new FileInfo(tempPath));
                var ws = package.Workbook.Worksheets.First();

                DataTable dt = new();
                int startRow = 5;
                int dataStartRow = 7;

                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    string colName = ws.Cells[startRow, col].Text.Trim();
                    if (string.IsNullOrEmpty(colName))
                        colName = $"Column{col}";
                    dt.Columns.Add(colName);
                }

                for (int row = dataStartRow; row <= ws.Dimension.End.Row; row++)
                {
                    var dataRow = dt.NewRow();
                    for (int col = 1; col <= ws.Dimension.End.Column; col++)
                        dataRow[col - 1] = ws.Cells[row, col].Text;
                    dt.Rows.Add(dataRow);
                }

                var regex = new Regex(@"تعداد\s+([\d,]+).*?\((\D*)(\d*)\).*?نرخ\s+([\d,]+)", RegexOptions.Compiled);

                List<Transactions> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    string desc = row["شرح"]?.ToString()?.Trim() ?? "";
                    string bedehkar = row.Table.Columns.Contains("بدهکار") ? row["بدهکار"]?.ToString() ?? "" : "";

                    string transactionDate = row?.ItemArray[0]?.ToString() ?? "";
                    transactionDate = transactionDate.Replace("/", "-");
                    var dateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(transactionDate);

                    if (string.IsNullOrWhiteSpace(desc))
                        continue;


                    if (!desc.StartsWith("علی الحساب خريد"))
                        continue;


                    if (desc.StartsWith("تخفيف"))
                        continue;

                    var match = regex.Match(desc);
                    if (!match.Success)
                        continue;

                    string tedad = match.Groups[1].Value.Replace(",", "").Trim();
                    string prefix = match.Groups[2].Value.Trim();
                    string number = match.Groups[3].Value.Trim();
                    string price = match.Groups[4].Value.Replace(",", "").Trim();
                    string akhza_code;
                    decimal bondID = 0; 
                    if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(number))
                    {
                        akhza_code = "N/A";
                    }
                    else
                    {
                        if (number.Length > 3)
                            number = number[..3];
                        akhza_code = prefix + " " + number;
                        bondID = await bondsService.GetBondIdWithName(akhza_code);
                    }
                    extractedRows.Add(new Transactions()
                    {
                        Quantity = int.Parse(tedad),
                        //Akhza = akhzaFinal,
                        PricePerUnit = decimal.Parse(price),
                        InvestmentPrice = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0,
                        TransactionDate = dateOnly,
                        TransactionType = "Buy",
                        BondId = bondID,
                        Commission = 0,
                        BrokerId = (int)BrokersEnum.Mellat,
                        Status = (byte)TransactionStatusEnum.Unspecified
                    });
                }
                return extractedRows;

            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error extracting Mellat Excel file", ex, _serviceLogNumber);
                LogAdd(null, nameof(UploadFileExcelMelli), innerEx);
                throw innerEx;
            }
        }
        
        
        public async Task<List<Transactions>> UploadFileExcelKeshavarzi(IFormFile keshavarziFile)
        {
            try
            {
                if (keshavarziFile == null || keshavarziFile.Length == 0)
                    throw new UploadFileException("No file uploaded.");

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await keshavarziFile.CopyToAsync(stream);

                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
                using var package = new ExcelPackage(new FileInfo(tempPath));

                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    throw new UploadFileException("The Excel file is empty or the first worksheet is invalid.");

                DataTable dt = new();
                int dataStartRow = 8;

                int columnCount = ws.Dimension.End.Column;
                for (int i = 0; i < columnCount; i++)
                {
                    dt.Columns.Add();
                }

                for (int rowNum = dataStartRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, columnCount];
                    if (wsRow.All(c => string.IsNullOrWhiteSpace(c.Text))) continue;

                    DataRow row = dt.Rows.Add();
                    for (int col = 1; col <= columnCount; col++)
                    {
                        if (col - 1 < dt.Columns.Count)
                        {
                            row[col - 1] = ws.Cells[rowNum, col].Text;
                        }
                    }
                }

                var regex = new Regex(@"^خريد[^\d]*([\d,]+).*?(\d{6})\s*(?:\((\D*)(\d*)\))?.*?([\d,]+)",
                        RegexOptions.Compiled | RegexOptions.CultureInvariant);

                List<Transactions> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    if (row.ItemArray.Length <= 3) continue;

                    string transactionDateStr = row[1]?.ToString()?.Trim() ?? "";
                    string desc = row[2]?.ToString()?.Trim() ?? "";
                    string bedehkar = row[3]?.ToString()?.Trim() ?? "";




                    string descNormalized = desc
                        .Replace('\u06CC', '\u064A')
                        .Replace('\u200C', ' ')
                        .Replace('\u00A0', ' ')
                        .Trim();

                    if (!descNormalized.StartsWith("خريد"))
                        continue;

                    var match = regex.Match(descNormalized);
                    if (!match.Success)
                        continue;

                    string tedad = match.Groups[1].Value.Replace(",", "");
                    string instrumentCode = match.Groups[2].Value;
                    string number = match.Groups[4].Value.Trim();
                    string price = match.Groups[5].Value.Replace(",", "");

                    string bondDateString = "14" + instrumentCode;
                    DateOnly bondGregorianDate;

                    try
                    {
                        string formattedBondDate = $"{bondDateString.Substring(0, 4)}-{bondDateString.Substring(4, 2)}-{bondDateString.Substring(6, 2)}";
                        DateOnly bondDateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(formattedBondDate);
                        bondGregorianDate = bondDateOnly;
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Invalid instrument code date format: {bondDateString}", ex);
                        continue;
                    }

                    decimal bondID = 0;
                    try
                    {
                        bondID = await bondsService.GetBondIdWithDate(bondGregorianDate);
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Failed to get BondID for date {bondGregorianDate}", ex);
                        continue;
                    }

                    if (bondID == 0)
                        continue;

                    DateOnly dateOnly;
                    try
                    {
                        string formattedDateStr = transactionDateStr.Replace('/', '-');
                        dateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(formattedDateStr);
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Invalid transaction date format: {transactionDateStr}", ex);
                        continue;
                    }

                    extractedRows.Add(new Transactions()
                    {
                        Quantity = int.Parse(tedad),
                        //Akhza = akhzaFinal,
                        PricePerUnit = decimal.Parse(price),
                        InvestmentPrice = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0,
                        TransactionDate = dateOnly,
                        TransactionType = "Buy",
                        BondId = bondID,
                        Commission = 0,
                        BrokerId = (int)BrokersEnum.Keshavarzi,
                        Status = (byte)TransactionStatusEnum.Unspecified
                    });
                }
                return extractedRows;
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error extracting Mellat Excel file", ex, _serviceLogNumber);
                LogAdd(null, nameof(UploadFileExcelMelli), innerEx);
                throw innerEx;
            }
        }
        
        
        public async Task<List<Transactions>> UploadFileExcelSanat(IFormFile sanatFile)
        {
            try
            {
                if (sanatFile == null || sanatFile.Length == 0)
                    throw new UploadFileException("No file uploaded.");

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await sanatFile.CopyToAsync(stream);

                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
                using var package = new ExcelPackage(new FileInfo(tempPath));

                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    throw new UploadFileException("The Excel file is empty or the first worksheet is invalid.");

                DataTable dt = new();
                int headerRow = 7;
                int dataStartRow = 8;

                int columnCount = ws.Dimension.End.Column;
                for (int i = 0; i < columnCount; i++)
                {
                    dt.Columns.Add();
                }

                for (int rowNum = dataStartRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, columnCount];
                    if (wsRow.All(c => string.IsNullOrWhiteSpace(c.Text))) continue;

                    DataRow row = dt.Rows.Add();
                    for (int col = 1; col <= columnCount; col++)
                    {
                        if (col - 1 < dt.Columns.Count)
                        {
                            row[col - 1] = ws.Cells[rowNum, col].Text;
                        }
                    }
                }
                var regex = new Regex(@"^خريد[^\d]*([\d,]+).*?(\d{6})\s*(?:\((\D*)(\d*)\))?.*?([\d,]+)",
                        RegexOptions.Compiled | RegexOptions.CultureInvariant);

                List<Transactions> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    if (row.ItemArray.Length <= 3) continue;
                    string transactionDateStr = row[1]?.ToString()?.Trim() ?? "";
                    string desc = row[2]?.ToString()?.Trim() ?? "";
                    string bedehkar = row[3]?.ToString()?.Trim() ?? "";
                    string descNormalized = desc
                        .Replace('\u06CC', '\u064A')
                        .Replace('\u200C', ' ')
                        .Replace('\u00A0', ' ')
                        .Trim();

                    var match = regex.Match(descNormalized);
                    if (!match.Success)
                        continue;


                    string tedad = match.Groups[1].Value.Replace(",", "");
                    string instrumentCode = match.Groups[2].Value;
                    string price = match.Groups[5].Value.Replace(",", "");

                    string bondDateString = "14" + instrumentCode;
                    DateOnly bondGregorianDate;

                    try
                    {
                        string formattedBondDate = $"{bondDateString.Substring(0, 4)}-{bondDateString.Substring(4, 2)}-{bondDateString.Substring(6, 2)}";
                        DateOnly bondDateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(formattedBondDate);
                        bondGregorianDate = bondDateOnly;
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Invalid instrument code date format: {bondDateString}", ex);
                        continue;
                    }

                    decimal bondID = 0;
                    try
                    {
                        bondID = await bondsService.GetBondIdWithDate(bondGregorianDate);
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Failed to get BondID for date {bondGregorianDate}", ex);
                        continue;
                    }

                    if (bondID == 0)
                        continue;

                    DateOnly dateOnly;
                    try
                    {
                        string formattedDateStr = transactionDateStr.Replace('/', '-');
                        dateOnly = BackEndInfrastructure.Utility.DateConvertor.ShamsiToMiladi(formattedDateStr);
                    }
                    catch (Exception ex)
                    {
                        LogAdd(null, $"Invalid transaction date format: {transactionDateStr}", ex);
                        continue;
                    }

                    extractedRows.Add(new Transactions()
                    {
                        Quantity = int.Parse(tedad),
                        //Akhza = akhzaFinal,
                        PricePerUnit = decimal.Parse(price),
                        InvestmentPrice = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0,
                        TransactionDate = dateOnly,
                        TransactionType = "Buy",
                        BondId = bondID,
                        Commission = 0,
                        BrokerId = (int)BrokersEnum.Keshavarzi,
                        Status = (byte)TransactionStatusEnum.Unspecified
                    });
                }
                return extractedRows;
            }
            catch (Exception ex)
            {
                var innerEx = new ServiceStorageException("Error extracting Mellat Excel file", ex, _serviceLogNumber);
                LogAdd(null, nameof(UploadFileExcelMelli), innerEx);
                throw innerEx;
            }
        }

        public async Task<decimal> AddMutipleAsync(List<Transactions> items)
        {
            try
            {
                // Convert domain models to database entities
                var dbEntities = items.Select(i => (TransactionsEntity)Activator.CreateInstance(typeof(TransactionsEntity), i))
                                      .ToList();
                await _coreUnitOfWork.TransactionRP.InsertMultipleAsync(dbEntities);
                foreach (var item in items)
                {
                    //await _coreUnitOfWork.MandehTransactionsRP.InsertAsync(entity);
                    LogAdd(item, $"Item Added Successfully with transactionDate : {item.TransactionDate} and BondId : {item.BondId} and BrokerID : {item.BrokerId}");
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

        public async Task<string> ApproveMultiTask(List<decimal> Ids)
        {
            try
            {
                foreach (var id in Ids)
                {
                    var transaction = await _coreUnitOfWork.TransactionRP.GetByIdAsync(id);
                    if (transaction != null)
                    {
                        transaction.Status = (byte)TransactionStatusEnum.Approved;
                        await _coreUnitOfWork.TransactionRP.UpdateAsync(transaction);
                        LogModify(transaction, $"Transaction with ID {id} approved.");
                    }
                    else
                    {
                        LogModify(null, $"Transaction with ID : {id} not found.");
                        throw new KeyNotFoundException($"Transaction with ID : {id} not found Exception");
                    }
                }
                await _coreUnitOfWork.CommitAsync();
                return $"Successfully updated transactions , counts are : {Ids.Count}";
            }
            catch (Exception ex)
            {
                if(ex is KeyNotFoundException)
                    throw;  
                var innerEx = new ServiceStorageException("Error approving items", ex, _serviceLogNumber);
                LogAdd(null, "Error approving items", innerEx);
                throw innerEx;
            }   
        }

        public async Task<string> RejectMultiTask(List<decimal> Ids)
        {
            try
            {
                foreach (var id in Ids)
                {
                    var transaction = await _coreUnitOfWork.TransactionRP.GetByIdAsync(id);
                    if (transaction != null)
                    {
                        transaction.Status = (byte)TransactionStatusEnum.Rejected;
                        await _coreUnitOfWork.TransactionRP.UpdateAsync(transaction);
                        LogModify(transaction, $"Transaction with ID {id} approved.");
                    }
                    else
                    {
                        LogModify(null, $"Transaction with ID : {id} not found.");
                        throw new KeyNotFoundException($"Transaction with ID : {id} not found Exception");
                    }
                }
                await _coreUnitOfWork.CommitAsync();
                return $"Successfully updated transactions , counts are : {Ids.Count}";
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException)
                    throw;
                var innerEx = new ServiceStorageException("Error approving items", ex, _serviceLogNumber);
                LogAdd(null, "Error rejecting items", innerEx);
                throw innerEx;
            }
        }

        public override Task<LinqDataResult<Transactions>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task ModifyAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        public override Task<Transactions> RetrieveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        public async Task<LinqDataResult<TransactionResult>> GetAllTransactionViewService(LinqDataRequest linqDataRequest, int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var rtnView = await _coreUnitOfWork.TransactionRP.GetAllItemsView(linqDataRequest, bondId, brokerId, transactionDateFrom, transactionDateTo);

                var mappedData = _mapper.Map<IEnumerable<TransactionResult>>(rtnView.Data);

                LinqDataResult<TransactionResult> linqDataResult = new LinqDataResult<TransactionResult>
                {
                    Data = mappedData,
                    RecordsTotal = rtnView.RecordsTotal,
                    RecordsFiltered = rtnView.RecordsFiltered   
                };

                return linqDataResult;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, linqDataRequest, ex);
                throw new ServiceStorageException("Error retrieving the TransactionView list ", ex, _serviceLogNumber);
            }
        }
        
        public async Task<LinqDataResult<AggregatedTransactionReportItem>> GetAggregatedReportService(LinqDataRequest linqDataRequest, int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var result = await _coreUnitOfWork.TransactionRP.GetAggregatedTransactionReportAsync(
                    linqDataRequest,
                    bondId,
                    brokerId,
                    transactionDateFrom,
                    transactionDateTo);

                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, linqDataRequest, ex);
                throw new ServiceStorageException("Error retrieving the TransactionView list ", ex, _serviceLogNumber);
            }
        }

        public async Task<byte[]> ExportAggregatedReportService(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            try
            {
                var data = await _coreUnitOfWork.TransactionRP.GetAggregatedTransactionReportForExportAsync(
                    bondId,
                    brokerId,
                    transactionDateFrom,
                    transactionDateTo);

                var excelBytes = GenerateAggregatedReportExcel(data);
                return excelBytes;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error exporting the TransactionView list ", ex, _serviceLogNumber);
            }
        }
        
        public async Task<byte[]> ExportAllTransactionReportService(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            try
            {
                var data = await _coreUnitOfWork.TransactionRP.GetAllItemsReportForExport(
                    bondId,
                    brokerId,
                    transactionDateFrom,
                    transactionDateTo);

                var excelBytes = GenerateAllTransactionReportExcel(data);
                return excelBytes;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error exporting the TransactionView list ", ex, _serviceLogNumber);
            }
        }

        private byte[] GenerateAggregatedReportExcel(List<AggregatedTransactionReportItem> data)
        {
            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Aggregated Report");

                worksheet.View.RightToLeft = true;

                worksheet.Cells[1, 1].Value = "نماد";               // Symbol
                worksheet.Cells[1, 2].Value = "تاریخ سررسید";      // DateOfMaturity
                worksheet.Cells[1, 3].Value = "تعداد";              // Quantity
                worksheet.Cells[1, 4].Value = "مبلغ کل خرید";      // TotalPurchasePrice
                worksheet.Cells[1, 5].Value = "ارزش اسمی کل";     // TotalFaceValue

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    int row = i + 2; 

                    worksheet.Cells[row, 1].Value = item.Symbol;
                    worksheet.Cells[row, 2].Value = item.DateOfMaturity.ToDateTime(TimeOnly.MinValue); 
                    worksheet.Cells[row, 3].Value = item.Quantity;
                    worksheet.Cells[row, 4].Value = item.TotalPurchasePrice;
                    worksheet.Cells[row, 5].Value = item.TotalFaceValue;
                }

                worksheet.Cells[2, 2, data.Count + 1, 2].Style.Numberformat.Format = "yyyy-mm-dd"; 
                worksheet.Cells[2, 3, data.Count + 1, 5].Style.Numberformat.Format = "#,##0"; 

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        
        private byte[] GenerateAllTransactionReportExcel(List<TransactionBasicView> data)
        {
            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transaction Report");

                worksheet.View.RightToLeft = true;

                int col = 1;
                worksheet.Cells[1, col++].Value = "نماد";
                worksheet.Cells[1, col++].Value = "نام کارگزار";
                worksheet.Cells[1, col++].Value = "نوع تراکنش";
                worksheet.Cells[1, col++].Value = "تاریخ تراکنش";
                worksheet.Cells[1, col++].Value = "تاریخ سررسید";
                worksheet.Cells[1, col++].Value = "تاریخ انتشار";
                worksheet.Cells[1, col++].Value = "تعداد";
                worksheet.Cells[1, col++].Value = "قیمت هر واحد";
                worksheet.Cells[1, col++].Value = "مبلغ سرمایه گذاری";
                worksheet.Cells[1, col++].Value = "کارمزد";
                worksheet.Cells[1, col++].Value = "ارزش اسمی";
                worksheet.Cells[1, col++].Value = "وضعیت";

                using (var range = worksheet.Cells[1, 1, 1, 12])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    int row = i + 2;

                    col = 1;
                    worksheet.Cells[row, col++].Value = item.Symbol;
                    worksheet.Cells[row, col++].Value = item.BrokerName;
                    worksheet.Cells[row, col++].Value = item.TransactionType;
                    worksheet.Cells[row, col++].Value = item.TransactionDate.ToDateTime(TimeOnly.MinValue);
                    worksheet.Cells[row, col++].Value = item.MaturityDate.ToDateTime(TimeOnly.MinValue);
                    worksheet.Cells[row, col++].Value = item.IssueDate?.ToDateTime(TimeOnly.MinValue);
                    worksheet.Cells[row, col++].Value = item.Quantity;
                    worksheet.Cells[row, col++].Value = item.PricePerUnit;
                    worksheet.Cells[row, col++].Value = item.InvestmentPrice;
                    worksheet.Cells[row, col++].Value = item.Commission;
                    worksheet.Cells[row, col++].Value = item.FaceValue;
                    worksheet.Cells[row, col++].Value = item.StatusText;
                }


                worksheet.Cells[2, 4, data.Count + 1, 6].Style.Numberformat.Format = "yyyy-mm-dd";

                worksheet.Cells[2, 7, data.Count + 1, 11].Style.Numberformat.Format = "#,##0.##";

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }


        public async Task<InvestmentReportResult> GetInvestmentReportAsync(
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            try
            {
                var effectiveDateTo = transactionDateTo ?? DateOnly.FromDateTime(DateTime.Now);

                var result = await _coreUnitOfWork.TransactionRP.GetInvestmentReportAsync(
                    effectiveDateTo, 
                    transactionDateFrom);

                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error retrieving the investment report ", ex, _serviceLogNumber);
            }
        }

        public async Task<CashInflowReportResult> GetCashInflowReportAsync(
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            try
            {
                var dateFrom = transactionDateFrom ?? DateOnly.FromDateTime(DateTime.Now);
                var result = await _coreUnitOfWork.TransactionRP.GetCashInflowReportAsync(dateFrom, transactionDateTo);
                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error retrieving the cash inflow report", ex, _serviceLogNumber);
            }
        }

        protected override Task ValidateOnAddAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(Transactions recievedItem, Transactions storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
