using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WDHAN
{
    public class Collection
    {
        public Dictionary<string, object> entries { get; set; }
        public Collection()
        {
            
        }
    }
}