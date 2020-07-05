using System.Collections.Generic;
using System.ComponentModel;

namespace ChartServerConfiguration.Model
{
    public class DataSetItem
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public int BorderWidth { get; set; }
        public List<string> BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        [DefaultValue(true)]
        public bool Fill { get; set; } = true;

    }
}