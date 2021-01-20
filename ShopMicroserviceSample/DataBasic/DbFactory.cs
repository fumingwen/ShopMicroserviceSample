using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace DataBasic
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class DbFactory
    { 
        public IDbContext db { get; private set; }

        public DbFactory(string configKey, string defaultKey = "Default")
        {
            var config = ConfigurationManager.ConnectionStrings[configKey];
            if (config == null)
            {
                config = ConfigurationManager.ConnectionStrings[defaultKey];
            }
            var provider = config.ProviderName;
            var connectionString = config.ConnectionString;

            switch (provider)
            {
                case "System.Data.SqlClient":
                    db = new MSSQLContext(connectionString);
                    break;
                case "Odbc":
                    db = new OdbcContext(connectionString);
                    break;
                case "Oledb":
                    db = new OledbContext(connectionString);
                    break;
                default:
                    db = new MSSQLContext(connectionString);
                    break;
            }
        }

        public DbFactory(IConfiguration configuration)
        { 
            var provider = configuration.GetSection("AppSettings:ProviderName");
            var connectionString = configuration.GetSection("AppSettings:ConnectionString").Value;

            switch (provider.Value)
            {
                case "System.Data.SqlClient":
                    db = new MSSQLContext(connectionString);
                    break;
                case "Odbc":
                    db = new OdbcContext(connectionString);
                    break;
                case "Oledb":
                    db = new OledbContext(connectionString);
                    break;
                default:
                    db = new MSSQLContext(connectionString);
                    break;
            }
        }
    }
}
