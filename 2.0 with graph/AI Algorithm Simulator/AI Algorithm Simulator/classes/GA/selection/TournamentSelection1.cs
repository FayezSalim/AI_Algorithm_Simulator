using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    class TournamentSelection1:ISelectionAlgorithm
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
            TournamentSelection1 ts = new TournamentSelection1();
            ts.Initialize();
            return ts;
        }
        public void execute(List<genome> data, int noofparents)
        {
            parents.Clear();
            Random rd = new Random();
            int no1, no2, r;
            for (;parents.Count<noofparents ; )
            {
                no1 = rd.Next(0, data.Count);
                no2 = rd.Next(0, data.Count);
                r = rd.Next(0, 2);
                if(r==0)
                {
                    parents.Add(new genome(data[no1]));
                }
                else
                {
                    parents.Add(new genome(data[no2]));
                }
            }
        }
    }
}
