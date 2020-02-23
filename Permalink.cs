using System;
using System.Collections.Generic;
using System.IO;

namespace WDHAN
{
    public class Permalink
    {
        public string permalink { get; set; }
        public Permalink()
        {
            
        }
        public static Permalink GetPermalink(Page page)
        {
            try
            {
                return new Permalink { permalink = page.frontmatter["permalink"].ToString() };
            }
            catch(NullReferenceException)
            {
                return new Permalink { permalink = GlobalConfiguration.getConfiguration().permalink };
            }
        }
        public string parsePostPermalink(string collectionName, Post post)
        {
            if(permalink.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:collection/:year/:month/:day/:title:output_ext";
                parsePostPermalink(collectionName, post);
            }
            else if(permalink.Equals("pretty", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:collection/:year/:month/:day/:title:output_ext";
                parsePostPermalink(collectionName, post);
            }
            else if(permalink.Equals("ordinal", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:collection/:year/:y_day/:title:output_ext";
                parsePostPermalink(collectionName, post);
            }
            else if(permalink.Equals("weekdate", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:collection/:year/W:week/:short_day/:title:output_ext";
                parsePostPermalink(collectionName, post);
            }
            else if(permalink.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:collection/:title:output_ext";
                parsePostPermalink(collectionName, post);
            }
            else if(permalink.Equals("", StringComparison.OrdinalIgnoreCase))
            {
                permalink = Path.GetDirectoryName(post.path) + "/" + GlobalConfiguration.getConfiguration().destination + "/" + Path.GetFileName(post.path);
            }
            else
            {
                permalink = permalink = permalink.Replace(":collection", collectionName);
                permalink = permalink = permalink.Replace(":path", post.path);
                permalink = permalink = permalink.Replace(":name", post.name);
                permalink = permalink = permalink.Replace(":title", post.getTitle());
                permalink = permalink = permalink.Replace(":output_ext", ".html");
                permalink = permalink.Replace(":basename", post.basename);
                permalink = permalink.Replace(":year", post.year);
                permalink = permalink.Replace(":short_year", post.short_year);
                permalink = permalink.Replace(":month", post.month);
                permalink = permalink.Replace(":i_month", post.i_month);
                permalink = permalink.Replace(":short_month", post.short_month);
                permalink = permalink.Replace(":long_month", post.long_month);
                permalink = permalink.Replace(":day", post.day);
                permalink = permalink.Replace(":i_day", post.i_day);
                permalink = permalink.Replace(":y_day", post.y_day);
                permalink = permalink.Replace(":w_year", post.w_year);
                permalink = permalink.Replace(":week", post.week);
                permalink = permalink.Replace(":w_day", post.w_day.ToString());
                permalink = permalink.Replace(":short_day", post.short_day);
                permalink = permalink.Replace(":long_day", post.long_day);
                permalink = permalink.Replace(":hour", post.hour);
                permalink = permalink.Replace(":minute", post.minute);
                permalink = permalink.Replace(":second", post.second);
            }
            return permalink;
        }
        public string parsePagePermalink(Page page)
        {
            if(permalink.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:year/:month/:day/:title:output_ext";
                parsePagePermalink(page);
            }
            else if(permalink.Equals("pretty", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:year/:month/:day/:title:output_ext";
                parsePagePermalink(page);
            }
            else if(permalink.Equals("ordinal", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:year/:y_day/:title:output_ext";
                parsePagePermalink(page);
            }
            else if(permalink.Equals("weekdate", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:year/W:week/:short_day/:title:output_ext";
                parsePagePermalink(page);
            }
            else if(permalink.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                permalink = "/:title:output_ext";
                parsePagePermalink(page);
            }
            else if(permalink.Equals("", StringComparison.OrdinalIgnoreCase))
            {
                permalink = Path.GetDirectoryName(page.path) + "/" + GlobalConfiguration.getConfiguration().destination + "/" + Path.GetFileName(page.path);
            }
            else
            {
                permalink = permalink.Replace(":path", page.path);
                permalink = permalink.Replace(":name", page.name);
                if(!page.getTitle().Equals("", StringComparison.OrdinalIgnoreCase))
                {
                    permalink = permalink.Replace(":title", page.getTitle());
                }
                else
                {
                    permalink = permalink.Replace(":title", page.basename);
                }
                if(GlobalConfiguration.isMarkdown(Path.GetExtension(page.path).Substring(1)))
                {
                    permalink = permalink.Replace(":output_ext", ".html");
                }
                else
                {
                    permalink = permalink.Replace(":output_ext", Path.GetExtension(page.path));
                }
                permalink = permalink.Replace(":basename", page.basename);
                permalink = permalink.Replace(":year", page.year);
                permalink = permalink.Replace(":short_year", page.short_year);
                permalink = permalink.Replace(":month", page.month);
                permalink = permalink.Replace(":i_month", page.i_month);
                permalink = permalink.Replace(":short_month", page.short_month);
                permalink = permalink.Replace(":long_month", page.long_month);
                permalink = permalink.Replace(":day", page.day);
                permalink = permalink.Replace(":i_day", page.i_day);
                permalink = permalink.Replace(":y_day", page.y_day);
                permalink = permalink.Replace(":w_year", page.w_year);
                permalink = permalink.Replace(":week", page.week);
                permalink = permalink.Replace(":w_day", page.w_day.ToString());
                permalink = permalink.Replace(":short_day", page.short_day);
                permalink = permalink.Replace(":long_day", page.long_day);
                permalink = permalink.Replace(":hour", page.hour);
                permalink = permalink.Replace(":minute", page.minute);
                permalink = permalink.Replace(":second", page.second);
            }
            return permalink;
        }
    }
}