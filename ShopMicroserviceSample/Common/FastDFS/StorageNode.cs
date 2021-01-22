using System;
using System.Net;

namespace Common.FastDFS
{
    public class StorageNode
    {
        public string GroupName;

        public IPEndPoint EndPoint;

        public byte StorePathIndex;

        public StorageNode()
        {
        }
    }
}