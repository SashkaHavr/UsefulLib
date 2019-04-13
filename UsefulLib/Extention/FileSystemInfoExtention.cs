using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulLib.Extention
{
    public static class FileSystemInfoExtention
    {
        static public string ConvertSize(this FileSystemInfo fsi, double size)
        {
            int c = 0;
            while (size > 10000)
            {
                size /= 1024;
                c++;
            }
            string res = Math.Round(size, 2).ToString();
            switch (c)
            {
                case 0:
                    res += " b";
                    break;
                case 1:
                    res += " KB";
                    break;
                case 2:
                    res += " MB";
                    break;
                case 3:
                    res += " GB";
                    break;
            }
            return res;
        }
    }
}
