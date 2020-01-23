using System;
using System.Collections.Generic;
using System.Text;

namespace WDHAN
{
    public class Collection
    {
        //public string name { get; set; }
        //public string[] variableNames { get; set; }
        //public object[] variableValues { get; set; }
        public List<Dictionary<string, object>> variables;
        public Collection(List<Dictionary<string, object>> variables)
        {
            //this.name = name;
            //this.variableNames = variableNames;
            //this.variableValues = variableValues;
            this.variables = variables;
        }
    }
}
