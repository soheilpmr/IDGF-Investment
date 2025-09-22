using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndInfrastructure.Infrastructure.Exceptions
{
    public class PasswordValidateException : ServiceException
    {
        public PasswordValidateException(string message, Exception innerException) : base(message, innerException, 300)
        {

        }
        public PasswordValidateException(string message) : base(message, 300)
        {

        }
        public PasswordValidateException() : base()
        {
            _code = 300;
        }
    }
}
