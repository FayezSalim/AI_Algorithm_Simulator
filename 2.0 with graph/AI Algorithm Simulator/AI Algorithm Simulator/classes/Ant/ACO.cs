using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AI_Algorithm_Simulator.classes
{
    public class ACO:Algorithm<Ant>
    {
        protected List<Edge> area = new List<Edge>();//place where ants move
        protected int initpheromone = 0;//initial pheromone
        protected int alpha = 0;//control influence of phermone >=0
        protected int beta = 1;//control influence of fitness >=1
        protected double row = 0;//pheromone evaporation coefficient;
        protected int Qconst = 0;//constant q for updating pheromones
        protected List<City_data> city_details = new List<City_data>();// all city details
        protected int noofants;//no of ants that has to be run parallely in 1 iteration
        private int iterations;//no of iterations
        private Delegate ant_updater, pheromone_updater;//primary updaters
        private Delegate ant_sec_updater;//secondary updater
        Random rd = new Random();
        public Delegate Ant_sec_updater
        {
            get { return ant_sec_updater; }
            set { ant_sec_updater = value; }
        }

        public Delegate Pheromone_updater
        {
            get { return pheromone_updater; }
            set { pheromone_updater = value; }
        }

        public Delegate Ant_updater
        {
            get { return ant_updater; }
            set { ant_updater = value; }
        }
        public List<Edge> Area
        {
            get { return area; }
        }
        public int Initpheromone
        {
            get { return initpheromone; }
            set { initpheromone = value; }
        }
        public int Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public int Beta
        {
            get { return beta; }
            set { beta = value; }
        }

        public double Row
        {
            get { return row; }
            set { row = value; }
        }

        public int QConst
        {
            get { return Qconst; }
            set { Qconst = value; }
        }

        public int Noofants
        {
            get { return noofants; }
            set { noofants = value; }
        }

        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        public ACO(int inip,int alpha,int beta,double row,int q,int noofants,int iterations)
        {
            type = AlgorithmType.AntColonyOptimization;
            initpheromone = inip;
            this.alpha = alpha;
            this.beta = beta;
            this.row = row;
            Qconst = q;
            this.noofants=noofants;
            Localbest = new List<Ant>();
            Localbest_time = new List<long>();
            this.Workingset = new List<Ant>();
            for (int i = 0; i < noofants; i++)
            {
                Workingset.Add(new Ant());
            }
            this.iterations=iterations;
        }

        public override void set_data(City_list input)
        {
            foreach (var item in input.Metric_list)
            {
                area.Add(new Edge { city1 = item.city1, city2 = item.city2, cost = item.cost, time = item.time, phermone = initpheromone});//set initial pheromones
            }
            foreach (var item in input.Cities_list)
            {
                city_details.Add(new City_data(item));
            }
        }
        
        protected virtual void constructsoln(Ant currentant)
        {
            List<double> probabilities = new List<double>();
            double baseprob=0;
            List<Edge> possible = new List<Edge>();
             //find all possible moves to other cities
            foreach (var item in area)
            {
                if(item.city1==currentant.Data[currentant.Data.Count-1].Name)
                {
                    City_data c= currentant.Data.Find(o => o.Name ==item.city2 );//already traversed
                    if(c==null)
                    {
                        possible.Add(new Edge(item));
                        //find sum of all E fxy^ alpha * pxy^beta ---baseprob
                        if (Algorithm_base.Mode == "cost")
                        {
                            baseprob += (Math.Pow((double)item.cost, (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                        else if (Algorithm_base.Mode == "time")
                        {
                            baseprob += (Math.Pow((double)item.time, (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                        else if (Algorithm_base.Mode == "cost-time")
                        {
                            baseprob += (Math.Pow((double)get_cost_time_fitness_of_path(item), (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                    }
                }
                else if (item.city2 == currentant.Data[currentant.Data.Count - 1].Name)
                {
                    City_data c = currentant.Data.Find(o => o.Name == item.city1);//already traversed
                    if(c==null)
                    {
                        possible.Add(new Edge (item));
                        //find sum of all E fxy^ alpha * pxy^beta ---baseprob
                        if (Algorithm_base.Mode == "cost")
                        {
                            baseprob += (Math.Pow((double)item.cost, (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                        else if (Algorithm_base.Mode == "time")
                        {
                            baseprob += (Math.Pow((double)item.time, (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                        else if (Algorithm_base.Mode == "cost-time")
                        {
                            baseprob += (Math.Pow((double)get_cost_time_fitness_of_path(item), (double)beta) * Math.Pow((double)item.phermone, (double)alpha));
                        }
                    }
                }
            }
            
            //calculate probability list
            for (int i = 0; i < possible.Count; i++)
            {
                if(Algorithm_base.Mode=="cost")
                {
                    int a = (int)(Math.Pow((double)possible[i].cost, (double)beta) * Math.Pow((double)possible[i].phermone, (double)alpha));
                    probabilities.Add(a / baseprob);
                }
                else if(Algorithm_base.Mode=="time")
                {
                    int a = (int)(Math.Pow((double)possible[i].time, (double)beta) * Math.Pow((double)possible[i].phermone, (double)alpha));
                   probabilities.Add(a / baseprob);
                }
                else//cost-time
                {
                    int a = (int)(Math.Pow((double)get_cost_time_fitness_of_path(possible[i]), (double)beta) * Math.Pow((double)possible[i].phermone, (double)alpha));
                    probabilities.Add(a / baseprob);
                }
            }
            double rand = rd.NextDouble()*baseprob;
            int leastindx=0;
            for (int i = 0; i < probabilities.Count; i++)
            {
               if(probabilities[i]<=rand)
               {
                   if(probabilities[i]>probabilities[leastindx])
                   {
                       leastindx = i;
                   }
               }
            }
            if(possible[leastindx].city1==currentant.Data[currentant.Data.Count-1].Name)
            {
                var item1 = city_details.FirstOrDefault(o => o.Name == possible[leastindx].city2);
                currentant.addcity(new City_data(item1));
            }
            else if(possible[leastindx].city2==currentant.Data[currentant.Data.Count-1].Name)
            {
                var item1 = city_details.FirstOrDefault(o => o.Name == possible[leastindx].city1);
                currentant.addcity(new City_data(item1));
            }
        }

        private double get_cost_time_fitness_of_path(Edge item)
        {
            double Fitness = item.cost + item.time;
            if (item.cost > Recommended_Cost/city_details.Count)
            {
                Fitness += (item.cost - Recommended_Cost);
            }

            if (item.time > Recommended_Time / city_details.Count)
            {
                Fitness += (item.time - Recommended_Time);
            }
            return Fitness;
        }

        protected virtual void updatephermones()
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
               
                item.phermone = item.phermone +currpheromone;//update phermone for that path
            }
            if (Algorithm_base.Performancerun == 0)
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

        protected virtual void evaporation()
        {
            if (Algorithm_base.Performancerun == 0)
            {
                for (int i = 0; i < Area.Count; i++)
                {
                    Area[i].phermone = (1 - Row) * Area[i].phermone;
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
                    Uidispatcher.BeginInvoke(ant_sec_updater, returnalgoobj(),-1);
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
                for (int i = 0; i < Area.Count; i++)
                {
                    Area[i].phermone = (1 - Row) * Area[i].phermone;
                }
            }
        }

        public override void execute()
        {
            this.Algo_exec_status = AI_Algorithm_Simulator.classes.exec_status.running;
           try
           {
                string info = "";
                this.Watch = new System.Diagnostics.Stopwatch();
                this.Watch.Start();
                if (Algorithm_base.Performancerun == 0)
                {
                    this.Watch.Stop();
                    while (UI_lock)
                    {
                    }
                    if (this.Algo_anim_status == 1)
                    {
                        UI_lock = true;
                        UIDispatcher.BeginInvoke(Pheromone_updater, true, "update", returnalgoobj(), info, null);
                    }
                    else if (this.Algo_anim_status == 2)
                    {
                        UI_lock = true;
                        Uidispatcher.BeginInvoke(ant_sec_updater, returnalgoobj(),-1);
                        while (sleep_value == 0)
                        { }
                        Thread.Sleep(1 / sleep_value);
                    }
                    while (UI_lock)
                    { }
                    this.Watch.Start();
                    for (int iterat = 0; iterat < Iterations; iterat++)
                    {
                      for (int j = 0; j < Workingset.Count; j++)
                        {
                            for (int i = 0; i < city_details.Count - 1; i++)
                            {
                                if (workingset[j].Data.Count == 0)
                                {
                                    Random rd = new Random();
                                    int no = rd.Next(0, this.city_details.Count);
                                    Workingset[j].addcity(city_details[no]);
                                }
                                constructsoln(Workingset[j]);
                                this.Watch.Stop();
                                while (UI_lock)
                                {
                                    //get UI lock
                                }
                                if (this.Algo_anim_status == 1)
                                {
                                    Thread.Sleep(100);
                                    info = "Step1" + Environment.NewLine + "Step2" + Environment.NewLine + "Step3" + Environment.NewLine + "Step4";
                                    UI_lock = true;
                                    UIDispatcher.BeginInvoke(Ant_updater, j, returnalgoobj(), info, iterat);
                                    Thread.Sleep(100);
                                }
                                else if (this.Algo_anim_status == 2)
                                {
                                    UI_lock = true;
                                    Uidispatcher.BeginInvoke(ant_sec_updater, returnalgoobj(),iterat);
                                    while (sleep_value == 0)
                                    { }
                                    Thread.Sleep(1 / sleep_value);

                                }
                                else if (Algo_anim_status == 0)
                                {
                                    while (sleep_value == 0)
                                    { }
                                    Thread.Sleep(1 / sleep_value);
                                }
                                while (UI_lock)
                                { }
                                this.Watch.Start();
                                //call each ant update
                                //wait until lock relieved
                            }
                        }

                        if (localbest.Count == 0)
                        {
                            localbest.Add(new Ant(this.workingset[0]));
                            Localbest_time.Add(this.Watch.ElapsedTicks);
                        }
                        foreach (var item in Workingset)
                        {
                            item.fitnesscalc();
                            find_localbest(item);
                        }

                        updatephermones();
                        evaporation();
                        //store best ant
                        while (UI_lock)
                        {
                            //get UI lock
                        }
                        for (int i = 0; i < this.Workingset.Count; i++)
                        {
                            this.Workingset[i] = new Ant();
                        }

                    }
                    this.Watch.Stop();
                }
               else//performance run
              {
                    for (int iterat = 0; iterat < Iterations; iterat++)
                    {
                        for (int j = 0; j < Workingset.Count; j++)
                        {
                            for (int i = 0; i < city_details.Count - 1; i++)
                            {
                                if (workingset[j].Data.Count == 0)
                                {
                                    Random rd = new Random();
                                    int no = rd.Next(0, this.city_details.Count);
                                    Workingset[j].addcity(city_details[no]);
                                }
                                constructsoln(Workingset[j]);
                                //call each ant update
                                //wait until lock relieved
                            }
                        }
                        if (localbest.Count == 0)
                        {
                            localbest.Add(new Ant(this.workingset[0]));
                            Localbest_time.Add(this.Watch.ElapsedTicks);
                        }
                        foreach (var item in Workingset)
                        {
                            item.fitnesscalc();
                            find_localbest(item);
                        }

                        updatephermones();
                        evaporation();
                        //store best ant
                        for (int i = 0; i < this.Workingset.Count; i++)
                        {
                            this.Workingset[i] = new Ant();
                        }
                    }
                    this.Watch.Stop();
                }
           }
           catch(Exception t)
            {
              System.Windows.Forms.MessageBox.Show(t.ToString());
            }
            finally
            {
                this.Algo_exec_status = AI_Algorithm_Simulator.classes.exec_status.completed;
            }
        }

        public override object returnalgoobj()
        {
            return this;
        }
    }
}
