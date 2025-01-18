using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// DbConfigurationProvider
    /// </summary>
    public class DbConfigurationProvider : ConfigurationProvider
    {
        private readonly Func<DbConnection> _connectionProvider;
        private readonly string _tableName;
        private readonly string _keyField;
        private readonly string _valueField;
        private readonly string _keyPrefix;

        /// <summary>
        /// Create a provider with overridden values for table name and field names
        /// </summary>
        /// <param name="connectionProvider"></param>
        /// <param name="tableName"></param>
        /// <param name="keyField"></param>
        /// <param name="valueField"></param>
        /// <param name="keyPrefix"></param>
        internal DbConfigurationProvider(
            Func<DbConnection> connectionProvider,
            string tableName = "Settings",
            string keyField = "Key",
            string valueField = "Value",
            string keyPrefix = null)
        {
            _connectionProvider = connectionProvider;
            _tableName = tableName;
            _keyField = keyField;
            _valueField = valueField;
            _keyPrefix = keyPrefix;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            using var dbConnection = _connectionProvider();

            dbConnection.Open();
            var factory = DbProviderFactories.GetFactory(dbConnection);
            var commandBuilder = factory.CreateCommandBuilder();

            var quotedTableName = commandBuilder?.QuoteIdentifier(_tableName) ?? _tableName;
            var quotedKeyField = commandBuilder?.QuoteIdentifier(_keyField) ?? _keyField;
            var quotedValueField = commandBuilder?.QuoteIdentifier(_valueField) ?? _valueField;
            
            using var cmd = dbConnection.CreateCommand();
            cmd.CommandText = $"Select {quotedKeyField}, {quotedValueField} From {quotedTableName}";

            if (_keyPrefix != null)
            {
                cmd.CommandText += $" Where {quotedKeyField} Like @keyPrefix";
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "keyPrefix";
                parameter.Value = _keyPrefix + "%";
                cmd.Parameters.Add(parameter);
            }
            
            using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                var result = new Dictionary<string, string>();

                while (reader.Read())
                {
                    result.Add(
                        TransformKey(reader[_keyField].ToString()), 
                        reader[_valueField].ToString());
                }

                Data = result;
            }
            else
            {
                Data = new Dictionary<string, string>();
            }
        }

        private string TransformKey(string key)
        {
            return _keyPrefix == null
                ? key
                : key.Substring(_keyPrefix.Length);
        }
    }
}