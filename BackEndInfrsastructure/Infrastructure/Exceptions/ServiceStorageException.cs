using System;

namespace BackEndInfrastructure.Infrastructure.Exceptions
{
    public class ServiceStorageException : ServiceException
    {
        public ServiceStorageException(string message, Exception innerException, int codeException) : base(message, innerException, codeException)
        {

        }
    }
}
