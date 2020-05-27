using System;

namespace Pigmeat
{
    public class Layout
    {
        public Layout()
        {

        }
        public static string getLayoutContents(string layout, string filePath)
        {
            var layoutPath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().layouts_dir + "/" + layout + ".html";
            var layoutContents = PigmeatFile.getFileContents(layoutPath);
            //layoutContents = Include.evalInclude(layoutPath); - Design change (2020-03-31): Evaluate includes in file, not layout (allows for using document context)
            //layoutContents = PigmeatFile.getFileContents(layoutPath);
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
