using System;
using System.IO;
using CSScriptLib;

namespace WDHAN
{
    public class Plugins
    {
        public Plugins()
        {
            
        }
        public static void getPlugins(string[] args)
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            try
            {
                if(args.Length == 1 || !args[args.Length - 2].Equals("-f", StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var file in Directory.GetFiles(siteConfig.source + "/" + siteConfig.plugins_dir, "*", SearchOption.AllDirectories))
                    {
                        Console.WriteLine("Found plugins directory, " + siteConfig.source + "/" + siteConfig.plugins_dir);
                        if(siteConfig.plugins.Contains(Path.GetFileNameWithoutExtension(file)))
                        {
                            Console.WriteLine("Loading plugin: " + Path.GetFileName(file));
                            dynamic script = CSScript.Evaluator.LoadCode(File.ReadAllText(file));
                            script.Main(args);
                        }
                    }
                }
                else
                {
                    var pluginsDirectory = siteConfig.source + "/" + siteConfig.plugins_dir;
                    if(args[2].Equals("-f", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Loading plugin: " + args[1]);
                        dynamic script = CSScript.Evaluator.LoadCode(File.ReadAllText(pluginsDirectory + "/" + args[1]));
                        script.Main(args);
                    }
                }
            }
            catch(IndexOutOfRangeException)
            {
                var pluginsDirectory = siteConfig.source + "/" + siteConfig.plugins_dir;
                if(siteConfig.plugins.Contains(args[1]))
                {
                    Console.WriteLine("Loading plugin: " + args[1]);
                    dynamic script = CSScript.Evaluator.LoadCode(File.ReadAllText(pluginsDirectory + "/" + args[1]));
                    script.Main(args);
                }
            }
        }
    }
}