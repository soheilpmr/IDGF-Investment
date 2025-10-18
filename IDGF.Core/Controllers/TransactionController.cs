using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using IDGF.Core.Infrastructure.UnitOfWork;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.Data;
using System.Text.RegularExpressions;
using System.Transactions;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService transactionService;
        private readonly BondsService bondsService;
        public TransactionController(TransactionService transactionService, BondsService bondsService)
        {
             this.transactionService = transactionService;
            this.bondsService = bondsService;
        }

        /// <summary>
        /// import kargozari bank melli excel file  
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost(nameof(UploadMelliExcel))]
        public async Task<IActionResult> UploadMelliExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelMelli(file);
                await transactionService.AddMutipleAsync(extractedRows);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (ServiceException ex)
            {
                if(ex is UploadFileException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(nameof(UploadMellatExcel))]
        public async Task<IActionResult> UploadMellatExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelMellat(file);
                await transactionService.AddMutipleAsync(extractedRows);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (ServiceException ex)
            {
                if (ex is UploadFileException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(nameof(UploadBkiExcel))]
        public async Task<IActionResult> UploadBkiExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
                using var package = new ExcelPackage(new FileInfo(tempPath));

                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    return BadRequest("The Excel file is empty or the first worksheet is invalid.");

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

                List<dynamic> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    if (row.ItemArray.Length <= 3) continue; 

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
                    string prefix = match.Groups[3].Value.Trim();
                    string number = match.Groups[4].Value.Trim();
                    string price = match.Groups[5].Value.Replace(",", "");

                    string akhza_code;
                    if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(number))
                    {
                        akhza_code = "N/A";
                    }
                    else
                    {
                        if (number.Length > 3)
                            number = number[..3];
                        akhza_code = prefix + number;
                    }

                    extractedRows.Add(new
                    {
                        Tedad = long.TryParse(tedad, out var t) ? t : 0,
                        InstrumentCode = instrumentCode,
                        Akhza = akhza_code,
                        Price = long.TryParse(price, out var p) ? p : 0,
                        Bedehkar = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0
                    });
                }

                System.IO.File.Delete(tempPath);

                return Ok(new
                {
                    Message = "✅ Bank Keshavarzi file processed successfully.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost(nameof(UploadSanatExcel))]
        public async Task<IActionResult> UploadSanatExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
                using var package = new ExcelPackage(new FileInfo(tempPath));

                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    return BadRequest("The Excel file is empty or the first worksheet is invalid.");

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

                List<dynamic> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    if (row.ItemArray.Length <= 3) continue; 

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
                    string prefix = match.Groups[3].Value.Trim();  
                    string number = match.Groups[4].Value.Trim();  
                    string price = match.Groups[5].Value.Replace(",", "");

                    string akhza_code;
                    if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(number))
                    {
                        akhza_code = "N/A"; 
                    }
                    else
                    {
                        if (number.Length > 3)
                            number = number[..3]; 
                        akhza_code = prefix + number;
                    }

                    extractedRows.Add(new
                    {
                        Tedad = long.TryParse(tedad, out var t) ? t : 0,
                        InstrumentCode = instrumentCode,
                        Akhza = akhza_code,        
                        Price = long.TryParse(price, out var p) ? p : 0,
                        Bedehkar = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0
                    });
                }

                System.IO.File.Delete(tempPath);

                return Ok(new
                {
                    Message = "✅ Bank Sanat va Madan file processed successfully.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}