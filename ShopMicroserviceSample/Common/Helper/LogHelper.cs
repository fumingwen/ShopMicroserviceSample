using log4net;
using log4net.Config;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Common.Helper
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {
        private static bool configed = false;
        private static readonly ConcurrentDictionary<string, ILog> logs = new ConcurrentDictionary<string, ILog>();

        private static Task<ILog> GetLogger(StackFrame[] frames, string logType, out string message)
        {
            return GetLogger(frames, logType, out message, true);
        }
        private static Task<ILog> GetLogger(StackFrame[] frames, string logType, out string message, bool stackTrace)
        {
            if (!configed)
            {
                var fileName = string.Format("{0}Config\\log4.config", AppDomain.CurrentDomain.BaseDirectory);
                XmlConfigurator.ConfigureAndWatch(new FileInfo(fileName));
            }
            else
            {
                var fileName = string.Format("{0}Config\\log4.config", AppDomain.CurrentDomain.BaseDirectory);
                XmlConfigurator.ConfigureAndWatch(new FileInfo(fileName));
                configed = true;
            }
            if (stackTrace && frames != null && frames.Length > 1)
            {
                var className = "";
                var methodName = "";
                if (frames != null && frames.Length > 1)
                {
                    var frame = frames[1];
                    var method = frame.GetMethod();
                    if (method != null)
                    {
                        methodName = method.Name;
                        className = method.DeclaringType.FullName;
                    }
                }
                message = $"{className}.{methodName}:";
            }
            else
            {
                message = "";
            }

            var exists = logs.TryGetValue(logType, out ILog log);
            if (exists && log != null) return Task.FromResult(log);

            log = LogManager.GetLogger(logType);
            logs.AddOrUpdate(logType, log, (key, value) => log);
            return Task.FromResult(log);
        }

        /// <summary>
        /// 写入警告类日志
        /// </summary>
        /// <param name="message">警告信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        public static void Warn(string message, bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogWarn", out string info, stackTrace)).Warn(info + message));
        }

        /// <summary>
        /// 写入警告类日志
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        /// <param name="message">警告信息</param>
        public static void Warn(Exception exception, string message = "", bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogWarn", out string info, stackTrace)).Warn(info + message, exception));
        }

        /// <summary>
        /// 写入错误类日志
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        public static void Error(string message, bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogError", out string info, stackTrace )).Error(info + message));
        }

        /// <summary>
        /// 写入错误类日志
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="message">错误信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        public static void Error(Exception exception, string message = "", bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogError", out string info, stackTrace)).Error(info + message, exception));
        }

        /// <summary>
        /// 写入灾难性日志
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        public static void Fatal(string message, bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogFatal", out string info, stackTrace)).Fatal(info + message));
        }

        /// <summary>
        /// 写入灾难性日志
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="message">错误信息</param>
        /// <param name="stackTrace">追溯错误来源</param>
        public static void Fatal(Exception exception, string message = "", bool stackTrace = true)
        {
            var frames = new StackTrace().GetFrames();
            Task.Run(async () => (await GetLogger(frames, "LogFatal", out string info, stackTrace)).Fatal(info + message, exception));
        }
    }
}
