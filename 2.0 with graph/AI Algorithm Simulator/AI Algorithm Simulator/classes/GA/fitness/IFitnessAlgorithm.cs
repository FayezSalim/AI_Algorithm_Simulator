using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public interface IFitnessAlgorithm
    {
        //void Initialize();//init object
         void execute(List<genome> data,string mode);
        //run gunc
    }
}
