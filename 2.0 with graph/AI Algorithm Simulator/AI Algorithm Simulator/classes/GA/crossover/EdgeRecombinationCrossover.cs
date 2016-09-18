using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class EdgeRecombinationCrossover:ICrossoverAlgorithm
    {
        List<string> alphas = new List<string>();
        public static ICrossoverAlgorithm Create()
        {
            EdgeRecombinationCrossover w = new EdgeRecombinationCrossover();
            return w;
        }

        public genome execute(genome parent1, genome parent2)
        {
            
            List<List<string>> neighbourslist = new List<List<string>>();
            genome child = new genome();
            foreach (var item in parent1.Data)
            {
              if(!alphas.Contains(item.Name))
              {
                  alphas.Add(item.Name);
              }
            }
            foreach (var item in alphas)
            {
                List<string> list=new List<string>();
                for (int i = 0; i < parent1.Data.Count; i++)
                {
                 if(item==parent1.Data[i].Name)
                 {
                     if (i > 0 && i + 1 != parent1.Data.Count-1)
                     {
                         list.Add(parent1.Data[i - 1].Name);
                         list.Add( parent1.Data[i + 1].Name);
                     }
                     else if(i==0)
                     {
                         list.Add(parent1.Data[i + 1].Name);
                         list.Add(parent1.Data[parent1.Data.Count-1].Name);
                     }
                     else
                     {
                         list.Add(parent1.Data[0].Name) ;
                         list.Add(parent1.Data[i- 1].Name);
                     }
                 }
                }
                for (int i = 0; i < parent2.Data.Count; i++)
                {
                    if (item == parent2.Data[i].Name)
                    {
                        if (i > 0 && i + 1 != parent2.Data.Count - 1)
                        {
                            if (!list.Contains(parent2.Data[i - 1].Name))
                            {
                                list.Add( parent2.Data[i - 1].Name);
                            }
                            if(!list.Contains(parent2.Data[i + 1].Name))
                            {
                                list.Add(parent2.Data[i + 1].Name);
                            }
                        }
                        else if (i == 0)
                        {
                            if(!list.Contains(parent2.Data[i + 1].Name ))
                            {
                                list.Add(parent2.Data[i + 1].Name);
                            }

                            if(!list.Contains(parent2.Data[parent2.Data.Count - 1].Name))
                            {
                                list.Add( parent2.Data[parent2.Data.Count - 1].Name);
                            }
                        }
                        else
                        {
                            if(!list.Contains(parent2.Data[0].Name ))
                            {
                                list.Add(parent2.Data[0].Name);
                            }
                            if(!list.Contains(parent2.Data[i - 1].Name))
                            {
                                list.Add( parent2.Data[i - 1].Name);
                            }
                        }
                    }
                }
                neighbourslist.Add(list);
                
            }//end of foreach

            Random rd = new Random();
            int no = rd.Next(0,2);
            child.addcity(parent1.Data[no]);
            removeneighbour(neighbourslist,child,no);
            int indx=findleast(neighbourslist, neighbourslist[no]);
            return child;
        }

        private void removeneighbour(List<List<string>> neighbourslist,genome child,int no)
        {
            foreach (var item in neighbourslist)
            {
                item.Remove(child.Data[no].Name);
            }
        }

        private int findleast(List<List<string>> neighbourslist,List<string> neighbour)
        {
            int minindex=0,minlist = 5;
            for (int i = 0; i < neighbourslist.Count; i++)
            {
                if(neighbourslist[i].Count<minlist)
                {
                    minlist = neighbourslist[i].Count;
                    minindex = i;
                }
            }
            return minindex;
        }
    }
}
