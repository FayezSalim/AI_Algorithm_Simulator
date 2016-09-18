using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Algorithm_Simulator.classes
{
    public class genome:Path_data
    {
        private List<City_data> data;

        public List<City_data> Data
        {
            get { return data; }
            set { data = value; }
        }
       public genome()
       {
           data = new List<City_data>();
           Fitness = 0;
       }

        public genome(genome g)
        {
            this.Cost = g.Cost;
            this.Time = g.Time;
            this.Fitness = g.Fitness;
            data = new List<City_data>();
            foreach (var item in g.data)
            {
                this.data.Add(item);
            }
        }

       public override void addcity(City_data d)
       {
        List<string> usd = new List<string>(); 
        foreach (var item in d.Myypaths)
          {
            if(data.Count!=0)
              {
                if(((item.city1== data[data.Count-1].Name)&&(item.city2==d.Name))||((item.city1==d.Name)&&(item.city2==data[data.Count-1].Name)))
                  {
                    Cost+=item.cost;
                    Time+=item.time;
                    break;
                  }
              }
            if (!usd.Contains(item.city1))
            {
                usd.Add(item.city1);
            }
           if(!usd.Contains(item.city2))
           {
               usd.Add(item.city2);
           }
        }
         
       data.Add(d);
       if(usd.Count==data.Count)//add last city to starting
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

       public bool validateGenome()
       {

           for (int i = 0; i < data.Count;i++ )//for all other elements
           {
               bool got=false;
               if (i != data.Count - 1)
               {
                   foreach (var item in data[i].Myypaths)
                   {
                       if (((item.city1 == data[i].Name) && (item.city2 == data[i + 1].Name)) || ((item.city1 == data[i + 1].Name) && (item.city2 == data[i].Name)))
                       {
                           got = true;
                           break;
                       }
                   }
               }
               else//for last element
               {
                   foreach (var item in data[i].Myypaths)
                   {
                       if (((item.city1 == data[0].Name) && (item.city2 == data[i ].Name)) || ((item.city1 == data[i].Name) && (item.city2 == data[0].Name)))
                       {
                           got = true;
                           break;
                       }
                   }
               }
               if(!got)
               {
                   return false;
               }
           }
           return true;
       }

       public void recalc()//calc cost and time
       {

       }

       public bool equalTo(genome k)
       {
           bool status = true;
           for (int i = 0; i < k.Data.Count; i++)
           {
               if(this.Data[i].Name==k.Data[i].Name)
               {
                   continue;
               }
               else
               {
                   status = false;
                   break;
               }
           }
           return status;
       }
    }
}
