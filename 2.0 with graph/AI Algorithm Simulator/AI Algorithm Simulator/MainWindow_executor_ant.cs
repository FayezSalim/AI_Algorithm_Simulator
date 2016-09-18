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
    public delegate void UpdateProgressDelegate(int recordCount);
    public delegate void Draw_ant_delegate(int antno, ACO curr, string info, int iteration);
    public delegate void Update_pheromone_delegate(bool firsttime, string process, ACO curr, string info, Ant best);
    public delegate void Ant_sec_updater(ACO curr,int gen);
    public partial class MainWindow:Window
    {
        //our delegate used for updating the UI
        BackgroundWorker ant_walker = new BackgroundWorker();
        BackgroundWorker ant_update_pheromone = new BackgroundWorker();
        BackgroundWorker[] algo_threads;
        Storyboard animate_ant;//ant animator
        double speed_val = 0;
        ObservableCollection<secondary_display_data> ant_data = new ObservableCollection<secondary_display_data>();
        ObservableCollection<secondary_display_data> path_data = new ObservableCollection<secondary_display_data>();
        bool ignoreswap = true;

        private double Xcityoffset = 0.75,Ycityoffset=0.95;

         //-----------------------delegate implementation
       

        public void draw_ant(int antno,ACO curr,string info,int iteration)
        {
           curr.UI_lock = true;//lock algorithm for ui update
           this.ant_img.RenderTransform = null;
           this.ant_img.Visibility = Visibility.Visible;
           this.wizard_info.Content = info;
           this.ant_anim_no_txt.Text = "Ant " + antno.ToString();
           this.ant_process_text.Text = "Constructing Solution";
           this.ant_gen_txt.Text = iteration.ToString() ;
           Point[] p=new Point[2];
           p[0] = new Point { X = curr.Workingset[antno].Data[curr.Workingset[antno].Data.Count - 2].X * Xcityoffset, Y =( curr.Workingset[antno].Data[curr.Workingset[antno].Data.Count - 2].Y*Ycityoffset)-10 };
           p[1] = new Point { X = curr.Workingset[antno].Data[curr.Workingset[antno].Data.Count - 1].X * Xcityoffset, Y = (curr.Workingset[antno].Data[curr.Workingset[antno].Data.Count - 1].Y * Ycityoffset)-10 };
           double angle=find_angle(p[0],p[1]);
           if(p[0].X<=p[1].X)//clockwise - positive angle
           {
               if(p[0].Y<p[1].Y)//outside 90 ---y logic is oppsoite donno y
               {
                   angle += 90;
               }
               else if(p[0].Y>=p[1].Y)//inside 90
               {
                   if(p[0].Y!=p[1].Y)
                   {
                       angle = 90 - angle;
                   }
               }
           }
           else if(p[0].X>p[1].X)//anti-clockwise negative angle
           {
               if (p[0].Y <p[1].Y)//outside 90
               {
                   angle += 90;
                   angle *= -1;
               }
               else if (p[0].Y >= p[1].Y)//inside 90
               {
                   if (p[0].Y != p[1].Y)
                   {
                       angle = 90 - angle;
                   }
                   angle *= -1;
               }
           }
          RotateTransform rotateTransform1 = new RotateTransform(angle);
           this.ant_img.RenderTransform = rotateTransform1;
           Object[] obj = new Object[2];
           obj[0] = p;
           obj[1] = curr;
            while(ant_walker.IsBusy==true)
            {

            }
           ant_walker.RunWorkerAsync(obj);
          // TranslateTransform trans = new TranslateTransform();
           
           //this.ant_img.RenderTransform = trans;
            /*
           animate_ant = new Storyboard();
           double time = (find_distance(p[0], p[1]) / 30)-this.speed_slider.Value*2;//multipy with speed qoutient or add
           if(time<1)
           {
                time = 0.5;
           }
           DoubleAnimation anim1 = new DoubleAnimation(p[0].Y+12, p[1].Y+12 , TimeSpan.FromSeconds(time));
           DoubleAnimation anim2 = new DoubleAnimation(p[0].X+16, p[1].X+16 , TimeSpan.FromSeconds(time));
           Storyboard.SetTargetName(anim1, this.ant_img.Name);
           Storyboard.SetTargetProperty(anim1, new PropertyPath(Canvas.TopProperty));
           animate_ant.Children.Add(anim1);
           Storyboard.SetTargetName(anim2, this.ant_img.Name);
           Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.LeftProperty));
           animate_ant.Children.Add(anim2);
           animate_ant.Completed += animate_ant_Completed;
           animate_ant.Begin(this);
           */
           
           foreach (var item in Cities.used_alphas)
	       {
               if(curr.Workingset[antno].Data.Find((o)=>o.Name==item)!=null)//travelled by ant
               {
                   if(curr.Workingset[antno].Data[curr.Workingset[antno].Data.Count-1].Name==item) //check if it is the current city
                   {
                       //make yellow blink
                       Ellipse elip=(Ellipse)canvasArea1.FindName(item+"_shape_ant");
                       /*GradientStop one = new GradientStop(Colors.White, 0);
                       GradientStop two;//animate city color
                       two = new GradientStop((Color)ColorConverter.ConvertFromString("#FF7400"), 1);
                      /* if (FindName("gradtwo") == null)
                        {
                            this.RegisterName("gradtwo", two);
                        }*/
                       /* Storyboard anim_shape_color = new Storyboard();
                        ColorAnimation anim_color = new ColorAnimation((Color)ColorConverter.ConvertFromString("#153FC4"), (Color)ColorConverter.ConvertFromString("#FF7400"), TimeSpan.FromSeconds(1));
                        Storyboard.SetTargetName(anim_color, item+"_shape_ant");
                        Storyboard.SetTargetProperty(anim_color, new PropertyPath((Ellipse.FillProperty)(RadialGradientBrush.GradientStopsProperty)[1].(GradientStop.ColorProperty));
                        anim_shape_color.Children.Add(anim_color);
                        anim_shape_color.AutoReverse = true;
                        anim_shape_color.RepeatBehavior = RepeatBehavior.Forever;
                        anim_shape_color.Begin(this);*/
                       RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#FBFB28"));
                       //RadialGradientBrush b = new RadialGradientBrush();
                       b.GradientOrigin = new Point(0.3, 0.3);
                       b.RadiusX = b.RadiusX * 1.35;
                       b.RadiusY = b.RadiusY * 1.35;
                       elip.Fill=b;
                  }
                   else
                   {
                       Ellipse elip=(Ellipse)canvasArea1.FindName(item+"_shape_ant");
                       RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#979797"));
                       b.GradientOrigin = new Point(0.3, 0.3);
                       b.RadiusX = b.RadiusX * 1.35;
                       b.RadiusY = b.RadiusY * 1.35;
                       elip.Fill=b;
                       //make grey
                   }
               }
               else//not yet travelled
               {
                   //make blue
                   Ellipse elip=(Ellipse)canvasArea1.FindName(item+"_shape_ant");
                   RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#153FC4"));
                   b.GradientOrigin = new Point(0.3, 0.3);
                   b.RadiusX = b.RadiusX * 1.35;
                   b.RadiusY = b.RadiusY * 1.35;
                   elip.Fill=b;
               }
          }

           if ((antno == curr.Workingset.Count - 1)&&(curr.Workingset[antno].Data.Count==this.Cities.used_alphas.Count))//all ants have completed one round
           {
               this.ant_img.Visibility = Visibility.Hidden;
           }
            
         }

        public void Update_pheromone(bool firsttime,string process,ACO curr,string info,Ant best)
        {
            
           curr.UI_lock=true;
           if (process == "update")
           {
               this.ant_process_text.Text = "Updating pheromones";
           }
           else if (process == "evaporate")
           {
               this.ant_process_text.Text = "Evaporation";
               this.ant_lb_cost_txt.Text = best.Cost.ToString();
               this.ant_lb_time_txt.Text = best.Time.ToString();
               string tool = "";
               foreach (var item  in best.Data)
               {
                   tool += item.Name;
               }
               this.ant_lb_txt.ToolTip = tool;
           }
           this.wizard_info.Content = info;
           Object[] obj = new Object[2];
           obj[0] = firsttime;
           obj[1] = curr;
           ant_update_pheromone.RunWorkerAsync(obj);
       }

        private void animate_ant_Completed(object sender, EventArgs e)
        {
            foreach (var item in algorithm_list)
            {
                item.UI_lock = false;
            }
            
        }

        public void ant_sec_update(ACO curr,int gen)
        {
            path_data.Clear();
            if (gen != -1)
            {
                this.ant_sec_gen_txt.Text = gen.ToString();
            }
            foreach (var item in curr.Area)
            {
                path_data.Add(new secondary_display_data { name = item.city1 + item.city2, path = Math.Round(item.phermone,3).ToString() });
            }
            ant_data.Clear();
            int i = 1;
            string path = "";
            foreach (var item in curr.Workingset)//set ant data
            {
                foreach (var item1 in item.Data)
                {
                    path += item1.Name;
                }
                ant_data.Add(new secondary_display_data { name = "Ant " + i.ToString(), cost = item.Cost, time = item.Time, path = path });
                i++;
            }
            if (curr.Localbest.Count!=0)
            {
                this.ant_sec_cost_txt.Text = curr.Localbest[curr.Localbest.Count-1].Cost.ToString();//set localbest data
                this.ant_sec_time_txt.Text = curr.Localbest[curr.Localbest.Count - 1].Time.ToString();
                path = "";
                foreach (var item1 in curr.Localbest[curr.Localbest.Count - 1].Data)
                {
                    path += item1.Name;
                }
                this.ant_sec_lb.ToolTip = path;
            }
            if(gen+1==curr.Iterations)
            {
                this.ant_sec_gen_txt.Text = "Completed";
            }
            curr.UI_lock = false;
        }

        //----------------------------------------------worker methods
       

       void ant_update_pheromone_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Object[] obj=(Object[]) e.UserState;
            Edge way = (Edge)obj[0];
            bool firsttime = (bool)obj[1];
            ACO curr = (ACO)obj[2];
            this.ant_anim_no_txt.Text = "Path : " + way.city1 + way.city2;
            Label l = new Label();
            Line lne = new Line();
            if (FindName("ant_info_" + way.city1 + way.city2) != null)
            {
                l = (Label)FindName("ant_info_" + way.city1 + way.city2);
                lne = (Line)FindName("ant_link_" + way.city1 + way.city2);
            }
            else if (FindName("ant_info_" + way.city2 + way.city1) != null)
            {
                l = (Label)FindName("ant_info_" + way.city1 + way.city2);
                lne = (Line)FindName("ant_link_" + way.city1 + way.city2);
            }
            string newpheromone = l.Content.ToString();
            if (!firsttime)
            {
               // newpheromone = l.Content.ToString().Remove(l.Content.ToString().LastIndexOf(','));
        
            }
            //newpheromone = newpheromone + "," +Math.Round(way.phermone,2);
            newpheromone = Math.Round(way.phermone, 2).ToString();
            lne.StrokeThickness = way.phermone * 0.2;
            l.Content = newpheromone;
            if (e.ProgressPercentage == 1)
            {
                curr.UI_lock = false;
            }
        }

        void ant_update_pheromone_DoWork(object sender, DoWorkEventArgs e)
        {
            Object[] obj = (Object[])e.Argument;
            bool firsttime = (bool)obj[0];
            ACO curr = (ACO)obj[1];
            foreach (var item in curr.Area)
            {
                if ((int)speed_val != 0)
                {
                    Thread.Sleep(3000 / (int)speed_val);
                }
                else
                {
                    Thread.Sleep(3000 / 1);
                }
                Object[] obj1=new Object[3];
                obj1[0]=item;
                obj1[1]=firsttime;
                obj1[2]=curr;
                if(curr.Area[curr.Area.Count-1]==item)
                {
                    ant_update_pheromone.ReportProgress(1, obj1);
                }
                else
                {
                    ant_update_pheromone.ReportProgress(0, obj1);
                }
            }

        }

        double find_angle(Point p1,Point p2)//p1 should be from
        {
            double angle;
            double hyp = find_distance(p1,p2);
            Point nw = new Point();
            nw.X = p2.X;
            nw.Y = p1.Y;
            double adj = find_distance(p1,nw);
            angle = Math.Acos((adj / hyp));//in radians
            angle = angle * (180 / Math.PI);//in degree
            return angle;
        }

        double find_distance(Point p1,Point p2)
        {
            double num = Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2);
            double distp1p2 = Math.Sqrt(num);
            return distp1p2;
        }

        void ant_walker_DoWork(object sender, DoWorkEventArgs e)
        {
            Object[] obj = (Object[])e.Argument;
            Point[] city = (Point[])obj[0];
            ACO curr = (ACO)obj[1];
            double m = ((city[1].Y - city[0].Y) / (city[1].X - city[0].X));
            double c = city[0].Y - (city[0].X * m);
            if (Math.Abs(city[0].X - city[1].X) > Math.Abs(city[0].Y - city[1].Y))//travel on x
            {

                if (city[0].X < city[1].X)//forward
                {
                    for (double i = city[0].X ; i < city[1].X ; i += 2*speed_val)//offseted
                    {
                        //-100,-200
                        Point p = new Point();
                        p.X = i;
                        p.Y = m * p.X + c;
                        ant_walker.ReportProgress(1, p);
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
                        ant_walker.ReportProgress(1, p);
                        Thread.Sleep(20);
                    }
                }
             
            }
            else//travel on y
            {
                if(city[0].Y<city[1].Y)//forward
                {
                    for (double i = city[0].Y; i < city[1].Y; i += 2 * speed_val)
                    {
                        
                        Point p = new Point();
                        p.Y = i;
                        p.X = (p.Y - c) / m;
                        ant_walker.ReportProgress(1, p);
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
                        ant_walker.ReportProgress(1, p);
                        Thread.Sleep(20);
                    }
                }
                
            }
            curr.UI_lock = false;
        }

        Point centre(Point p,Point q)//find midpoint
        {
            Point Y = new Point();
            Y.X = (p.X + q.X) /2;
            Y.Y = (p.Y + q.Y) /2;
            return Y;
        }
  
        void ant_walker_ProgressChanged(object sender, ProgressChangedEventArgs e)//update ant position
        {
            Point loc =(Point) e.UserState;
            Canvas.SetLeft(ant_img, loc.X+16);
            Canvas.SetTop(ant_img, loc.Y+12);
        }

        void ant_walker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
        }

       private void create_cityUI_ant(string nm, double X, double Y)
        {
            X = X * Xcityoffset;
            Y = Y * Ycityoffset;
            Shape Rendershape = null;
            Rendershape = new Ellipse() { Height = 40, Width = 40 };
            RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#153FC4"));
            b.GradientOrigin = new Point(0.3, 0.3);
            b.RadiusX = b.RadiusX * 1.35;
            b.RadiusY = b.RadiusY * 1.35;
            TextBlock t = new TextBlock();
            Rendershape.Name = nm + "_shape_ant";
            t.Text = nm;
            t.Name = nm + "_txt_ant";
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

        private void draw_connections_ant(string nm)
        {
            City_data currcity = Cities.get_city(nm);
            City_data tocity;
            Line l = new Line();
            Label info;
            ObservableCollection<Cost_Time_Dependency_Data> data = Cities.getmetriclist_asobservable(nm);
            //remove all connection
            remove_connections_ant(nm);
            List<Line> links = new List<Line>();
            List<Point> points = new List<Point>();
            List<Label> infos = new List<Label>();
            foreach (var item in data)
            {
                if ((item.CityCost > 0) || (item.CityTime > 0))
                {
                    l = new Line();
                    info = new Label();
                    //info.IsEnabled = false;
                    l.X1 = currcity.X*Xcityoffset;
                    l.Y1 = currcity.Y * Ycityoffset;
                    tocity = Cities.get_city(item.CityName);
                    l.X2 = tocity.X * Xcityoffset;
                    l.Y2 = tocity.Y * Ycityoffset;
                    l.Name = "ant_link_" + nm + item.CityName;
                    l.StrokeThickness = 0.2;
                    l.Stroke = Brushes.Red;
                    //add labels
                    info.UseLayoutRounding = true;
                    points.Add(get_midpoint(l.X1, l.Y1, l.X2, l.Y2));
                    /*Double[] p = new Double[2];
                    p[0] = l.X2 + 10;
                    p[1] = l.Y2+ 10;
                    points.Add(p);*/
                    info.Content = item.CityCost + "," + item.CityTime;
                    info.Foreground = Brushes.Blue;
                    info.Name = "ant_info_" + nm + item.CityName;
                    infos.Add(info);
                    links.Add(l);
                }
            }
            for (int i = 0; i < links.Count; i++)
            {
                Addline_ant(links[i]);
                Addlableinfo_ant(infos[i], points[i]);
            }
        }

       private void remove_connections_ant(string nm)
        {
            List<AI_Algorithm_Simulator.classes.Path> Presentpaths = Cities.getmetriclist_asPath(nm);
            if (Presentpaths != null)
            {
                foreach (var item in Presentpaths)
                {
                    Line l;
                    Label t;
                    if (exec_grid.FindName("ant_link_" + nm + item.city2) as Line != null)
                    {
                        l = exec_grid.FindName("ant_link_" + nm + item.city2) as Line;
                        t = exec_grid.FindName("ant_info_" + nm + item.city2) as Label;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("ant_link_" + nm + item.city2);
                        canvasArea1.Children.Remove(t);
                        canvasArea1.UnregisterName("ant_info_" + nm + item.city2);
                    }
                    else if (exec_grid.FindName("ant_link_" + item.city2 + nm) as Line != null)
                    {
                        l = exec_grid.FindName("ant_link_" + item.city2 + nm) as Line;
                        t = exec_grid.FindName("ant_info_" + item.city2 + nm) as Label;
                        canvasArea1.Children.Remove(l);
                        canvasArea1.UnregisterName("ant_link_" + item.city2 + nm);
                        canvasArea1.Children.Remove(t);
                        canvasArea1.UnregisterName("ant_info_" + item.city2 + nm);
                    }
                }
            }
        }

       private void  Addline_ant(Line newline)
       {
           Canvas.SetZIndex(newline, -1);
           newline.Effect = create_shadow();
           canvasArea1.Children.Add(newline);
           canvasArea1.RegisterName(newline.Name, newline);
       }

       private void Addlableinfo_ant(Label info, Point p)
       {
           Canvas.SetZIndex(info, -1);
           Canvas.SetLeft(info, p.X);
           Canvas.SetTop(info, p.Y);
           canvasArea1.Children.Add(info);
           canvasArea1.RegisterName(info.Name, info);
       }

      
        //-------------------Ui binding 
       public ObservableCollection<secondary_display_data> AntDataCollection
       {
           get { return ant_data; }
       }
       public ObservableCollection<secondary_display_data> PathDataCollection
       {
           get { return path_data; }
       }
        
    }
}
