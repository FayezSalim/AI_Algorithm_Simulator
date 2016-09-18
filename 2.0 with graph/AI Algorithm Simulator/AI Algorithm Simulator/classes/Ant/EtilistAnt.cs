using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class EtilistAnt:ACO
    {
        int econst;//value for mutliplying with best soln pheromone best value between 4 and 6

        public int Econst
        {
            get { return econst; }
            set { econst = value; }
        }

        //create constructor and call base too
        public EtilistAnt(int e, int inip, int alpha, int beta, double row, int q, int noofants, int iterations) : base(inip, alpha, beta, row, q,noofants,iterations)
        {
            econst=e;
        }
        protected override void updatephermones()
        {
           //take each edge and c which all ants took that path
            foreach (var item in area)//foreach path
            {
                double currpheromone = 0;
                foreach (var ant in Workingset)//take each ant
                {
                    for (int i=0;i<ant.Data.Count;i++)//go through paths f each ant and add if path used
                    {
                        if(i==ant.Data.Count-1)
                        {
                            if((ant.Data[0].Name==item.city1)&&(ant.Data[ant.Data.Count-1].Name==item.city2))
                            {
                                currpheromone += ((double)Qconst / (double)ant.Fitness);
                                break;
                            }
                            else if((ant.Data[0].Name==item.city2)&&(ant.Data[ant.Data.Count-1].Name==item.city1))
                            {
                                currpheromone += ((double)Qconst / (double)ant.Fitness);
                                break;
                            }
                        }
                        else if((ant.Data[i].Name==item.city1)&&(ant.Data[i+1].Name==item.city2))
                        {
                            currpheromone += ((double)Qconst / (double)ant.Fitness);
                            break;
                        }
                        else if((ant.Data[i].Name==item.city2)&&(ant.Data[i+1].Name==item.city1))
                        {
                            currpheromone += ((double)Qconst / (double)ant.Fitness);
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
                item.phermone = item.phermone + currpheromone;//update phermone for that path
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
