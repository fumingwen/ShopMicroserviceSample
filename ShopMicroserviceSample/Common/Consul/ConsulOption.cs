using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Consul
{
    public class ConsulOption
    {
        public string ServiceName { get; set; }
        public string ServiceIP { get; set; }
        public int ServicePort { get; set; }
        public string ServiceHealthCheck { get; set; }
        public string Address { get; set; } 
    }
}
