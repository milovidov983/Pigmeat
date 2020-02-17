using System;
using System.Collections.Generic;

namespace WDHAN
{
    public class Layout
    {
        public Layout()
        {
            
        }
        public static string getLayoutContents(string layout)
        {
            var layoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + layout + ".html";
            var layoutContents = WDHANFile.getFileContents(layoutPath);
            try
            {
                var subLayout = Page.parseFrontMatter(layoutPath)["layout"].ToString();
                Console.WriteLine(subLayout);
                var subLayoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + subLayout + ".html";
                Console.WriteLine(subLayoutPath);
                layoutContents = getLayoutContents(subLayout).Replace("{{ content }}", layoutContents);
                Console.WriteLine(layout + " - HASSUB:\n" + layoutContents);
                return layoutContents;
            }
            catch(Exception ex)
            {
                Console.WriteLine("SUBLAYOUTEXCEPTION:\n" + ex.ToString());
                Console.WriteLine(layout + ":\n" + layoutContents);
                return layoutContents;
            }
        }
    }
}