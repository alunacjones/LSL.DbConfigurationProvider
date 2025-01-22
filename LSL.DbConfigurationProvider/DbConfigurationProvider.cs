using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace LSL.DbConfigurationProvider
{
    /// <summary>
    /// DbConfigurationProvider
    /// </summary>
    public class DbConfigurationProvider : ConfigurationProvider
    {
        private readonly DbConfigurationProviderSource _source;

        internal DbConfigurationProvider(DbConfigurationProviderSource source)
        {
            _source = source;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            Data.Clear();

            try
            {
                InternalLoad();
            }
            catch (Exception ex)
            {
                var context = new LoadErrorContext(ex);
                _source.OnLoadError?.Invoke(context);

                if (context.RethrowException)
                {
                    throw new DbConfigurationProviderLoadException(ex);
                }
            }
        }

        private void InternalLoad()
        {
            using var dbConnection = _source.ConnectionProvider();

            dbConnection.Open();
            var factory = DbProviderFactories.GetFactory(dbConnection);
            var commandBuilder = factory.CreateCommandBuilder();

            var quotedTableName = commandBuilder.NullSafeQuoteIdentifier(_source.TableName);
            var quotedKeyField = commandBuilder.NullSafeQuoteIdentifier(_source.KeyField);
            var quotedValueField = commandBuilder.NullSafeQuoteIdentifier(_source.ValueField);
            
            using var cmd = dbConnection.CreateCommand();
            cmd.CommandText = $"Select {quotedKeyField}, {quotedValueField} From {quotedTableName}";

            if (_source.KeyPrefix != null)
            {
                cmd.CommandText += $" Where {quotedKeyField} Like @keyPrefix";
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "keyPrefix";
                parameter.Value = _source.KeyPrefix + "%";
                cmd.Parameters.Add(parameter);
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Data.Add(
                    TransformKey(reader[_source.KeyField].ToString()), 
                    reader[_source.ValueField].ToString());
            }
        }

        private string TransformKey(string key) => _source.KeyPrefix == null
            ? key
            : key.Substring(_source.KeyPrefix.Length);
    }
}