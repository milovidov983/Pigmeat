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
        public Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Parses through each <c>{! snippet !}</c> call in a page and evaluates them
        /// <para>See <see cref="Snippet.Render(string, JObject)"/></para>
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
                string ReaderString = "";
                bool HitFirstBrace = false;
                foreach(var character in Contents)
                {
                    if(character.Equals('{') && !HitFirstBrace)
                    {
                        HitFirstBrace = true;
                        ReaderString += character;
                        continue;
                    }
                    if(character.Equals('}') && HitFirstBrace)
                    {
                        HitFirstBrace = false;
                        ReaderString += character;
                        if(ReaderString.Contains("{! snippet "))
                        {
                            SnippetCalls.Add(ReaderString);
                        }
                        ReaderString = "";
                        continue;
                    }
                    if(HitFirstBrace)
                    {
                        ReaderString += character;
                        continue;
                    }
                }
                foreach(var snippetCall in SnippetCalls)
                {
                    Snippet CurrentSnippet = new Snippet { Input = snippetCall };
                    string SnippetPath = "./snippets/" + CurrentSnippet.GetArguments()[2];
                    Contents = Contents.Replace(snippetCall, CurrentSnippet.Render(SnippetPath, PageObject));
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
        string Render(string SnippetPath, JObject PageObject)
        {
            // Get outside data
            JObject Global = JObject.Parse(IO.GetGlobal());
            Global.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject Pigmeat = IO.GetPigmeat();

            SetVariables();
            JObject SnippetObject = JObject.Parse(JsonConvert.SerializeObject(Variables));
            var template = Template.ParseLiquid(File.ReadAllText(SnippetPath));
            return template.Render(new { snippet = SnippetObject, page = PageObject, global = Global, pigmeat = Pigmeat });
        }

        /// <summary>
        /// Gets the arguments given in the <c>Snippet</c> call, to be parsed through later
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing <c>Snippet</c> arguments
        /// </returns>
        string[] GetArguments()
        {
            List<string> Arguments = new List<string>();
            string CurrentArgument = "";
            foreach(var character in Input)
            {
                if(character.Equals(' '))
                {
                    Arguments.Add(CurrentArgument);
                    CurrentArgument = "";
                    continue;
                }
                else
                {
                    CurrentArgument += character;
                    continue;
                }
            }
            return Arguments.ToArray();
        }

        /// <summary>
        /// Combines data from <see cref="Snippet.GetKeys()"/> and <see cref="Snippet.GetValues()"/>
        /// </summary>
        void SetVariables()
        {
            Dictionary<string, string> GatheredVariables = new Dictionary<string, string>();
            for(int i = 0; i < GetKeys().Length; i++)
            {
                GatheredVariables.Add(GetKeys()[i], GetValues()[i]);
            }
            Variables = GatheredVariables;
        }

        /// <summary>
        /// Gets values of given arguments/variables when the <c>Snippet</c> was called
        /// <para> See <see cref="Snippet.GetKeys()"/> </para>
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing values of given arguments
        /// </returns>
        string[] GetValues()
        {
            List<string> Values = new List<string>();
            string CurrentValue = "";
            for(int i = 3; i < GetArguments().Length; i++)
            {
                bool HitEquals = false; // Whether or not we've reached the equals sign yet
                foreach(var character in GetArguments()[i])
                {
                    if(character.Equals('='))
                    {
                        HitEquals = true;
                        continue;
                    }
                    if(HitEquals)
                    {
                        CurrentValue += character;
                        continue;
                    }
                }
                try
                {
                    if(CurrentValue.ToCharArray()[0].Equals('"') && CurrentValue.ToCharArray()[CurrentValue.Length - 1].Equals('"'))
                    {
                        Values.Add(CurrentValue.Substring(1, CurrentValue.Length - 2));
                    }
                    else
                    {
                        Values.Add(CurrentValue);
                    }
                }
                catch(IndexOutOfRangeException)
                {
                    Values.Add(CurrentValue);
                }
                CurrentValue = "";
            }
            return Values.ToArray();
        }
        
        /// <summary>
        /// Gets the keys (names of variables) given when the Snippet was called
        /// <para> See <see cref="Snippet.GetValues()"/> </para>
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing names of given arguments
        /// </returns>
        string[] GetKeys()
        {
            List<string> Keys = new List<string>();
            string CurrentKey = "";
            for(int i = 3; i < GetArguments().Length; i++)
            {
                foreach(var character in GetArguments()[i])
                {
                    if(character.Equals('='))
                    {
                        Keys.Add(CurrentKey);
                        CurrentKey = "";
                        break;
                    }
                    else
                    {
                        CurrentKey += character;
                        continue;
                    }
                }
            }
            return Keys.ToArray();
        }
    }
}