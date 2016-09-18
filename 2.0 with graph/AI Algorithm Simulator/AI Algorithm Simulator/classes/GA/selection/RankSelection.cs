using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class RankSelection:ISelectionAlgorithm
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
            RankSelection rk = new RankSelection();
            rk.Initialize();
            return rk;
        }

        public void execute(List<genome> data, int noofparents)
        {
            parents.Clear();
            Random rd = new Random();
            int selected;
            for (;parents.Count<noofparents;) 
            {
                selected = rd.Next(1, data.Count);
                parents.Add(new genome(data[data.Count-selected]));
            }
        }

       
    }
}
