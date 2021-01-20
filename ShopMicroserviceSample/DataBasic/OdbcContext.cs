using System;
using System.Data;
using System.Data.Odbc;

namespace DataBasic
{
    /// <summary>
    /// Odbc操作
    /// </summary>
    public class OdbcContext : IDbContext
    {
        public string ConnectionString { get; set; }

        public OdbcContext()
        {

        }
        public OdbcContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IDbCommand CreateCommand()
        {
            return new OdbcCommand();
        }

        public IDbConnection CreateConnection()
        {
            return new OdbcConnection(ConnectionString);
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return new OdbcConnection(connectionString);
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            return new OdbcDataAdapter();
        }

        public IDataReader CreateDataReader(IDbCommand command)
        {
            return command.ExecuteReader();
        }

        public IDbTransaction CreateTransaction(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }
        public void CloseConnection(IDbConnection connection, Boolean dispose = true)
        {
            if (connection == null) return;
            if (connection.State != ConnectionState.Open) connection.Close();
            if (dispose) connection.Dispose();
        }
    }

}
