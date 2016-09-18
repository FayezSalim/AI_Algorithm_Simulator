using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class StochasticUniversalSampling:ISelectionAlgorithm
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
            StochasticUniversalSampling su = new StochasticUniversalSampling();
            su.Initialize();
            return su;
        }
        public void execute(List<genome> data, int noofparents)
        {
            parents.Clear();
            int totalfitness = 0;
            foreach (var item in data)
            {
                totalfitness += item.Fitness;
            }
            int pointerincrementvalue = data[(int)data.Count/4].Fitness;
            Random rd = new Random();
            int selectedfitness = rd.Next(1, totalfitness);
            for (; parents.Count < noofparents; )
            {
                int findfitness = 0;
                foreach (var item in data)
                {
                    findfitness += item.Fitness;
                    if (findfitness >= selectedfitness)
                    {
                        parents.Add(new genome(item));
                        break;
                    }
                }
                selectedfitness += pointerincrementvalue;
                if(selectedfitness>totalfitness)
                {
                    selectedfitness = (selectedfitness - totalfitness);
                }
            }
            
        }
    }
}
