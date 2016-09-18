using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AI_Algorithm_Simulator.classes
{
    public class Exponential:IFitnessAlgorithm
    {
        public static IFitnessAlgorithm Create()
        {
            Exponential w = new Exponential();
            return w;
        }
        public void execute(List<genome> data,string mode)
        {
            if(mode=="cost")
            {
                foreach (var item in data)
                {
                    item.Fitness = item.Cost;
                }
            }
            else if (mode=="time")
            {
                foreach (var item in data)
                {
                    item.Fitness = item.Time;
                }
            }
            foreach (var item in data)
            {
                item.Fitness = (int)Math.Round(Math.Sqrt(item.Fitness))+1;
            }
        }
    }

}
