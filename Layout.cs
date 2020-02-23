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
            var layoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + layout + ".html";
            var layoutContents = WDHANFile.getFileContents(layoutPath);
            layoutContents = Include.evalInclude(layoutPath);
            //layoutContents = WDHANFile.getFileContents(layoutPath);
            try
            {
                var subLayout = Page.parseFrontMatter(layoutPath)["layout"].ToString();
                var subLayoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + subLayout + ".html";
                layoutContents = getLayoutContents(subLayout, filePath).Replace("{{ content }}", layoutContents);
                return layoutContents;
            }
            catch(Exception)
            {
                return layoutContents;
            }
        }
    }
}