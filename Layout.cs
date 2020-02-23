using System;
using System.Collections.Generic;
using System.IO;
using Fluid;
using Fluid.Values;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace WDHAN
{
    public class Layout
    {
        public Layout()
        {
            
        }
        public static string getLayoutContents(string layout, string filePath)
        {
            Console.WriteLine("getLayoutContents - " + layout + ", " + filePath);
            var layoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + layout + ".html";
            var layoutContents = WDHANFile.getFileContents(layoutPath);
            Console.WriteLine("getLayoutContents WITHOUTINCLUDE:\n" + layoutContents);
            layoutContents = Include.evalInclude(layoutPath);
            Console.WriteLine("getLayoutContents WITH:\n" + layoutContents);
            //layoutContents = WDHANFile.getFileContents(layoutPath);
            try
            {
                var subLayout = Page.parseFrontMatter(layoutPath)["layout"].ToString();
                Console.WriteLine(subLayout);
                var subLayoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + subLayout + ".html";
                Console.WriteLine(subLayoutPath);
                layoutContents = getLayoutContents(subLayout, filePath).Replace("{{ content }}", layoutContents);
                Console.WriteLine(layout + " - HASSUB:\n" + layoutContents);
                return layoutContents;
                //return parseLayout(collectionName, filePath, layoutContents);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SUBLAYOUTEXCEPTION:\n" + ex.ToString());
                Console.WriteLine(layout + ":\n" + layoutContents);
                return layoutContents;
                //return parseLayout(collectionName, filePath, layoutContents);
            }
        }
    }
}