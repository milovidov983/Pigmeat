using System.IO;
using CSScriptLib;

namespace WDHAN
{
    public class Plugins
    {
        public Plugins()
        {
            
        }
        public static void getPlugins()
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            foreach(var file in Directory.GetFiles(siteConfig.source + "/" + siteConfig.plugins_dir, "*", SearchOption.AllDirectories))
            {
                if(siteConfig.plugins.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    dynamic script = CSScript.Evaluator.LoadCode(File.ReadAllText(file));
                    script.Main();
                }
            }
        }
    }
}