using System;
using System.Collections.Generic;
using System.Text;

namespace WDHAN
{
    public class ParsedVariable
    {
        string name { get; set; }
        string content { get; set; }
        public ParsedVariable(string name, string content)
        {
            this.name = name;
            this.content = content;
        }
    }
}
