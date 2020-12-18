using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Services.LearnWorlds
{
    public class Base
    {
        public bool success { get; set; }
    }

    public class SSOReturn : Base
    {
        public string url { get; set; }
        public string user_id { get; set; }
    }

}
