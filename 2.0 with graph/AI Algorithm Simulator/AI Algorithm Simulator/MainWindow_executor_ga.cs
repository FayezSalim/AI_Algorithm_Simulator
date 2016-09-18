using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using AI_Algorithm_Simulator.classes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.Threading;

namespace AI_Algorithm_Simulator
{
    //delegate definition
    public delegate void selection_updater_delegate(genome parent1, genome parent2, GeneticAlgorithm curr, int gen);
    public delegate void crossover_updater_delegate(genome child,GeneticAlgorithm curr);
    public delegate void mutation_updater_delegate(genome child, GeneticAlgorithm curr);
    public delegate void Ga_sec_updater(List<genome> newpop,GeneticAlgorithm curr,int gen);
    partial class MainWindow:Window
    {
       //------worker threads
        BackgroundWorker parent1_updater = new BackgroundWorker();
        BackgroundWorker parent2_updater = new BackgroundWorker();
        BackgroundWorker child_drawer = new BackgroundWorker();
        BackgroundWorker line_drawer = new BackgroundWorker();
        BackgroundWorker mutator = new BackgroundWorker();
        ObservableCollection<secondary_display_data> ga_parent_data = new ObservableCollection<secondary_display_data>();
        ObservableCollection<secondary_display_data> ga_child_data = new ObservableCollection<secondary_display_data>();


