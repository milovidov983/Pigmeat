using System.Collections.Generic;
using Fluid;
using Fluid.Values;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace WDHAN
{
    public class Include
    {
        public Dictionary<string, string> variables { get; set; }
        public string input { get; set; }
        public Include()
        {
            
        }
        public string parseInclude(string includePath)
        {
            setVariables();

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

            if (FluidTemplate.TryParse(includeFile, out var template))
            {
                context.SetValue("include", givenModel);
            }

            return template.Render(context);
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