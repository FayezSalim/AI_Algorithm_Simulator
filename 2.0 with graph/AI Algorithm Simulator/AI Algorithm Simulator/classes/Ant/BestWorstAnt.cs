using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class BestWorstAnt : ACO
    {
        public BestWorstAnt(int inip, int alpha, int beta, double row, int q, int noofants, int iterations): base(inip, alpha, beta, row, q,noofants,iterations)
        {
            
        }
         protected override void updatephermones()
        {
            List<Ant> rankedantlist = new List<Ant>();
            foreach (var item in Workingset)
            {
                rankedantlist.Add(new Ant(item));
            }
            rankedantlist.Sort((fit1, fit2) => fit1.Fitness.CompareTo(fit2.Fitness));//sorting using lambda expression
            foreach (var item in area)//foreach path
            {
                double currpheromone = 0;
                for (int i = 0; i < Localbest[Localbest.Count-1].Data.Count; i++)//go through paths f best ant and add if path used
                {
                    if (i == Localbest[Localbest.Count - 1].Data.Count - 1)
                    {
                        if ((Localbest[Localbest.Count - 1].Data[0].Name == item.city1) && (Localbest[Localbest.Count - 1].Data[Localbest[Localbest.Count - 1].Data.Count - 1].Name == item.city2))
                        {
                            currpheromone += ((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness);
                            break;
                        }
                        else if ((Localbest[Localbest.Count - 1].Data[0].Name == item.city2) && (Localbest[Localbest.Count - 1].Data[Localbest[Localbest.Count - 1].Data.Count - 1].Name == item.city1))
                        {
                            currpheromone += ((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness);
                            break;
                        }
                    }
                    else if ((Localbest[Localbest.Count - 1].Data[i].Name == item.city1) && (Localbest[Localbest.Count - 1].Data[i + 1].Name == item.city2))
                    {
                        currpheromone += ((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness);
                        break;
                    }
                    else if ((Localbest[Localbest.Count - 1].Data[i].Name == item.city2) && (Localbest[Localbest.Count - 1].Data[i + 1].Name == item.city1))
                    {
                        currpheromone += ((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness);
                        break;
                    }
                }
                item.phermone = item.phermone+currpheromone;//update phermone for that path
                if(currpheromone==0)//enter if not in best ant
                {
                 for (int i = 0; i < rankedantlist[rankedantlist.Count-1].Data.Count; i++)//go through paths f worst ant and add if path used
                   {
                    if (i == rankedantlist[rankedantlist.Count-1].Data.Count - 1)
                    {
                        if ((rankedantlist[rankedantlist.Count-1].Data[0].Name == item.city1) && (rankedantlist[rankedantlist.Count-1].Data[rankedantlist[rankedantlist.Count-1].Data.Count - 1].Name == item.city2))
                        {
                            currpheromone += ((double)Qconst / (double)rankedantlist[rankedantlist.Count - 1].Fitness);
                            break;
                        }
                        else if ((rankedantlist[rankedantlist.Count-1].Data[0].Name == item.city2) && (rankedantlist[rankedantlist.Count-1].Data[rankedantlist[rankedantlist.Count-1].Data.Count - 1].Name == item.city1))
                        {
                            currpheromone += ((double)Qconst / (double)rankedantlist[rankedantlist.Count - 1].Fitness);
                            break;
                        }
                    }
                    else if ((rankedantlist[rankedantlist.Count-1].Data[i].Name == item.city1) && (rankedantlist[rankedantlist.Count-1].Data[i + 1].Name == item.city2))
                    {
                        currpheromone += ((double)Qconst / (double)rankedantlist[rankedantlist.Count - 1].Fitness);
                        break;
                    }
                    else if ((rankedantlist[rankedantlist.Count-1].Data[i].Name == item.city2) && (rankedantlist[rankedantlist.Count-1].Data[i + 1].Name == item.city1))
                    {
                        currpheromone += ((double)Qconst / (double)rankedantlist[rankedantlist.Count - 1].Fitness);
                        break;
                    }
                }
                }
             item.phermone = item.phermone-currpheromone;//update phermone for that path subtract for worst ant
             }
            if (Performancerun == 0)
            {
                this.Watch.Stop();
                while (UI_lock)
                {
                    //get uilock
                }
                if (this.Algo_anim_status == 1)
                {

                    String info = "Step1" + Environment.NewLine + "Step2" + Environment.NewLine + "Step3" + Environment.NewLine + "Step4";
                    UI_lock = true;
                    UIDispatcher.BeginInvoke(Pheromone_updater, false, "update", returnalgoobj(), info, null);
                }
                this.Watch.Start();
            }
         }
    }
}