        //-------------------------------Delegates for ga
        public void selection_updater(genome parent1, genome parent2, GeneticAlgorithm curr,int gen)
        {
            //loop through all genomes and update fr each genome stop when parent found ///async thread
           // parent1_updater.RunWorkerAsync(new Object[]{parent1,curr.Workingset});
            //wait fr that to finish
            //start second parent from here
            try
            {
                curr.UI_lock = true;
                this.ga_algo_status_txt.Text = "Selection";
                this.ga_gen_txt.Text = gen.ToString();
                parent1_updater = new BackgroundWorker();
                parent2_updater = new BackgroundWorker();
                parent1_updater.DoWork += parent1_updater_DoWork;
                parent1_updater.WorkerReportsProgress = true;
                parent1_updater.ProgressChanged += parent1_updater_ProgressChanged;
                parent2_updater.DoWork += parent2_updater_DoWork;
                parent2_updater.WorkerReportsProgress = true;
                parent2_updater.ProgressChanged += parent2_updater_ProgressChanged;
                foreach (var item in Cities.Metric_list)
                {
                    Line l = null;
                    if (exec_grid.FindName("ga_link_" + item.city1 + item.city2 + "c") != null)
                    {
                        l = (Line)exec_grid.FindName("ga_link_" + item.city1 + item.city2 + "c");
                    }
                    else if (exec_grid.FindName("ga_link_" + item.city2 + item.city1 + "c") != null)
                    {
                        l = (Line)exec_grid.FindName("ga_link_" + item.city2 + item.city1 + "c");
                    }
                    if (l != null)
                    {
                        l.Visibility = Visibility.Hidden;
                    }
                }
                parent1_updater.RunWorkerAsync(new object[] { parent1, curr });
                parent2_updater.RunWorkerAsync(new object[] { parent2, curr });
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
       }

        public void crossover_updater(genome child,GeneticAlgorithm curr)
        {
            try
            {
                curr.UI_lock = true;
                this.ga_algo_status_txt.Text = "Crossover";
                child_drawer = new BackgroundWorker();
                child_drawer.DoWork += child_drawer_DoWork;
                child_drawer.WorkerReportsProgress = true;
                child_drawer.ProgressChanged += child_drawer_ProgressChanged;
                this.child_cost_txt.Text = "Cost :" + child.Cost.ToString();
                this.child_time_txt.Text = "Time :" + child.Time.ToString();
                child_drawer.RunWorkerAsync(new Object[] { new genome(child), curr });
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
        }

        public void mutation_updater(genome child,GeneticAlgorithm curr)
        {
            try
            {
                curr.UI_lock = true;
                this.ga_algo_status_txt.Text = "Mutation";
                this.child_cost_txt.Text = "Cost :" + child.Cost.ToString();
                this.child_time_txt.Text = "Time :" + child.Time.ToString();
                foreach (var item in Cities.Metric_list)
                {
                    if (FindName("ga_link_" + item.city1 + item.city2 + "c") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city1 + item.city2 + "c");
                        if (checkifindparent(item.city1, item.city2, child))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (FindName("ga_link_" + item.city2 + item.city1 + "c") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city2 + item.city1 + "c");
                        if (checkifindparent(item.city1, item.city2, child))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
                this.ga_lb_cost_txt.Text = curr.Localbest[curr.Localbest.Count - 1].Cost.ToString();
                this.ga_lb_time_txt.Text = curr.Localbest[curr.Localbest.Count - 1].Time.ToString();
                string tool = "";
                foreach (var item in curr.Localbest[curr.Localbest.Count - 1].Data)
                {
                    tool += item.Name;
                }
                this.ga_lb_txt.ToolTip = tool;
                curr.UI_lock = false;
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
            finally
            {
                curr.UI_lock = false;
            }
        }

        public void ga_sec_updater(List<genome> newpop,GeneticAlgorithm curr,int gen)
        {
            try
            {
                this.ga_sec_gen_txt.Text = gen.ToString();
                ga_parent_data.Clear();
                int i = 1;
                string path = "";
                foreach (var item in curr.Workingset)//set parent data
                {
                    foreach (var item1 in item.Data)
                    {
                        path += item1.Name;
                    }
                    ga_parent_data.Add(new secondary_display_data { name = "genome " + i.ToString(), cost = item.Cost, time = item.Time, path = path });
                    i++;
                }
                path = "";
                i = 1;
                ga_child_data.Clear();
                foreach (var item in newpop)//set child data
                {
                    foreach (var item1 in item.Data)
                    {
                        path += item1.Name;
                    }
                    ga_child_data.Add(new secondary_display_data { name = "child " + i.ToString(), cost = item.Cost, time = item.Time, path = path });
                    i++;
                }
                this.ga_sec_cost_txt.Text = curr.Localbest[curr.Localbest.Count - 1].Cost.ToString();//set localbest
                this.ga_sec_time_txt.Text = curr.Localbest[curr.Localbest.Count - 1].Time.ToString();
                path = "";
                foreach (var item1 in curr.Localbest[curr.Localbest.Count - 1].Data)
                {
                    path += item1.Name;
                }
                this.ga_sec_lb.ToolTip = path;
                curr.UI_lock = false;
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
            finally
            {
                if(gen+1==curr.MaxGen)
                {
                    this.ga_sec_gen_txt.Text = "Completed";
                }
                curr.UI_lock = false;
            }
        }


        //---------------------------------------- worker thread def

        void parent1_updater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                genome parent = (genome)(((Object[])e.UserState))[0];
                GeneticAlgorithm curr = (GeneticAlgorithm)(((Object[])e.UserState))[1];
                foreach (var item in Cities.Metric_list)
                {
                    if (FindName("ga_link_" + item.city1 + item.city2 + "p1") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city1 + item.city2 + "p1");
                        if (checkifindparent(item.city1, item.city2, parent))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (FindName("ga_link_" + item.city2 + item.city1 + "p1") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city2 + item.city1 + "p1");
                        if (checkifindparent(item.city1, item.city2, parent))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
                this.parent1_fit_txt.Text = "Fitness :" + parent.Fitness.ToString();
                this.parent1_cost_txt.Text = "Cost :" + parent.Cost.ToString();
                this.parent1_time_txt.Text = "Time :" + parent.Time.ToString();
                if (e.ProgressPercentage == 1)
                {
                    //over
                }
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
        }

        public bool checkifindparent(string city1,string city2,genome parent)
        {
            try
            { 
            for (int i = 0; i < parent.Data.Count; i++)
            {
                if (i + 1 < parent.Data.Count)
                {
                    if (parent.Data[i].Name == city1 && parent.Data[i + 1].Name == city2)
                    {
                        return true;
                    }
                    else if (parent.Data[i].Name == city2 && parent.Data[i + 1].Name == city1)
                    {
                        return true;
                    }
                }
                else
                {
                    if (parent.Data[i].Name == city1 && parent.Data[0].Name == city2)
                    {
                        return true;
                    }
                    else if (parent.Data[i].Name == city2 && parent.Data[0].Name == city1)
                    {
                        return true;
                    }
                }
             }
            return false;
             }
            catch (Exception t)
            {
                MessageBox.Show(t.ToString());
            }
           return false;
            
        }

        void parent1_updater_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                genome parent = (genome)(((Object[])e.Argument)[0]);
                List<genome> dataset = ((GeneticAlgorithm)(((Object[])e.Argument)[1])).Workingset;
                foreach (var item in dataset)
                {
                    while (speed_val == 0)
                    { }
                    Thread.Sleep(TimeSpan.FromSeconds(1 / speed_val));
                    if (item.equalTo(parent))
                    {
                        parent1_updater.ReportProgress(1, new Object[] { item, (GeneticAlgorithm)(((Object[])e.Argument)[1]) });
                        break;
                    }
                    parent1_updater.ReportProgress(0, new Object[] { item, (GeneticAlgorithm)(((Object[])e.Argument)[1]) });
                }
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
            
        }

        void parent2_updater_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                genome parent = (genome)(((Object[])e.Argument)[0]);
                List<genome> dataset = ((GeneticAlgorithm)(((Object[])e.Argument)[1])).Workingset;
                for (int i = dataset.Count - 1; i >= 0; i--)
                {
                    while (speed_val == 0)
                    { }
                    Thread.Sleep(TimeSpan.FromSeconds(1 / speed_val));
                    if (dataset[i].equalTo(parent))
                    {
                        while (parent1_updater.IsBusy)
                        { }
                        parent2_updater.ReportProgress(1, new Object[] { dataset[i], (GeneticAlgorithm)(((Object[])e.Argument)[1]) });
                        break;
                    }
                    parent2_updater.ReportProgress(0, new Object[] { dataset[i], (GeneticAlgorithm)(((Object[])e.Argument)[1]) });
                }
            }
            catch (Exception t)
            {
                MessageBox.Show(t.ToString());
            }
        }
       
        void parent2_updater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GeneticAlgorithm curr = (GeneticAlgorithm)(((Object[])e.UserState))[1];
            try
            {
                genome parent = (genome)(((Object[])e.UserState))[0];
                
                foreach (var item in Cities.Metric_list)
                {
                    if (FindName("ga_link_" + item.city1 + item.city2 + "p2") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city1 + item.city2 + "p2");
                        if (checkifindparent(item.city1, item.city2, parent))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (FindName("ga_link_" + item.city2 + item.city1 + "p2") != null)
                    {
                        Line l = (Line)FindName("ga_link_" + item.city2 + item.city1 + "p2");
                        if (checkifindparent(item.city1, item.city2, parent))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
                this.parent2_fit_txt.Text = "Fitness :" + parent.Fitness.ToString();
                this.parent2_cost_txt.Text = "Cost :" + parent.Cost.ToString();
                this.parent2_time_txt.Text = "Time :" + parent.Time.ToString();
                if (e.ProgressPercentage == 1)
                {
                    curr.UI_lock = false;
                }
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
            finally
            {
                if (e.ProgressPercentage == 1)
                {
                    curr.UI_lock = false;
                }
            }
        }

        void child_drawer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage == 1)
                {
                    GeneticAlgorithm curr = (GeneticAlgorithm)e.UserState;
                    curr.UI_lock = false;
                    return;
                }
                string city1 = (string)((Object[])e.UserState)[0];
                string city2 = (string)((Object[])e.UserState)[1];
                Point from = (Point)((Object[])e.UserState)[2];
                Point to = (Point)((Object[])e.UserState)[3];
                Line l = null;
                if (exec_grid.FindName("ga_link_" + city1 + city2 + "c") != null)
                {
                    l = (Line)exec_grid.FindName("ga_link_" + city1 + city2 + "c");
                }
                else if (exec_grid.FindName("ga_link_" + city2 + city1 + "c") != null)
                {
                    l = (Line)exec_grid.FindName("ga_link_" + city2 + city1 + "c");
                }
                if (l != null)
                {
                    l.X1 = from.X;
                    l.Y1 = from.Y;
                    l.X2 = to.X;
                    l.Y2 = to.Y;
                    l.Visibility = Visibility.Visible;
                }
            }
            catch(Exception t)
            {
                MessageBox.Show(t.ToString());
            }
        }

        void child_drawer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                genome child = (genome)((Object[])e.Argument)[0];
                for (int j = 0; j < child.Data.Count; j++)
                {
                    if (j + 1 < child.Data.Count)
                    {
                        string city1 = child.Data[j].Name;
                        string city2 = child.Data[j + 1].Name;
                        City_data[] cd = new City_data[2];
                        cd[0] = Cities.get_city(city1);
                        cd[1] = Cities.get_city(city2);
                        Point[] city = new Point[2];
                        city[0] = new Point();
                        city[1] = new Point();
                        city[0].X = cd[0].X * 0.68;
                        city[0].Y = cd[0].Y * 0.55;
                        city[1].X = cd[1].X * 0.68;
                        city[1].Y = cd[1].Y * 0.55;
                        double m = ((city[1].Y - city[0].Y) / (city[1].X - city[0].X));
                        double c = city[0].Y - (city[0].X * m);
                        if (Math.Abs(city[0].X - city[1].X) > Math.Abs(city[0].Y - city[1].Y))//travel on x
                        {

                            if (city[0].X < city[1].X)//forward
                            {
                                for (double i = city[0].X; i < city[1].X; i += 2 * speed_val)//offseted
                                {
                                    //-100,-200
                                    Point p = new Point();
                                    p.X = i;
                                    p.Y = m * p.X + c;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }
                            else//backward
                            {
                                for (double i = city[0].X; i > city[1].X; i -= 2 * speed_val)
                                {
                                    //-50,-100
                                    Point p = new Point();
                                    p.X = i;
                                    p.Y = m * p.X + c;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }

                        }
                        else//travel on y
                        {
                            if (city[0].Y < city[1].Y)//forward
                            {
                                for (double i = city[0].Y; i < city[1].Y; i += 2 * speed_val)
                                {

                                    Point p = new Point();
                                    p.Y = i;
                                    p.X = (p.Y - c) / m;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }
                            else//backward
                            {
                                for (double i = city[0].Y; i > city[1].Y; i -= 2 * speed_val)
                                {
                                    Point p = new Point();
                                    p.Y = i;
                                    p.X = (p.Y - c) / m;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }

                        }
                    }
                    else
                    {
                        string city1 = child.Data[j].Name;
                        string city2 = child.Data[0].Name;
                        City_data[] cd = new City_data[2];
                        foreach (var item in Cities.Cities_list)
                        {
                            if (item.Name == city1)
                            {
                                cd[0] = item;
                            }
                            else if (item.Name == city2)
                            {
                                cd[1] = item;
                            }
                        }
                        Point[] city = new Point[2];
                        city[0].X = cd[0].X * 0.68;
                        city[0].Y = cd[0].Y * 0.55;
                        city[1].X = cd[1].X * 0.68;
                        city[1].Y = cd[1].Y * 0.55;
                        double m = ((city[1].Y - city[0].Y) / (city[1].X - city[0].X));
                        double c = city[0].Y - (city[0].X * m);
                        if (Math.Abs(city[0].X - city[1].X) > Math.Abs(city[0].Y - city[1].Y))//travel on x
                        {

                            if (city[0].X < city[1].X)//forward
                            {
                                for (double i = city[0].X; i < city[1].X; i += 2 * speed_val)//offseted
                                {
                                    //-100,-200
                                    Point p = new Point();
                                    p.X = i;
                                    p.Y = m * p.X + c;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }
                            else//backward
                            {
                                for (double i = city[0].X; i > city[1].X; i -= 2 * speed_val)
                                {
                                    //-50,-100
                                    Point p = new Point();
                                    p.X = i;
                                    p.Y = m * p.X + c;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }

                        }
                        else//travel on y
                        {
                            if (city[0].Y < city[1].Y)//forward
                            {
                                for (double i = city[0].Y; i < city[1].Y; i += 2 * speed_val)
                                {

                                    Point p = new Point();
                                    p.Y = i;
                                    p.X = (p.Y - c) / m;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }
                            else//backward
                            {
                                for (double i = city[0].Y; i > city[1].Y; i -= 2 * speed_val)
                                {
                                    Point p = new Point();
                                    p.Y = i;
                                    p.X = (p.Y - c) / m;
                                    child_drawer.ReportProgress(0, new Object[] { city1, city2, city[0], p });
                                    Thread.Sleep(20);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString());
            }
            finally
            {
                GeneticAlgorithm curr = (GeneticAlgorithm)((Object[])e.Argument)[1];
                child_drawer.ReportProgress(1, curr);
            }
        }

        void mutator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            genome child = (genome)((Object[])e.UserState)[0];
            GeneticAlgorithm curr = (GeneticAlgorithm)((Object[])e.UserState)[1];
            curr.UI_lock = false;
        }

        void mutator_DoWork(object sender, DoWorkEventArgs e)
        {
            
            while(speed_val==0)
            { }
           // Thread.Sleep(TimeSpan.FromSeconds(1/speed_val ));
            mutator.ReportProgress(1, (Object[])e.Argument);
        }

        //----------------------------------- UI drawing
        private void create_cityUI_ga(string nm, double X, double Y,Canvas canvasArea1,double Xcityoffset,double Ycityoffset,string addinfo,int size)
        {
            X = X * Xcityoffset;
            Y = Y * Ycityoffset;
            Shape Rendershape = null;
            Rendershape = new Ellipse() { Height = size, Width = size };
            RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#153FC4"));
            b.GradientOrigin = new Point(0.3, 0.3);
            b.RadiusX = b.RadiusX * 1.35;
            b.RadiusY = b.RadiusY * 1.35;
            TextBlock t = new TextBlock();
            Rendershape.Name = nm + "_shape_ga"+addinfo;
            t.Text = nm;
            t.Name = nm + "_txt_ga" + addinfo;
            Rendershape.Fill = b;
            Rendershape.Effect = create_shadow();
            Canvas.SetLeft(Rendershape, X);
            Canvas.SetTop(Rendershape, Y);
            t.IsEnabled = false;
            Canvas.SetLeft(t, X + 16);
            Canvas.SetTop(t, Y + 12);
            canvasArea1.Children.Add(Rendershape);
            canvasArea1.RegisterName(Rendershape.Name, Rendershape);
            canvasArea1.Children.Add(t);
            canvasArea1.RegisterName(t.Name, t);
        }

        private void draw_connections_ga(string nm, Canvas canvasArea1,double Xcityoffset,double Ycityoffset,string addinfo)
        {
            City_data currcity = Cities.get_city(nm);
            City_data tocity;
            Line l = new Line();
            ObservableCollection<Cost_Time_Dependency_Data> data = Cities.getmetriclist_asobservable(nm);
            //remove all connection
            remove_connections_ga(nm,canvasArea1,addinfo);
            List<Line> links = new List<Line>();
            List<Point> points = new List<Point>();
            foreach (var item in data)
            {
                if ((item.CityCost > 0) || (item.CityTime > 0))
                {
                    l = new Line();
                   // l.Visibility = Visibility.Hidden;
                    l.X1 = currcity.X * Xcityoffset;
                    l.Y1 = currcity.Y * Ycityoffset;
                    tocity = Cities.get_city(item.CityName);
                    l.X2 = tocity.X * Xcityoffset;
                    l.Y2 = tocity.Y * Ycityoffset;
                    l.Name = "ga_link_" + nm + item.CityName+addinfo;
                    l.StrokeThickness = 1;
                    l.Stroke = Brushes.Red;
                    links.Add(l);
                }
            }
            for (int i = 0; i < links.Count; i++)
            {
                Addline_ga(links[i],canvasArea1);
            }
        }

        private void remove_connections_ga(string nm,Canvas canvasArea1,string addinfo)
        {
            List<AI_Algorithm_Simulator.classes.Path> Presentpaths = Cities.getmetriclist_asPath(nm);
            if (Presentpaths != null)
            {
                foreach (var item in Presentpaths)
                {
                    Line l;
                    if (exec_grid.FindName("ga_link_" + nm + item.city2+addinfo) as Line != null)
                    {
                        l = exec_grid.FindName("ga_link_" + nm + item.city2 + addinfo) as Line;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("ga_link_" + nm + item.city2 + addinfo);
                    }
                    else if (exec_grid.FindName("ga_link_" + item.city2 + nm + addinfo) as Line != null)
                    {
                        l = exec_grid.FindName("ga_link_" + item.city2 + nm + addinfo) as Line;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("ga_link_" + item.city2 + nm + addinfo);
                    }
                }
            }
        }

        private void Addline_ga(Line newline,Canvas canvasArea1)
        {
            Canvas.SetZIndex(newline, -1);
            newline.Effect = create_shadow();
            canvasArea1.Children.Add(newline);
            canvasArea1.RegisterName(newline.Name, newline);
        }

       //--------------------------------- ui binded collection
        public ObservableCollection<secondary_display_data> GaParentCollection
        {
            get { return ga_parent_data; }
        }

        public ObservableCollection<secondary_display_data> GaChildCollection
        {
            get { return ga_child_data; }
        }


    }
}
