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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace Pigmeat.Core
{
    /// <summary>
    /// The <c>Snippet</c> class.
    /// Contains all methods related to handling <c>{! snippet !}</c> calls.
    /// </summary>
    class Snippet
    {
        public string Input { get; set; }
        public static Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Parses through each <c>{! snippet !}</c> call in a page and evaluates them
        /// <para>See <see cref="Snippet.Render(string, JObject)"/></para>
        /// <seealso cref="IO.RenderPage(JObject, string, bool, bool, JObject)"/>
        /// </summary>
        /// <returns>
        /// Contents of a page with <c>Snippet</c>s evaluated
        /// </returns>
        /// <param name="Contents">A <c>string</c> containing the contents of the page being parsed</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        public static string Parse(string Contents, JObject PageObject)
        {
            if(Contents.Contains("{! snippet "))
            {
                List<string> SnippetCalls = new List<string>();
                StringBuilder Reader = new StringBuilder();
                int BraceCount = 0;
                foreach(var character in Contents)
                {
                    if(character.Equals('{') && BraceCount == 0)
                    {
                        BraceCount++;
                        Reader.Append(character);
                        continue;
                    }
                    if(character.Equals('}') && BraceCount == 1)
                    {
                        BraceCount = 0;
                        Reader.Append(character);
                        var ReaderString = Reader.ToString();
                        if(ReaderString.Contains("{! snippet ") || ReaderString.Contains("{!snippet "))
                        {
                            SnippetCalls.Add(WebUtility.HtmlDecode(ReaderString));
                        }
                        Reader = new StringBuilder();
                        continue;
                    }
                    if(BraceCount == 1)
                    {
                        Reader.Append(character);
                        continue;
                    }
                }
                foreach(var snippetCall in SnippetCalls)
                {
                    Variables = new Dictionary<string, string>(); // Reset variable data for each new call
                    Regex CallSplit = new Regex("=| (?=(\\w*)+=)"); // Regex to split based on space (not in quotes) and equals sign
                    // Do the split, remove bits of call syntax, trim, remove empty values created by the split
                    string[] CallArguments = CallSplit.Split(snippetCall.Replace("snippet ", "").Replace("{!", "").Replace("!}", "").Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    List<string> Keys = new List<string>();
                    List<string> Values = new List<string>();
                    for(int i = 0; i < CallArguments.Length - 1; i++) // Offset of one to ignore snippet filename
                    {
                        switch(i % 2)
                        {
                            case 0:
                                Keys.Add(CallArguments[i + 1]);
                                break;
                            case 1:
                                Values.Add(CallArguments[i + 1]);
                                break;
                        }
                    }
                    for(int i = 0; i < Keys.ToArray().Length; i++)
                    {
                        Variables.Add(Keys[i], Values[i]); // Merge `List`s into `Dictionary`
                    }
                    string SnippetPath = "./snippets/" + CallArguments[0]; // Trimming required for this to work
                    Contents = Contents.Replace(WebUtility.HtmlEncode(snippetCall), Snippet.Render(SnippetPath, PageObject));
                }
            }
            return Contents;
        }

        /// <summary>
        /// Renders <c>Snippet</c>s
        /// </summary>
        /// <returns>
        /// The evaluated <c>Snippet</c>
        /// </returns>
        /// <param name="SnippetPath">The path to the <c>Snippet</c> being rendered</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        static string Render(string SnippetPath, JObject PageObject)
        {
            // Get outside data
            JObject Global = JObject.Parse(IO.GetGlobal());
            Global.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject CollectionObject = new JObject {};
            if(PageObject.ContainsKey("collection"))
            {
                // Can't use `Collection` parameter, as sometimes we don't want the output filed as an entry for said collection
                CollectionObject = JObject.Parse(File.ReadAllText("./_" + PageObject["collection"].ToString() + "/collection.json"));

            }
            JObject SnippetObject = JObject.Parse("{\n\t\"exists\": true\n}");
            
            if(Variables != null)
            {
                SnippetObject = JObject.Parse(JsonConvert.SerializeObject(Variables, Formatting.None));
            }
            var template = Template.ParseLiquid(File.ReadAllText(SnippetPath));
            var SnippetContents = template.Render(new { snippet = SnippetObject, page = PageObject, collection = CollectionObject, global = Global, pigmeat = IO.GetPigmeat() });
            SnippetContents = Snippet.Parse(SnippetContents, PageObject); // Parse for snippets
            return SnippetContents;
        }
    }
}