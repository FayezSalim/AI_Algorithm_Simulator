using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    class LinearTransformation:IFitnessAlgorithm
    {
        public static IFitnessAlgorithm Create()
        {
            LinearTransformation w = new LinearTransformation();
            return w;
        }

        public void execute(List<genome> data,string mode)
        {
            int max=0;
            if (mode == "cost")
            {
                foreach (var item in data)
                {
                    item.Fitness = item.Cost;
                    if(item.Cost>max)
                    {
                        max=item.Cost;
                    }
                }
            }
            else if (mode == "time")
            {
                foreach (var item in data)
                {
                    item.Fitness = item.Time;
                    if(item.Time>max)
                    {
                        max=item.Time;
                    }
                }
            }
            Random rd = new Random();
            int a=rd.Next(1, (int)max / 2);
            int b = rd.Next(1, (int)max / 2);
            foreach (var item in data)
            {
                item.Fitness = (int)(item.Fitness - b) / a;
            }
        }
    }
}
