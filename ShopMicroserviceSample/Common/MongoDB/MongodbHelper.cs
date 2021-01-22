using Common.Helper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.MongoDB
{
    /// <summary>
        /// MongoDb操作帮助类
        /// </summary>
    public class MongoDbHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly string connectionString = null;
        /// <summary>
        /// 数据库名称
        /// </summary>
        private readonly string databaseName = null;
        /// <summary>
        /// 数据库实例
        /// </summary>
        private IMongoDatabase database = null;
        /// <summary>
        /// 自动创建数据库
        /// </summary>
        private readonly bool autoCreateDb = false;
        /// <summary>
        /// 自动创建文档集
        /// </summary>
        private readonly bool autoCreateCollection = true;
        /// <summary>
        /// 
        /// </summary>
        static MongoDbHelper()
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

        }
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="mongoConnStr"></param>
        /// <param name="dbName"></param>
        /// <param name="autoCreateDb"></param>
        /// <param name="autoCreateCollection"></param>
        public MongoDbHelper(string dbName, bool autoCreateDb = false, bool autoCreateCollection = true)
        {
            this.connectionString = ConfigurtaionHelper.Configuration["MongoDB"];
            this.databaseName = dbName;
            this.autoCreateDb = autoCreateDb;
            this.autoCreateCollection = autoCreateCollection;
        }

        /// <summary>
        /// 创建连接--指定数据库连接
        /// </summary>
        /// <param name="mongoConnStr"></param>
        /// <param name="dbName"></param>
        /// <param name="autoCreateDb"></param>
        /// <param name="autoCreateCollection"></param>
        public MongoDbHelper(string dbName, string dbConnectionString, bool autoCreateDb = false, bool autoCreateCollection = true)
        {
            this.connectionString = dbConnectionString;
            this.databaseName = dbName;
            this.autoCreateDb = autoCreateDb;
            this.autoCreateCollection = autoCreateCollection;
        }
        #region 私有方法
        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <returns></returns>
        private MongoClient CreateMongoClient()
        {
            return new MongoClient(connectionString);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        private IMongoDatabase GetMongoDatabase()
        {
            if (database == null)
            {
                var client = CreateMongoClient();
                if (!DatabaseExists(client, databaseName) && !autoCreateDb)
                {
                    throw new KeyNotFoundException("此MongoDB名称不存在：" + databaseName);
                }

                database = CreateMongoClient().GetDatabase(databaseName);
            }

            return database;
        }
        /// <summary>
        /// 检查数据库是否已存在
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private bool DatabaseExists(MongoClient client, string dbName)
        {
            try
            {
                var dbNames = client.ListDatabases().ToList().Select(db => db.GetValue("name").AsString);
                return dbNames.Contains(dbName);
            }
            catch //如果连接的账号不能枚举出所有DB会报错，则默认为true
            {
                return true;
            }

        }
        /// <summary>
        /// 检查文档集合是否已存在
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        private bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var options = new ListCollectionsOptions
            {
                Filter = Builders<BsonDocument>.Filter.Eq("name", collectionName)
            };

            return database.ListCollections(options).ToEnumerable().Any();
        }

        /// <summary>
        /// 获取文档集合
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private IMongoCollection<TDoc> GetMongoCollection<TDoc>(string name, MongoCollectionSettings settings = null)
        {
            var mongoDatabase = GetMongoDatabase();

            if (!CollectionExists(mongoDatabase, name) && !autoCreateCollection)
            {
                throw new KeyNotFoundException("此Collection名称不存在：" + name);
            }

            return mongoDatabase.GetCollection<TDoc>(name, settings);
        }
        /// <summary>
        /// 创建更新命令
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="doc"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<UpdateDefinition<TDoc>> BuildUpdateDefinition<TDoc>(object doc, string parent)
        {
            var updateList = new List<UpdateDefinition<TDoc>>();
            foreach (var property in typeof(TDoc).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var key = parent == null ? property.Name : $"{parent}.{property.Name}";
                //非空的复杂类型
                if ((property.PropertyType.IsClass || property.PropertyType.IsInterface) && property.PropertyType != typeof(string) && property.GetValue(doc) != null)
                {
                    if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        #region 集合类型
                        int i = 0;
                        var subObj = property.GetValue(doc);
                        foreach (var item in subObj as IList)
                        {
                            if (item.GetType().IsClass || item.GetType().IsInterface)
                            {
                                updateList.AddRange(BuildUpdateDefinition<TDoc>(doc, $"{key}.{i}"));
                            }
                            else
                            {
                                updateList.Add(Builders<TDoc>.Update.Set($"{key}.{i}", item));
                            }
                            i++;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 实体类型
                        //复杂类型，导航属性，类对象和集合对象 
                        var subObj = property.GetValue(doc);
                        foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                        {
                            updateList.Add(Builders<TDoc>.Update.Set($"{key}.{sub.Name}", sub.GetValue(subObj)));
                        }
                        #endregion
                    }
                }
                else //简单类型
                {
                    updateList.Add(Builders<TDoc>.Update.Set(key, property.GetValue(doc)));
                }
            }

            return updateList;
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="col"></param>
        /// <param name="indexFields"></param>
        /// <param name="options"></param>
        private async void CreateIndex<TDoc>(IMongoCollection<TDoc> col, string[] indexFields, CreateIndexOptions options = null)
        {
            if (indexFields == null)
            {
                return;
            }
            var indexKeys = Builders<TDoc>.IndexKeys;
            IndexKeysDefinition<TDoc> keys = null;
            if (indexFields.Length > 0)
            {
                keys = indexKeys.Descending(indexFields[0]);
            }
            for (var i = 1; i < indexFields.Length; i++)
            {
                var strIndex = indexFields[i];
                keys = keys.Descending(strIndex);
            }

            if (keys != null)
            {
                await col.Indexes.CreateOneAsync(keys, options);

            }

        }

        #endregion
        /// <summary>
        /// 创建文档集
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="indexFields"></param>
        /// <param name="options"></param>
        public void CreateCollectionIndex<TDoc>(string collectionName, string[] indexFields, CreateIndexOptions options = null)
        {
            CreateIndex(GetMongoCollection<TDoc>(collectionName), indexFields, options);
        }
        /// <summary>
        /// 建立文档集合
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="indexFields"></param>
        /// <param name="options"></param>
        public async void CreateCollection<TDoc>(string[] indexFields = null, CreateIndexOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            CreateCollection<TDoc>(collectionName, indexFields, options);
        }
        /// <summary>
        /// 创建文档集合
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="indexFields"></param>
        /// <param name="options"></param>
        public async void CreateCollection<TDoc>(string collectionName, string[] indexFields = null, CreateIndexOptions options = null)
        {
            var mongoDatabase = GetMongoDatabase();
            await mongoDatabase.CreateCollectionAsync(collectionName);
            CreateIndex(GetMongoCollection<TDoc>(collectionName), indexFields, options);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TDoc>> Find<TDoc>(Expression<Func<TDoc, bool>> filter, FindOptions<TDoc, TDoc> options = null)
        {
            string collectionName = typeof(TDoc).Name;
            return await Find<TDoc>(collectionName, filter, options);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TDoc>> Find<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, FindOptions<TDoc, TDoc> options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            var data = await colleciton.FindAsync<TDoc>(filter, options);
            return data.ToList();

        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filter"></param>
        /// <param name="keySelector"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rsCount"></param>
        /// <returns></returns>

        public List<TDoc> FindByPage<TDoc, TResult>(Expression<Func<TDoc, bool>> filter, Expression<Func<TDoc, TResult>> keySelector, int pageIndex, int pageSize, out int rsCount)
        {
            string collectionName = typeof(TDoc).Name;
            return FindByPage<TDoc, TResult>(collectionName, filter, keySelector, pageIndex, pageSize, out rsCount);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="keySelector"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rsCount"></param>
        /// <returns></returns>
        public List<TDoc> FindByPage<TDoc, TResult>(string collectionName, Expression<Func<TDoc, bool>> filter, Expression<Func<TDoc, TResult>> keySelector, int pageIndex, int pageSize, out int rsCount)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            rsCount = colleciton.AsQueryable().Where(filter).Count();

            int pageCount = rsCount / pageSize + ((rsCount % pageSize) > 0 ? 1 : 0);
            if (pageIndex > pageCount) pageIndex = pageCount;
            if (pageIndex <= 0) pageIndex = 1;

            return colleciton.AsQueryable(new AggregateOptions { AllowDiskUse = true }).Where(filter).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="doc"></param>
        /// <param name="options"></param>
        public async Task<bool> Insert<TDoc>(string collectionName, TDoc doc, InsertOneOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            await colleciton.InsertOneAsync(doc, options);
            return true;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="docs"></param>
        /// <param name="options"></param>
        public void InsertMany<TDoc>(IEnumerable<TDoc> docs, InsertManyOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            InsertMany<TDoc>(collectionName, docs, options);
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="docs"></param>
        /// <param name="options"></param>
        public void InsertMany<TDoc>(string collectionName, IEnumerable<TDoc> docs, InsertManyOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            colleciton.InsertMany(docs, options);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        public void Update<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
            colleciton.UpdateOne(filter, Builders<TDoc>.Update.Combine(updateList), options);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        public async Task<bool> Update<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
            await colleciton.UpdateOneAsync(filter, Builders<TDoc>.Update.Combine(updateList), options);
            return true;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="updateFields"></param>
        /// <param name="options"></param>

        public void Update<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> updateFields, UpdateOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            Update<TDoc>(collectionName, doc, filter, updateFields, options);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="updateFields"></param>
        /// <param name="options"></param>
        public void Update<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> updateFields, UpdateOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            colleciton.UpdateOne(filter, updateFields, options);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        public void UpdateMany<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            UpdateMany<TDoc>(collectionName, doc, filter, options);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        public void UpdateMany<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
            colleciton.UpdateMany(filter, Builders<TDoc>.Update.Combine(updateList), options);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="filter"></param>
        /// <param name="options"></param>

        public async void Delete<TDoc>(Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            await Delete<TDoc>(collectionName, filter, options);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        public async Task<bool> Delete<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            await colleciton.DeleteOneAsync(filter, options);
            return true;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="filter"></param>
        /// <param name="options"></param>

        public void DeleteMany<TDoc>(Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
        {
            string collectionName = typeof(TDoc).Name;
            DeleteMany<TDoc>(collectionName, filter, options);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>

        public void DeleteMany<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            colleciton.DeleteMany(filter, options);
        }
        /// <summary>
        /// 清空文档集
        /// </summary>
        /// <typeparam name="TDoc"></typeparam>
        /// <param name="collectionName"></param>
        public void ClearCollection<TDoc>(string collectionName)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            var inddexs = colleciton.Indexes.List();
            List<IEnumerable<BsonDocument>> docIndexs = new List<IEnumerable<BsonDocument>>();
            while (inddexs.MoveNext())
            {
                docIndexs.Add(inddexs.Current);
            }
            var mongoDatabase = GetMongoDatabase();
            mongoDatabase.DropCollection(collectionName);

            if (!CollectionExists(mongoDatabase, collectionName))
            {
                CreateCollection<TDoc>(collectionName);
            }

            if (docIndexs.Count > 0)
            {
                colleciton = mongoDatabase.GetCollection<TDoc>(collectionName);
                foreach (var index in docIndexs)
                {
                    foreach (IndexKeysDefinition<TDoc> indexItem in index)
                    {
                        try
                        {
                            colleciton.Indexes.CreateOne(indexItem);
                        }
                        catch
                        { }
                    }
                }
            }

        }



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<string> UpLoadFile(MongoDbFileDoc doc, GridFSUploadOptions options = null)
        {
            var bucket = new GridFSBucket(GetMongoDatabase(), new GridFSBucketOptions());
            var id = await bucket.UploadFromBytesAsync(doc.FileName, doc.FileBytes, options);
            return id.ToString();

        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MongoDbFileDoc> DownLoadFile(string id)
        {
            var model = new MongoDbFileDoc();
            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, new ObjectId(id));

            var bucket = new GridFSBucket(GetMongoDatabase(), new GridFSBucketOptions());
            var fileInfo = await bucket.FindAsync(filter);
            var file = await fileInfo.FirstOrDefaultAsync();
            if (file != null)
            {
                model.FileName = file.Filename;
                model.FileId = id;
                model.FileBytes = await bucket.DownloadAsBytesAsync(new ObjectId(id));

            }

            return model;

        }
        public Task<IAsyncCursor<TDoc>> FindAsync<TDoc>(string collectionName, FilterDefinition<TDoc> filter, FindOptions<TDoc, TDoc> options = null)
        {
            var colleciton = GetMongoCollection<TDoc>(collectionName);
            return colleciton.FindAsync<TDoc>(filter, options);
        }

    }
}
