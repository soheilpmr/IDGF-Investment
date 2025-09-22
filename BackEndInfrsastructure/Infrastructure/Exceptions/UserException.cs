using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure.Exceptions
{
    public class UserException : ServiceException
    {
        public UserException(string message, Exception innerException) : base(message, innerException, 200)
        {

        }
        public UserException(string message) : base(message, 200)
        {

        }
        public UserException() : base()
        {
            _code = 200;
        }
    }
}
