using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    class RouletteWheelSelection:ISelectionAlgorithm
    {
        private List<genome> parents;

        public List<genome> getparents()
        {
            return parents;
        }  
        public void Initialize()//initialize
        {
            parents = new List<genome>();
        }
        public static ISelectionAlgorithm Create()
        {
            RouletteWheelSelection rk = new RouletteWheelSelection();
            rk.Initialize();
            return rk;
        }

        public void execute(List<genome> data, int noofparents)
        {
            //expecting sorted list of data 
            
            int totalfitness=0;
            foreach (var item in data)
            {
                totalfitness += item.Fitness;
            }
            Random rd = new Random();
            for (; parents.Count < noofparents; )
            {
                int selectedfitness = rd.Next(0, totalfitness);
                int findfitness=0;
                foreach (var item in data)
                {
                    findfitness += item.Fitness;
                    if(findfitness>=selectedfitness)
                    {
                        parents.Add(new genome(item));
                        break;
                    }
                }
            }
         }
    }
}
