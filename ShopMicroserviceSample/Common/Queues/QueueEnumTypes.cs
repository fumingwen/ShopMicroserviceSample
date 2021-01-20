namespace Common.Queue
{
    /// <summary>
    /// 频道类型
    /// </summary>
    internal enum ChannelType
    {
        /// <summary>
        /// 发布
        /// </summary>
        Publish = 0,

        /// <summary>
        /// 读取
        /// </summary>
        Read = 1
    }

    /// <summary>
    /// 交换器类型
    /// </summary>
    public enum MatchType
    {
        /// <summary>
        /// 完全匹配
        /// </summary>
        Direct = 0,

        /// <summary>
        /// 不匹配
        /// </summary>
        Fanout = 1,

        /// <summary>
        /// 模式匹配
        /// </summary>
        Topic = 2,

        /// <summary>
        /// 其他
        /// </summary>
        Header = 3

    }

    /// <summary>
    /// 消息响应类型
    /// </summary>
    public enum AckType
    {
        /// <summary>
        /// 处理成功并删除
        /// </summary>
        SuccessAndDelete = 0,

        /// <summary>
        /// 处理失败但删除
        /// </summary>
        FailButDelete = 1,

        /// <summary>
        /// 处理失败但重新入队
        /// </summary>
        FailButRequeue = 2,

        /// <summary>
        /// 处理失败、重新入队并分给新的消费者
        /// </summary>
        FailButRequeueAndNewConsumer = 3,

        /// <summary>
        /// 处理失败、重新入队并分给原来的消费者
        /// </summary>
        FailButRequeueAndSameConsumer = 4,
    }
}
