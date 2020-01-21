using System;

namespace WDHAN
{
    class Program
    {
        /*
         * Commands:
         *  wdhan create
         *  wdhan create <string>
         *  wdhan build
         *  wdhan build <string>
         * File structure:
         *  /_includes
         *  /_layouts
         *  /_sass
         * _config.yml:
         *  title
         *  timezone
         *  tagline
         *  baseurl
         *  author
         *  collections:
         *      <string>:
         *          title: <string>
         *          output: <boolean>
         *  markdown
         *  plugins
         *  permalink
         *  paginate
         *  paginate_path (default is page:num)
         *  sass:
         *      style: <string>
         *  exclude: [<string>, <string>, ... ]
        */
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
