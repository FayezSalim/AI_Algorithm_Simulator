using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class RankBasedAnt : ACO
    {
        int econst;//value for mutliplying with best soln pheromone best value between 4 and 6

        public int Econst
        {
            get { return econst; }
            set { econst = value; }
        }

        //create constructor and call base too
        public RankBasedAnt(int e, int inip, int alpha, int beta, double row, int q, int noofants, int iterations) : base(inip, alpha, beta, row, q,noofants,iterations)
        {
            econst=e;
        }
        protected override void updatephermones()
        {
            List<Ant> rankedantlist = new List<Ant>();
            foreach (var item in Workingset)
            {
                rankedantlist.Add(new Ant(item));
            }
            rankedantlist.Sort((fit1, fit2) => fit1.Fitness.CompareTo(fit2.Fitness));//sorting using lambda expression should sort in ascending order
            Random rd = new Random();
            int rand = rd.Next(0, rankedantlist.Count);
            rand -= 1;
            foreach (var item in area)//foreach path
            {
                double currpheromone = 0;
                for (int j = 0; j < rand; j++) //for each w-1 ants
                {
                    for (int i = 0; i < rankedantlist[j].Data.Count; i++)//go through paths f each ant and add if path used
                    {
                        if (i == rankedantlist[j].Data.Count - 1)
                        {
                            if ((rankedantlist[j].Data[0].Name == item.city1) && (rankedantlist[j].Data[rankedantlist[j].Data.Count - 1].Name == item.city2))
                            {
                                currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                                break;
                            }
                            else if ((rankedantlist[j].Data[0].Name == item.city2) && (rankedantlist[j].Data[rankedantlist[j].Data.Count - 1].Name == item.city1))
                            {
                                currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                                break;
                            }
                        }
                        else if ((rankedantlist[j].Data[i].Name == item.city1) && (rankedantlist[j].Data[i + 1].Name == item.city2))
                        {
                            currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                            break;
                        }
                        else if ((rankedantlist[j].Data[i].Name == item.city2) && (rankedantlist[j].Data[i + 1].Name == item.city1))
                        {
                            currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                            break;
                        }
                    }
                }
                for (int i = 0; i < Localbest[Localbest.Count - 1].Data.Count; i++)//go through paths f best ant and add if path used
                {
                    if (i == Localbest[Localbest.Count - 1].Data.Count - 1)
                    {
                        if ((Localbest[Localbest.Count - 1].Data[0].Name == item.city1) && (Localbest[Localbest.Count - 1].Data[Localbest[Localbest.Count - 1].Data.Count - 1].Name == item.city2))
                        {
                            currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                            break;
                        }
                        else if ((Localbest[Localbest.Count - 1].Data[0].Name == item.city2) && (Localbest[Localbest.Count - 1].Data[Localbest[Localbest.Count - 1].Data.Count - 1].Name == item.city1))
                        {
                            currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                            break;
                        }
                    }
                    else if ((Localbest[Localbest.Count - 1].Data[i].Name == item.city1) && (Localbest[Localbest.Count - 1].Data[i + 1].Name == item.city2))
                    {
                        currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                        break;
                    }
                    else if ((Localbest[Localbest.Count - 1].Data[i].Name == item.city2) && (Localbest[Localbest.Count - 1].Data[i + 1].Name == item.city1))
                    {
                        currpheromone += (((double)Qconst / (double)Localbest[Localbest.Count - 1].Fitness) * (double)econst);
                        break;
                    }
                }
               item.phermone = item.phermone +currpheromone;//update phermone for that path
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
