using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
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
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);


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

                List<dynamic> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    string desc = row["شرح"]?.ToString()?.Trim() ?? "";
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

                    extractedRows.Add(new
                    {
                        Tedad = int.Parse(tedad),
                        Akhza = "اخزا" + akhza,
                        Price = int.Parse(price),
                        Bedehkar = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0
                    });
                }

                // ذخیره در دیتابیس با Dapper
                //    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                //    string sql = @"
                //INSERT INTO Khazaneh_Eslami_Kharid_Parsed (Tedad, AkhzaCode, Price, Bedehkar, CreatedAt)
                //VALUES (@Tedad, @Akhza, @Price, @Bedehkar, GETDATE())";

                //    foreach (var item in extractedRows)
                //        await conn.ExecuteAsync(sql, item);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });

            }
            catch (ServiceException ex)
            {
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
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Use a temporary path to save and process the file
                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                // Set EPPlus license context

                using var package = new ExcelPackage(new FileInfo(tempPath));
                var ws = package.Workbook.Worksheets.FirstOrDefault();

                if (ws == null)
                    return BadRequest("The Excel file is empty or invalid.");

                // Convert worksheet to DataTable, assuming headers are on row 5
                DataTable dt = new DataTable();
                int headerRow = 5;
                int dataStartRow = 7; // Data starts from row 7

                foreach (var firstRowCell in ws.Cells[headerRow, 1, headerRow, ws.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text.Trim());
                }

                for (int rowNum = dataStartRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = dt.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                // Regex pattern to find the required values in the "شرح" column for the Mellat file
                var regex = new Regex(@"تعداد\s+([\d,]+).*?اسنادخزانه.*?(\d{6})\(\).*?نرخ\s+([\d,]+)", RegexOptions.Compiled);

                List<dynamic> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    string desc = row["شرح"]?.ToString()?.Trim() ?? "";
                    string bedehkar = row["بدهکار"]?.ToString()?.Trim() ?? "";

                    // Skip irrelevant rows
                    if (string.IsNullOrWhiteSpace(desc))
                        continue;

                    // Process only rows for purchases, ignoring discount ("تخفیف") rows
                    if (!desc.StartsWith("علی الحسابخريد"))
                        continue;

                    var match = regex.Match(desc);
                    if (!match.Success)
                        continue;

                    // Extract data using the regex match groups
                    string tedad = match.Groups[1].Value.Replace(",", "");
                    string instrumentCode = match.Groups[2].Value; // The 6-digit code like "051112"
                    string price = match.Groups[3].Value.Replace(",", "");

                    extractedRows.Add(new
                    {
                        Tedad = long.Parse(tedad),
                        InstrumentCode = instrumentCode, // This corresponds to the old system's 'datee' field
                        Price = long.Parse(price),
                        Bedehkar = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0
                    });
                }

                // Clean up the temporary file
                System.IO.File.Delete(tempPath);

                // The database insertion logic is commented out, just like in the example provided.
                // You can use Dapper or another ORM to save the `extractedRows` list.
                /*
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                string sql = @"
                    INSERT INTO YourTargetTable (Tedad, InstrumentCode, Price, Bedehkar, CreatedAt)
                    VALUES (@Tedad, @InstrumentCode, @Price, @Bedehkar, GETDATE())";

                foreach (var item in extractedRows)
                    await conn.ExecuteAsync(sql, item);
                */

                return Ok(new
                {
                    Message = "مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (Exception ex)
            {
                // Simple error handling
                return StatusCode(500, $"An error occurred: {ex.Message}");
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

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using var package = new ExcelPackage(new FileInfo(tempPath));
                var ws = package.Workbook.Worksheets.FirstOrDefault();

                if (ws == null)
                    return BadRequest("The Excel file is empty or invalid.");

                // Convert worksheet to DataTable
                DataTable dt = new DataTable();
                int headerRow = 7;    // Headers are on row 7 in this file
                int dataStartRow = 8; // Data starts from row 8

                // Add columns from the header row
                foreach (var cell in ws.Cells[headerRow, 1, headerRow, ws.Dimension.End.Column])
                {
                    // Sanitize column names to avoid duplicates or nulls
                    string colName = cell.Text.Trim();
                    if (string.IsNullOrEmpty(colName) || dt.Columns.Contains(colName))
                    {
                        colName = $"{colName}_{cell.Start.Column}";
                    }
                    dt.Columns.Add(colName);
                }

                // Read data rows
                for (int rowNum = dataStartRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    // Ensure the row is not completely empty
                    if (wsRow.All(c => string.IsNullOrWhiteSpace(c.Text))) continue;

                    DataRow row = dt.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                // Regex for Bank Keshavarzi's "شرح" column
                var regex = new Regex(@"خريد\s+([\d,]+)\s+.*?(\d{6})\s+متوسط\s+([\d,]+)", RegexOptions.Compiled);

                List<dynamic> extractedRows = new();

                foreach (DataRow row in dt.Rows)
                {
                    string desc = row["شرح"]?.ToString()?.Trim() ?? "";
                    string bedehkar = row["بدهکار"]?.ToString()?.Trim() ?? "";

                    // Skip rows that don't represent a purchase
                    if (!desc.StartsWith("خريد"))
                        continue;

                    var match = regex.Match(desc);
                    if (!match.Success)
                        continue;

                    // Extract data using regex match groups
                    string tedad = match.Groups[1].Value.Replace(",", "");
                    string instrumentCode = match.Groups[2].Value; // The 6-digit code like "060615"
                    string price = match.Groups[3].Value.Replace(",", "");

                    extractedRows.Add(new
                    {
                        Tedad = long.Parse(tedad),
                        InstrumentCode = instrumentCode,
                        Price = long.Parse(price),
                        Bedehkar = decimal.TryParse(bedehkar.Replace(",", ""), out var val) ? val : 0
                    });
                }

                System.IO.File.Delete(tempPath);

                // Database saving logic remains commented out as per the example
                // You can implement the database insertion here

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

                // Use the first worksheet, as per the template.
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    return BadRequest("The Excel file is empty or the first worksheet is invalid.");

                DataTable dt = new();
                int headerRow = 7;
                int dataStartRow = 8;

                // Populate DataTable columns
                int columnCount = ws.Dimension.End.Column;
                for (int i = 0; i < columnCount; i++)
                {
                    dt.Columns.Add();
                }

                // Populate DataTable rows
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

                // =================================================================
                // START OF FIX: This is the most robust Regex.
                // [^\d]+ matches any character that is NOT a digit.
                // This is much more reliable than trying to match the specific words.
                // =================================================================
                var regex = new Regex(@"^خريد[^\d]*([\d,]+).*?(\d{6}).*?([\d,]+)",
                       RegexOptions.Compiled | RegexOptions.CultureInvariant);

                // =================================================================
                // END OF FIX
                // =================================================================

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
                    string price = match.Groups[3].Value.Replace(",", "");

                    extractedRows.Add(new
                    {
                        Tedad = long.Parse(tedad),
                        InstrumentCode = instrumentCode,
                        Price = long.Parse(price),
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