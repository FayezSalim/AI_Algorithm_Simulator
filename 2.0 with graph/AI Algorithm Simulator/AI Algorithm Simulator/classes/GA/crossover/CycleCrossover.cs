using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class CycleCrossover:ICrossoverAlgorithm
    {
        public static ICrossoverAlgorithm Create()
        {
            CycleCrossover w = new CycleCrossover();
            return w;
        }
        public genome execute(genome parent1, genome parent2)
        {
            throw new NotImplementedException();
        }
    }
}
