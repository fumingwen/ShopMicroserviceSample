using Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// Restful风格接口通用访问类
    /// </summary>
    public class RestClient
    {
        public string Token { get; set; }

        public Dictionary<string,object> Headers { get; set; }



        public RestClient(string token = null)
        {
            this.Token = token;
        }

        public RestClient SetToken(string token = null)
        {
            this.Token = token;
            return this;
        }

        public RestClient SetHeader(Dictionary<string, object> headers)
        {
            this.Headers = headers;
            return this;
        }

        /// <summary>
        /// 访问接口并返回OperateResult类型对象及数据
        /// </summary>
        /// <typeparam name="T1">OperateResult类型对象</typeparam>
        /// <typeparam name="T2">数据</typeparam>
        /// <param name="url">接口地址</param>
        /// <param name="type">访问类型，HttpType</param>
        /// <param name="param">参数，对象形式自动转换，或json字符串</param>
        /// <param name="replaceList">替换数组</param>
        /// <param name="standardResult">是否返回标准的OperateResult，默认是，false则会自动组装</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>返回OperateResult类型对象及数据</returns>
        public T1 HttpRequest<T1, T2>(string url, HttpType type = HttpType.Get, object param = null, IDictionary<int, List<string>> replaceList = null, bool standardResult = true, int timeOut = 300000) where T1 : OperateResult<T2>, new()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            Stream outStream = null;

            try
            {
                //构造http请求的对象
                request = (HttpWebRequest)WebRequest.Create(url);

                //设置
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = type.ToString().Replace("Form","Post");
                request.KeepAlive = false;
                request.Timeout = timeOut;

                var data = "";
                if (param != null)
                {
                    var name = param.GetType().Name;
                    if (!("String,Int32,Int64").Contains(name))
                    {
                        if (type.Equals(HttpType.Form))
                        {
                            data = ObjectUtil.ToFormUrlEncoded(param);
                        }
                        else
                        {
                            data = JsonUtil.ToJson(param);
                        }
                    }
                    else
                    {
                        data = param.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(this.Token))
                {
                    request.Headers["Authorization"] = string.Format("Bearer {0}", this.Token);
                }

                if (Headers!=null && Headers.Count>0)
                {
                    foreach (var h in this.Headers)
                    {
                        if (h.Value != null)
                        {
                            request.Headers[h.Key] = h.Value.ToString();
                        }
                    }
                }

                if (data.Trim() != "")
                {
                    if (type.Equals(HttpType.Form))
                    {
                        request.ContentType = @"application/x-www-form-urlencoded";
                    }
                    else
                    {
                        request.ContentType = @"application/json";
                    }

                    request.MediaType = "application/json";
                    request.Accept = "application/json";

                    request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
                    request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";

                    //转成网络流
                    byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
                    request.ContentLength = buf.Length;
                    outStream = request.GetRequestStream();
                    outStream.Flush();
                    outStream.Write(buf, 0, buf.Length);
                    outStream.Flush();
                    outStream.Close();
                }
                else if (type.Equals(HttpType.Post) || type.Equals(HttpType.Form) || type.Equals(HttpType.Put))
                {
                    request.ContentLength = 0;
                }

                // 获得接口返回值
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();
                reader.Close();
                response.Close();
                request.Abort();

                if (replaceList != null && replaceList.Count == 2)
                {
                    for (var i = 0; i < replaceList[0].Count; i++)
                    {
                        content = content.Replace(replaceList[0][i], replaceList[1][i]);
                    }
                }

                if (!standardResult)
                {
                    content = string.Format("{{\"Data\":{0}, \"OperateStatus\":1}}", content);
                }

                var result = JsonUtil.FromJson<T1>(content);
                return result;
            }
            catch (Exception ex)
            {
                if (outStream != null) outStream.Close();
                if (reader != null) reader.Close();
                if (response != null) response.Close();
                if (request != null) request.Abort();

                string error = ex.Message;
                var result = new T1() { OperateStatus = Common.Enums.OperateStatus.Failed, Message = error };
                return result;
            }
        }
    }
}
