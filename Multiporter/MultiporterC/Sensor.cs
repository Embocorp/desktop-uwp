using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class Sensor
    {
        public Sensor(String name, String part, String man, Measurement[] measure)
        {
            Name = name;
            Part = part;
            Manufacturer = man;
            Measurements = measure;
        }

        public Sensor() { }

        public string GetMeasurements ()
        {
            StringBuilder s = new StringBuilder();
            foreach (Measurement i in Measurements)
            {
                s.Append(i.ToString() + ", ");
            }
            s.Length -= 2;
            return s.ToString();
        }

        public string Name { get; set; }
        public string Part { get; set; }
        public string Manufacturer { get; set; }
        public Measurement[] Measurements { get; set; }
    }
}
