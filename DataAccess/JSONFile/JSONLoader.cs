using Newtonsoft.Json;
using System.IO;

namespace UsefulLib.DataAccess.JSONFile
{
    public class JSONLoader<T> : IFileLoader<T> where T : new()
    {
        public T Load(string fileName = "Data.bin")
        {
            if (File.Exists(fileName))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
            else
                return new T();
        }
    }
}