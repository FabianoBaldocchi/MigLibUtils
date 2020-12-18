using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigLibUtils.Agent
{
    public class AgentData
    {
        public string Key { get; set; }

        public Dictionary<string, object> Content = new Dictionary<string, object>();
        
        public List<KeyValuePair<DateTime, string>> Hist = new List<KeyValuePair<DateTime, string>>();

        public virtual bool IsEquals(AgentData other)
        {
            return false;
        }

        public virtual void ProcessDiscarded(AgentData discardedby)
        {
        }
    }
}
