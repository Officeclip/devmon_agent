using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class ChartPoint
    {
        /// <summary>
        /// Minutes before the current time
        /// </summary>
        public int Minutes { get; set; }
        public int Value { get; set; }
    }

    public class ChartLine
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public List<ChartPoint> ChartPoints { get; set; }
        public List<int> ChartPointValues
        {
            get
            {
                return ChartPoints.Select(x => x.Value).ToList();
            }
        }
        public List<int> ChartPointMinutes
        {
            get
            {
                return ChartPoints.Select(x => x.Minutes).ToList();
            }
        }
    }
}