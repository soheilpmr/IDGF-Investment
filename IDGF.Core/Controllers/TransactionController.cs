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
    }
}