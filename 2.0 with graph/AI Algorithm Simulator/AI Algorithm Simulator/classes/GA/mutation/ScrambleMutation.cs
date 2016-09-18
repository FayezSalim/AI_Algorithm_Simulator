using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class ScrambleMutation :IMutationAlgorithm
    {
        public static IMutationAlgorithm Create()
        {
            ScrambleMutation w = new ScrambleMutation();
            return w;
        }

        public genome execute(genome child)//swap two random swaths
        {
            genome mutatedchild = new genome();
            Random rd = new Random();
            int x1 = rd.Next(0, child.Data.Count);
            int y1 = rd.Next(x1+1, child.Data.Count);
            int x2 = rd.Next(y1+1, child.Data.Count);
            int y2 = rd.Next(x2+1, child.Data.Count);
            for (int i = 0; i < child.Data.Count; i++)
            {
                if(i==x1)
                {
                    for(int j=x2;i<=y2;j++)
                    {
                        mutatedchild.addcity(child.Data[j]);
                        
                    }
                    for(;i<y1;i++)
                    {}
                }
                else if (i == x2)
                {
                    for (int j = x1; i <= y1; j++)
                    {
                        mutatedchild.addcity(child.Data[j]);
                    }
                    for (; i < y2; i++)
                    { }
                }
                else
                {
                    mutatedchild.addcity(child.Data[i]);
                }
            }
            return mutatedchild;

        }
    }
}
