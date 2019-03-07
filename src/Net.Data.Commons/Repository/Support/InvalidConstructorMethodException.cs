using System;
using System.Runtime.Serialization;

namespace Net.Data.Commons.Repository.Support
{
    [Serializable]
    public class InvalidBuildingMethodException : Exception
    {
        public InvalidBuildingMethodException() { }
        public InvalidBuildingMethodException(string message) : base(message) { }
        public InvalidBuildingMethodException(string message, Exception inner) : base(message, inner) { }
        protected InvalidBuildingMethodException(SerializationInfo info, StreamingContext context) : base(info, context) 
        { 
            
        }
    }
}