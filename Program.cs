using System;

namespace WDHAN
{
    class Program
    {
        /*
         * Commands:
         *  wdhan new - Creates an empty WDHAN project in the current directory.
         *  wdhan new <string> - Creates an empty WDHAN project at the specified directory.
         *  wdhan build - Outputs a publishable WDHAN project to the /_site directory.
         *  wdhan b - Same as above.\n" +
         *  wdhan serve - Rebuilds the site anytime a change is detected.
         *  wdhan s - Same as above.\n" +
         *  wdhan clean - Deletes all generated files.
         *  wdhan help - Prints a message outlining these commands.
         *  wdhan help <string> - Displays a message outlining the usage of a given parameter (e.g. 'wdhan help serve')
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
            if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
            {

            }
            else if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
            {

            }
            else if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                printHelpMsg(args);
            }
        }
        static void printHelpMsg(string[] args)
        {
            try
            {
                if (args[1].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Creates an empty WDHAN project in the current directory. A path can be specified after to create a project at a given directory (e.g. 'wdhan new \"C:/Path/to/website\"')");
                }
                else if (args[1].Equals("build", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable WDHAN project to the /_site directory.");
                }
                else if (args[1].Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable WDHAN project to the /_site directory.");
                }
                else if (args[1].Equals("serve", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Rebuilds the site anytime a change is detected.");
                }
                else if (args[1].Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Rebuilds the site anytime a change is detected.");
                }
                else if (args[1].Equals("clean", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deletes all generated files.");
                }
                else if (args[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Prints a message outlining WDHAN's commands. A subparameter may be specified, displaying a message outlining the usage of the given parameter (e.g. 'wdhan help serve')");
                }
                else
                {
                    Console.WriteLine("Please specify a parameter (e.g. 'wdhan help new,' 'wdhan help build,' 'wdhan help serve,' 'wdhan help clean')");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(
                    args[0] + " is not a recognized argument.\n" +
                    "WDHAN supports the following commands:\n" +
                    "   wdhan new - Creates an empty WDHAN project in the current directory.\n" +
                    "   wdhan new <string> - Creates an empty WDHAN project at the specified directory.\n" +
                    "   wdhan build - Outputs a publishable WDHAN project to the /_site directory.\n" +
                    "   wdhan b - Same as above.\n" +
                    "   wdhan serve - Rebuilds the site anytime a change is detected.\n" +
                    "   wdhan s - Same as above.\n" +
                    "   wdhan clean - Deletes all generated files.\n" +
                    "   wdhan help - Shows this message.\n" +
                    "   wdhan help <string> - Displays a message outlining the usage of a given parameter (e.g. 'wdhan help serve')"
                    );
            }
        }
    }
}
