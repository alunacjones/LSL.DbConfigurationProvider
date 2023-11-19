using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// DbConfigurationBuilderExtensions
    /// </summary>
    public static class DbConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds a configuration provider that fetches settings from a DB table using the give connectionProvider
        /// </summary>
        /// <param name="source"></param>
        /// <param name="connectionProvider"></param>
        /// <param name="tableName"></param>
        /// <param name="keyField"></param>
        /// <param name="valueField"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddDbConfiguration(
            this IConfigurationBuilder source,
            Func<DbConnection> connectionProvider,
            string tableName = "Settings",
            string keyField = "Key",
            string valueField = "Value") => source.Add(new DbConfigurationProviderSource(connectionProvider, tableName, keyField, valueField));
    }
}