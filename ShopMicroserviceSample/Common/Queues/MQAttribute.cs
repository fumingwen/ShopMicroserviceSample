using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Queues
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MQAttribute : Attribute
    {
        /// <summary>
        /// 显示的名称
        /// </summary>
        public string Name { get; set; }
        public MQAttribute(string queueName)
        {
            var ddd = queueName;
        }
        public void show()
        {
            Console.WriteLine("This Is MQAttribute");
        }
    }
}
