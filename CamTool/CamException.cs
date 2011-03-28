using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace CamTool
{
    /// <summary>
    /// Represents errors that occur when interfacing with the webcam device.
    /// </summary>
    public sealed class CamException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CamException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CamException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CamException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CamException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CamException"/> class.
        /// </summary>
        public CamException()
            : base()
        {
        }
    }
}
