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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace AI_Algorithm_Simulator
{
    partial class MainWindow:Window
    {
        List<Brush> mybrushes=new List<Brush>();
        private string get_time(long ticks)
        {
            string time="";
            double ns = 1000000000.0 * (double)ticks /System.Diagnostics.Stopwatch.Frequency;
            double ms = ns / 1000.0;
            double mis = ms / 1000;
            double s = mis / 1000;
            double min = s / 60;
            double hrs = min / 60;
            if(hrs>1)
            {
                time = Math.Round(hrs, 2).ToString() + " hours";
            }
            else if(min>1)
            {
                time = Math.Round(min, 2).ToString() + " minutes";
            }
            else if(s>1)
            {
                time = Math.Round(s,2).ToString() + " seconds";
            }
            else if(mis>1)
            {
                time = Math.Round(ms,2).ToString() + " miliseconds";
            }
            else if(ms>1)
            {
                time = Math.Round(mis,2).ToString() + " microseconds";
            }
            else
            {
                time = Math.Round(ns, 2).ToString() + " nanoseconds";
            }
            return time;
        }

        private void set_brush()
        {
            mybrushes.Add(Brushes.Cyan);
            mybrushes.Add(Brushes.Gold);
            mybrushes.Add(Brushes.LightGreen);
            mybrushes.Add(Brushes.HotPink);
            mybrushes.Add(Brushes.Orange);
            mybrushes.Add(Brushes.Red);
            mybrushes.Add(Brushes.Yellow);
            mybrushes.Add(Brushes.Lavender);
        }
        private void initial_report_prep()
        {
            try
            {
                for (int i = 0; i < Cities.Cities_list.Count; i++)//create currsoln  city
                {
                    create_cityUI_report(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12, curr_soln_canvas, 0.34, 0.37, "curr", 10);
                }
                for (int i = 0; i < Cities.used_alphas.Count; i++)//draw connections currsoln 
                {
                    draw_connections_report(Cities.used_alphas[i], curr_soln_canvas, 0.34, 0.37, "curr");//do for all used alphas
                }
                for (int i = 0; i < Cities.Cities_list.Count; i++)//create currsoln  city
                {
                    create_cityUI_report(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12, best_soln_canvas, 0.34, 0.37, "best", 10);
                }
                for (int i = 0; i < Cities.used_alphas.Count; i++)//draw connections currsoln 
                {
                    draw_connections_report(Cities.used_alphas[i], best_soln_canvas, 0.34, 0.37, "best");//do for all used alphas
                }
            }
            catch(Exception)
            {

            }
        }

        private void create_report()
        {
            if(mybrushes.Count==0)
            {
                set_brush();
            }
            this.soln_list.Items.Clear();
            this.graph.Visibility = Visibility.Hidden;
            initial_report_prep();
            sort();
            foreach (var item in algorithm_list)
            {
                this.soln_list.Items.Add(item.Name);
            }
            this.soln_list.SelectedIndex = 0;
            Algorithm_base best=algorithm_list[0];
            /*foreach (var item in algorithm_list)
            {
                if(give_localbest(item)<give_localbest(best))
                {
                    best = item;
                }
            }*/
            foreach (var item in algorithm_list)
            {
                if(item.Rank==1)
                {
                    best = item;
                }
            }
            //rank algorithms
            //find best and put

            if (best.getalgotype() == "GeneticAlgorithm")
            {
                GeneticAlgorithm ga = (GeneticAlgorithm)best.returnalgoobj();
                this.bestsoln_type_txt.Text = "Genetic Algorithm";
                this.bestsoln_name_txt.Text = ga.Name;
                this.bestsoln_runtime_txt.Text = get_time(ga.Watch.ElapsedTicks);
                this.bestsoln_foundat_txt.Text = get_time(ga.Localbest_time[ga.Localbest_time.Count-1]);
                this.bestsoln_cost_txt.Text = ga.Localbest[ga.Localbest.Count - 1].Cost.ToString();
                this.bestsoln_time_text.Text = ga.Localbest[ga.Localbest.Count - 1].Time.ToString();
                foreach (var item in Cities.Metric_list)
                {
                    if (FindName("report_link_" + item.city1 + item.city2 + "best") != null)
                    {
                        Line l = (Line)FindName("report_link_" + item.city1 + item.city2 + "best");
                        if (checkifindparent(item.city1, item.city2, ga.Localbest[ga.Localbest.Count - 1]))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (FindName("report_link_" + item.city2 + item.city1 + "best") != null)
                    {
                        Line l = (Line)FindName("report_link_" + item.city2 + item.city1 + "best");
                        if (checkifindparent(item.city1, item.city2, ga.Localbest[ga.Localbest.Count - 1]))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else if (best.getalgotype() == "AntColonyOptimization")
            {
                ACO ga = (ACO)best.returnalgoobj();
                this.bestsoln_type_txt.Text = "Ant Colony Optimization";
                this.bestsoln_name_txt.Text = ga.Name;
                this.bestsoln_runtime_txt.Text = get_time(ga.Watch.ElapsedTicks);
                this.bestsoln_foundat_txt.Text = get_time(ga.Localbest_time[ga.Localbest_time.Count - 1]);
                this.bestsoln_cost_txt.Text = ga.Localbest[ga.Localbest.Count-1].Cost.ToString();
                this.bestsoln_time_text.Text = ga.Localbest[ga.Localbest.Count - 1].Time.ToString();
                this.currsoln_rank_txt.Text = "";
                foreach (var item in Cities.Metric_list)
                {
                    if (FindName("report_link_" + item.city1 + item.city2 + "best") != null)
                    {
                        Line l = (Line)FindName("report_link_" + item.city1 + item.city2 + "best");
                        if (checkifindparent(item.city1, item.city2, new genome { Data = ga.Localbest[ga.Localbest.Count - 1].Data }))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (FindName("report_link_" + item.city2 + item.city1 + "best") != null)
                    {
                        Line l = (Line)FindName("report_link_" + item.city2 + item.city1 + "best");
                        if (checkifindparent(item.city1, item.city2, new genome { Data = ga.Localbest[ga.Localbest.Count - 1].Data }))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private void sort()
        {
            foreach (var item in algorithm_list)
            {
                item.Rank = 0;
            }
            int r = 1;
            Algorithm_base best;
            for (int i = 0; i < algorithm_list.Count; i++)
            {
                if (algorithm_list[i].Rank == 0)
                {
                    best = algorithm_list[i];
                    for (int j = 0; j < algorithm_list.Count; j++)
                    {
                        if (algorithm_list[j].Rank == 0)
                        {
                           if (give_localbest(best) > give_localbest(algorithm_list[j]))
                           {
                               best = algorithm_list[j];
                           }
                           else if (give_localbest(best) == give_localbest(algorithm_list[j]))
                            {
                               if (give_sec_localbest(best) > give_sec_localbest(algorithm_list[j]))
                                {
                                  best = algorithm_list[j];
                                }
                            }
                        }
                    }
                    best.Rank = r;
                    i = -1;
                    r += 1;
                }
            }
        }

        private int give_localbest(Algorithm_base t)
        {
            if(t.getalgotype()=="GeneticAlgorithm")
            {
                GeneticAlgorithm cur = (GeneticAlgorithm)t.returnalgoobj();
                if (Algorithm_base.Mode == "cost")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Cost;
                }
                else if (Algorithm_base.Mode == "time")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Time;
                }
                else if (Algorithm_base.Mode == "cost-time")
                {
                    cur.Localbest[cur.Localbest.Count - 1].Fitness = cur.Localbest[cur.Localbest.Count - 1].Cost + cur.Localbest[cur.Localbest.Count - 1].Time;
                    if (cur.Localbest[cur.Localbest.Count - 1].Cost > Algorithm_base.Recommended_Cost)
                    {
                        cur.Localbest[cur.Localbest.Count - 1].Fitness += (cur.Localbest[cur.Localbest.Count - 1].Cost - Algorithm_base.Recommended_Cost);
                    }

                    if (cur.Localbest[cur.Localbest.Count - 1].Time > Algorithm_base.Recommended_Time)
                    {
                        cur.Localbest[cur.Localbest.Count - 1].Fitness += (cur.Localbest[cur.Localbest.Count - 1].Time - Algorithm_base.Recommended_Time);
                    }
                    return cur.Localbest[cur.Localbest.Count - 1].Fitness;
                }
            }
            else if(t.getalgotype()=="AntColonyOptimization")
            {
                ACO cur = (ACO)t.returnalgoobj();
                if(Algorithm_base.Mode=="cost")
                {
                    return cur.Localbest[cur.Localbest.Count-1].Cost;
                }
                else if(Algorithm_base.Mode=="time")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Time;
                }
                else if(Algorithm_base.Mode=="cost-time")
                {
                    cur.Localbest[cur.Localbest.Count - 1].Fitness = cur.Localbest[cur.Localbest.Count - 1].Cost + cur.Localbest[cur.Localbest.Count - 1].Time;
                    if (cur.Localbest[cur.Localbest.Count - 1].Cost > Algorithm_base.Recommended_Cost)
                    {
                        cur.Localbest[cur.Localbest.Count - 1].Fitness += (cur.Localbest[cur.Localbest.Count - 1].Cost - Algorithm_base.Recommended_Cost);
                    }

                    if (cur.Localbest[cur.Localbest.Count - 1].Time > Algorithm_base.Recommended_Time)
                    {
                        cur.Localbest[cur.Localbest.Count - 1].Fitness += (cur.Localbest[cur.Localbest.Count - 1].Time - Algorithm_base.Recommended_Time);
                    }
                    return cur.Localbest[cur.Localbest.Count - 1].Fitness;
                }
            }
            return 0;
        }

        private int give_sec_localbest(Algorithm_base t)
        {
            if (t.getalgotype() == "GeneticAlgorithm")
            {
                GeneticAlgorithm cur = (GeneticAlgorithm)t.returnalgoobj();
                if (Algorithm_base.Mode == "cost")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Time;
                }
                else if (Algorithm_base.Mode == "time")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Cost;
                }
             }
            else if (t.getalgotype() == "AntColonyOptimization")
            {
                ACO cur = (ACO)t.returnalgoobj();
                if (Algorithm_base.Mode == "cost")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Time;
                }
                else if (Algorithm_base.Mode == "time")
                {
                    return cur.Localbest[cur.Localbest.Count - 1].Cost;
                }
            }
            return 0;
        }

        private void soln_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.soln_list.Items.Count != 0)
            {
                Algorithm_base selected = algorithm_list.Find((o) => o.Name == this.soln_list.SelectedItem.ToString());
                if (selected.getalgotype() == "GeneticAlgorithm")
                {
                    GeneticAlgorithm ga = (GeneticAlgorithm)selected.returnalgoobj();
                    this.currsoln_type_txt.Text = "Genetic Algorithm";
                    this.currsoln_runtime_txt.Text = get_time(ga.Watch.ElapsedTicks);
                    this.currsoln_time_txt.Text = ga.Localbest[ga.Localbest.Count - 1].Time.ToString();
                    this.currsoln_cost_txt.Text = ga.Localbest[ga.Localbest.Count - 1].Cost.ToString();
                    this.currsoln_foundat_txt.Text = get_time(ga.Localbest_time[ga.Localbest_time.Count - 1]);
                    this.currsoln_rank_txt.Text = ga.Rank.ToString();
                    foreach (var item in Cities.Metric_list)
                    {
                        if (FindName("report_link_" + item.city1 + item.city2 + "curr") != null)
                        {
                            Line l = (Line)FindName("report_link_" + item.city1 + item.city2 + "curr");
                            if (checkifindparent(item.city1, item.city2, ga.Localbest[ga.Localbest.Count - 1]))
                            {
                                l.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                l.Visibility = Visibility.Hidden;
                            }
                        }
                        else if (FindName("report_link_" + item.city2 + item.city1 + "curr") != null)
                        {
                            Line l = (Line)FindName("report_link_" + item.city2 + item.city1 + "curr");
                            if (checkifindparent(item.city1, item.city2, ga.Localbest[ga.Localbest.Count-1]))
                            {
                                l.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                l.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
                else if (selected.getalgotype() == "AntColonyOptimization")
                {
                    ACO ga = (ACO)selected.returnalgoobj();
                    this.currsoln_type_txt.Text = "Ant Colony Optimization";
                    this.currsoln_runtime_txt.Text = get_time(ga.Watch.ElapsedTicks);
                    this.currsoln_time_txt.Text = ga.Localbest[ga.Localbest.Count-1].Time.ToString();
                    this.currsoln_cost_txt.Text = ga.Localbest[ga.Localbest.Count - 1].Cost.ToString();
                    this.currsoln_foundat_txt.Text = get_time(ga.Localbest_time[ga.Localbest_time.Count - 1]);
                    this.currsoln_rank_txt.Text = ga.Rank.ToString();
                    foreach (var item in Cities.Metric_list)
                    {
                        if (FindName("report_link_" + item.city1 + item.city2 + "curr") != null)
                        {
                            Line l = (Line)FindName("report_link_" + item.city1 + item.city2 + "curr");
                            if (checkifindparent(item.city1, item.city2, new genome { Data = ga.Localbest[ga.Localbest.Count - 1].Data }))
                            {
                                l.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                l.Visibility = Visibility.Hidden;
                            }
                        }
                        else if (FindName("report_link_" + item.city2 + item.city1 + "curr") != null)
                        {
                            Line l = (Line)FindName("report_link_" + item.city2 + item.city1 + "curr");
                            if (checkifindparent(item.city1, item.city2, new genome { Data = ga.Localbest[ga.Localbest.Count - 1].Data }))
                            {
                                l.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                l.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
            }
        }

        private void create_cityUI_report(string nm, double X, double Y, Canvas canvasArea1, double Xcityoffset, double Ycityoffset, string addinfo, int size)
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
            Rendershape.Name = nm + "_shape_report" + addinfo;
            t.Text = nm;
            t.Name = nm + "_txt_report" + addinfo;
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

        private void draw_connections_report(string nm, Canvas canvasArea1, double Xcityoffset, double Ycityoffset, string addinfo)
        {
            City_data currcity = Cities.get_city(nm);
            City_data tocity;
            Line l = new Line();
            ObservableCollection<Cost_Time_Dependency_Data> data = Cities.getmetriclist_asobservable(nm);
            //remove all connection
            remove_connections_report(nm, canvasArea1, addinfo);
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
                    l.Name = "report_link_" + nm + item.CityName + addinfo;
                    l.StrokeThickness = 1;
                    l.Stroke = Brushes.Red;
                    links.Add(l);
                }
            }
            for (int i = 0; i < links.Count; i++)
            {
                Addline_ga(links[i], canvasArea1);
            }
        }

        private void remove_connections_report(string nm, Canvas canvasArea1, string addinfo)
        {
            List<AI_Algorithm_Simulator.classes.Path> Presentpaths = Cities.getmetriclist_asPath(nm);
            if (Presentpaths != null)
            {
                foreach (var item in Presentpaths)
                {
                    Line l;
                    if (exec_grid.FindName("report_link_" + nm + item.city2 + addinfo) as Line != null)
                    {
                        l = exec_grid.FindName("report_link_" + nm + item.city2 + addinfo) as Line;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("report_link_" + nm + item.city2 + addinfo);
                    }
                    else if (exec_grid.FindName("report_link_" + item.city2 + nm + addinfo) as Line != null)
                    {
                        l = exec_grid.FindName("report_link_" + item.city2 + nm + addinfo) as Line;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("report_link_" + item.city2 + nm + addinfo);
                    }
                }
            }
        }

        private void Addline_report(Line newline, Canvas canvasArea1)
        {
            Canvas.SetZIndex(newline, -1);
            newline.Effect = create_shadow();
            canvasArea1.Children.Add(newline);
            canvasArea1.RegisterName(newline.Name, newline);
        }
        private double get_time_double(long ticks)
        {
            double time;
            double ns = 1000000000.0 * (double)ticks / System.Diagnostics.Stopwatch.Frequency;
            double ms = ns / 1000.0;
            double mis = ms / 1000;
            double s = mis / 1000;
            double min = s / 60;
            double hrs = min / 60;
            return ms;
        }

        private void graph_btn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.graph.Visibility = Visibility.Visible;
            
            for (int i = 13; i < plotter.Children.Count; i++)
            {
                this.plotter.Children.RemoveAt(i);
            }
            List<CompositeDataSource> compositeDataSource1 = new List<CompositeDataSource>();
            foreach (var item in algorithm_list)
            {
                int[] cost=null;
                double[] time=null;
                if(item.getalgotype()=="GeneticAlgorithm")
                {
                    GeneticAlgorithm ga = (GeneticAlgorithm)item.returnalgoobj();
                    cost = new int[ga.Localbest.Count];
                    time = new double[ga.Localbest.Count];
                    for (int i = 0; i < ga.Localbest.Count; i++)
                    {
                        if (Algorithm_base.Mode == "cost")
                        {
                            cost[i] = ga.Localbest[i].Cost;
                        }
                        else if(Algorithm_base.Mode=="time")
                        {
                            cost[i] = ga.Localbest[i].Time;
                        }
                        else if(Algorithm_base.Mode=="cost-time")
                        {
                            cost[i] = ga.Localbest[i].Fitness;
                        }
                        time[i] = get_time_double(ga.Localbest_time[i]);
                    }
                }
                else if (item.getalgotype() == "AntColonyOptimization")
                {
                    ACO ga = (ACO)item.returnalgoobj();
                    cost= new int[ga.Localbest.Count];
                    time = new double[ga.Localbest.Count];
                    for (int i = 0; i < ga.Localbest.Count; i++)
                    {
                        if (Algorithm_base.Mode == "cost")
                        {
                            cost[i] = ga.Localbest[i].Cost;
                        }
                        else if (Algorithm_base.Mode == "time")
                        {
                            cost[i] = ga.Localbest[i].Time;
                        }
                        else if (Algorithm_base.Mode == "cost-time")
                        {
                            cost[i] = ga.Localbest[i].Fitness;
                        }
                        time[i] = get_time_double(ga.Localbest_time[i]);
                    }
                }
                var timeDataSource = new EnumerableDataSource<double>(time);
                timeDataSource.SetXMapping(x => x);

                var costDataSource = new EnumerableDataSource<int>(cost);
                costDataSource.SetYMapping(y => y);
                compositeDataSource1.Add(new CompositeDataSource(timeDataSource, costDataSource));
                
            }
            for (int i = 0; i < compositeDataSource1.Count; i++)
			{
                plotter.AddLineGraph(compositeDataSource1[i], new Pen(mybrushes[i], 2), new CirclePointMarker { Size = 5.0, Fill = Brushes.White }, new PenDescription(algorithm_list[i].Name +" "+algorithm_list[i].getalgotype()));
            }
            
            
        }

        private void graph_back_btn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.graph.Visibility = Visibility.Hidden;
            
        }       

    }
}
