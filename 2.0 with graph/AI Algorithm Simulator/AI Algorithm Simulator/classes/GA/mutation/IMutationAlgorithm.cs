using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public interface IMutationAlgorithm
    {
        genome execute(genome child);
    }
}
