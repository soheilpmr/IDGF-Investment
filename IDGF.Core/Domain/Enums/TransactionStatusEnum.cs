using System.ComponentModel.DataAnnotations;

namespace IDGF.Core.Domain.Enums
{
    public enum TransactionStatusEnum
    {
        /// <summary>
        /// The transaction status is not specified or unknown (نامشخص).
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The transaction is under review (در حال بررسی).
        /// </summary>
        Pending = 1,

        /// <summary>
        /// The transaction has been approved (تأیید شده).
        /// </summary>
        Approved = 2,

        /// <summary>
        /// The transaction has been rejected (رد شده).
        /// </summary>
        Rejected = 3
    }
}
