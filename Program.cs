using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Pigmeat.Core;
using SharpScss;

namespace Pigmeat
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    if (!args[0].Equals("help", StringComparison.OrdinalIgnoreCase) && args.Length > 1)
                    {
                        Directory.SetCurrentDirectory(args[args.Length - 1]);
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("The specified directory does not exist.\n" + e);
                }

                GetCommand(args);
            }
            catch(IndexOutOfRangeException)
            {
                GetCommand(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
        static void GetCommand(string[] args)
        {
            if(args.Length == 0)
            {
                ShowHelp(args);
            }
            else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
            {
                Create();
            }
            else if (args[0].Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                Create();
            }
            else if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
            {
                Build();
            }
            else if (args[0].Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                Build();
            }
            else if (args[0].Equals("clean", StringComparison.OrdinalIgnoreCase))
            {
                Clean();
            }
            else if (args[0].Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                Clean();
            }
            else if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp(args);
            }
            else if (args[0].Equals("h", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp(args);
            }
            else
            {
                ShowHelp(args);
            }
        }
        static void Build()
        {
            for(int i = 0; i < 2; i++)
            {
                foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
                {
                    foreach(var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                    {
                        if(Path.GetExtension(file).Equals(".md") || Path.GetExtension(file).Equals(".html"))
                        {
                            JObject PageObject = Page.GetPageObject(file);
                            IO.RenderPage(PageObject, directory.Substring(3));
                            if(i == 1)
                            {
                                Console.WriteLine(PageObject["dir"].ToString() + "/" + PageObject["name"].ToString() + " → " + "./output/" + PageObject["url"].ToString());
                            }
                        }
                        else if(Path.GetExtension(file).Equals(".scss") || Path.GetExtension(file).Equals(".sass"))
                        {
                            File.WriteAllText("./output/" + file, Scss.ConvertToCss(File.ReadAllText(file)).Css);
                            if(i == 1)
                            {
                                Console.WriteLine(file + " → " + "./output/" + file);
                            }
                        }
                    }
                }
            }
            IO.CleanCollections();
        }
        static void Create()
        {
            Directory.CreateDirectory("./_posts");
            Directory.CreateDirectory("./_pages");
            Directory.CreateDirectory("./drafts");
            Directory.CreateDirectory("./layouts");
            File.WriteAllText("./_posts/collection.json", "{\n\t\"name\": \"posts\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_pages/collection.json", "{\n\t\"name\": \"pages\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_global.yml", "title: Pigmeat Project");
            File.WriteAllText("./drafts/README", "Store Markdown and HTML documents here if you don't want them to be published.");
            File.WriteAllText("./layouts/README", "This is where your HTML page templates go.");
        }
        static void Clean()
        {
            IO.CleanCollections();
            try
            {
                Directory.Delete("./output", true);
                Console.WriteLine("Cleaned project directory … ");
            }
            catch(DirectoryNotFoundException)
            {
                // This is expected if there is no directory to clean.
                Environment.Exit(1);
            }
        }
        static void ShowHelp(string[] args)
        {
            try
            {
                if (args[1].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Creates an empty Pigmeat project. A path may be specified, otherwise a project will be created where Pigmeat is running.");
                }
                else if (args[1].Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Creates an empty Pigmeat project. A path may be specified, otherwise a project will be created where Pigmeat is running.");
                }
                else if (args[1].Equals("build", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable Pigmeat project. A path may be specified, otherwise a project will be built where Pigmeat is running.");
                }
                else if (args[1].Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable Pigmeat project. A path may be specified, otherwise a project will be built where Pigmeat is running.");
                }
                else if (args[1].Equals("clean", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deletes all generated files as a result of building.");
                }
                else if (args[1].Equals("c", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deletes all generated data that results from the build process.");
                }
                else if (args[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Prints a message outlining Pigmeat's commands. A subparameter may be specified, displaying a message outlining the usage of the given parameter (e.g. 'pigmeat help serve').");
                }
                else if (args[1].Equals("h", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Prints a message outlining Pigmeat's commands. A subparameter may be specified, displaying a message outlining the usage of the given parameter (e.g. 'pigmeat help serve').");
                }
                else
                {
                    Console.WriteLine("Please specify a parameter (e.g. 'pigmeat help new,' 'pigmeat help build,' 'pigmeat help serve,' 'pigmeat help clean').");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(
                    "Pigmeat supports the following commands:\n" +
                    "\tpigmeat new - Creates an empty Pigmeat project.\n" +
                    "\tpigmeat build - Outputs a publishable Pigmeat project.\n" +
                    "\tpigmeat b - Outputs a publishable Pigmeat project. Same as above.\n" +
                    "\tpigmeat clean - Deletes all generated data that results from the build process.\n" +
                    "\tpigmeat help - Shows this message.\n" +
                    "\tpigmeat help <string> - Displays a message outlining the usage of a given parameter (e.g. 'pigmeat help serve')."
                    );
            }
        }
    }
}
