using Data;
using InternalNewtonsoft.Json;

namespace Assets.Scripts.Factories.DataFactories.JsonFactories
{
    public class JsonFactory
    {
        private static JsonFactory _instance;
        public static JsonFactory Instance
        {
            get { return _instance ?? (_instance = new JsonFactory()); }
        }

        public T Create<T>( string jsonString_) where T : DataItem, new()
        {
            return JsonConvert.DeserializeObject<T>(jsonString_);
        }

        public string ParseData<T>(T data) where T : DataItem
        {
            return JsonConvert.SerializeObject(data);
        }
    }



}
