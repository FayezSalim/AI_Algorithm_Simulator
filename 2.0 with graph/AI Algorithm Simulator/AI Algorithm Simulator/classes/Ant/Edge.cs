using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class Edge:Path //store all the paths
    {

        public double phermone { get; set; }
        
        public Edge()
        {
           
        }

        public Edge(Edge c): base(c)
        {
            this.phermone = c.phermone;
        }
    }
}
