using System.Collections.Generic;
using Fluid;
using Fluid.Values;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace WDHAN
{
    public class Include
    {
        public Dictionary<string, string> variables { get; set; }
        public string input { get; set; }
        public Include()
        {
            
        }
        public static string evalInclude(string filePath, string fileContents)
        {
            if(fileContents.Contains("{% inc "))
            {
                List<string> includeCalls = new List<string>();
                string readerString = "";
                Boolean hitOnce = false;
                foreach(var character in fileContents)
                {
                    if(character.Equals('{') && !hitOnce)
                    {
                        hitOnce = true;
                        readerString += character;
                        continue;
                    }
                    if(character.Equals('}') && hitOnce)
                    {
                        hitOnce = false;
                        readerString += character;
                        if(readerString.Contains("{% inc "))
                        {
                            includeCalls.Add(readerString);
                        }
                        readerString = "";
                        continue;
                    }
                    if(hitOnce)
                    {
                        readerString += character;
                        continue;
                    }
                }
                foreach(var includeCall in includeCalls)
                {
                    Include currentInclude = new Include { input = includeCall };
                    string includePath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().includes_dir + "/" + currentInclude.getCallArgs()[2];
                    fileContents = fileContents.Replace(includeCall, currentInclude.parseInclude(includePath, filePath));
                }
            }
            return fileContents;
        }
        public static string evalInclude(string filePath)
        {
            var fileContents = WDHANFile.getFileContents(filePath);
            if(fileContents.Contains("{% inc "))
            {
                List<string> includeCalls = new List<string>();
                string readerString = "";
                Boolean hitOnce = false;
                foreach(var character in fileContents)
                {
                    if(character.Equals('{') && !hitOnce)
                    {
                        hitOnce = true;
                        readerString += character;
                        continue;
                    }
                    if(character.Equals('}') && hitOnce)
                    {
                        hitOnce = false;
                        readerString += character;
                        if(readerString.Contains("{% inc "))
                        {
                            includeCalls.Add(readerString);
                        }
                        readerString = "";
                        continue;
                    }
                    if(hitOnce)
                    {
                        readerString += character;
                        continue;
                    }
                }
                foreach(var includeCall in includeCalls)
                {
                    Include currentInclude = new Include { input = includeCall };
                    string includePath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().includes_dir + "/" + currentInclude.getCallArgs()[2];
                    fileContents = fileContents.Replace(includeCall, currentInclude.parseInclude(includePath, filePath));
                }
            }
            return fileContents;
        }
        public string parseInclude(string includePath, string filePath)
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            var fileContents = WDHANFile.getFileContents(includePath);

            // Expand layouts, then parse includes

            // When a property of a JObject value is accessed, try to look into its properties
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

            // Convert JToken to FluidValue
            FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
            FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));

            var siteModel = JObject.Parse(File.ReadAllText("./_config.json"));
            var dataSet = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_data.json"));
            var pageFrontmatter = WDHANFile.parseFrontMatter(filePath);
            var pageModel = JObject.Parse(JsonConvert.SerializeObject(Page.getDefinedPage(new Page() { frontmatter = WDHANFile.parseFrontMatter(filePath), path = filePath })));

            setVariables();
            var includeModel = JObject.Parse(JsonConvert.SerializeObject(variables));
            var includeFrontmatter = WDHANFile.parseFrontMatter(includePath);
            includeModel.Merge(includeFrontmatter, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            try
            {
                if(FluidTemplate.TryParse(fileContents, out var template))
                {
                    var context = new TemplateContext();
                    context.CultureInfo = new CultureInfo(siteConfig.culture);

                    siteModel.Merge(dataSet, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    pageModel.Merge(pageFrontmatter, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    context.SetValue("wdhan", JObject.Parse("{\"version\": \"" + Program.version + "\"}"));
                    
                    foreach(var collection in siteConfig.collections)
                    {
                        context.SetValue(collection, JObject.Parse(File.ReadAllText(siteConfig.source + "/_" + collection + "/_config.json")));
                    }
                    context.SetValue("include", includeModel);
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR [parseInclude]: Could not parse Liquid context for include " + includePath + ".");
                    return fileContents;
                }
            }
            catch(ArgumentNullException)
            {
                Console.WriteLine("Include " + includePath + " has no Liquid context to parse.");
                return fileContents;
            }
        }
        public string parseInclude(string includePath, JObject pageModel, string collectionName, string filePath)
        {
            setVariables();
            var siteConfig = GlobalConfiguration.getConfiguration();

            // When a property of a JObject value is accessed, try to look into its properties
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

            // Convert JToken to FluidValue
            FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
            FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));

            string includeFile = WDHANFile.getFileContents(includePath);

            var context = new TemplateContext();
            var givenModel = JObject.Parse(JsonConvert.SerializeObject(variables));
            var frontmatterModel = WDHANFile.parseFrontMatter(includePath);

            givenModel.Merge(frontmatterModel, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            var siteModel = JObject.Parse(File.ReadAllText("./_config.json"));
            Dictionary<string, object> collectionConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(siteConfig.source + "/_" + collectionName + "/_config.json"));
            var collectionModel = JObject.Parse(File.ReadAllText(siteConfig.source + "/_" + collectionName + "/_config.json"));
            var collectionPosts = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_" + collectionName + "/_entries.json"));
            var dataSet = JObject.Parse(File.ReadAllText(siteConfig.source + "/temp/_data.json"));
            try
            {
                JObject pageObjectModel = Page.getPage(Page.getDefinedPage(new Page { frontmatter = pageModel, content = WDHANFile.getFileContents(filePath), path = filePath }));
                pageModel.Merge(pageObjectModel, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }
            catch
            {

            }

            siteModel.Merge(dataSet, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            try
            {
                if (FluidTemplate.TryParse(includeFile, out var template))
                {
                    context.SetValue("include", givenModel);
                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    collectionModel.Merge(collectionPosts, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    context.SetValue(collectionName, collectionModel);
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR [parseInclude]: Could not parse Liquid context.");
                    return includeFile;
                }
            }
            catch(ArgumentNullException)
            {
                Console.WriteLine("No Liquid context to parse.");
                return includeFile;
            }            
        }
        public void setVariables()
        {
            Dictionary<string, string> gatheredVariables = new Dictionary<string, string>();
            for(int i = 0; i < getKeys().Length; i++)
            {
                gatheredVariables.Add(getKeys()[i], getValues()[i]);
            }
            variables = gatheredVariables;
        }
        public string[] getValues()
        {
            List<string> values = new List<string>();
            string currentValue = "";
            for(int i = 3; i < getCallArgs().Length; i++)
            {
                Boolean hitEquals = false;
                foreach(var character in getCallArgs()[i])
                {
                    if(character.Equals('='))
                    {
                        hitEquals = true;
                        continue;
                    }
                    if(hitEquals)
                    {
                        currentValue += character;
                        continue;
                    }
                }
                try
                {
                    if(currentValue.ToCharArray()[0].Equals('"') && currentValue.ToCharArray()[currentValue.Length - 1].Equals('"'))
                    {
                        values.Add(currentValue.Substring(1, currentValue.Length - 2));
                    }
                    else
                    {
                        values.Add(currentValue);
                    }
                }
                catch(IndexOutOfRangeException)
                {
                    values.Add(currentValue);
                }
                currentValue = "";
            }
            return values.ToArray();
        }
        public string[] getKeys()
        {
            List<string> keys = new List<string>();
            string currentKey = "";
            for(int i = 3; i < getCallArgs().Length; i++)
            {
                foreach(var character in getCallArgs()[i])
                {
                    if(character.Equals('='))
                    {
                        keys.Add(currentKey);
                        currentKey = "";
                        break;
                    }
                    else
                    {
                        currentKey += character;
                        continue;
                    }
                }
            }
            return keys.ToArray();
        }
        public string[] getCallArgs()
        {
            List<string> callArgs = new List<string>();
            string currentArg = "";
            foreach(var character in input)
            {
                if(character.Equals(' '))
                {
                    callArgs.Add(currentArg);
                    currentArg = "";
                    continue;
                }
                else
                {
                    currentArg += character;
                    continue;
                }
            }
            return callArgs.ToArray();
        }
    }
}