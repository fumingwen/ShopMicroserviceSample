using Common.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace Common.Caches
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public class Redis : ICache
    {
        /// <summary>
        /// Redis的Json配置路径
        /// </summary>
        public string ConfigPath { get; set; }

        private static RedisConfig redisConfig = null;

        private static readonly object syslock = new object();

        private static readonly ConcurrentDictionary<string, IConnectionMultiplexer> connections = new ConcurrentDictionary<string, IConnectionMultiplexer>();

        private static IConnectionMultiplexer connection = null;

        private static IDatabase database = null;

        public Redis()
        {

        }

        public Redis(string configPath)
        {
            this.ConfigPath = configPath;
        }

        private void SetConfigpath()
        {
            if (string.IsNullOrEmpty(ConfigPath))
            {
                ConfigPath = string.Format("{0}Config\\RedisConfig.json", AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        private IConnectionMultiplexer CreateConnection()
        {
            if (redisConfig == null)
            {
                lock (syslock)
                {
                    if (redisConfig == null)
                    {
                        this.SetConfigpath();
                        redisConfig = FileUtil.FromJsonFile<RedisConfig>(ConfigPath);
                    }
                }
            }
            var clientName = redisConfig.ClientName;

            var exists = connections.TryGetValue(clientName, out IConnectionMultiplexer connection);
            if (exists && connection != null && connection.IsConnected) return connection;

            lock (syslock)
            {
                exists = connections.TryGetValue(clientName, out connection);
                if (exists && connection != null && connection.IsConnected) return connection;

                connection = ConnectionMultiplexer.Connect(redisConfig.CreateConfigurationOptions());
                connections.AddOrUpdate(clientName, connection, (key, value) => connection);
                return connection;
            }
        }

        private bool CheckDatabase()
        {
            if (connection == null || !connection.IsConnected)
            {
                connection = this.CreateConnection();
            }
            if (connection == null || !connection.IsConnected) return false;

            if (database == null)
            {
                database = connection.GetDatabase(redisConfig.DatabaseIndex);
            }
            return database != null;
        }

        private TimeSpan ExpiryTimeSpan(int seconds)
        {
            return new TimeSpan(0, 0, 0, seconds);
        }

        public bool Add<T>(string key, T value, int timeOut = 1200, bool overwrite = true)
        {
            if (!CheckDatabase() || string.IsNullOrEmpty(key) || value == null) return false;

            if (timeOut < 1) timeOut = 1200;

            try
            {
                if (!overwrite && this.Exists(key)) return true;
                return database.StringSet(key, JsonUtil.ToJson(value), this.ExpiryTimeSpan(timeOut));
            }
            catch
            {
                return false;
            }
        }

        public bool Exists(string key)
        {
            if (!CheckDatabase() || string.IsNullOrEmpty(key)) return false;
            return database.KeyExists(key);
        }

        public string Get(string key) 
        {
            return this.Get<string>(key);
        }

        public T Get<T>(string key) where T : class
        {
            if (!CheckDatabase() || string.IsNullOrEmpty(key)) return default;
            try
            {
                var value = database.StringGet(key);
                if (!value.HasValue || value.IsNullOrEmpty) return default;
                return JsonUtil.FromJson<T>(value);
            }
            catch
            {
                return default;
            }
        }

        public bool Update<T>(string key, T value, int timeOut = 1200)
        {
            return this.Add(key, value, timeOut, true);
        }

        public bool Delete(string key)
        {
            if (!CheckDatabase() || string.IsNullOrEmpty(key)) return false;
            return database.KeyDelete(key);
        }

        public bool RemoveAll()
        {
            throw new System.NotImplementedException();
        }

        public bool IsConnected()
        {
            var connection = this.CreateConnection();
            return (connection != null && connection.IsConnected);
        }

        public void RefreshConfig()
        {
            lock (syslock)
            {
                this.SetConfigpath();
                redisConfig = FileUtil.FromJsonFile<RedisConfig>(ConfigPath);
            }
        }
    }
}
