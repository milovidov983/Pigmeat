using System;
using System.IO;
using Newtonsoft.Json;
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
                Help(args);
            }
            else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
            {
                New();
            }
            else if (args[0].Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                New();
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
                Help(args);
            }
            else if (args[0].Equals("h", StringComparison.OrdinalIgnoreCase))
            {
                Help(args);
            }
            else
            {
                Help(args);
            }
        }
        static void Build()
        {
            JObject Global = JObject.Parse(IO.GetGlobal());
            string[] IncludedFiles = JsonConvert.DeserializeObject<string[]>(Global["include"].ToString());
            string[] IncludedFilesHTML = JsonConvert.DeserializeObject<string[]>(Global["include-html"].ToString());
            string[] IncludedFilesRaw = JsonConvert.DeserializeObject<string[]>(Global["include-raw"].ToString());
            for(int i = 0; i < 2; i++)
            {
                foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
                {
                    foreach(var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                    {
                        if(Path.GetExtension(file).Equals(".md") || Path.GetExtension(file).Equals(".html"))
                        {
                            JObject PageObject = Page.GetPageObject(file, true);
                            IO.RenderPage(PageObject, directory.Substring(3), file, true);
                            if(i == 1)
                            {
                                Console.WriteLine(file + " → " + "./output/" + PageObject["url"].ToString());
                            }
                        }
                        else if(Path.GetExtension(file).Equals(".scss") || Path.GetExtension(file).Equals(".sass"))
                        {
                            Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                            File.WriteAllText("./output/" + Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".css", Scss.ConvertToCss(File.ReadAllText(file)).Css);
                            if(i == 1)
                            {
                                Console.WriteLine(file + " → " + "./output/" + Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".css");
                            }
                        }
                        else if(!Path.GetExtension(file).Equals(".json") && !Path.GetExtension(file).Equals(".yml"))
                        {
                            Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                            File.Copy(file, "./output/" + file, true);
                            if(i == 1)
                            {
                                Console.WriteLine(file + " → " + "./output/" + file);
                            }
                        }
                    }
                }
                try
                {
                    foreach(var file in IncludedFiles)
                    {
                        Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                        File.Copy(file, "./output/" + file, true);
                        if(i == 1)
                        {
                            Console.WriteLine(file + " → " + "./output/" + file);
                        }
                    }
                }
                catch(Exception e)
                {
                    if(i == 1)
                    {
                        Console.WriteLine(e);
                    }
                }
                try
                {
                    foreach(var file in IncludedFilesRaw)
                    {
                        JObject PageObject = Page.GetPageObject(file, false);
                        IO.RenderPage(PageObject, "", file, false);
                        if(i == 1)
                        {
                            Console.WriteLine(file + " → " + "./output/" + PageObject["url"].ToString());
                        }
                    }
                }
                catch(Exception e)
                {
                    if(i == 1)
                    {
                        Console.WriteLine(e);
                    }
                }
                try
                {
                    foreach(var file in IncludedFilesHTML)
                    {
                        JObject PageObject = Page.GetPageObject(file, true);
                        IO.RenderPage(PageObject, "", file, true);
                        if(i == 1)
                        {
                            Console.WriteLine(file + " → " + "./output/" + PageObject["url"].ToString());
                        }
                    }
                }
                catch(Exception e)
                {
                    if(i == 1)
                    {
                        Console.WriteLine(e);
                    }
                }
                IO.CleanCollections();
            }
        }

        static void New()
        {
            Directory.CreateDirectory("./_posts");
            Directory.CreateDirectory("./_pages");
            Directory.CreateDirectory("./_files");
            Directory.CreateDirectory("./drafts");
            Directory.CreateDirectory("./layouts");
            Directory.CreateDirectory("./includes");
            Directory.CreateDirectory("./sass");

            File.WriteAllText("./_posts/collection.json", "{\n\t\"name\": \"posts\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_pages/collection.json", "{\n\t\"name\": \"pages\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_files/collection.json", "{\n\t\"name\": \"files\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_posts/README", "This is where your posts should go.");
            File.WriteAllText("./_pages/README", "This is where your pages should go.");
            File.WriteAllText("./_files/README", "This is where your loose files (data, media, stylesheets, etc.) should go.");
            File.WriteAllText("./_global.yml", "title: Pigmeat Project\nculture: \"en-US\"\ninclude:\ninclude-raw:\ninclude-html:");
            File.WriteAllText("./drafts/README", "This is where your Markdown and HTML documents should go if you don't want them to be published.");
            File.WriteAllText("./layouts/README", "This is where your HTML page templates go.");
            File.WriteAllText("./includes/README", "This is where your HTML snippets go.");
            File.WriteAllText("./sass/README", "This is where your Sass stylesheet dependencies go.");
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
                Console.WriteLine("Nothing to clean … ");
            }
        }

        static void Help(string[] args)
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
