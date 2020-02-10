using Markdig;
using Fluid;
using Newtonsoft.Json;
using System;
using System.IO;
using SharpScss;
using System.Text;
using System.Collections.Generic;
using Fluid.Values;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection;

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
        static void cleanSite(string[] args)
        {
            try
            {
                GlobalConfiguration siteConfig = GlobalConfiguration.getConfiguration();
                Console.WriteLine("Cleaning project directory, " + siteConfig.destination + " ... ");
                System.IO.DirectoryInfo outputDir = new DirectoryInfo(siteConfig.destination);

                foreach (var file in Directory.GetFiles(siteConfig.destination, "*.*", SearchOption.AllDirectories))
                {
                    Console.WriteLine("Deleting " + file.Substring(siteConfig.destination.Length + 1));
                    File.Delete(file); 
                }
                    
                foreach (DirectoryInfo dir in outputDir.EnumerateDirectories())
                {
                    Console.WriteLine("Deleting " + dir.Name);
                    dir.Delete(true); 
                }

                Console.WriteLine("Cleaning temporary files, " + siteConfig.source + "/temp" + " ... ");
                System.IO.DirectoryInfo tempDir = new DirectoryInfo(siteConfig.source + "/temp");

                foreach (var file in Directory.GetFiles(siteConfig.destination, "*.*", SearchOption.AllDirectories))
                {
                    Console.WriteLine("Deleting " + file.Substring(siteConfig.source.Length + "/temp".Length + 1));
                    File.Delete(file); 
                }
                    
                foreach (DirectoryInfo dir in tempDir.EnumerateDirectories())
                {
                    Console.WriteLine("Deleting " + dir.Name);
                    dir.Delete(true); 
                }
            }   
            catch(DirectoryNotFoundException ex)
            {
                // This is expected if either _site or temp are not found.
                /*
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [DirectoryNotFoundException]: The project directory you're trying to clean cannot be found.");
                */
                Environment.Exit(1);
            }
            catch(System.Security.SecurityException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [System.Security.SecurityException]: You do not have write access to the project directory you're trying to clean.");
                Environment.Exit(1);
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [ArgumentException]: The project directory you're trying to clean cannot be found.");
                Environment.Exit(1);
            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [UnauthorizedAccessException]: You do not have write access to the project directory you're trying to clean.");
                Environment.Exit(1);
            }
            catch(IOException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [IOException]: An I/O error has occured. Ensure the project directory is not read-only, and no other programs are using files in the directory.");
                Environment.Exit(1);
            }
            catch(Exception ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [Exception]: Cannot clean project files. Please ensure the directory you're trying to clean exists and can be accessed.");
                Environment.Exit(1);
            }
        }
        static void createSite(string[] args)
        {
            Console.WriteLine("Creating project files ... ");
            try 
            {
                // Create blank site scaffolding
                if (args[1].Equals("--blank", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        /*
                        var defaultCollections = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                        var postCollection = new Dictionary<string, List<Dictionary<string, object>>>();
                        var postsVariables = new Dictionary<string, object>();
                        postsVariables.Add("output", true);
                        postCollection.Add("posts", new List<Dictionary<string, object>>() { postsVariables });
                        defaultCollections.Add(postCollection);
                        */
                        var defaultCollections = new List<string>() { "posts" };

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
                        Console.WriteLine("Creating _config.json");

                        GlobalConfiguration defaultConfig = new GlobalConfiguration
                        {
                            source = ".",
                            destination = @"./_site",
                            collections_dir = ".",
                            plugins_dir = "_plugins",
                            layouts_dir = "_layouts",
                            data_dir = "_data",
                            includes_dir = "_includes",
                            sass_dir = "_sass",
                            collections = defaultCollections,
                            safe = false,
                            include = new string[] { ".htaccess" },
                            exclude = new string[] { },
                            keep_files = new string[] { ".git", ".svn" },
                            encoding = "utf-8",
                            culture = "en-US",
                            markdown_ext = "markdown,mkdown,mkdn,mkd,md",
                            strict_front_matter = false,
                            //show_drafts = false, //Intentionally blank, generates null field on output
                            limit_posts = 0,
                            future = false,
                            unpublished = false,
                            whitelist = new string[] { },
                            plugins = new string[] { },
                            excerpt_separator = @"\n\n",
                            detach = false,
                            port = 4000,
                            host = "127.0.0.1",
                            baseurl = "",
                            show_dir_listing = false,
                            permalink = "date",
                            paginate_path = "/page:num",
                            //timezone = "null", //Intentionally blank, generates true null field on output
                            quiet = false,
                            verbose = false
                        };
                        string defaultConfigSerialized = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                        Console.WriteLine(defaultConfigSerialized);
                        using (FileStream fs = File.Create("./_config.json"))
                        {
                            fs.Write(Encoding.UTF8.GetBytes(defaultConfigSerialized), 0, Encoding.UTF8.GetBytes(defaultConfigSerialized).Length);
                        }

                        Console.WriteLine("Creating _posts/_config.json");
                        var postsVariables = new Dictionary<string, object>();
                        postsVariables.Add("output", true);
                        string postsConfigSerialized = JsonConvert.SerializeObject(postsVariables, Formatting.Indented);
                        using (FileStream fs = File.Create("./_posts" + "/_config.json"))
                        {
                            fs.Write(Encoding.UTF8.GetBytes(postsConfigSerialized), 0, Encoding.UTF8.GetBytes(postsConfigSerialized).Length);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [ArgumentNullException]: Issue with generating default configuration file in WDHAN project.\nPlease report this at https://github.com/MadeByEmil/WDHAN/issues/new?assignees=limeschool&labels=bug&template=bug_report.md&title=%5BBUG%5D+-+");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [ArgumentException]: Issue with generating default configuration file in WDHAN project.\nPlease report this at https://github.com/MadeByEmil/WDHAN/issues/new?assignees=limeschool&labels=bug&template=bug_report.md&title=%5BBUG%5D+-+");
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [DirectoryNotFoundException]: The path to your WDHAN project is inaccessible. Verify it still exists.");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [UnauthorizedAccessException]: Access to WDHAN files is denied. Try changing file permissions, or run with higher privileges.");
                        Environment.Exit(1);
                    }
                    catch (PathTooLongException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [PathTooLongException]: The path to your WDHAN project is too long for your file system to handle.");
                        Environment.Exit(1);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [IOException]: A problem has occured with writing data to your system. Verify your OS and data storage device are working correctly.");
                        Environment.Exit(1);
                    }
                    catch (NotSupportedException ex)
                    {
                        Console.WriteLine("For developers: " + ex);
                        Console.WriteLine("ERROR [NotSupportedException]: WDHAN cannot create your project's output directory. Verify your OS and data storage device are working correctly, and you have proper permissions.");
                        Environment.Exit(1);
                    }
                }
            }    
            catch (IndexOutOfRangeException)
            {
                // Create site with default theme
            }
            catch(Exception ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [Exception]: Cannot create project files. Please ensure the directory you're trying to create is supported in your file system.");
                Environment.Exit(1);
            }
        }
        static void buildSite(string[] args)
        {
            Console.WriteLine("Building project files ... ");
            GlobalConfiguration siteConfig = GlobalConfiguration.getConfiguration();
            Directory.CreateDirectory(siteConfig.destination);
            Directory.CreateDirectory(siteConfig.source + "/temp");
            foreach(var file in Directory.GetFiles(siteConfig.source, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("1 " + Path.GetDirectoryName(file));
                Console.WriteLine("2 " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                Console.WriteLine("3 " + file);
                if(!Path.GetDirectoryName(file).StartsWith("./_"))
                {
                    string fileDest = Path.GetDirectoryName(file) + "/" + siteConfig.destination + "/" + Path.GetFileName(file);
                    Console.WriteLine(fileDest);
                    Directory.CreateDirectory(Path.GetDirectoryName(fileDest));

                    Boolean first = false;
                    Boolean second = false;
                    string fileContents = "";

                    if(File.ReadAllLines(file)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach(var line in File.ReadAllLines(file))
                        {
                            if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                            {
                                first = true;
                                continue;
                            }
                            if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                            {
                                second = true;
                                continue;
                            }
                            if(first && second)
                            {
                                fileContents += line;
                            }
                        }
                        if(first && second)
                        {
                            using (FileStream fs = File.Create(fileDest))
                            {
                                fs.Write(Encoding.UTF8.GetBytes(fileContents), 0, Encoding.UTF8.GetBytes(fileContents).Length);
                            }
                        }
                        else
                        {
                            // Copy file over
                            File.Copy(file, fileDest);
                        }
                    }
                }
            }
            // Handle _layout, _include, _data, _collection
            buildCollection(args);
        }
        static void buildCollection(string[] args)
        {
            Console.WriteLine("Building collection files ... ");
            GlobalConfiguration siteConfig = GlobalConfiguration.getConfiguration();
            Directory.CreateDirectory(siteConfig.destination);
            Directory.CreateDirectory(siteConfig.source + "/temp");
            foreach(var collection in siteConfig.collections)
            {
                //foreach(var key in collection.Keys){
                    Post.generateEntires(collection);
                    Data.generateDataIndex();

                    foreach(var file in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                    {
                        string fileContents = "";
                        Boolean first = false;
                        Boolean second = false;
                        if(File.ReadAllLines(file)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach(var line in File.ReadAllLines(file)){
                                if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                                {
                                    first = true;
                                    continue;
                                }
                                if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                                {
                                    second = true;
                                    continue;
                                }
                                if(first && second)
                                {
                                    fileContents += (line + "\n");
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        // Configure the pipeline with all advanced extensions active
                        Console.WriteLine("Outputting " + file + " to " + siteConfig.destination + "/" + collection + "/" + Path.GetFileNameWithoutExtension(file) + ".html");
                        Console.WriteLine(fileContents + "\nis the content being parsed.\n");
                        var result = "";
                        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                        try
                        {
                            result = Markdown.ToHtml(parsePage(args, collection, file, fileContents), pipeline);
                        }
                        catch(ArgumentNullException ex)
                        {
                            Console.WriteLine("For developers: " + ex);
                            result = "ERROR [ArgumentNullException]: Pagewrite failed. Page contents are either corrupted or blank.";
                            Environment.Exit(1);
                        }

                        //TODO: Implement permalinks in accordance with _config.json settings
                        Directory.CreateDirectory(siteConfig.destination + "/" + collection); // No longer needed when permalink support is added
                        using (FileStream fs = File.Create(siteConfig.destination + "/" + collection + "/" + Path.GetFileNameWithoutExtension(file) + ".html"))
                        {
                            fs.Write(Encoding.UTF8.GetBytes(result), 0, Encoding.UTF8.GetBytes(result).Length);
                        }
                    }
                //}
            }
        }

        /*
        OK, how are we going to tackle this? We have a Liquid parser, but it's on us to find those variables to pass to the parser.
        Hmm ... what variables do we actually have to work with?
        Well, pages can have page.variable, site.variable, site.data.variable
        Includes can have site.variable, site.data.variable, and include.variable
        We don't know what include.variable is, UNTIL we get that value from the page, so ... there's an order to the parsing:
        1. site
        2. site.data
        3. page (from file header AND/OR from collections in 'site,' find path, parse)
        4. include (get references include from /_includes/<string>.html)
        5. layout (since this can reference page data not yet generated)
        6. misc. leftovers (rss.xml for example)

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
        Site variables, including site.data variables, come from JSON data.
        */
        static string parsePage(string[] args, string collectionName, string filePath, string fileContents)
        {
            try
            {
                GlobalConfiguration siteConfig = GlobalConfiguration.getConfiguration();

                // When a property of a JObject value is accessed, try to look into its properties
                TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

                // Convert JToken to FluidValue
                FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
                FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));

                var postModel = new JObject();

                //TODO: Get values from content
                //DO NOT USE ON INCLUDES & LAYOUTS -- They rely on page data, so rendering them requires
                //all these values PLUS the page's values, before the page itself is rendered

                //var pageContent = "{{ Model.Name }}"; // String containing page's contents

                //var siteModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing site's values
                var siteModel = JObject.Parse(File.ReadAllText("./_config.json"));

                //var siteDataModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing site's data values

                //var collectionModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing collection's values
                //NOTE: Not needed. Collection data is in _config.json. Actually, nevermind (see #8)
                Dictionary<string, object> collectionConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(siteConfig.source + "/_" + collectionName + "/_config.json"));
                var collectionModel = JObject.Parse(File.ReadAllText(siteConfig.source + "/_" + collectionName + "/_config.json"));
                var collectionPosts = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_" + collectionName + "/_entries.json"));

                var dataSet = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_data.json"));

                JObject pageModel = Post.parseFrontMatter(filePath);

                if (FluidTemplate.TryParse(fileContents, out var template))
                {
                    var context = new TemplateContext();
                    context.CultureInfo = new CultureInfo(siteConfig.culture);

                    siteModel.Merge(dataSet, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    Console.WriteLine(siteModel.ToString());

                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    //context.SetValue(collectionName, JObject.Parse(collectionPosts));
                    
                    collectionModel.Merge(collectionPosts, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });

                    context.SetValue(collectionName, collectionModel);
                    //context.SetValue(collectionName, collectionPosts);

                    Console.WriteLine(collectionModel.ToString());

                    Console.WriteLine(template.Render(context) + "\nis the result.\n");
                    Console.WriteLine();

                    // Generate a sum JObject of all these context values
                    postModel.Merge(siteModel, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    postModel.Merge(collectionModel, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    postModel.Merge(pageModel, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });

                    // Output page's sum JSON values
                    Directory.CreateDirectory(siteConfig.source + "/temp/_" + collectionName);
                    using (FileStream fs = File.Create(siteConfig.source + "/temp/_" + collectionName + "/" + Path.GetFileNameWithoutExtension(filePath) + ".json"))
                    {
                        fs.Write(Encoding.UTF8.GetBytes(postModel.ToString()), 0, Encoding.UTF8.GetBytes(postModel.ToString()).Length);
                    }

                    // Return the page's render context.
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR: Could not parse Liquid context.");
                    return null;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [UnauthorizedAccessException]: Access to WDHAN files is denied. Try changing file permissions, or run with higher privileges.");
                return null;
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [PathTooLongException]: The path to your WDHAN project is too long for your file system to handle.");
                return null;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [DirectoryNotFoundException]: The path to your WDHAN project is inaccessible. Verify it still exists.");
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [IOException]: A problem has occured with writing data to your system. Verify your OS and data storage device are working correctly.");
                return null;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [NotSupportedException]: WDHAN cannot create your project's output directory. Verify your OS and data storage device are working correctly, and you have proper permissions.");
                return null;
            }
        }
        static string parseFile(string[] args, string filePath, string fileContents)
        {
            try
            {
                GlobalConfiguration siteConfig = GlobalConfiguration.getConfiguration();

                // When a property of a JObject value is accessed, try to look into its properties
                TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

                // Convert JToken to FluidValue
                FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
                FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));

                var postModel = new JObject();

                //TODO: Get values from content
                //DO NOT USE ON INCLUDES & LAYOUTS -- They rely on page data, so rendering them requires
                //all these values PLUS the page's values, before the page itself is rendered

                //var pageContent = "{{ Model.Name }}"; // String containing page's contents

                //var siteModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing site's values
                var siteModel = JObject.Parse(File.ReadAllText("./_config.json"));

                //var siteDataModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing site's data values

                //var collectionModel = JObject.Parse("{\"Name\": \"Bill\"}"); // String containing collection's values
                //NOTE: Not needed. Collection data is in _config.json. Actually, nevermind (see #8)
                var dataSet = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_data.json"));

                JObject pageModel = Post.parseFrontMatter(filePath);

                if (FluidTemplate.TryParse(fileContents, out var template))
                {
                    var context = new TemplateContext();
                    context.CultureInfo = new CultureInfo(siteConfig.culture);

                    siteModel.Merge(dataSet, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    Console.WriteLine(siteModel.ToString());

                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    //context.SetValue(collectionName, JObject.Parse(collectionPosts));
                    
                    Console.WriteLine(template.Render(context) + "\nis the result.\n");
                    Console.WriteLine();

                    // Generate a sum JObject of all these context values
                    postModel.Merge(siteModel, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    postModel.Merge(pageModel, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });

                    // Output page's sum JSON values
                    Directory.CreateDirectory(siteConfig.source + "/temp/");
                    using (FileStream fs = File.Create(siteConfig.source + "/temp/" + Path.GetFileNameWithoutExtension(filePath) + ".json"))
                    {
                        fs.Write(Encoding.UTF8.GetBytes(postModel.ToString()), 0, Encoding.UTF8.GetBytes(postModel.ToString()).Length);
                    }

                    // Return the page's render context.
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR: Could not parse Liquid context.");
                    return null;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [UnauthorizedAccessException]: Access to WDHAN files is denied. Try changing file permissions, or run with higher privileges.");
                return null;
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [PathTooLongException]: The path to your WDHAN project is too long for your file system to handle.");
                return null;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [DirectoryNotFoundException]: The path to your WDHAN project is inaccessible. Verify it still exists.");
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [IOException]: A problem has occured with writing data to your system. Verify your OS and data storage device are working correctly.");
                return null;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine("ERROR [NotSupportedException]: WDHAN cannot create your project's output directory. Verify your OS and data storage device are working correctly, and you have proper permissions.");
                return null;
            }
        }

        static void printHelpMsg(string[] args)
        {
            try
            {
                if (args[1].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Creates an empty WDHAN project in the current directory.");
                }
                else if (args[1].Equals("build", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable WDHAN project.");
                }
                else if (args[1].Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Outputs a publishable WDHAN project.");
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
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("For developers: " + ex);
                Console.WriteLine(
                    "WDHAN supports the following commands:\n" +
                    "   wdhan new - Creates an empty WDHAN project in the current directory.\n" +
                    "   wdhan build - Outputs a publishable WDHAN project.\n" +
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
