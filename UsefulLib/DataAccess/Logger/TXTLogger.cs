using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulLib.DataAccess.Logger
{
    public class TXTLogger : IFileSaver<string>
    {
        public void Save(string obj, string fileName = "Data.bin")
        {
            using (var writer = File.AppendText(fileName))
                writer.Write($"{DateTime.Now.ToString()} - {obj}{Environment.NewLine}");

        }
    }
}
