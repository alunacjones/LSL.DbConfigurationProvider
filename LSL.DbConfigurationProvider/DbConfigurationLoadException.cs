using System;
using System.Runtime.Serialization;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// Db Configuration Provider Load Error Exception
    /// </summary>
    /// <remarks>
    /// Allows for a consuming application to catch for this specific exception type
    /// and inspect the inner exception for further details.
    /// </remarks>
    public class DbConfigurationProviderLoadException : Exception
    {
        internal DbConfigurationProviderLoadException(Exception innerException): base(
            "DbConfigurationProvider load error. See InnerException for details",
            innerException) {}
    }
}