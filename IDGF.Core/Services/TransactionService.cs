using AutoMapper;
using Azure.Core;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using IDGF.Core.Domain.Views;
using IDGF.Core.Infrastructure;
using IDGF.Core.Infrastructure.UnitOfWork;
using OfficeOpenXml;
using System;
using System.Data;
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
        string? brokerName = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var rtnView = await _coreUnitOfWork.TransactionRP.GetAllItemsView(linqDataRequest, bondId, brokerName, transactionDateFrom, transactionDateTo);

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
