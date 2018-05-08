using System;
using System.Collections.Generic;
using System.Text;

namespace Monithor.ConsoleReceiver
{
    public class Displayer
    {
        public void LogException(string message, Exception exception)
        {
            Console.WriteLine($"Exception : {message} - {exception}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"Error : {message}");

        }
        public void LogTrace(string message)
        {
            Console.WriteLine($"Trace : {message}");
        }
    }
}
