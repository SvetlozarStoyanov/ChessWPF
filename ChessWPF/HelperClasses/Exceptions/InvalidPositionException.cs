using System;

namespace ChessWPF.HelperClasses.Exceptions
{
    public class InvalidPositionException : Exception
    {
        public InvalidPositionException(string message) : base(message) 
        {

        }
        
    }
}
