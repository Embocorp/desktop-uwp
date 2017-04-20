using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class Device
    {
        public Device(String name)
        {
            Name = name;
        }

        public Device() { }

        public static bool operator == (Device a, Device b)
        {
            Liscence aL = a.Liscence;
            Liscence bL = b.Liscence;
            return aL.Key == bL.Key;
        }

        public static bool operator != (Device a, Device b)
        {
            Liscence aL = a.Liscence;
            Liscence bL = b.Liscence;
            return aL.Key != bL.Key;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public String Name { get; set; }
        public String Version { get; set; }
        public Liscence Liscence { get; set; }
        public Sensor[] Sensors { get; set; }
        public Boolean Connected { get; set; }

    }

    public class Liscence
    {
        public Liscence() { }
        
        public string GetLiscenceType()
        {
            return LiscenceName[(int)Type];
        }

        public String Name { get; set; }
        public String Key { get; set; }
        public LiscenceType Type { get; set; }

        private string[] LiscenceName = { "Individual", "Class", "Group" };
        public enum LiscenceType { Individual, Class, Group };
    }
}
