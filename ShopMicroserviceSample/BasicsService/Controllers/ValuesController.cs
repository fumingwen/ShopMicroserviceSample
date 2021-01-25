using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicsService.Controllers
{
    [Authorize("Permission")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        static string _url = "http://127.0.0.1:6800";
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var client = new RestClient(_url); 
            var request = new RestRequest("/salary/api/user", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return new string[] { "BasicsService", $"所在服务器：{Environment.MachineName}  OS:{Environment.OSVersion.VersionString }  时间：{DateTime.Now}  内容：{content}" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
