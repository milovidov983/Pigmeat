using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace Pigmeat.Core
{
    class Page
    {
        public string year { get; set; }
        public string short_year { get; set; }
        public string month { get; set; }
        public string i_month { get; set; }
        public string short_month { get; set; }
        public string long_month { get; set; }
        public string day { get; set; }
        public string i_day { get; set; }
        public string y_day { get; set; }
        public int w_year { get; set; }
        public string week { get; set; }
        public int w_day { get; set; }
        public string short_day { get; set; }
        public string long_day { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
        public string second { get; set; }
        public DateTime date { get; set; }
        public string dir { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
        public string content { get; set; }

        /// <summary>
        /// Parses a given page into a <c>JObject</c>
        /// </summary>
        /// <returns>
        /// The JSON representation of a page and its metadata
        /// </returns>
        /// <param name="PagePath">The path to the page being parsed</param>
        /// <param name="RenderHTML">Whether or not to run it through Markdig</param>
        /// <para> See <see cref="IO.GetCollections"/> </para>
        public static JObject GetPageObject(string PagePath, Boolean RenderHTML)
        {
            string PageFrontmatter = GetFrontmatter(PagePath);
            JObject FrontmatterObject = IO.GetYamlObject(PageFrontmatter);
            Page page = JsonConvert.DeserializeObject<Page>(FrontmatterObject.ToString(Formatting.None));

            page.name = Path.GetFileNameWithoutExtension(PagePath);
            page.dir = Path.GetDirectoryName(PagePath);
            page.content = File.ReadAllText(PagePath).Replace(PageFrontmatter + "---", "");

            JObject GlobalObject = JObject.Parse(IO.GetGlobal());
            GlobalObject.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            CultureInfo Culture = new CultureInfo(GlobalObject["culture"].ToString()); // Get 'culture' from Global
            page.year = page.date.ToString("yyyy");
            page.short_year = page.date.ToString("y");
            page.month = page.date.ToString("MM");
            page.i_month = page.date.ToString("M");
            page.short_month = page.date.ToString("MMM");
            page.long_month = page.date.ToString("MMMM");
            page.day = page.date.ToString("dd");
            page.i_day = page.date.ToString("d");
            page.y_day = GetDayOfYear(page.date);
            page.week = Culture.Calendar.GetWeekOfYear(page.date, Culture.DateTimeFormat.CalendarWeekRule, Culture.DateTimeFormat.FirstDayOfWeek).ToString();
            page.w_day = (int) Culture.Calendar.GetDayOfWeek(page.date);
            page.w_year = ISOWeek.GetYear(page.date);
            page.short_day = Culture.Calendar.GetDayOfWeek(page.date).ToString().Substring(0, 2);
            page.long_day = Culture.Calendar.GetDayOfWeek(page.date).ToString();
            page.hour = page.date.ToString("HH");
            page.minute = page.date.ToString("mm");
            page.second = page.date.ToString("ss");

            JObject EarlyPageObject = JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None));
            EarlyPageObject.Merge(JObject.Parse(FrontmatterObject.ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            page.url = GetPermalink(EarlyPageObject); // Generate output path based on other variables
            page.content = IO.RenderRaw(EarlyPageObject, RenderHTML);

            JObject PageObject = JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None));
            PageObject.Merge(JObject.Parse(FrontmatterObject.ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            return PageObject; // Return JObject of page
        }
        
        /// <summary>
        /// Gets the YAML of the frontmatter for a given page
        /// </summary>
        /// <returns>
        /// The YAML <c>string</c> for a given page
        /// </returns>
        /// <param name="PagePath">The path of the page being parsed</param>
        /// <para> See <see cref="IO.GetLayoutContents(string, string)"/> </para>
        /// <seealso cref="Page.GetPageObject(string, Boolean)"/>
        public static string GetFrontmatter(string PagePath)
        {
            string FrontMatter = "";
            foreach(var line in File.ReadAllLines(PagePath))
            {
                if(!line.Equals("---"))
                {
                    FrontMatter += line + "\n";
                }
                else
                {
                    break;
                }
            }
            return FrontMatter;
        }
        
        /// <summary>
        /// Parses the permalink using given metadata to generate an output path
        /// </summary>
        /// <returns>
        /// A <c>string</c> pointing to the page's output path
        /// </returns>
        /// <param name="PageObject">The <c>JObject</c> holding the page's metadata</param>
        /// <para> See <see cref="Page.GetPageObject(string, Boolean)"/> </para>
        static string GetPermalink(JObject PageObject)
        {
            string Permalink = PageObject["permalink"].ToString();
            if(Permalink.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("pretty", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("ordinal", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.y_day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("weekdate", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/W{{ page.week }}/{{ page.short_day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.title }}.html";
            }
            var template = Template.ParseLiquid(Permalink);
            return template.Render(new { page = PageObject, global =  IO.GetGlobal()});
        }
        static string GetDayOfYear(DateTime date)
        {
            if(date.DayOfYear.ToString().Length == 1)
            {
                return "00" + date.DayOfYear.ToString();
            }
            else if(date.DayOfYear.ToString().Length == 2)
            {
                return "0" + date.DayOfYear.ToString();
            }
            else
            {
                return date.DayOfYear.ToString();
            }
        }
    }
}