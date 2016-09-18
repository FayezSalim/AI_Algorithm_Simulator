using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    class LinearNormalization:IFitnessAlgorithm
    {
        public static IFitnessAlgorithm Create()
        {
            LinearNormalization w = new LinearNormalization();
            return w;
        }

        public void execute(List<genome> data,string mode)
        {
            int incrementvalue = 100/data.Count;
            int curfitness = 100;
            if (mode == "cost")
            {
                sortcost(data);
            }
            else if (mode == "time")
            {
                sorttime(data);
            }
            else if(mode=="cost-time")
            {
                sortfitness(data);
            }
            foreach (var item in data)
            {
                item.Fitness = curfitness;
                curfitness -= incrementvalue;
            }
        }

        private void sortcost(List<genome> data)
        {
            genome temp;
            for(int i=0;i<data.Count;i++)
            {
                 for (int j = i; j < data.Count; j++)
                {
                    if(data[i].Cost>data[j].Cost)
                    {
                        temp = new genome(data[i]);
                        data[i] = new genome(data[j]);
                        data[j] = new genome(temp);
                    }
                }
            }
        }

        private void sorttime(List<genome> data)
        {
            genome temp;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i; j < data.Count; j++)
                {
                    if (data[i].Time > data[j].Time)
                    {
                        temp = new genome(data[i]);
                        data[i] = new genome(data[j]);
                        data[j] = new genome(temp);
                    }
                }
            }
        }

        private void sortfitness(List<genome> data)
        {
            genome temp;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i; j < data.Count; j++)
                {
                    if (data[i].Fitness > data[j].Fitness)
                    {
                        temp = new genome(data[i]);
                        data[i] = new genome(data[j]);
                        data[j] = new genome(temp);
                    }
                }
            }
        }
    }
}
