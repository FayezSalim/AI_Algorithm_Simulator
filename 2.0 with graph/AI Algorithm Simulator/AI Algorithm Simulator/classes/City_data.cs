using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Algorithm_Simulator.classes
{
    public class City_data
    {
        String name;
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        List<Path> mypaths;
        public List<Path> Myypaths
        {
            get
            {
                return mypaths;
            }
        }


        double x, y;//UI location

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }


        
        public City_data(string nm,double xval,double yval)
        {
            name = nm;
            this.X=xval;
            this.y = yval;
            mypaths = new List<Path>();

        }

        public City_data(City_data d)
        {
            this.name = d.name;
            this.mypaths = new List<Path>();
            foreach (var item in d.Myypaths)
            {
                this.AddPath(new Path(item));
            }
            this.X = d.X;
            this.Y = d.Y;
        }

        public string Shape_name()
        {
            return name + "_shape";
        }

        public string Text_name()
        {
            return name + "_txt";
        }

        public void AddPath(Path c)
        {
            mypaths.Add(c);
        }

        public void EditPath(Path c)
        {
            foreach (var item in mypaths)
            {
                if((item.city1+item.city2)==(c.city1+c.city2))
                {
                    item.cost = c.cost;
                    item.time = c.time;
                }
            }
        }

        public void DeletePath(string nm)
        {
            for (int i = 0; i < mypaths.Count; i++)
            {
                if (mypaths[i].city2 == nm)
                {
                    mypaths.Remove(mypaths[i]);
                    i--;
                }

            }
        }

        public void ClearPaths()
        {
            mypaths.Clear();
        }

    }
}
