using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public interface ISelectionAlgorithm///create private constructors for classes
    {
        List<genome> getparents(); 
        
        void execute(List<genome> data, int noofparents);

        void Initialize();//init object

       
      //run
    }
}
