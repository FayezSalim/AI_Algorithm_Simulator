using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class Windowing:IFitnessAlgorithm
    {
        public static IFitnessAlgorithm Create()
        {
            Windowing w = new Windowing();
            return w;
        }

        public void execute(List<genome> data,string mode)
        {
            if (mode == "cost")
            {
                genome least = new genome();
                int maxcost = 0;//find worst subject ie maximum cost 
                foreach (var item in data)
                {
                    if (maxcost < item.Cost)
                    {
                        maxcost = item.Cost;
                    }
                }
                foreach (var item in data)//subtract maximum from all subjects
                {
                    item.Fitness = maxcost - item.Cost;
                }
            }
            else if(mode=="time")
            {
                genome least = new genome();
                int maxtime = 0;//find worst subject ie maximum time
                foreach (var item in data)
                {
                    if (maxtime < item.Time)
                    {
                        maxtime = item.Time;
                    }
                }
                foreach (var item in data)//subtract maximum from all subjects
                {
                   item.Fitness = maxtime - item.Time;
                }
            }
            else if(mode=="cost-time")//if mode is to optimize both
            {
                genome least = new genome();
                int max = 0;//find worst subject ie maximum fitness value as of now
                foreach (var item in data)
                {
                    if (max < item.Fitness)
                    {
                        max = item.Fitness;
                    }
                }
                foreach (var item in data)//subtract maximum from all subjects
                {
                    item.Fitness = max - item.Fitness;
                }
            }
        }

        
    }
}
