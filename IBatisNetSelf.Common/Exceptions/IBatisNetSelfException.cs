using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Exceptions
{
    /// <summary>
    /// The DalException is thrown when an error in the Dal occurs.
    /// </summary>
    /// <remarks>
    /// This is the base exception for all exceptions thrown in the framework. 
    /// </remarks>
    [Serializable]
    public class IBatisNetSelfException : System.ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBatisNet.Common.Exceptions.IBatisNetException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the <para>Message</para> property of the new instance 
        /// to a system-supplied message that describes the error.
        /// </remarks>
        public IBatisNetSelfException() : base("IBatisNetSelf framework caused an exception.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBatisNet.Common.Exceptions.IBatisNetException"/> 
        /// class with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the Message property of the new instance 
        /// using the message parameter.
        /// </remarks>
        /// <param name="ex">
        /// The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference (Nothing in Visual Basic), 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public IBatisNetSelfException(Exception ex) : base("IBatisNetSelf framework caused an exception.", ex) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBatisNet.Common.Exceptions.IBatisNetException"/> 
        /// class with a specified error message.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the Message property of the new instance to 
        /// the Message property of the passed in exception. 
        /// </remarks>
        /// <param name="message">The message that describes the error.</param>
        public IBatisNetSelfException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBatisNet.Common.Exceptions.IBatisNetException"/> 
        /// class with a specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <remarks>
        /// An exception that is thrown as a direct result of a previous exception should include a reference to the previous 
        /// exception in the InnerException property. 
        /// The InnerException property returns the same value that is passed into the constructor, or a null reference 
        /// (Nothing in Visual Basic) if the InnerException property does not supply the inner exception value to the constructor.
        /// </remarks>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that caused the error</param>
        public IBatisNetSelfException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="IBatisNet.Common.Exceptions.IBatisNetException"/> 
        /// class with serialized data.
        /// </summary>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute the 
        /// exception object transmitted over a stream.
        /// </remarks>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. 
        /// </param>
        protected IBatisNetSelfException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
