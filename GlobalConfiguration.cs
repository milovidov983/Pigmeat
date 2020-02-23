using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

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
        public List<string> include { get; set; }
        public List<string> exclude { get; set; }
        public List<string> keep_files { get; set; }
        public string encoding { get; set; }
        public string culture { get; set; }
        public string markdown_ext { get; set; }
        public Boolean strict_front_matter { get; set; }
        public Boolean show_drafts { get; set; }
        public int limit_posts { get; set; }
        public Boolean future { get; set; }
        public Boolean unpublished { get; set; }
        public List<string> whitelist { get; set; }
        public List<string> plugins { get; set; }
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
        public string url { get; set; }
        public Dictionary<string, object> TAGS { get; set; }
        public DateTime time { get; set; }
        public List<Page> pages { get; set; }
        public List<WDHANFile> static_files { get; set; }
        public List<HTMLFile> html_files { get; set; }
        public List<Page> html_pages { get; set; }
        public Defaults defaults { get; set; }
        public Dictionary<string, object> user { get; set; }
        public GlobalConfiguration()
        {
            
        }
        public static List<string> filesWithTag(string tag)
        {
            var siteConfig = getConfiguration();
            List<string> postsWithTag = new List<string>();
            foreach(var collection in siteConfig.collections)
            {
                foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                {
                    if(isMarkdown(Path.GetExtension(post).Substring(1)))
                    {
                        try
                        {
                            var postJSON = JArray.Parse(Page.parseFrontMatter(post)["tags"].ToString());
                            Console.WriteLine("TAGCCC: ");
                            foreach(var foundTag in postJSON)
                            {
                                if(foundTag.ToString().Equals(tag.ToString(), StringComparison.Ordinal))
                                {
                                    postsWithTag.Add(Path.GetDirectoryName(post) + "/" + Path.GetFileName(post));
                                }
                            }
                        }
                        catch(NullReferenceException)
                        {
                            continue;
                        }
                    }
                }
            }
            return postsWithTag;
        }
        public static Dictionary<string, object> getTaggedPosts()
        {
            var siteConfig = getConfiguration();
            Dictionary<string, object> taggedPosts = new Dictionary<string, object>();
            List<string> tags = new List<string>();
            foreach(var collection in siteConfig.collections)
            {
                foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                {
                    if(isMarkdown(Path.GetExtension(post).Substring(1)))
                    {
                        //var postJSON = JsonConvert.DeserializeObject<Post>(File.ReadAllText(siteConfig.source + "/temp/_" + collection + "/" + Path.GetFileNameWithoutExtension(post) + ".json"));
                        try
                        {
                            var postJSON = JArray.Parse(Page.parseFrontMatter(post)["tags"].ToString());
                            Console.WriteLine("TAGCCC: ");
                            foreach(var tag in postJSON)
                            {
                                if(!tags.Contains(tag.ToString()))
                                {
                                    tags.Add(tag.ToString());
                                    Console.WriteLine("TAGBBB: " + tag.ToString());
                                    taggedPosts.Add(tag.ToString(), filesWithTag(tag.ToString()));
                                }
                            }
                        }
                        catch(NullReferenceException)
                        {
                            continue;
                        }
                    }
                }
            }
            return taggedPosts;
        }
        public static void outputConfiguration(GlobalConfiguration siteConfig)
        {
            string siteConfigSerialized = JsonConvert.SerializeObject(siteConfig, Formatting.Indented);
            Console.WriteLine(siteConfigSerialized);
            using (FileStream fs = File.Create("./_config.json"))
            {
                fs.Write(Encoding.UTF8.GetBytes(siteConfigSerialized), 0, Encoding.UTF8.GetBytes(siteConfigSerialized).Length);
            }
        }
        public static void includeTime()
        {
            var siteConfig = getConfiguration();
            siteConfig.time = DateTime.UtcNow; // Get the time in UTC (international, timezone neutral)

            outputConfiguration(siteConfig);
        }
        public static void includeTags()
        {
            GlobalConfiguration siteConfig = getConfiguration();

            siteConfig.TAGS = getTaggedPosts();
            Console.WriteLine("TAGAAA!");
            foreach(var tag in getTaggedPosts())
            {
                Console.WriteLine("TAGAAA:" + tag);
            }

            outputConfiguration(siteConfig);
        }
        public static GlobalConfiguration getConfiguration()
        {
            return JsonConvert.DeserializeObject<GlobalConfiguration>(File.ReadAllText("./_config.json"));
        }
        private static bool isJSON(string input)
        {
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(input);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static Boolean isMarkdown(string fileExt)
        {
            foreach(var ext in GlobalConfiguration.getMarkdownExts())
            {
                if(ext.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        public static JObject yamlInterop(string input)
        {
            if(isJSON(input))
            {
                return JObject.Parse(input);
            }
            else
            {
                var deserializer = new Deserializer();
                StringReader reader = new StringReader(input);
                var yamlObject = deserializer.Deserialize(reader);
                return JObject.Parse(JsonConvert.SerializeObject(yamlObject, Formatting.Indented));
            }
        }
        public static List<string> getMarkdownExts()
        {
            List<string> exts = new List<string>();
            string currentExt = "";
            int latestInd = 0;
            for(int i = 0; i < getConfiguration().markdown_ext.Length; i++)
            {
                if(getConfiguration().markdown_ext[i].Equals(','))
                {
                    exts.Add(currentExt);
                    currentExt = "";
                    latestInd = i;
                    continue;
                }
                else
                {
                    currentExt += getConfiguration().markdown_ext[i];
                }
            }
            exts.Add(getConfiguration().markdown_ext.Substring(latestInd + 1));
            return exts;
        }
    }
}