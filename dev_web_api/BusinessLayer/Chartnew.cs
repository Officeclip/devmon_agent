using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class ChartNew
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public List<int> Values { get; set; }

        public ChartNew (int agentId, string agentName, int count)
        {
            AgentId = agentId;
            AgentName = agentName;
            Values = Enumerable.Repeat(-1, count).ToList();
        }

        /// <summary>
        /// If there is a -1 in the middle then replace it with the average to two values
        /// </summary>
        public void FixChart()
        {
            for (int i=1; i<Values.Count-1; i++)
            {
                if (
                    (Values[i] == -1) && (Values[i-1] > 0) && (Values[i+1] > 0))
                {
                    Values[i] = (int)(Values[i - 1] + Values[i + 1]) / 2;
                }
            }
        }
    }
}