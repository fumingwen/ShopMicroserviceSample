using Common.Helper;
using Common.Log;
using Exceptionless;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class ExceptionlessTest
    {
        [TestMethod]
        public void SaveMethod()
        {
            ExceptionlessClient.Default.Configuration.ApiKey = ConfigurtaionHelper.Configuration["Exceptionless:ApiKey"];
            ExceptionlessClient.Default.Configuration.ServerUrl = ConfigurtaionHelper.Configuration["Exceptionless:ServerUrl"];
              
            #region Exceptionless测试
            try
            {
                ExceptionlessClient.Default.SubmitLog("调试Exceptionless.Logging.LogLevel.Debu", Exceptionless.Logging.LogLevel.Debug);
                ExceptionlessClient.Default.SubmitLog("错误Exceptionless.Logging.LogLevel.Error", Exceptionless.Logging.LogLevel.Error);
                ExceptionlessClient.Default.SubmitLog("大错Exceptionless.Logging.LogLevel.fatal", Exceptionless.Logging.LogLevel.Fatal);
                ExceptionlessClient.Default.SubmitLog(" Exceptionless.Logging.LogLevel.Info", Exceptionless.Logging.LogLevel.Info);
                ExceptionlessClient.Default.SubmitLog(" Exceptionless.Logging.LogLevel.Off", Exceptionless.Logging.LogLevel.Off);
                ExceptionlessClient.Default.SubmitLog(" Exceptionless.Logging.LogLevel.Other", Exceptionless.Logging.LogLevel.Other);
                ExceptionlessClient.Default.SubmitLog(" Exceptionless.Logging.LogLevel.Trace", Exceptionless.Logging.LogLevel.Trace);
                ExceptionlessClient.Default.SubmitLog("Exceptionless.Logging.LogLevel.Warn", Exceptionless.Logging.LogLevel.Warn);


                var data = new Exceptionless.Models.DataDictionary();
                data.Add("data1key", "data1value");
                ExceptionlessClient.Default.SubmitEvent(new Exceptionless.Models.Event { Count = 1, Date = DateTime.Now, Data = data, Geo = "geo", Message = "message", ReferenceId = "referencelId", Source = "source", Tags = new Exceptionless.Models.TagSet() { "tags" }, Type = "type", Value = 1.2m });
                ExceptionlessClient.Default.SubmitFeatureUsage("feature");
                ExceptionlessClient.Default.SubmitNotFound("404 not found");
                ExceptionlessClient.Default.SubmitException(new Exception("自定义异常"));

                throw new DivideByZeroException("throw DivideByZeroException的异常：" + DateTime.Now);
            }
            catch (Exception exc)
            {
                exc.ToExceptionless().Submit();
            }
            #endregion

        }
    }
}
