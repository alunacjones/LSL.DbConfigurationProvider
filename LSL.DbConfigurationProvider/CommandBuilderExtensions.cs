using System.Data.Common;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// CommandBuilderExtensions
    /// </summary>
    public static class CommandBuilderExtensions
    {
        /// <summary>
        /// A null safe quote identifier producer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="unquotedIdentifier"></param>
        /// <returns></returns>
        public static string NullSafeQuoteIdentifier(this DbCommandBuilder source, string unquotedIdentifier) => 
            source == null
                ? unquotedIdentifier
                : source.QuoteIdentifier(unquotedIdentifier);
    }
}