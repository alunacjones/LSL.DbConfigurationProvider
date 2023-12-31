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
        private readonly Func<DbConnection> _connectionProvider;
        private readonly string _tableName;
        private readonly string _keyField;
        private readonly string _valueField;

        /// <summary>
        /// Initialise the source wiht a given connection provider
        /// </summary>
        /// <param name="connectionProvider"></param>
        /// <param name="tableName"></param>
        /// <param name="keyField"></param>
        /// <param name="valueField"></param>
        public DbConfigurationProviderSource(
            Func<DbConnection> connectionProvider,
            string tableName = "Settings",
            string keyField = "Key",
            string valueField = "Value")
        {
            _connectionProvider = connectionProvider;
            _tableName = tableName;
            _keyField = keyField;
            _valueField = valueField;
        }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new DbConfigurationProvider(
            _connectionProvider,
            _tableName,
            _keyField,
            _valueField);
    }
}