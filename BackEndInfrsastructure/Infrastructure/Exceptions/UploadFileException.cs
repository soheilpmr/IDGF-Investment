using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure.Exceptions
{
    public class UploadFileException : ServiceException
    {
        public UploadFileException(string message, Exception innerException, int codeException) : base(message, innerException, codeException + 5)
        {

        }
        public UploadFileException(string message) : base(message)
        {

        }
        public UploadFileException(string message, int codeException) : base(message, codeException + 5)
        {

        }
    }
}
