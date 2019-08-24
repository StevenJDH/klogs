using System;
using System.Collections.Generic;
using System.Text;

namespace klogs.Classes
{
    /// <summary>
    /// Exception thrown when a <see cref="Shell"/> redirects a stream to the standard error output device.
    /// </summary>
    [Serializable]
    class StandardErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        public StandardErrorException() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public StandardErrorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public StandardErrorException(string message, Exception inner) : base(message, inner) { }
    }
}
