using System;

namespace Monithor.Client
{
    public class MonithorClientException : Exception
    {
        public MonithorClientException(string message, Exception e) : base(message, e)
        {

        }

        public MonithorClientException(string message) : base(message)
        {

        }
    }
}
