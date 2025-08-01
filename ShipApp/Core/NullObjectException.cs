using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.Core
{
    public class NullObjectException : Exception
    {
        public NullObjectException() { }

        public NullObjectException(string message)
            : base(message) { }

        public NullObjectException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
