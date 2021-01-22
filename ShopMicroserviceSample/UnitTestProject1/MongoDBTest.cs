using Common.Helper;
using Common.MongoDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class MongoDBTest
    { 
        [TestMethod]
        public void SaveMethod()
        {
            //第3个参数为true，当数据库中不存在集合时，会自动创建
            var helper = new MongoDbHelper("EdayingDB", false, true);

            var entity = new TestUser()
            {
                id = 2,
                name = "姓名2"
            };

            //如果保存日志，尽量开启新线程，不要阻塞主线程
            //Task.Run(() => helper.Insert("TestUser", entity));
            helper.Insert("TestUser", entity).Wait();
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        [TestMethod]
        public void GetMethod()
        {
            var helper = new MongoDbHelper("EdayingDB");
            Expression<Func<TestUser, bool>> filter = c => true;
            try
            {
                filter = filter.And(c => c.name.Equals("姓名2"));

                var list = helper.Find("TestUser", filter);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class TestUser
    {
        public int id { set; get; }
        public string name { set; get; }
        public DateTime addtime { set; get; } = DateTime.Now;
    }
}
