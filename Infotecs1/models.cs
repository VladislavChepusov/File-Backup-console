using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infotecs1
{
    //Класс для десериализации json
    public class models
    {
        [Newtonsoft.Json.JsonProperty("Source")]
        //public string Source { get; set; }
        public List<string> Source { get; set; }

        [Newtonsoft.Json.JsonProperty("Target")]
        public string Target { get; set; }
    }

}
