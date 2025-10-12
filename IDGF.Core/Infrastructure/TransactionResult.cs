namespace IDGF.Core.Infrastructure
{
    public class TransactionResult
    {
        public decimal TransactionId { get; set; }
        public decimal BondId { get; set; }
        public string Symbol { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly MaturityDate { get; set; }
        public string? BrokerName { get; set; }
        public decimal FaceValue { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Quantity { get; set; }
        public DateOnly TransactionDate { get; set; }
        public decimal Commission { get; set; }
        public short Status { get; set; }
        public string TransactionType { get; set; }

        // 🧮 Derived fields (computed in C#)

        public decimal InvestmentAmount => PricePerUnit * Quantity;
        public decimal MaturityAmount => FaceValue * Quantity;
        public int DaysToMaturity => (MaturityDate.ToDateTime(TimeOnly.MinValue) - TransactionDate.ToDateTime(TimeOnly.MinValue)).Days;

        // Simple Yield (%)
        public decimal SimpleYield
        {
            get
            {
                if (PricePerUnit <= 0 || DaysToMaturity <= 0)
                    return 0;
                var yield = ((FaceValue - PricePerUnit) / PricePerUnit) * (365m / DaysToMaturity) * 100m;
                return Math.Round(yield, 2);
            }
        }

        // Yield to Maturity (%)
        public decimal YieldToMaturity
        {
            get
            {
                if (PricePerUnit <= 0 || DaysToMaturity <= 0)
                    return 0;
                var ytm = (decimal)(Math.Pow((double)(FaceValue / PricePerUnit), 365.0 / DaysToMaturity) - 1) * 100m;
                return Math.Round(ytm, 2);
            }
        }

        // Total Purchase Amount (with commission)
        public decimal TotalPurchase => (PricePerUnit * Quantity) + Commission;

        // Transaction status text
        public string StatusText => Status switch
        {
            1 => "در حال بررسی",
            2 => "تأیید شده",
            3 => "رد شده",
            _ => "نامشخص"
        };
    }
}
