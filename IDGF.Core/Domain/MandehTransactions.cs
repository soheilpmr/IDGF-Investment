using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class MandehTransactions : Model<long>
    {
        public DateTime TransactionDateTime { get; set; }
        public DateOnly TransactionDate { get; set; }
        public TimeOnly TransactionTime { get; set; }
        public double Mablagh { get; set; }
        public double DarRah { get; set; }
        public byte Taeed { get; set; }
        public double? Hazineh { get; set; }

       
    }
}
