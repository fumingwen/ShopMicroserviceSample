using System;

namespace Common.Queue
{
    /// <summary>
    /// 消息队列接口类
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// 事件处理
        /// </summary>
        event EventHandler<QueueEventArgs> QueueHanlder;

        /// <summary>
        /// 发布消息队列
        /// </summary>
        /// <param name="queueName">队列名</param>
        /// <param name="message">队列信息</param>
        /// <param name="setting">设置项</param>
        /// <returns>发布是否成功</returns>
        bool Publish<T>(string queueName, T message, QueueSetting setting = null);

        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="queueName">队列名</param>
        /// <param name="handler">事件处理方法</param>
        /// <param name="setting">设置项</param>
        void BeginReceive(string queueName, EventHandler<QueueEventArgs> handler, QueueSetting setting = null);

        /// <summary>
        /// 应答处理
        /// </summary>
        /// <param name="sender">发起消息的对象</param>
        /// <param name="index">当前消息的序列</param>
        /// <param name="multipleAck">是否批量应答</param>
        /// <param name="ackType">回应类型</param>
        /// <returns>应答是否成功</returns>
        bool AfterReceive(object sender, ulong index, bool multipleAck = false, AckType ackType = AckType.SuccessAndDelete);

        /// <summary>
        /// 结束
        /// </summary>
        void Dispose();
    }
}
