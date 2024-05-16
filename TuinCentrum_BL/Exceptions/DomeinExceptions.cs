using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuinCentrum_BL.Exceptions
{
    public class DomeinException : Exception
    {
        public DomeinException(string? message) : base(message)
        { 
        }

        public DomeinException(string? message, Exception? innerException) : base(message, innerException)
        { 
        }
    }
}
