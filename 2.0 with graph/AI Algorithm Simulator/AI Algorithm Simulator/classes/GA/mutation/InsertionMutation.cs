using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class InsertionMutation:IMutationAlgorithm
    {
        public static IMutationAlgorithm Create()
        {
            InsertionMutation w = new InsertionMutation();
            return w;
        }

        public genome execute(genome child)//working
        {
            genome mutatedchild = new genome();
            Random rd = new Random();
            int no = rd.Next(0, child.Data.Count);
            List<int> indx = new List<int>();
            for (int i = 0; i < no; i++)
            {
                int num ;
                do
                {
                    num = rd.Next(0, child.Data.Count);
                } while (indx.Contains(num));
                indx.Add(num);
                
            }
            for (int i = 0; i < indx.Count; i++)
            {
                mutatedchild.addcity(new City_data(child.Data[indx[i]]));
            }
            for (int i = 0; i < child.Data.Count; i++)
            {
                if (!indx.Contains(i))
                {
                    mutatedchild.addcity(new City_data(child.Data[i]));
                }
            }
            return mutatedchild;
        }
    }
}
