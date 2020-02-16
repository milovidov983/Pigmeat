using System;
using System.Collections.Generic;
using System.IO;

namespace WDHAN
{
    public class HTMLFile : WDHANFile
    {
        public HTMLFile()
        {
            modified_time = System.IO.File.GetLastWriteTimeUtc("./" + path);
            basename = Path.GetFileNameWithoutExtension("./" + path);
            extname = ".html";
        }
    }
}