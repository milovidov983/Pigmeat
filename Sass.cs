using System;
using System.Collections.Generic;
using System.Text;

namespace WDHAN
{
    public class Sass
    {
        public Sass()
        {
            
        }
        public static string getSassContents(string file)
        {
            var fileContents = WDHANFile.getFileContents(file);
            var includeSection = getIncludeSection(fileContents);
            int lastInd = 0;
            for(int i = 0; i < includeSection.Split('"').Length - 1; i++)
            {
                Console.WriteLine("i: " + i);
                if(i % 2 == 0)
                {
                    lastInd = includeSection.IndexOf('"', lastInd + 1);
                    Console.WriteLine("LASTIND: " + lastInd);
                    includeSection = includeSection.Insert(lastInd + 1, GlobalConfiguration.getConfiguration().sass_dir + "/");
                    continue;
                }
                else
                {
                    lastInd = includeSection.IndexOf('"', lastInd + 1);
                    continue;
                }
            }
            Console.WriteLine("SASSINCLUDESECTION:\n" + includeSection);
            Console.WriteLine(fileContents.Replace(getIncludeSection(fileContents), includeSection));
            return fileContents.Replace(getIncludeSection(fileContents), includeSection);
        }
        public static string getIncludeSection(string fileContents)
        {
            Console.WriteLine(fileContents.IndexOf("@import"));
            Console.WriteLine(fileContents.IndexOf(';', fileContents.IndexOf("@import")));
            Console.WriteLine(fileContents.Length);
            var firstHalf = fileContents.Substring(fileContents.IndexOf("@import"));
            Console.WriteLine("FIRSTHALF:\n" + firstHalf);
            var secondHalf = firstHalf.Substring(0, firstHalf.IndexOf(';') + 1);
            Console.WriteLine("GETINCLUDESECTION:\n" + secondHalf);
            return secondHalf;
            //return fileContents.Substring(fileContents.IndexOf("@import"), fileContents.IndexOf(';', fileContents.IndexOf("@import")));
        }
    }
}