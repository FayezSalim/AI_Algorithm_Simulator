using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
   
    public abstract class  Path_data
    {
        private int cost;
        private int time;
        private int fitness;//fitness

        public int Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        
        public int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }

        public int Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public abstract void addcity(City_data d);

        public abstract List<City_data> returnAsList();
    }
}
