using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace Pigmeat.Core
{
    /// <summary>
    /// The <c>Include</c> class.
    /// Contains all methods related to handling <c>{! inc !}</c> calls.
    /// </summary>
    class Include
    {
        public string Input { get; set; }
        public Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Parses through each <c>{! inc !}</c> call in a page and evaluates them
        /// <para>See <see cref="Include.Render(string, JObject)"/></para>
        /// </summary>
        /// <returns>
        /// Contents of a page with <c>Include</c>s evaluated
        /// </returns>
        /// <param name="Contents">A <c>string</c> containing the contents of the page being parsed</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        public static string Parse(string Contents, JObject PageObject)
        {
            if(Contents.Contains("{! inc "))
            {
                List<string> IncludeCalls = new List<string>();
                string ReaderString = "";
                Boolean HitFirstBrace = false;
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
                        if(ReaderString.Contains("{! inc "))
                        {
                            IncludeCalls.Add(ReaderString);
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
                foreach(var includeCall in IncludeCalls)
                {
                    Include CurrentInclude = new Include { Input = includeCall };
                    string IncludePath = "./includes/" + CurrentInclude.GetArguments()[2];
                    Contents = Contents.Replace(includeCall, CurrentInclude.Render(IncludePath, PageObject));
                }
            }
            return Contents;
        }

        /// <summary>
        /// Renders <c>Include</c>s
        /// </summary>
        /// <returns>
        /// The evaluated <c>Include</c>
        /// </returns>
        /// <param name="IncludePath">The path to the <c>Include</c> being rendered</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        string Render(string IncludePath, JObject PageObject)
        {
            // Get outside data
            JObject Global = JObject.Parse(IO.GetGlobal());
            Global.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject Pigmeat = IO.GetPigmeat();

            SetVariables();
            JObject IncludeObject = JObject.Parse(JsonConvert.SerializeObject(Variables));
            var template = Template.ParseLiquid(File.ReadAllText(IncludePath));
            return template.Render(new { include = IncludeObject, page = PageObject, global = Global, pigmeat = Pigmeat });
        }

        /// <summary>
        /// Gets the arguments given in the <c>Include</c> call, to be parsed through later
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing <c>Include</c> arguments
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
        /// Combines data from <see cref="Include.GetKeys()"/> and <see cref="Include.GetValues()"/>
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
        /// Gets values of given arguments/variables when the <c>Include</c> was called
        /// <para> See <see cref="Include.GetKeys()"/> </para>
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
                Boolean HitEquals = false; // Whether or not we've reached the equals sign yet
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
        /// Gets the keys (names of variables) given when the Include was called
        /// <para> See <see cref="Include.GetValues()"/> </para>
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