using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WDHAN
{
    public class Post
    {
        public string title { get; set; }
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public Post()
        {
            
        }
    }
}