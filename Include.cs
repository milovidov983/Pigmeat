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
        public static string evalInclude(string filePath)
        {
            var fileContents = WDHANFile.getFileContents(filePath);
            if(fileContents.Contains("{% include "))
            {
                Console.WriteLine("INCLUDESPOTTED");
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
                        if(readerString.Contains("{% include "))
                        {
                            includeCalls.Add(readerString);
                            Console.WriteLine("INCLUDEADDED");
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
                    Console.WriteLine("INCLUDEAAA: " + fileContents);
                }
            }
            else
            {
                //Console.WriteLine("NOINCLUDE - " + filePath);
            }
            Console.WriteLine("evalInclude:\n" + fileContents);
            return fileContents;
        }
        public string parseInclude(string includePath, string filePath)
        {
            Console.WriteLine();
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
            var pageModel = WDHANFile.parseFrontMatter(filePath);

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
                    context.SetValue("site", siteModel);
                    context.SetValue("page", pageModel);
                    foreach(var collection in siteConfig.collections)
                    {
                        context.SetValue(collection, JObject.Parse(File.ReadAllText(siteConfig.source + "/_" + collection + "/_config.json")));
                    }
                    context.SetValue("include", includeModel);
                    return template.Render(context);
                }
                else
                {
                    Console.WriteLine("ERROR: Could not parse Liquid context for include " + includePath + ".");
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

            Console.WriteLine("INCFILE: \n" + includeFile);

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
                    Console.WriteLine("ERROR: Could not parse Liquid context.");
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
                Console.WriteLine("INCLUDEVAR: " + getKeys()[i] + ", " + getValues()[i]);
            }
            variables = gatheredVariables;
        }
        public string[] getValues()
        {
            List<string> values = new List<string>();
            string currentValue = "";
            for(int i = 3; i < getCallArgs().Length; i++)
            {
                Console.WriteLine("INCLUDEARGS: " + getCallArgs()[i]);
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
                        Console.WriteLine(currentValue);
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
                Console.WriteLine("INCLUDEVALUE: " + currentValue);
                currentValue = "";
            }
            return values.ToArray();
                /*
                if(currentValue.ToCharArray()[0].Equals('"')) //&& (currentValue.ToCharArray()[currentValue.Length - 1].Equals('"')
                {
                    values.Add(currentValue.Substring(1, currentValue.Length - 1));
                }
                else
                {
                    values.Add(currentValue);
                }
                */

        }
        public string[] getKeys()
        {
            List<string> keys = new List<string>();
            string currentKey = "";
            for(int i = 3; i < getCallArgs().Length; i++)
            {
                Console.WriteLine("INCLUDEARGS: " + getCallArgs()[i]);
                foreach(var character in getCallArgs()[i])
                {
                    if(character.Equals('='))
                    {
                        keys.Add(currentKey);
                        Console.WriteLine("INCLUDEKEY: " + currentKey);
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
        public static FluidValue IncludeFilter(FluidValue input, FilterArguments arguments, TemplateContext context)
        {

            // When a property of a JObject value is accessed, try to look into its properties
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((source, name) => source[name]);

            // Convert JToken to FluidValue
            FluidValue.SetTypeMapping<JObject>(o => new ObjectValue(o));
            FluidValue.SetTypeMapping<JValue>(o => FluidValue.Create(o.Value));

            var includePath = GlobalConfiguration.getConfiguration().source + "/" + GlobalConfiguration.getConfiguration().includes_dir + "/" + arguments.At(0).ToStringValue();
            string includeFile = WDHANFile.getFileContents(includePath);

            var includeContext = new TemplateContext();
            var includeJSON = WDHANFile.parseFrontMatter(includePath);

            if (FluidTemplate.TryParse(includeFile, out var template))
            {
                for(int i = 1; i < arguments.Count - 1; i++)
                {
                    includeContext.SetValue(arguments.At(i).ToStringValue(), arguments.At(i));
                }
                includeContext.SetValue("include", includeJSON);
            }

            return new StringValue(template.Render(includeContext));
        }
    }
}