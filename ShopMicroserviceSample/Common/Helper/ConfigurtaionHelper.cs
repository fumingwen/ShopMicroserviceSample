using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Helper
{
    public class ConfigurtaionHelper
    {
        public static IConfiguration Configuration { get; set; }

        static ConfigurtaionHelper()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .Build();
        }

    }
}
