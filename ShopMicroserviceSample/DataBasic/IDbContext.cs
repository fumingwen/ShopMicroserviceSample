using System;
using System.Data;

namespace DataBasic
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 建立Connection对象
        /// </summary>
        /// <returns>Connection对象</returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// 根据连接字符串建立Connection对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>Connection对象</returns>
        IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// 建立Command对象
        /// </summary>
        /// <returns>Command对象</returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// 建立DataAdapter对象
        /// </summary>
        /// <returns>DataAdapter对象</returns>
        IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 根据Connection建立Transaction
        /// </summary>
        /// <param name="connection">Connection对象</param>
        /// <returns>Transaction对象</returns>
        IDbTransaction CreateTransaction(IDbConnection connection);

        /// <summary>
        /// 根据Command对象建立DataReader
        /// </summary>
        /// <param name="myComm">Command对象</param>
        /// <returns>DataReader对象</returns>
        IDataReader CreateDataReader(IDbCommand myComm);

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="dispose">是否释放资源</param>
        void CloseConnection(IDbConnection connection, Boolean dispose=true);
    }

}
