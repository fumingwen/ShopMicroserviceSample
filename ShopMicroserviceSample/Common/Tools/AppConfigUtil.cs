namespace Common.Tools
{
    public static class AppConfigUtil
    {
        public static string AppConfig(string name)
        {
            var config = System.Configuration.ConfigurationManager.AppSettings[name];
            return config;
        }
    }
}
