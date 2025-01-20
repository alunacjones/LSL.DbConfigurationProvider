using System;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// Context that is given to the onLoadError delegate
    /// </summary>
    public class LoadErrorContext
    {
        internal LoadErrorContext(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// If set to <c>true</c> then the exception will be re-thrown
        /// </summary>
        /// <remarks>
        /// The thrown exception is a <c>DbConfigurationProviderLoadException</c>
        /// that allows for an application to catch generically and act accordingly
        /// against the <c>InnerException</c>
        /// </remarks>
        /// <value></value>
        public bool RethrowException { get; set; }

        /// <summary>
        /// The exception that occurred whilst fetching the DB settings
        /// </summary>
        /// <value></value>
        public Exception Exception { get; }
    }
}