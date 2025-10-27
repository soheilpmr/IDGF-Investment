using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Utility
{
    public class DateConvertor
    {
        public static DateOnly ShamsiToMiladi(string shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                throw new ArgumentException("Invalid date string.", nameof(shamsiDate));

            var persianCalendar = new PersianCalendar();

            // Expected format: yyyy-MM-dd (e.g., 1404-07-01)
            var parts = shamsiDate.Split('-');
            if (parts.Length != 3)
                throw new FormatException("Date must be in format yyyy-MM-dd.");

            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            // Convert to DateTime first, then wrap into DateOnly
            var dateTime = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return DateOnly.FromDateTime(dateTime);
        }

        public static (string Date, string Time) GetCurrentPersianDateTimeStrings()
        {
            var pc = new PersianCalendar();
            var now = DateTime.Now;

            string date = $"{pc.GetYear(now)}{pc.GetMonth(now):D2}{pc.GetDayOfMonth(now):D2}";
            string time = $"{now.Hour:D2}{now.Minute:D2}{now.Second:D2}";

            return (date, time);
        }
    }
}
