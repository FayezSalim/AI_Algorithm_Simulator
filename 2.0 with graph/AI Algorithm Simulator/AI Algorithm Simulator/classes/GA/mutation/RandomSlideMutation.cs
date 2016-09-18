using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class RandomSlideMutation:IMutationAlgorithm
    {

        public static IMutationAlgorithm Create()
        {
            RandomSlideMutation w = new RandomSlideMutation();
            return w;
        }
        public genome execute(genome child)//slide
        {
            genome mutatedchild = new genome();
            Random rd = new Random();
            int x = rd.Next(1, child.Data.Count - 1);
            int y=rd.Next(x,child.Data.Count);
            int pos;
            pos = rd.Next(y+x,child.Data.Count);
            throw new NotImplementedException();
            return mutatedchild;
        }
    }
}
