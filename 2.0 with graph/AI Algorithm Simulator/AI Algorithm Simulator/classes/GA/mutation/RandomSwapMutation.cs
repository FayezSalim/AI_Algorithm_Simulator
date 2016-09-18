using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class RandomSwapMutation:IMutationAlgorithm
    {
        public static IMutationAlgorithm Create()
        {
            RandomSwapMutation w = new RandomSwapMutation();
            return w;
        }
        //working
        public genome execute(genome child)
        {
            genome mutatedchild = new genome();
            Random rd = new Random();
            int no = rd.Next(1, child.Data.Count - 2);
            for (int j=child.Data.Count-1; j > no;j-- )
            {
                mutatedchild.addcity(new City_data(child.Data[j]));
            }
            for (int i = no; i >= 0; i--)
            {
                mutatedchild.addcity(new City_data(child.Data[i]));
            }
            return mutatedchild;
        }
    }
}
