namespace Common.Caches
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">名称</param>
        /// <param name="value">值</param>
        /// <param name="timeOut">有效时间，默认1200秒</param>
        /// <param name="overwrite">如果存在是否覆盖</param>
        /// <returns>成功失败</returns>
        bool Add<T>(string key, T value, int timeOut = 1200, bool overwrite = true);

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">名称</param>
        /// <param name="value">值</param>
        /// <param name="timeOut">有效时间，默认1200秒</param>
        /// <returns>成功失败</returns>
        bool Update<T>(string key, T value, int timeOut = 1200);

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns>是否存在</returns>
        bool Exists(string key);

        /// <summary>
        /// 获取缓存，返回一个字符串
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns>缓存对象</returns>
        string Get(string key);

        /// <summary>
        /// 获取缓存，返回一个对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">名称</param>
        /// <returns>缓存对象</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns>成功失败</returns>
        bool Delete(string key);

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        bool RemoveAll();

        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected();

        /// <summary>
        /// 刷新配置
        /// </summary>
        void RefreshConfig();
    }
}
