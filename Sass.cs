namespace Pigmeat
{
    public class Sass
    {
        protected Sass()
        {

        }
        public static string getSassContents(string file)
        {
            var fileContents = PigmeatFile.getFileContents(file);
            var includeSection = getIncludeSection(fileContents);
            int lastInd = 0;
            for(int i = 0; i < includeSection.Split('"').Length - 1; i++)
            {
                if(i % 2 == 0)
                {
                    lastInd = includeSection.IndexOf('"', lastInd + 1);
                    includeSection = includeSection.Insert(lastInd + 1, GlobalConfiguration.getConfiguration().sass_dir + "/");
                    continue;
                }
                else
                {
                    lastInd = includeSection.IndexOf('"', lastInd + 1);
                    continue;
                }
            }
            return fileContents.Replace(getIncludeSection(fileContents), includeSection);
        }
        public static string getIncludeSection(string fileContents)
        {
            var firstHalf = fileContents.Substring(fileContents.IndexOf("@import"));
            var secondHalf = firstHalf.Substring(0, firstHalf.IndexOf(';') + 1);
            return secondHalf;
            //return fileContents.Substring(fileContents.IndexOf("@import"), fileContents.IndexOf(';', fileContents.IndexOf("@import")));
        }
    }
}
