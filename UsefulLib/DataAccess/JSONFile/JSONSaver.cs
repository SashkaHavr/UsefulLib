using Newtonsoft.Json;
using System.IO;

namespace UsefulLib.DataAccess.JSONFile
{
    public class JSONSaver<T> : IFileSaver<T>
    {
        public void Save(T obj, string fileName = "Data.bin")
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
