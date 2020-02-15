using System;
using System.Collections.Generic;
using System.IO;

namespace WDHAN
{
    public class StaticFile
    {
        public string path { get; set; }
        public DateTime modified_time { get; set; }
        public string name { get; set; }
        public string basename { get; set; }
        public string extname { get; set; }
        public StaticFile()
        {
            modified_time = System.IO.File.GetLastWriteTimeUtc("./" + path);
            basename = Path.GetFileNameWithoutExtension("./" + path);
            extname = Path.GetExtension("./" + path);
        }
    }
}