using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WDHAN
{
    public class GlobalConfiguration
    {
        public string source { get; set; }
        public string destination { get; set; }
        public string collections_dir { get; set; }
        public string plugins_dir { get; set; }
        public string layouts_dir { get; set; }
        public string data_dir { get; set; }
        public string includes_dir { get; set; }
        public string sass_dir { get; set; }
        public List<string> collections { get; set; }
        public Boolean safe { get; set; }
        public string[] include { get; set; }
        public string[] exclude { get; set; }
        public string[] keep_files { get; set; }
        public string encoding { get; set; }
        public string culture { get; set; }
        public string markdown_ext { get; set; }
        public Boolean strict_front_matter { get; set; }
        public Boolean show_drafts { get; set; }
        public int limit_posts { get; set; }
        public Boolean future { get; set; }
        public Boolean unpublished { get; set; }
        public string[] whitelist { get; set; }
        public string[] plugins { get; set; }
        public string excerpt_separator { get; set; }
        public Boolean detach { get; set; }
        public int port { get; set; }
        public string host { get; set; }
        public string baseurl { get; set; }
        public Boolean show_dir_listing { get; set; }
        public string permalink { get; set; }
        public string paginate_path { get; set; }
        public string timezone { get; set; }
        public Boolean quiet { get; set; }
        public Boolean verbose { get; set; }
        public GlobalConfiguration()
        {
            
        }
        public static GlobalConfiguration getConfiguration()
        {
            return JsonConvert.DeserializeObject<GlobalConfiguration>(File.ReadAllText("./_config.json"));
        }

        public static List<string> getMarkdownExts()
        {
            List<string> exts = new List<string>();
            string currentExt = "";
            foreach(var character in getConfiguration().markdown_ext)
            {
                if(character.Equals(','))
                {
                    exts.Add(currentExt);
                    currentExt = "";
                }
                else
                {
                    currentExt += character;
                }
            }
            return exts;
        }
    }
}