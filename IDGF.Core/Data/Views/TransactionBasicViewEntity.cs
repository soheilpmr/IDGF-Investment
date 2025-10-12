using IDGF.Core.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Views
{

    [Keyless]
    [NotMapped]
    public class TransactionBasicViewEntity : TransactionBasicView
    {
    }
}
