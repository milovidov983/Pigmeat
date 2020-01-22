using Markdig;
using Fluid;
using System;
using System.IO;
using SharpScss;
using SharpYaml;
using SharpYaml.Serialization;
//using ShellProgressBar;
using System.Text;
using System.Collections.Generic;

namespace WDHAN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    createSite(args);
                }
                else if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
                {
                    buildSite(args);
                }
                else if (args[0].Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    buildSite(args);
                }
                else if (args[0].Equals("serve", StringComparison.OrdinalIgnoreCase))
                {

                }
                else if (args[0].Equals("s", StringComparison.OrdinalIgnoreCase))
                {

                }
                else if (args[0].Equals("clean", StringComparison.OrdinalIgnoreCase))
                {
                    cleanSite(args);
                }
                else if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    printHelpMsg(args);
                }
                else
                {
                    printHelpMsg(args);
                }
            }
            catch (IndexOutOfRangeException)
            {
                printHelpMsg(args);
            }
        }
        static void cleanSite(string[] args){
            try
            {
                Console.WriteLine("Cleaning project directory, " + args[1] + "/_site" + " ... ");
                System.IO.DirectoryInfo di = new DirectoryInfo(args[1] + "/_site");
                //String fileName = "";
                /*
                var options = new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true
                };
                using (var progressBar = new ProgressBar(Directory.GetFiles(args[1] + "/_site", "*.*", SearchOption.AllDirectories).Length, "Deleting " + fileName, options))
                {
                    */
                    foreach (var file in Directory.GetFiles(args[1] + "/_site", "*.*", SearchOption.AllDirectories))
                    {
                        //fileName = file.Remove(args[1].Length + "/_site".Length);
                        Console.WriteLine("Deleting " + file.Substring(args[1].Length + "/_site".Length + 1));
                        File.Delete(file); 
                        //progressBar.Tick();
                    }
                //}
                
                
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    Console.WriteLine("Deleting " + dir.Name);
                    dir.Delete(true); 
                }
                

            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine("Cleaning project directory, " + "./_site" + " ... ");
                System.IO.DirectoryInfo di = new DirectoryInfo("./_site");
                //String fileName = "";
                /*
                var options = new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = true
                };
                using (var progressBar = new ProgressBar(Directory.GetFiles("./_site", "*.*", SearchOption.AllDirectories).Length, "Deleting " + fileName, options))
                {
                    */
                    foreach (var file in Directory.GetFiles("./_site", "*.*", SearchOption.AllDirectories))
                    {
                        //fileName = file.Remove("/_site".Length);
                        Console.WriteLine("Deleting " + file.Substring("/_site".Length + 1));
                        File.Delete(file); 
                        //progressBar.Tick();
                    }
                //}
                
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    Console.WriteLine("Deleting " + dir.Name);
                    dir.Delete(true); 
                }
                
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("ERROR: The project directory you're trying to clean cannot be found.");
                Environment.Exit(1);
            }
            catch(System.Security.SecurityException)
            {
                Console.WriteLine("ERROR: You do not have write access to the project directory you're trying to clean.");
                Environment.Exit(1);
            }
            catch(ArgumentException)
            {
                Console.WriteLine("ERROR: The project directory you're trying to clean cannot be found.");
                Environment.Exit(1);
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("ERROR: You do not have write access to the project directory you're trying to clean.");
                Environment.Exit(1);
            }
            catch(IOException)
            {
                Console.WriteLine("ERROR: An I/O error has occured. Ensure the project directory is not read-only, and no other programs are using files in the directory.");
                Environment.Exit(1);
            }
            catch(Exception)
            {
                Console.WriteLine("ERROR: Cannot clean project files. Please ensure the directory you're trying to clean exists and can be accessed.");
                Environment.Exit(1);
            }
        }
        static void createSite(string[] args)
        {
            try 
            {
                // Create blank site scaffolding
                if (args[1].Equals("--blank", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        try
                        {
                            Console.WriteLine("Creating /_plugins");
                            Directory.CreateDirectory(args[2] + "./_plugins");
                            Console.WriteLine("Creating /_includes");
                            Directory.CreateDirectory(args[2] + "./_includes");
                            Console.WriteLine("Creating /_layouts");
                            Directory.CreateDirectory(args[2] + "./_layouts");
                            Console.WriteLine("Creating /_sass");
                            Directory.CreateDirectory(args[2] + "./_sass");
                            Console.WriteLine("Creating _/posts");
                            Directory.CreateDirectory(args[2] + "./_posts");
                            Console.WriteLine("Creating /_drafts");
                            Directory.CreateDirectory(args[2] + "./_drafts");
                            Console.WriteLine("Creating /_data");
                            Directory.CreateDirectory(args[2] + "./_data");
                            Console.WriteLine("Creating _config.yml");
                            var yamlSerializer = new Serializer();
                            var defaultConfig = yamlSerializer.Serialize(new { source = '.', destination = "./_site", collections_dir = '.', plugins_dir = "_plugins", layouts_dir = "_layouts", data_dir = "_data", includes_dir = "_includes", collections = new List<Collection>() { new Collection("posts", true), new Collection("drafts", false) }, safe = false, include = new string[] { ".htaccess" }, exclude = new string[] { }, keep_files = new string[] { ".git", ".svn" }, encoding = "utf-8", markdown_ext = "markdown,mkdown,mkdn,mkd,md", strict_front_matter = false, show_drafts = false, limit_posts = 0, future = false, unpublished = false, whitelist = new string[] { }, plugins = new string[] { }, lsi = false, excerpt_seperator = @"\n\n", incremental = false, detach = false, port = 4000, host = "127.0.0.1", baseurl = ' ', show_dir_listing = false, permalink = "date", paginate_path = "/page:num", timezone = "null", quiet = false, verbose = false, defaults = new string[] { } });
                            using (FileStream fs = File.Create(args[2] + "./_config.yml"))
                            {
                                fs.Write(Encoding.UTF8.GetBytes(defaultConfig), 0, Encoding.UTF8.GetBytes(defaultConfig).Length);
                            }

                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Creating /_plugins");
                            Directory.CreateDirectory("./_plugins");
                            Console.WriteLine("Creating /_includes");
                            Directory.CreateDirectory("./_includes");
                            Console.WriteLine("Creating /_layouts");
                            Directory.CreateDirectory("./_layouts");
                            Console.WriteLine("Creating /_sass");
                            Directory.CreateDirectory("./_sass");
                            Console.WriteLine("Creating _/posts");
                            Directory.CreateDirectory("./_posts");
                            Console.WriteLine("Creating /_drafts");
                            Directory.CreateDirectory("./_drafts");
                            Console.WriteLine("Creating /_data");
                            Directory.CreateDirectory("./_data");
                            Console.WriteLine("Creating _config.yml");
                            var yamlSerializer = new Serializer();
                            var defaultConfig = yamlSerializer.Serialize(new { source = '.', destination = "./_site", collections_dir = '.', plugins_dir = "_plugins", layouts_dir = "_layouts", data_dir = "_data", includes_dir = "_includes", collections = new List<Collection>() { new Collection("posts", true), new Collection("drafts", false) }, safe = false, include = new string[] { ".htaccess" }, exclude = new string[] { }, keep_files = new string[] { ".git", ".svn" }, encoding = "utf-8", markdown_ext = "markdown,mkdown,mkdn,mkd,md", strict_front_matter = false, show_drafts = false, limit_posts = 0, future = false, unpublished = false, whitelist = new string[] { }, plugins = new string[] { }, lsi = false, excerpt_seperator = @"\n\n", incremental = false, detach = false, port = 4000, host = "127.0.0.1", baseurl = ' ', show_dir_listing = false, permalink = "date", paginate_path = "/page:num", timezone = "null", quiet = false, verbose = false, defaults = new string[] { } });
                            using (FileStream fs = File.Create("./_config.yml"))
                            {
                                fs.Write(Encoding.UTF8.GetBytes(defaultConfig), 0, Encoding.UTF8.GetBytes(defaultConfig).Length);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("ERROR: Access to WDHAN files is denied. Try changing file permissions, or run with higher privileges.");
                        Environment.Exit(1);
                    }
                    catch (PathTooLongException)
                    {
                        Console.WriteLine("ERROR: The path to your WDHAN project is too long for your file system to handle.");
                        Environment.Exit(1);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Console.WriteLine("ERROR: The path to your WDHAN project is inaccessible. Verify it still exists.");
                        Environment.Exit(1);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("ERROR: A problem has occured with writing data to your system. Verify your OS and data storage device are working correctly.");
                        Environment.Exit(1);
                    }
                    catch (NotSupportedException)
                    {
                        Console.WriteLine("ERROR: WDHAN cannot create your project's output directory. Verify your OS and data storage device are working correctly, and you have proper permissions.");
                        Environment.Exit(1);
                    }

                }
            }
            catch (IndexOutOfRangeException)
            {
                // Create site with default theme
            }
            catch(Exception){
                Console.WriteLine("ERROR: Cannot create project files. Please ensure the directory you're trying to create is supported in your file system.");
                Environment.Exit(1);
            }
        }
        static void getSiteVars(string[] args)
        {

        }

        /*
        OK, how are we going to tackle this? We have a Liquid parser, but it's on us to find those variables to pass to the parser.
        Hmm ... what variables do we actually have to work with?
        Well, posts can have post.variable, site.variable, site.data.variable
        Includes can have site.variable, site.data.variable, and include.variable
        We don't know what include.variable is, UNTIL we get that value from the post, so ... there's an order to the parsing:
        1. site
        2. site.data
        3. post (from collections, find path, parse)
        4. include (get references include from /_includes/<string>.html)
        5. layout (since this can reference post data not yet generated)
        6. misc. leftovers (rss.xml for example, readme, etc.)

        How do we get the variables into the parser (Fluid)?
        var model = new { ParamOne = "Hello", ParamTwo = "world." };
        var source = "{{ p.ParamOne }} {{ p.ParamTwo }}";
        if (FluidTemplate.TryParse(source, out var template))
        {   
            var context = new TemplateContext();
            context.MemberAccessStrategy.Register(model.GetType());
            context.SetValue("p", model);

            Console.WriteLine(template.Render(context));
        }
        So what does this mean? It means we have to get our variables and store them in classes or variable sets (models), before
        passing them to template.Render(context), which we can then pass to Markdig to generate HTML (in cunjunction with SharpScss).
        Site variables, including site.data variables, come from YAML data, which we can use SharpYaml to get (though reading back data will be hard).
        */
        static void buildSite(string[] args)
        {
            try
            {
                Directory.CreateDirectory("./_site");
                // Run through every file in generated directories, export to proper directory
                // If file is in root, export to root. If in a collection, export according to rules in config file.

                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build(); // Setup Markdig

            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("ERROR: Access to WDHAN files is denied. Try changing file permissions, or run with higher privileges.");
                Environment.Exit(1);
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("ERROR: The path to your WDHAN project is too long for your file system to handle.");
                Environment.Exit(1);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("ERROR: The path to your WDHAN project is inaccessible. Verify it still exists.");
                Environment.Exit(1);
            }
            catch (IOException)
            {
                Console.WriteLine("ERROR: A problem has occured with writing data to your system. Verify your OS and data storage device are working correctly.");
                Environment.Exit(1);
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("ERROR: WDHAN cannot create your project's output directory. Verify your OS and data storage device are working correctly, and you have proper permissions.");
                Environment.Exit(1);
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
                    Console.WriteLine("Rebuilds the site anytime a change is detected and hosts it.");
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
