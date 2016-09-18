using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AI_Algorithm_Simulator.classes
{
    public class City_list
    {
        public List<City_data> Cities_list;
        public List<String> used_alphas;
        List<Path> metric_list;

        public List<Path> Metric_list
        {
            get { return metric_list; }
            set { metric_list = value; }
        }
        public City_list()
        {
            Cities_list = new List<City_data>();
            used_alphas = new List<string>();
            metric_list = new List<Path>();
        }

        private void find_used_alphas()
        {
            List<String> used = new List<string>();
            foreach (var item in Cities_list)
            {
                used.Add(item.Name);
            }
            used_alphas = used;
        }

        public City_data get_city(string nm)
        {
            City_data k=null;
            foreach (var item in Cities_list)
            {
                if(item.Name==nm)
                {
                    k = item;
                    break;
                }
            }
            return k;
        }

        public void move_city(string nm,double x,double y)
        {
            foreach (var item in Cities_list)
            {
                if (item.Name == nm)
                {
                    item.X = x;
                    item.Y = y;
                    break;
                }
            }
        }

        public void Add(String nm,double xval,double yval)//add new city and its metrics data,lovation on UI
        {
            Cities_list.Add(new City_data(nm,xval,yval));
            Addmetrics(nm);//always run only initially
            find_used_alphas();
        }

        public void Addmetrics(string nm)//add available cities metrics data put 0 fr evrythin
        {
            City_data city=null;
            foreach (var item in Cities_list)
            {
                if(item.Name==nm)
                {
                    city = item;
                    break;
                }
            }
            foreach (var item in used_alphas)
            {
                metric_list.Add(new Path { city1 = nm, city2 = item, cost = 0, time = 0 });
                foreach (var item1 in Cities_list)
                {
                    if(item1.Name==item)
                    {
                        item1.AddPath(new Path { city1 = item, city2 = nm, cost = 0, time = 0 });
                    }
                }
                city.AddPath(new Path { city1 = nm, city2 = item, cost = 0, time = 0 });
            }
            
        }

        public void Editmetrics(String cityname,List<Cost_Time_Dependency_Data> path_data)//function to facilitate safe editing of metrics data
        {
            bool edited = false;
            foreach (var item in path_data)
	        {
                
                foreach (var item1 in metric_list)
                {
                    if(((item1.city1==cityname)&&(item1.city2==item.CityName))||((item1.city2==cityname)&&(item1.city1==item.CityName)))
                    {
                        item1.cost = item.CityCost;
                        item1.time = item.CityTime;
                        edited = true;
                    }
                }
                if (!edited)
                {
                    metric_list.Add(new Path { city1 = cityname, city2 = item.CityName, cost = item.CityCost, time = item.CityTime });
                }
                edited = false;
            }
            City_data city = null;
            foreach (var item in Cities_list)
            {
                if (item.Name == cityname)
                {
                    city = item;
                    break;
                }
            }
            city.ClearPaths();
            foreach (var item in metric_list)
	        {
		       if (item.city1 == cityname)
               {
                    city.AddPath(new Path {city1=cityname, city2 = item.city2, cost = item.cost, time = item.time });
                    foreach (var item1 in Cities_list)
                    {
                        if(item1.Name==item.city2)
                        {
                            item1.EditPath(new Path { city1 = item.city2, city2 = cityname, cost = item.cost, time = item.time });
                        }
                    }
               }
               else if(item.city2==cityname)
               {
                   city.AddPath(new Path { city1 = cityname, city2 = item.city1, cost = item.cost, time = item.time });
                   foreach (var item1 in Cities_list)
                   {
                       if (item1.Name == item.city1)
                       {
                           item1.EditPath(new Path { city1 = item.city1, city2 = cityname, cost = item.cost, time = item.time });
                       }
                   }
               }
	        }
        }

        public void Remove(String nm)
        {
            foreach (var item in Cities_list)
            {
                if (item.Name == nm)
                {
                    Cities_list.Remove(item);
                    break;
                }
            }
            Removemetrics(nm);
            find_used_alphas();
                    
        }

        private void Removemetrics(String nm)
        {
            for (int i = 0; i < metric_list.Count;i++ )
            {
                if ((metric_list[i].city1 == nm)||(metric_list[i].city2 == nm))
                {
                    metric_list.Remove(metric_list[i]);
                    foreach (var item in Cities_list)
                    {
                        item.DeletePath(nm);
                    }
                    i--;
                }
            }
        }

        public ObservableCollection<Cost_Time_Dependency_Data> getmetriclist_asobservable(string city)
        {
            ObservableCollection<Cost_Time_Dependency_Data> t = new ObservableCollection<Cost_Time_Dependency_Data>();
            foreach (var item in metric_list)
            {
                if (item.city1 == city)
                {
                    t.Add(new Cost_Time_Dependency_Data { CityName = item.city2, CityCost = item.cost, CityTime = item.time });
                }
                else if(item.city2==city)
                {
                    t.Add(new Cost_Time_Dependency_Data { CityName = item.city1, CityCost = item.cost, CityTime = item.time });
                }
            }
            return t;
        }

        public List<Path> getmetriclist_asPath(string city)//for drawing lines need as path list
        {
            foreach (var item in Cities_list)
            {
                if(item.Name==city)
                {
                    return item.Myypaths;
                }
            }
            return null;
        }

        public City_list(City_list mastercopy)
        {
            Cities_list = new List<City_data>();
            foreach (var item in mastercopy.Cities_list)
            {
                this.Cities_list.Add(item);
            }
            this.metric_list = mastercopy.metric_list;
            this.used_alphas = mastercopy.used_alphas;
        }
    }
}
