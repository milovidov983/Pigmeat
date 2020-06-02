using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pigmeat.Core;

namespace Pigmeat
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    if (!args[0].Equals("help", StringComparison.OrdinalIgnoreCase) && args.Length > 1)
                    {
                        Directory.SetCurrentDirectory(args[args.Length - 1]);
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("The specified directory does not exist.\n" + e);
                }

                GetCommand(args);
            }
            catch(IndexOutOfRangeException)
            {
                GetCommand(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
        static void GetCommand(string[] args)
        {
            if(args.Length == 0)
            {
                //ShowHelp(args);
            }
            else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
            {
                //Create(args);
            }
            else if (args[0].Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                //Create(args);
            }
            else if (args[0].Equals("build", StringComparison.OrdinalIgnoreCase))
            {
                Build();
            }
            else if (args[0].Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                Build();
            }
            else if (args[0].Equals("clean", StringComparison.OrdinalIgnoreCase))
            {
                Clean();
            }
            else if (args[0].Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                Clean();
            }
            else if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                //ShowHelp(args);
            }
            else if (args[0].Equals("h", StringComparison.OrdinalIgnoreCase))
            {
                //ShowHelp(args);
            }
            else
            {
                //ShowHelp(args);
            }
        }
        static void Build()
        {
            PopulateEntries();
            PopulateEntries();
            /*
            foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
            {
                String Collection = directory.Substring(3);

                // Get Collection data, deserialize entries into List of JObjects
                JObject CollectionObject = JObject.Parse(File.ReadAllText("./_" + Collection + "/collection.json"));
                List<JObject> Entries = JsonConvert.DeserializeObject<List<JObject>>(CollectionObject["entries"].ToString(Formatting.Indented));

                foreach(JObject Entry in Entries)
                {
                    IO.RenderPage(Entry);
                }
            }
            */
            IO.CleanCollections();
        }
        static void PopulateEntries()
        {
            foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
            {
                foreach(var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    if(Path.GetExtension(file).Equals(".md") || Path.GetExtension(file).Equals(".html"))
                    {
                        //IO.AppendEntry(directory.Substring(3), Page.GetPageObject(file));
                        IO.RenderPage(Page.GetPageObject(file), directory.Substring(3));
                    }
                }
            }
        }
        static void Clean()
        {
            IO.CleanCollections();
            try
            {
                Directory.Delete("./output", true);
                Console.WriteLine("Cleaned project directory … ");
            }
            catch(DirectoryNotFoundException)
            {
                // This is expected if there is no directory to clean.
                Environment.Exit(1);
            }
        }
    }
}
