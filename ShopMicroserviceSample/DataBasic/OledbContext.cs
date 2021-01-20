using System;
using System.Data;
using System.Data.OleDb;

namespace DataBasic
{
    /// <summary>
    /// Oledb操作
    /// </summary>
    public class OledbContext : IDbContext
    {
        public string ConnectionString { get; set; }

        public OledbContext()
        {

        }
        public OledbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IDbCommand CreateCommand()
        {
            return new OleDbCommand();
        }

        public IDbConnection CreateConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
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
