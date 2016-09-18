using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class Path
    {
        public String city1{get;set;}
        public String city2 { get; set; }
        public int cost{get;set;}
        public int time{get;set;}

        public Path()
        {

        }
        public Path(Path c)
        {
            this.city1 = c.city1;
            this.city2 = c.city2;
            this.cost = c.cost;
            this.time = c.time;
        }

    }
}
