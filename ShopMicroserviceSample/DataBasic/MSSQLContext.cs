using System;
using System.Data;
using System.Data.SqlClient;

namespace DataBasic
{
    /// <summary>
    /// MS SQL Server 数据库操作
    /// </summary>
    public class MSSQLContext : IDbContext
    {
        public  string ConnectionString { get; set; }

        public MSSQLContext()
        { 
        
        }

        public MSSQLContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public  IDbConnection CreateConnection(string ConnectionString)
        {
            return new SqlConnection(ConnectionString);
        }

        public  IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public  IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public  IDbTransaction CreateTransaction(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }

        public  IDataReader CreateDataReader(IDbCommand command)
        {
            return command.ExecuteReader();
        }
        public void CloseConnection(IDbConnection connection, Boolean dispose = true)
        {
            if (connection == null) return;
            if (connection.State != ConnectionState.Open) connection.Close();
            if (dispose) connection.Dispose();
        }
    }

}
