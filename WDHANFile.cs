using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Fluid;
using Fluid.Values;
using System.Globalization;

namespace WDHAN
{
    public class WDHANFile
    {
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public string path { get; set; }
        public DateTime modified_time { get; set; }
        public string basename { get; set; }
        public string extname { get; set; }
        public string title { get; set; }
        public WDHANFile()
        {
            
        }
        public static string getFileContents(string filePath)
        {
            string fileContents = "";
            Boolean first = false;
            Boolean second = false;
            if(File.ReadAllLines(filePath)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
            {
                foreach(var line in File.ReadAllLines(filePath)){
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                    {
                        first = true;
                        continue;
                    }
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                    {
                        second = true;
                        continue;
                    }
                    if(first && second)
                    {
                        fileContents += (line + "\n");
                    }
                }
            }
            else
            {
                fileContents = File.ReadAllText(filePath);
            }
            return fileContents;
        }
        public static JObject parseFrontMatter(string filePath)
        {
            Boolean first = false, second = false;
            string pageFrontMatter = "";
            if(File.ReadAllLines(filePath)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
            {
                foreach(var line in File.ReadAllLines(filePath)){
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                    {
                        first = true;
                        continue;
                    }
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                    {
                        second = true;
                        continue;
                    }
                    if(first && !second)
                    {
                        pageFrontMatter += (line + "\n");
                    }
                }
                if(pageFrontMatter.Equals("", StringComparison.OrdinalIgnoreCase))
                {
                    return JObject.Parse("{\"exists\": true}");
                }
                else
                {
                    return GlobalConfiguration.yamlInterop(pageFrontMatter);
                    /*
                    if(isJSON(pageFrontMatter))
                    {
                        return JObject.Parse(pageFrontMatter);
                    }
                    else
                    {
                        var deserializer = new Deserializer();
                        StringReader reader = new StringReader(pageFrontMatter);
                        var yamlObject = deserializer.Deserialize(reader);
                        return JObject.Parse(JsonConvert.SerializeObject(yamlObject, Formatting.Indented));
                    }
                    */
                }
            }
            else
            {
                return JObject.Parse("{\"exists\": false}");
            }
        }
        public static WDHANFile getDefinedFile(WDHANFile file)
        {
            file.modified_time = System.IO.File.GetLastWriteTimeUtc("./" + file.path);
            file.basename = Path.GetFileNameWithoutExtension("./" + file.path);
            file.extname = Path.GetExtension("./" + file.path);
            return file;
        }
        public string getTitle()
        {
            try
            {
                return frontmatter["title"].ToString();
            }
            catch(NullReferenceException)
            {
                return "";
            }
        }
        public static string parseRaw(string filePath)
        {            
            var siteConfig = GlobalConfiguration.getConfiguration();
            var fileContents = WDHANFile.getFileContents(filePath);
            fileContents = Include.evalInclude(filePath); // Expand includes (must happen after layouts are retreived, as layouts can have includes)

            // When a property of a JObject value is accessed, try to look into its properties
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

            // Convert JToken to FluidValue
            FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
            FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));
            

            var siteModel = JObject.Parse(File.ReadAllText("./_config.json"));
            var dataSet = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_data.json"));
            var pageModel = WDHANFile.parseFrontMatter(filePath);
            
            try
            {
                if(FluidTemplate.TryParse(fileContents, out var template))
                {
                    var context = new TemplateContext();
                    context.CultureInfo = new CultureInfo(siteConfig.culture);

                    siteModel.Merge(dataSet, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    context.SetValue("wdhan", JObject.Parse("{\"version\": \"" + Program.version + "\"}"));

                    foreach(var collection in siteConfig.collections)
                    {
                        if(File.Exists(siteConfig.source + "/temp/_" + collection + "/_entries.json"))
                        {
                            var collectionModel = JObject.Parse(File.ReadAllText(siteConfig.source + "/_" + collection + "/_config.json"));
                            collectionModel.Merge(JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_" + collection + "/_entries.json")), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                            context.SetValue(collection, collectionModel);
                        }
                    }
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR [parseRaw]: Could not parse Liquid context for file " + filePath + ".");
                    return fileContents;
                }
            }
            catch(ArgumentNullException ex)
            {
                Console.WriteLine("File " + filePath + " has no Liquid context to parse.\n" + ex.ToString());
                return fileContents;
            }
        }
    }
}