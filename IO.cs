// Copyright (C) 2020 Emil Sayahi
/*
This file is part of Pigmeat.

    Pigmeat is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pigmeat is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pigmeat.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers.Normalize;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using YamlDotNet.Serialization;

namespace Pigmeat.Core
{
    class IO
    {
        static string release = typeof(Program).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        /// <summary>
        /// Convert YAML data into JObject
        /// </summary>
        /// <returns>
        /// JObject form of given YAML data
        /// </returns>
        /// <param name="YamlString">A <c>string</c> containing the YAML data to be converted</param>
        public static JObject GetYamlObject(string YamlString)
        {
            return JObject.Parse(JsonConvert.SerializeObject(new Deserializer().Deserialize(new StringReader(YamlString)), Formatting.None));
        }

        /// <summary>
        /// Get file representing project's <c>Global</c> context
        /// </summary>
        /// <returns>
        /// File contents of <c>./_global.yml</c> as a <c>string</c>
        /// </returns>
        public static string GetGlobal()
        {
            if(File.Exists("./_global.yml"))
            {
                return GetYamlObject(File.ReadAllText("./_global.yml")).ToString(Formatting.None);
            }
            else
            {
                return File.ReadAllText("./_global.json");
            }
        }

        /// <summary>
        /// Get <c>JObject</c> representing project's <c>Pigmeat</c> context
        /// </summary>
        /// <returns>
        /// JSON representation of Pigmeat's internal Liquid context
        /// </returns>
        public static JObject GetPigmeat()
        {
            return JObject.Parse(JsonConvert.SerializeObject(new { version = release, time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }));
        }

        /// <summary>
        /// Adds <c>JObject</c> representations of pages in a collection to the collection's <c>entries</c> field in its <c>collection.json</c> file
        /// <para> See <see cref="IO.RenderPage(JObject, string, string, Boolean)"/> </para>
        /// </summary>
        /// <param name="Collection">The name of the collection the page is in</param>
        /// <param name="Entry">The <c>JObject</c> form of the page</param>
        public static void AppendEntry(string Collection, JObject Entry)
        {
            // Get Collection data, deserialize entries into List of JObjects
            JObject CollectionObject = JObject.Parse(File.ReadAllText("./_" + Collection + "/collection.json"));
            List<JObject> Entries = JsonConvert.DeserializeObject<List<JObject>>(CollectionObject["entries"].ToString(Formatting.Indented));
            Entries.Add(Entry); // Add current entry to List

            try
            {
                // Sort by date, then title
                Entries.Sort((y, x) => x["date"].ToString().CompareTo(y["date"].ToString()));
                Entries.Sort((y, x) => x["title"].ToString().CompareTo(y["title"].ToString()));
            }
            catch(NullReferenceException)
            {
                Entries.Sort((y, x) => x["title"].ToString().CompareTo(y["title"].ToString()));
            }
            catch(InvalidOperationException)
            {

            }

            var DeserializedCollection = JsonConvert.DeserializeObject<Dictionary<string, object>>(CollectionObject.ToString(Formatting.Indented));
            DeserializedCollection["entries"] = Entries.ToArray(); // Add List into JSON

            File.WriteAllText("./_" + Collection + "/collection.json", JsonConvert.SerializeObject(DeserializedCollection, Formatting.Indented));
        }

        /// <summary>
        /// Clean out the <c>entries</c> field in every <c>collection.json</c> file
        /// </summary>
        public static void CleanCollections()
        {
            foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
            {
                string Collection = directory.Substring(3);
                // Get Collection data, deserialize entries into List of JObjects
                JObject CollectionObject = JObject.Parse(File.ReadAllText("./_" + Collection + "/collection.json"));
                object[] Entries = new object[0];

                var DeserializedCollection = JsonConvert.DeserializeObject<Dictionary<string, object>>(CollectionObject.ToString(Formatting.Indented));
                DeserializedCollection["entries"] = new int[] { }; // Add empty List into JSON

                File.WriteAllText("./_" + Collection + "/collection.json", JsonConvert.SerializeObject(DeserializedCollection, Formatting.Indented));
            }
        }

        /// <summary>
        /// Create a <c>JObject</c> to merge with the <c>Global</c> context containing each collection's <c>collection.json</c> data
        /// <para> See <see cref="IO.RenderRaw(JObject, Boolean)"/></para>
        /// <seealso cref="IO.RenderPage(JObject, string, string, Boolean)"/>
        /// <seealso cref="Include.Render(string, JObject)"/>
        /// <seealso cref="Page.GetPageObject(string, Boolean)"/>
        /// </summary>
        /// <returns>
        /// A <c>JObject</c> containing collections as keys and their data as values
        /// </returns>
        public static JObject GetCollections()
        {
            Dictionary<string, JObject> Collections = new Dictionary<string, JObject>();
            foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
            {
                Collections.Add(directory.Substring(3), JObject.Parse(File.ReadAllText(directory + "/collection.json")));
            }
            return JObject.Parse(JsonConvert.SerializeObject(Collections));
        }

        /// <summary>
        /// Render with Scriban and Markdig
        /// </summary>
        /// <returns>
        /// Rendered form of given file
        /// </returns>
        /// <param name="PageObject">A <c>JObject</c> representing the file</param>
        /// <param name="RenderHTML">Whether or not to run it through Markdig</param>
        public static string RenderRaw(JObject PageObject, Boolean RenderHTML)
        {
            string PageContents = PageObject["content"].ToString();
            if(RenderHTML)
            {
                // Turn Markdown into HTML
                var builder = new MarkdownPipelineBuilder().UsePipeTables().UseEmphasisExtras().UseAutoLinks().UseTaskLists().UseListExtras().UseMediaLinks().UseMathematics().UseDiagrams();
                builder.BlockParsers.TryRemove<IndentedCodeBlockParser>();
                var pipeline = builder.Build();
                PageContents = Markdown.ToHtml(Markdown.Normalize(PageContents, new NormalizeOptions() { ExpandAutoLinks = true }, pipeline), pipeline);
            }

            // Get outside data
            JObject Global = JObject.Parse(GetGlobal());
            Global.Merge(JObject.Parse(GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject Pigmeat = GetPigmeat();

            // Parse for includes
            PageContents = Include.Parse(PageContents, PageObject);

            var template = Template.ParseLiquid(PageContents);
            PageContents = template.Render(new { page = PageObject, global = Global, pigmeat = Pigmeat }); // Render with Scriban

            return PageContents;
        }

        /// <summary>
        /// Take layout, place Markdig-parsed content in layout, evaluate includes, render with Scriban
        /// </summary>
        /// <param name="PageObject">The <c>JObject</c> representing the page being rendered</param>
        /// <param name="Collection">The name of the collection the page is in</param>
        /// <param name="FilePath">The path of the page</param>
        /// <param name="RenderHTML">Whether or not to run it through Markdig</param>
        /// <para> See <see cref="IO.AppendEntry(string, JObject)"/> </para>
        /// <seealso cref="IO.GetCollections"/>
        public static void RenderPage(JObject PageObject, string Collection, string FilePath, Boolean RenderHTML)
        {
            string PageContents = PageObject["content"].ToString();
            if(RenderHTML)
            {
                // Turn Markdown into HTML
                var builder = new MarkdownPipelineBuilder().UsePipeTables().UseEmphasisExtras().UseAutoLinks().UseTaskLists().UseListExtras().UseMediaLinks().UseMathematics().UseDiagrams();
                builder.BlockParsers.TryRemove<IndentedCodeBlockParser>();
                var pipeline = builder.Build();
                PageContents = Markdown.ToHtml(Markdown.Normalize(PageContents, new NormalizeOptions() { ExpandAutoLinks = true }, pipeline), pipeline);
            }
            
            // Get outside data
            JObject Global = JObject.Parse(GetGlobal());
            Global.Merge(JObject.Parse(GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject Pigmeat = GetPigmeat();

            // If a page has a layout, use it
            if(PageObject.ContainsKey("layout"))
            {
                string OldPageContents = PageContents;
                PageContents = GetLayoutContents("./layouts/" + PageObject["layout"].ToString() + ".html", FilePath).Replace("{{ content }}", PageContents);
            }

            //Render with Scriban
            PageContents = Include.Parse(PageContents, PageObject); // Parse for includes
            var template = Template.ParseLiquid(PageContents);
            PageContents = template.Render(new { page = PageObject, global = Global, pigmeat = Pigmeat });

            Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
            File.WriteAllText("./output/" + PageObject["url"].ToString(), PageContents);
            if(!string.IsNullOrEmpty(Collection))
            {
                IO.AppendEntry(Collection, PageObject);
            }
        }

        /// <summary>
        /// Get the contents of <c>Layout</c>s recursively
        /// </summary>
        /// <returns>
        /// The contents of the <c>Layout</c> given
        /// </returns>
        /// <param name="LayoutPath">The path to the <c>Layout</c></param>
        /// <param name="FilePath">The path to the file requesting this <c>Layout</c></param>
        /// <para>See <see cref="IO.RenderPage(JObject, string, string, Boolean)"/> </para>
        static string GetLayoutContents(string LayoutPath, string FilePath)
        {
            string PageFrontmatter = Page.GetFrontmatter(LayoutPath);
            string LayoutContents = File.ReadAllText(LayoutPath).Replace(PageFrontmatter + "---", "");
            try
            {
                string SubLayout = IO.GetYamlObject(PageFrontmatter)["layout"].ToString();
                string SubLayoutPath = "./layouts/" + SubLayout + ".html";
                LayoutContents = GetLayoutContents(SubLayoutPath, FilePath).Replace("{{ content }}", LayoutContents);
                return LayoutContents;
            }
            catch(Exception)
            {
                return LayoutContents;
            }
        }

        /// <summary>
        /// Copy a directory recursively, for if in <c>{{{ global.include }}}</c>
        /// </summary>
        /// <param name="SourceDirectory">The directory included</param>
        /// <param name="DestinationDirectory">Where the directory will be output</param>
        public static void IncludeDirectory(string SourceDirectory, string DestinationDirectory)
        {
            DirectoryInfo DirectoryObject = new DirectoryInfo(SourceDirectory);
            DirectoryInfo[] SubDirectories = DirectoryObject.GetDirectories();
            Directory.CreateDirectory(DestinationDirectory);
            FileInfo[] Files = DirectoryObject.GetFiles();
            foreach (var file in Files)
            {
                file.CopyTo(Path.Combine(DestinationDirectory, file.Name), true);
            }
            foreach (var SubDirectory in SubDirectories)
            {
                IncludeDirectory(SubDirectory.FullName, Path.Combine(DestinationDirectory, SubDirectory.Name));
            }
        }
    }
}