using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace LSL.DbConfigurationProvider.Tests
{
    public class SqliteKeepAliveDbConnection : DbConnection
    {
        private readonly DbConnection _innerConnection;

        public SqliteKeepAliveDbConnection(DbConnection innerConnection)
        {
            _innerConnection = innerConnection;
        }

        public override string ConnectionString { get => _innerConnection.ConnectionString; set => _innerConnection.ConnectionString = value; }

        public override string Database => _innerConnection.Database;

        public override string DataSource => _innerConnection.DataSource;

        public override string ServerVersion => _innerConnection.ServerVersion;

        public override ConnectionState State => _innerConnection.State;

        public override void ChangeDatabase(string databaseName) => _innerConnection.ChangeDatabase(databaseName);

        public override void Close() => _innerConnection.Close();

        public override void Open() => _innerConnection.Open();

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => _innerConnection.BeginTransaction(isolationLevel);

        protected override DbCommand CreateDbCommand() => _innerConnection.CreateCommand();

        protected override DbProviderFactory DbProviderFactory => SqliteFactory.Instance;
    }
}
