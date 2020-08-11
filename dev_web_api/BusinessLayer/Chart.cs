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
        public int Unit { get; set; }
        public int Value { get; set; }
    }

    public class ChartLine
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public List<ChartPoint> ChartPoints { get; set; }

        public ChartLine(int agentId, string agentName, int count)
        {
            AgentId = agentId;
            AgentName = agentName;
            ChartPoints = new List<ChartPoint>();
            for (int i = 0; i < count; i++)
            {
                var chartPoint = new ChartPoint()
                {
                    Unit = i,
                    Value = -1
                };
                ChartPoints.Add(chartPoint);
            }
        }

        /// <summary>
        /// If there is a -1 in the middle then replace it with the average to two values
        /// </summary>
        public void FixChart()
        {
            for (int i = 1; i < ChartPoints.Count - 1; i++)
            {
                var currentValue = ChartPoints[i].Value;
                var previousValue = ChartPoints[i-1].Value;
                var nextValue = ChartPoints[i+1].Value;
                if (
                    (currentValue == -1) && 
                    (previousValue > 0) && 
                    (nextValue > 0))
                {
                    ChartPoints[i].Value = (int)(previousValue + nextValue) / 2;
                }
            }
        }
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
                return ChartPoints.Select(x => x.Unit).ToList();
            }
        }
    }
}