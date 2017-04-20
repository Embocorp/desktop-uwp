using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class DataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string XLabel { get; set; }
        public string YLabel { get; set; }
        public string Series { get; set; }

        public DataPoint() { }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class Measurement
    {
        public Measurement() { }

        public Measurement(string name, string[] unit)
        {
            Name = name;
            Unit = unit;
        }

        override public string ToString ()
        {
            StringBuilder s = new StringBuilder();
            foreach (string i in Unit)
            {
                s.Append(i + ", ");
            }
            s.Length -= 2;
            return Name + " (" + s.ToString() + ")"; 
        }
        public string Name { get; set; }
        public string[] Unit { get; set; }
        public bool Device { get; set; }
    }
}
