using System.IO;

namespace Pigmeat
{
    public class HTMLFile : PigmeatFile
    {
        public HTMLFile()
        {
            modified_time = System.IO.File.GetLastWriteTimeUtc("./" + path);
            basename = Path.GetFileNameWithoutExtension("./" + path);
            extname = ".html";
        }
    }
}
