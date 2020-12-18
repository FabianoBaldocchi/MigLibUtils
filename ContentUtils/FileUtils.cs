using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ContentUtils
{
    public class FileUtils
    {

        public static Encoding GetFileEncoding(string fileName, Encoding defaultencoding = null)
        {
            if (defaultencoding == null)
                defaultencoding = Encoding.GetEncoding("windows-1252");

            using (var reader = new StreamReader(fileName, defaultencoding, true))
            {
                reader.Peek(); // you need this!
                var encoding = reader.CurrentEncoding;
                return encoding;
            }
        }
    }
}
