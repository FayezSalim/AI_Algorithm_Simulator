using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class Order1Crossover : ICrossoverAlgorithm
    {
        public static ICrossoverAlgorithm Create()
        {
            Order1Crossover w = new Order1Crossover();
            return w;
        }
        public genome execute(genome parent1, genome parent2)//working
        {
            genome child = new genome();
            Random rd = new Random();
            int start=rd.Next(0, parent1.Data.Count-1);
            int end = rd.Next(start+1, parent1.Data.Count-1);
            for (int i = 0; i < parent2.Data.Count;i++ )//i for keeping track of child ,k for parent 2 cities
            {
                if (i == start)//when reaching ith position of child
                {
                    for (int j=i; j <= end; j++)
                    {
                        child.addcity(new City_data(parent1.Data[j]));
                    }
                    if (checknotin(parent1, parent2.Data[i], start, end))
                    {
                        child.addcity(new City_data(parent2.Data[i]));
                    }
                }
                else if(checknotin(parent1,parent2.Data[i],start,end))//check if kth city of parent2 not in parent1 swath
                {
                   child.addcity(new City_data(parent2.Data[i]));
                }
            }
            return child;
        }

        private bool checknotin(genome parent1,City_data d,int start,int end)
        {
            for (int i = start; i <= end; i++)
            {
                if(parent1.Data[i].Name==d.Name)
                {
                    return false;
                }
            }
            return true;
        }
    
    }
}
