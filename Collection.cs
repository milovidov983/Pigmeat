using System;
using System.Collections.Generic;
using System.Text;

namespace WDHAN
{
    public class Collection
    {
        //public string name { get; set; }
        public List<string> variableNames { get; set; }
        public List<Object> variableValues { get; set; }
        public Collection(List<string> variableNames, List<Object> variableValues)
        {
            //this.name = name;
            this.variableNames = variableNames;
            this.variableValues = variableValues;
        }
    }
}
