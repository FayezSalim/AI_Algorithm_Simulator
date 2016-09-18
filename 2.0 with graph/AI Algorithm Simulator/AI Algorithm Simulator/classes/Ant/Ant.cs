using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class Ant : Path_data
    {

        private List<City_data> data;

        public Ant()
        {
            data = new List<City_data>();
        }

        public Ant(Ant a)
        {
            this.Cost = a.Cost;
            this.Fitness = a.Fitness;
            this.Time = a.Time;
            data = new List<City_data>();
            foreach (var item in a.Data)
            {
                this.Data.Add(new City_data(item));
            }
        }
        
        public List<City_data> Data
        {
            get { return data; }
            set { data = value; }
        }

        public override void addcity(City_data d)
        {
            List<string> usd = new List<string>();
            foreach (var item in d.Myypaths)
            {
                if (data.Count != 0)
                {
                    if (((item.city1 == data[data.Count - 1].Name) && (item.city2 == d.Name)) || ((item.city1 == d.Name) && (item.city2 == data[data.Count - 1].Name)))
                    {
                        Cost += item.cost;
                        Time += item.time;
                        break;
                    }
                }
                if (!usd.Contains(item.city1))
                {
                    usd.Add(item.city1);
                }
                if (!usd.Contains(item.city2))
                {
                    usd.Add(item.city2);
                }
            }

            data.Add(d);
            if (usd.Count == data.Count)//add last city to starting
            {
                foreach (var item in d.Myypaths)
                {
                    if (data.Count != 0)
                    {
                        if (((item.city1 == data[data.Count - 1].Name) && (item.city2 == data[0].Name)) || ((item.city1 == data[0].Name) && (item.city2 == data[data.Count - 1].Name)))
                        {
                            Cost += item.cost;
                            Time += item.time;
                            break;
                        }
                    }
                }
            }

        }

       public override List<City_data> returnAsList()//not implemented
       {
           List<City_data> d = new List<City_data>();
           foreach (var item in data)
           {
               
           }
           throw new NotImplementedException();
       }

       public void fitnesscalc()//calc combined fitness or single cost/time in ant colony cost is directly used
       {
           if(Algorithm_base.Mode=="cost")
           {
               Fitness = Cost;
           }
           else if(Algorithm_base.Mode=="time")
           {
               Fitness = Time;
           }
           else
           {
               Fitness = Cost + Time;
               if (Cost > Algorithm_base.Recommended_Cost)
               {
                   Fitness += (Cost - Algorithm_base.Recommended_Cost);
               }

               if (Time > Algorithm_base.Recommended_Time)
               {
                   Fitness += (Time - Algorithm_base.Recommended_Time);
               }
           }
       }

      
    }
}
