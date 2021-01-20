using Newtonsoft.Json;

namespace Common.Tools
{
    public static class JsonUtil
    {
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJson<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /*
        public static string ToJson<T>(T t)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, t);

            string json = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return json;
        }

        public static T FromJson<T>(string json) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return serializer.ReadObject(stream) as T;
        }
        */
    }
}
