using System;
using System.Collections.Generic;
using System.Text;

namespace WDHAN
{
    public class Collection
    {
        public string name { get; set; }
        public Boolean output { get; set; }
        public Collection(string name, Boolean output)
        {
            this.name = name;
            this.output = output;
        }
        public override string ToString()
        {
            return name + ": \n" + "output: " + output;
        }
    }
}
