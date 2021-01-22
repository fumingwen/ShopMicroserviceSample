using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.MongoDB
{
    /// <summary>
    /// mongoDb文件类型文档
    /// </summary>
    [BsonIgnoreExtraElements]
    public class MongoDbFileDoc
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>

        public string FileName { get; set; }
        /// <summary>
        /// 文件二进制
        /// </summary>

        public byte[] FileBytes { get; set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string DbName { get; set; }

    }
}
