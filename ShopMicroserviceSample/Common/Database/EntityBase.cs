namespace Common.Database
{
    /// <summary>
    /// 实体对象基类
    /// </summary>
    /// <typeparam name="T">主键</typeparam>
    public abstract class EntityBase<T>
    {
        private T primaryKey = default;

        /// <summary>
        /// 获取主键
        /// </summary>
        public T GetPrimaryKey()
        {
            return this.primaryKey;
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        public void SetPrimaryKey(T primaryKey)
        {
            this.primaryKey = primaryKey;
        }
    }
}
