using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public interface ICrossoverAlgorithm
    {

     genome execute(genome parent1,genome parent2);
    }
}
