using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        public int DefaultFontSize { get; set; } = 50;
        public Title Title { get; set; }
        public bool Responsive { get; set; } = true;       

    }

    public class Title
    {
        public string Text { get; set; } = "Your Chart Title";
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
            XAxes = new List<Ticks>();
            YAxes = new List<Ticks>();
        }
        public List<Ticks> XAxes { get; set; }
        public List<Ticks>  YAxes { get; set; }
    }

    public class XAxes
    {
        public List<Ticks> ticks { get; set; }
    }

    public class YAxes
    {
        public List<Ticks> ticks { get; set; }
    }

    public class Ticks
    {
        public bool Display { get; set; }
        public bool BeginAtZero { get; set; }
        public int Max { get; set; }
        public int MaxTickLimit { get; set; }
        public JRaw Callback { get; set; } //https://stackoverflow.com/a/16800514/89256
    }
}