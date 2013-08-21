using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOCL.Shared.Utils
{
    public class EasyScript
    {

        public class ESProperty
        {
            public string Value;
            public Dictionary<string, ESProperty> SubValues; 
        }
    }

}
