using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AI_Algorithm_Simulator.classes
{
    public class MinMaxAnt : ACO
    {
        double min, max;//create constructor

        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        public double Max
        {
            get { return max; }
            set { max = value; }
        }

        public MinMaxAnt(double min,double max,int inip,int alpha,int beta,double row,int q,int noofants,int iterations):base(inip,alpha,beta,row,q,noofants,iterations)
        {
            this.min = min;
            this.max = max;
        }
         protected override void evaporation()
        {
            if (Performancerun == 0)
            {
                foreach (var item in area)
                {
                    item.phermone = Maxof((1 - row) * item.phermone);
                }
                this.Watch.Stop();
                while (UI_lock)
                { }
                if (this.Algo_anim_status == 1)
                {
                    String info = "Step1" + Environment.NewLine + "Step2" + Environment.NewLine + "Step3" + Environment.NewLine + "Step4";
                    UI_lock = true;
                    UIDispatcher.BeginInvoke(Pheromone_updater, false, "evaporate", returnalgoobj(), info, localbest);
                }
                else if (this.Algo_anim_status == 2)
                {
                    UI_lock = true;
                    Uidispatcher.BeginInvoke(Ant_sec_updater, returnalgoobj(),-1);
                    while (sleep_value == 0)
                    { }
                    Thread.Sleep(1 / sleep_value);
                }
                while (UI_lock)
                { }
                this.Watch.Start();
            }
            else
            {
                foreach (var item in area)
                {
                    item.phermone = Maxof((1 - row) * item.phermone);
                }
            }
        }

         private double Maxof(double a)
         {
             if(a>min)
             {
                 return a;
             }
             else
             {
                 return min;
             }
         }

        private double Minof(double a)
         {
            if(a<max)
            {
                return a;
            }
            else
            {
                return max;
            }
         }

        protected override void updatephermones()
        {
            //take each edge and c which all ants took that path
            foreach (var item in area)//foreach path
            {
                double currpheromone = 0;
                foreach (var ant in Workingset)//take each ant
                {
                    for (int i = 0; i < ant.Data.Count; i++)//go through paths f each ant and add if path used
                    {
                        if (i == ant.Data.Count - 1)
                        {
                            if ((ant.Data[0].Name == item.city1) && (ant.Data[ant.Data.Count - 1].Name == item.city2))
                            {
                                currpheromone += ((double)Qconst / (double)ant.Fitness);
                                break;
                            }
                            else if ((ant.Data[0].Name == item.city2) && (ant.Data[ant.Data.Count - 1].Name == item.city1))
                            {
                                currpheromone += ((double)Qconst / (double)ant.Fitness);
                                break;
                            }
                        }
                        else if ((ant.Data[i].Name == item.city1) && (ant.Data[i + 1].Name == item.city2))
                        {
                            currpheromone += ((double)Qconst / (double)ant.Fitness);
                            break;
                        }
                        else if ((ant.Data[i].Name == item.city2) && (ant.Data[i + 1].Name == item.city1))
                        {
                            currpheromone += ((double)Qconst / (double)ant.Fitness);
                            break;
                        }
                    }
                }
                item.phermone =Minof(item.phermone +currpheromone);//update phermone for that path
                
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
