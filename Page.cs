using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace WDHAN
{
    public class Page : WDHANFile
    {
        //public JObject frontmatter { get; set; }
        //public string content { get; set; }
        //public string path { get; set; }

        // The above three variables should be specified. The rest are defined by the Constructor.
        public string url { get; set; }
        public DateTime date { get; set; }
        public List<string> tags { get; set; }
        public string dir { get; set; }
        //public string name { get; set; }
        public string excerpt { get; set; }
        // Time-related values
        public string year { get; set; }
        public string short_year { get; set; }
        public string month { get; set; }
        public string i_month { get; set; }
        public string short_month { get; set; }
        public string long_month { get; set; }
        public string day { get; set; }
        public string i_day { get; set; }
        public string y_day { get; set; }
        public string w_year { get; set; }
        public string week { get; set; }
        public int w_day { get; set; }
        public string short_day { get; set; }
        public string long_day { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
        public string second { get; set; }

        public Page()
        {
            getDefinedPage(this);
        }
        public static Page getDefinedPage(Page page)
        {
            page = (Page) getDefinedFile(page);

            page.name = Path.GetFileName(page.path);
            page.dir = Path.GetDirectoryName(page.path);

            try
            {
                page.tags = JsonConvert.DeserializeObject<List<string>>(page.frontmatter.GetValue("tags").ToString());
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.date = JsonConvert.DeserializeObject<DateTime>(page.frontmatter.GetValue("date").ToString());

                // Get time values
                CultureInfo globalCulture = new CultureInfo(GlobalConfiguration.getConfiguration().culture);
                page.year = page.date.ToString("yyyy");
                page.short_year = page.date.ToString("y");
                page.month = page.date.ToString("MM");
                page.i_month = page.date.ToString("M");
                page.short_month = page.date.ToString("MMM");
                page.long_month = page.date.ToString("MMMM");
                page.day = page.date.ToString("dd");
                page.i_day = page.date.ToString("d");
                page.y_day = getDayOfYear(page.date);
                page.w_day = ISOWeek.GetYear(page.date);
                page.week = globalCulture.Calendar.GetWeekOfYear(page.date, globalCulture.DateTimeFormat.CalendarWeekRule, globalCulture.DateTimeFormat.FirstDayOfWeek).ToString();
                page.w_day = (int) globalCulture.Calendar.GetDayOfWeek(page.date);
                page.short_day = globalCulture.Calendar.GetDayOfWeek(page.date).ToString().Substring(0, 2);
                page.long_day = globalCulture.Calendar.GetDayOfWeek(page.date).ToString();
                page.hour = page.date.ToString("HH");
                page.minute = page.date.ToString("mm");
                page.second = page.date.ToString("ss");
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.excerpt = getExcerpt(page);
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.title = page.frontmatter["title"].ToString();
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.url = Permalink.GetPermalink(page).parsePagePermalink(page);
            }
            catch(NullReferenceException)
            {

            }

            return page;
        }
        public static string getDayOfYear(DateTime date)
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
        public static string getExcerptSeparator(Page page)
        {
            try
            {
                return page.frontmatter["excerpt_separator"].ToString();
            }
            catch(NullReferenceException)
            {
                return GlobalConfiguration.getConfiguration().excerpt_separator;
            }
        }
        public static string getExcerpt(Page page)
        {
            string excerpt = "";
            foreach(var currentChar in page.content)
            {
                excerpt += currentChar;
                if(excerpt.Contains(getExcerptSeparator(page)))
                {
                    excerpt = excerpt.Substring(0, getExcerptSeparator(page).Length);
                    break;
                }
                /*
                try
                {
                    if(excerpt.Contains(page.frontmatter["excerpt_separator"].ToString()))
                    {
                        excerpt = excerpt.Substring(0, page.frontmatter["excerpt_separator"].ToString().Length);
                        break;
                    }
                }
                catch(NullReferenceException)
                {
                    if(excerpt.Contains(GlobalConfiguration.getConfiguration().excerpt_separator))
                    {
                        excerpt = excerpt.Substring(0, GlobalConfiguration.getConfiguration().excerpt_separator.Length);
                        break;
                    }
                }
                */
            }
            return excerpt;
        }
        public static JObject getPage(Page page)
        {
            return JObject.Parse(JsonConvert.SerializeObject(page));
        }
    }
}