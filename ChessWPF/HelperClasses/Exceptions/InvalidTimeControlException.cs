using System;

namespace ChessWPF.HelperClasses.Exceptions
{
    public class InvalidTimeControlException : Exception
    {
        public InvalidTimeControlException() : base("Invalid time control!") 
        {
            
        }
    }
}
