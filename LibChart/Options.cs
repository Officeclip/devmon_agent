using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.AccessControl;

namespace ChartServerConfiguration.Model
{
    public class Options
    {
        public Options()
        {
            Title = new Title();
        }
        public Scales Scales { get; set; }
        public string DefaultColor { get; set; }

        [DefaultValue(50)]
        public int DefaultFontSize { get; set; } = 50;
        public Title Title { get; set; }
        [DefaultValue(true)]
        public bool Responsive { get; set; } = true;       

    }

    public class Title
    {
        public string Text { get; set; }
        public string Position { get; set; } = "top";
        public bool Display { get; set; } = true;
    }

    public class Animation
    {

    }

    public class Scales
    {
        public Scales()
        {
            XAxes = new List<TicksItem>();
            YAxes = new List<TicksItem>();
        }
        public List<TicksItem> XAxes { get; set; }
        public List<TicksItem>  YAxes { get; set; }
    }

    public class TicksItem
    {
        public Ticks ticks { get; set; }
    }

    public class Ticks
    {
        [DefaultValue(true)]
        public bool Display { get; set; } = true;

        [DefaultValue(true)]
        public bool BeginAtZero { get; set; } = true;

        [DefaultValue(0)]
        public int Max { get; set; } = 0;

        [DefaultValue(0)]
        public int MaxTicksLimit { get; set; } = 0;

        public JRaw Callback { get; set; } //https://stackoverflow.com/a/16800514/89256
    }
}