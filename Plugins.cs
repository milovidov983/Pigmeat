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
            foreach(var file in Directory.GetFiles(siteConfig.source + "/" + siteConfig.plugins_dir, "*", SearchOption.AllDirectories))
            {
                Console.WriteLine("Found plugins directory. - " + siteConfig.source + "/" + siteConfig.plugins_dir);
                if(siteConfig.plugins.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    Console.WriteLine("Loading plugin: " + Path.GetFileName(file));
                    dynamic script = CSScript.Evaluator.LoadCode(File.ReadAllText(file));
                    script.Main(args);
                }
            }
        }
    }
}