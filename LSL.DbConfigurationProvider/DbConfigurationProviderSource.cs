using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// DbConfigurationProviderSource
    /// </summary>
    public class DbConfigurationProviderSource : IConfigurationSource
    {
        /// <summary>
        /// Initialise the source with a given connection provider
        /// </summary>
        /// <param name="connectionProvider"></param>
        /// <param name="tableName"></param>
        /// <param name="keyField"></param>
        /// <param name="valueField"></param>
        /// <param name="keyPrefix"></param>
        /// <param name="onLoadError"></param>
        public DbConfigurationProviderSource(
            Func<DbConnection> connectionProvider,
            string tableName = "Settings",
            string keyField = "Key",
            string valueField = "Value",
            string keyPrefix = null,
            Action<LoadErrorContext> onLoadError = null)
        {
            ConnectionProvider = connectionProvider;
            TableName = tableName;
            KeyField = keyField;
            ValueField = valueField;
            KeyPrefix = keyPrefix;
            OnLoadError = onLoadError;
        }

        internal Func<DbConnection> ConnectionProvider { get; }
        internal string TableName { get; }
        internal string KeyField { get; }
        internal string ValueField { get; }
        internal string KeyPrefix { get; }
        internal Action<LoadErrorContext> OnLoadError { get; }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new DbConfigurationProvider(this);
    }
}