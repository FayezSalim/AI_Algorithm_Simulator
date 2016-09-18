using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AI_Algorithm_Simulator.classes
{
    public abstract class Algorithm<T> :Algorithm_base
    where T: Path_data
    {
        protected List<T> workingset;//working data

        public List<T> Workingset
        {
            get { return workingset; }
            set { workingset = value; }
        }

        protected List<T> localbest;//algorithm specific best soln

        public List<T> Localbest
        {
            get { return localbest; }
            set { localbest = value; }
        }

        protected void costtimefitness()///calc combined cost-time
        {
            foreach (var item in workingset)
	        {
		      item.Fitness=item.Cost+item.Time;
                if(item.Cost>Recommended_Cost)
                {
                    item.Fitness += (item.Cost - Recommended_Cost);
                }

                if(item.Time>Recommended_Time)
                {
                    item.Fitness += (item.Time - Recommended_Time);
                }
	        }
        }

        public  void find_localbest(T g)
        {
            if (Algorithm_base.Mode == "cost")
            {
                if (this.Localbest[Localbest.Count - 1].Cost > g.Cost)
                {
                    this.Localbest.Add((T)Activator.CreateInstance(typeof(T), g));
                    this.Localbest_time.Add(this.Watch.ElapsedTicks);
                }
            }
            else if (Algorithm_base.Mode == "time")
            {
                if (this.Localbest[Localbest.Count - 1].Time > g.Time)
                {
                    this.Localbest.Add((T)Activator.CreateInstance(typeof(T), g));
                    this.Localbest_time.Add(this.Watch.ElapsedTicks);
                }
            }
            else if (Algorithm_base.Mode == "cost-time")
            {
                //lb fitness
                this.Localbest[Localbest.Count - 1].Fitness = this.Localbest[Localbest.Count - 1].Cost + this.Localbest[Localbest.Count - 1].Time;
                if (this.Localbest[Localbest.Count - 1].Cost > Recommended_Cost)
                {
                    this.Localbest[Localbest.Count - 1].Fitness += (this.Localbest[Localbest.Count - 1].Cost - Recommended_Cost);
                }

                if (this.Localbest[Localbest.Count - 1].Time > Recommended_Time)
                {
                    this.Localbest[Localbest.Count - 1].Fitness += (this.Localbest[Localbest.Count - 1].Time - Recommended_Time);
                }
                ///given fitness
                g.Fitness = g.Cost + g.Time;
                if (g.Cost > Recommended_Cost)
                {
                    g.Fitness += (g.Cost - Recommended_Cost);
                }

                if (g.Time > Recommended_Time)
                {
                    g.Fitness += (g.Time - Recommended_Time);
                }


                if (this.Localbest[Localbest.Count - 1].Fitness > g.Fitness)
                {
                    this.Localbest.Add((T)Activator.CreateInstance(typeof(T), g));
                    this.Localbest_time.Add(this.Watch.ElapsedTicks);
                }
            }
        }
    }
}
