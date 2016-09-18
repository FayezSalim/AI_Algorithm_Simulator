using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    class TruncationSelection:ISelectionAlgorithm
    {
        private List<genome> parents;

        public List<genome> getparents()
        {
            return parents;
        }
        public void Initialize()//initialize
        {
           parents = new List<genome>();
        }
        public static ISelectionAlgorithm Create()
        {
            TruncationSelection tk = new TruncationSelection();
            tk.Initialize();
            return tk;
        }
        public void execute(List<genome> data, int noofparents)
        {
            parents.Clear();
            Random rd = new Random();
            int popsize,no;
            for (;parents.Count<noofparents ; )
            {
                popsize = rd.Next(1, 10);
                no = (int)((1 / popsize)*data.Count);
                for (int i = 0; i < no; i++)
                {
                    parents.Add(data[i]);
                }
            }
        }

       
    }
}
