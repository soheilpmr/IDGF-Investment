using System;
using System.Collections.Generic;
using System.Text;

namespace BackEndInfrastructure.Infrastructure.Exceptions
{
    public class ServiceObjectNotFoundException : ServiceException
    {
        public ServiceObjectNotFoundException(string message, Exception innerException, int codeException) : base(message, innerException, codeException + 5)
        {

        }
        public ServiceObjectNotFoundException(string message) : base(message)
        {

        }
        public ServiceObjectNotFoundException(string message, int codeException) : base(message, codeException + 5)
        {

        }
    }
}
