using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Analisador_Léxico
{
    public class IdentifyDelimiter
    {
        public string Delimiter { get; set; }

        public string Identifier { get; set; }

        public static List<IdentifyDelimiter> List()
        {
            List<IdentifyDelimiter> list = new List<IdentifyDelimiter>();

            list.Add(new IdentifyDelimiter { Delimiter = "\n", Identifier = "New Line" });
            list.Add(new IdentifyDelimiter { Delimiter = "\t", Identifier = "TAB" });
            list.Add(new IdentifyDelimiter { Delimiter = "\f", Identifier = "Form Feed" });

            return list;
        }
    
    }
}