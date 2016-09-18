using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class SingleSwapMutation:IMutationAlgorithm
    {
        public static IMutationAlgorithm Create()
        {
            SingleSwapMutation w = new SingleSwapMutation();
            return w;
        }
        //working
        public genome execute(genome child)//swap two random positions
        {
            City_data temp ;
            genome mutatedchild=new genome();
            Random rd = new Random();
            int x = rd.Next(0, child.Data.Count);
            int y = rd.Next(0, child.Data.Count);
            temp = child.Data[x];
            for (int i = 0; i < child.Data.Count; i++)
            {
                if(i==x)
                {
                    mutatedchild.addcity(new City_data(child.Data[y]));
                }
                else if (i == y)
                {
                    mutatedchild.addcity(new City_data(child.Data[x]));
                }
                else
                {
                    mutatedchild.addcity(new City_data(child.Data[i]));
                }
            }
            return mutatedchild;

        }
    }
}
