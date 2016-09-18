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
using System.Xml;


namespace AI_Algorithm_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int ApplicationStatus = 1;
        Storyboard datawait, algorithmwait, executionwait, reportwait;
        String[] Alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        City_list Cities;// cities in list
        ObservableCollection<Cost_Time_Dependency_Data> City_Metric_Data = new ObservableCollection<Cost_Time_Dependency_Data>();
        private static Action EmptyDelegate = delegate() { };//refresh element not used as of nw
        string editmode = "edit";//editmode of canvas edit and drag
        Ellipse draggedobject = null;//store object being dragged
        public static RoutedCommand Change_mode = new RoutedCommand();// routed event for key press ctrl+d

        public MainWindow()
        {
            InitializeComponent();
            //initialize waitin animations
            datawait = (Storyboard)TryFindResource("dataset_waiting");
            datawait.RepeatBehavior = RepeatBehavior.Forever;
            algorithmwait = (Storyboard)TryFindResource("algorithm_waiting");
            algorithmwait.RepeatBehavior = RepeatBehavior.Forever;
            executionwait = (Storyboard)TryFindResource("execution_waiting");
            executionwait.RepeatBehavior = RepeatBehavior.Forever;
            reportwait = (Storyboard)TryFindResource("report_waiting");
            reportwait.RepeatBehavior = RepeatBehavior.Forever;
            ChangeAppStatus(1);
            //initialize objects and variables
            Cities = new City_list();
            draggedobject = null;
            //add command bindings
            CommandBinding cb = new CommandBinding(Change_mode, Change_mode_Executed);
            this.CommandBindings.Add(cb);
            Change_mode.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));//ctrl+D
            //ant workers
            ant_walker.DoWork += ant_walker_DoWork;
            ant_walker.WorkerReportsProgress = true;
            ant_walker.ProgressChanged += ant_walker_ProgressChanged;
            ant_walker.RunWorkerCompleted += ant_walker_RunWorkerCompleted;
            ant_update_pheromone.DoWork += ant_update_pheromone_DoWork;
            ant_update_pheromone.WorkerReportsProgress = true;
            ant_update_pheromone.ProgressChanged += ant_update_pheromone_ProgressChanged;
            //ga workers
            mutator.DoWork += mutator_DoWork;
            mutator.WorkerReportsProgress = true;
            mutator.ProgressChanged += mutator_ProgressChanged;
            //report prep
            
            }

        private void ChangeAppStatus(int newstat)//menubar animations
        {
            if (newstat == 1)//dataset
            {
                algorithmwait.Stop();
                executionwait.Stop();
                reportwait.Stop();
                datawait.Begin();
                ApplicationStatus = newstat;
                this.dataset_grid.Visibility = Visibility.Visible;
                this.algorithm_grid.Visibility = Visibility.Collapsed;
                this.exec_grid.Visibility = Visibility.Collapsed;
                this.report_grid.Visibility = Visibility.Collapsed;
            }
            else if (newstat == 2)//algorithm
            {
                //validation code
                try
                {
                    int s = Convert.ToUInt16(this.rec_cost.Text);
                    s = Convert.ToUInt16(this.rec_time.Text);
                    if (Cities.Cities_list.Count <= 2)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception t)
                {
                    MessageBox.Show("Please check your input parameters", "Message");
                    return;
                }
                ///end validation
                datawait.Stop();
                executionwait.Stop();
                reportwait.Stop();
                algorithmwait.Begin();
                this.menurect1.Fill = Brushes.GreenYellow;
                ApplicationStatus = newstat;
                this.dataset_grid.Visibility = Visibility.Collapsed;
                this.algorithm_grid.Visibility = Visibility.Visible;
                this.exec_grid.Visibility = Visibility.Collapsed;
                this.report_grid.Visibility = Visibility.Collapsed;
                this.nextbtn.IsEnabled = true;
            }
            else if (newstat == 3)//execution
            {
                //create_algorithm();// delete old ones and create all algorithms
                if ((ApplicationStatus == 4))
                {
                    this.progress_circle.Visibility = Visibility.Hidden;
                    this.loading.Visibility = Visibility.Hidden;
                    this.COMPLETED.Visibility = Visibility.Hidden;
                    ChangeAppStatus(2);
                    this.algo_list.Items.Clear();
                    this.algorithm_list.Clear(); 
                    ignoreswap = true;
                    this.algo_list_1.Items.Clear();
                    ignoreswap = true;
                    this.algo_list_2.Items.Clear();
                    ignoreswap = false;
                    algo_threads = null;
                    this.algo_selector_combo.SelectedIndex = 0;
                    return;
                }
                MessageBoxResult j= MessageBox.Show("Are you sure you want to start the execution ?", "Message",MessageBoxButton.YesNo);//ask cost,time
                if(j==MessageBoxResult.No)
                {
                    return;
                }
                datawait.Stop();
                reportwait.Stop();
                algorithmwait.Stop();
                executionwait.Begin();
                this.menurect1.Fill = Brushes.GreenYellow;
                this.menurect2.Fill = Brushes.GreenYellow;
                ApplicationStatus = newstat;
                pre_exec_prep();//start preparations
                this.exec_grid.Visibility = Visibility.Visible;
                this.dataset_grid.Visibility = Visibility.Collapsed;
                this.algorithm_grid.Visibility = Visibility.Collapsed;
                this.report_grid.Visibility = Visibility.Collapsed;
                start_execution();//exec start
            }
            else//report
            {
                create_report();
                datawait.Stop();
                algorithmwait.Stop();
                executionwait.Stop();
                reportwait.Begin();
                this.menurect1.Fill = Brushes.GreenYellow;
                this.menurect2.Fill = Brushes.GreenYellow;
                this.menurect3.Fill = Brushes.GreenYellow;
                ApplicationStatus = newstat;
                this.nextbtn.IsEnabled = false;
                this.dataset_grid.Visibility = Visibility.Collapsed;
                this.exec_grid.Visibility = Visibility.Collapsed;
                this.algorithm_grid.Visibility = Visibility.Collapsed;
                this.report_grid.Visibility = Visibility.Visible;
                this.backbtn.IsEnabled = true;

            }
        }

        private void closebtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }//close 

        private void canvasArea_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)//draw new city
        {
            //check if clicked on a city
            Point pt = e.GetPosition((Canvas)sender);
            HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);
            if (editmode == "edit")
            {
                if ((result.VisualHit as TextBlock == null) && (result.VisualHit as Shape == null))
                {
                    Shape Rendershape = null;
                    Rendershape = new Ellipse() { Height = 40, Width = 40 };
                    RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#153FC4"));
                    b.GradientOrigin = new Point(0.3, 0.3);
                    b.RadiusX = b.RadiusX * 1.35;
                    b.RadiusY = b.RadiusY * 1.35;
                    TextBlock t = new TextBlock();
                    for (int i = 0; i < 25; i++)
                    {
                        if (!Cities.used_alphas.Contains(Alphabets[i]))
                        {
                            Rendershape.Name = Alphabets[i] + "_shape";
                            t.Text = Alphabets[i];
                            t.Name = Alphabets[i] + "_txt";
                            break;
                        }
                    }
                    //goto properties and collect metircs data
                    City_Metric_Data.Clear();
                    Cities.Add(t.Text, e.GetPosition(canvasArea).X + 16, e.GetPosition(canvasArea).Y + 12);
                    //put empty list in properties
                    foreach (var item in Cities.getmetriclist_asobservable(t.Text))
                    {
                        City_Metric_Data.Add(item);
                    }
                    this.city_name_UI.Text = t.Text;
                    if (Cities.used_alphas.Count != 0)
                    {
                        Storyboard show = (Storyboard)TryFindResource("show_cityprop");
                        show.Begin();
                    }
                    Rendershape.Fill = b;
                    Rendershape.Effect = create_shadow();
                    Canvas.SetLeft(Rendershape, e.GetPosition(canvasArea).X);
                    Canvas.SetTop(Rendershape, e.GetPosition(canvasArea).Y);
                    t.IsEnabled = false;
                    Canvas.SetLeft(t, e.GetPosition(canvasArea).X + 16);
                    Canvas.SetTop(t, e.GetPosition(canvasArea).Y + 12);
                    canvasArea.Children.Add(Rendershape);
                    canvasArea.RegisterName(Rendershape.Name, Rendershape);
                    canvasArea.Children.Add(t);
                    canvasArea.RegisterName(t.Name, t);
                }
                else//edit city
                {
                    TextBlock t = result.VisualHit as TextBlock;
                    if (t != null)
                    {
                        if (Cities.used_alphas.Count > 0)
                        {
                            City_Metric_Data.Clear();
                            this.city_name_UI.Text = t.Text;
                            foreach (var item in Cities.getmetriclist_asobservable(t.Text))
                            {
                                City_Metric_Data.Add(item);
                            }
                            Storyboard show = (Storyboard)TryFindResource("show_cityprop");//open city prop for that city
                            show.Begin();
                        }
                    }
                    else
                    {
                        Shape s = result.VisualHit as Shape;
                        if (Cities.used_alphas.Count > 0)
                        {
                            City_Metric_Data.Clear();
                            this.city_name_UI.Text = s.Name.Replace("_shape", "");
                            foreach (var item in Cities.getmetriclist_asobservable(s.Name.Replace("_shape", "")))
                            {
                                City_Metric_Data.Add(item);
                            }
                            Storyboard show = (Storyboard)TryFindResource("show_cityprop");//open city prop for that city
                            show.Begin();
                        }
                    }
                }
            }
            else if (editmode == "drag")
            {
                if (result.VisualHit as TextBlock != null)
                {
                    TextBlock t = result.VisualHit as TextBlock;
                    Ellipse circle = (Ellipse)dataset_grid.FindName(t.Name.Replace("_txt", "_shape"));
                    draggedobject = circle;
                    remove_connections(circle.Name.Replace("_shape", ""));
                }
                else if (result.VisualHit as Ellipse != null)
                {
                    Ellipse circle = result.VisualHit as Ellipse;
                    draggedobject = circle;
                    remove_connections(circle.Name.Replace("_shape", ""));
                }
            }
        }

        private void createCityUI(string nm, double X, double Y)
        {
            Shape Rendershape = null;
            Rendershape = new Ellipse() { Height = 40, Width = 40 };
            RadialGradientBrush b = new RadialGradientBrush(Colors.White, (Color)ColorConverter.ConvertFromString("#153FC4"));
            b.GradientOrigin = new Point(0.3, 0.3);
            b.RadiusX = b.RadiusX * 1.35;
            b.RadiusY = b.RadiusY * 1.35;
            TextBlock t = new TextBlock();
            Rendershape.Name = nm + "_shape";
            t.Text = nm;
            t.Name = nm + "_txt";
            Rendershape.Fill = b;
            Rendershape.Effect = create_shadow();
            Canvas.SetLeft(Rendershape, X);
            Canvas.SetTop(Rendershape, Y);
            t.IsEnabled = false;
            Canvas.SetLeft(t, X + 16);
            Canvas.SetTop(t, Y + 12);
            canvasArea.Children.Add(Rendershape);
            canvasArea.RegisterName(Rendershape.Name, Rendershape);
            canvasArea.Children.Add(t);
            canvasArea.RegisterName(t.Name, t);
        }

        private void canvasArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((Canvas)sender);
            HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);
            Ellipse circle = result.VisualHit as Ellipse;
            if (checkIfPointin(new Point(e.GetPosition(canvasArea).X, e.GetPosition(canvasArea).Y)))
            {
                if (circle != null)
                {
                    MouseMoveObject(sender, e, circle);
                }
                draggedobject = null;
            }
        }

        private void canvasArea_MouseMove(object sender, MouseEventArgs e)
        {
            if ((editmode == "drag") && (draggedobject != null) && (checkIfPointin(new Point(e.GetPosition(canvasArea).X, e.GetPosition(canvasArea).Y))))
            {
                MouseMoveObject(sender, e, draggedobject);
                draw_connections(draggedobject.Name.Replace("_shape", ""));
            }
        }

        private void MouseMoveObject(object sender, MouseEventArgs e, Ellipse element)
        {
            remove_connections(element.Name.Replace("_shape", ""));
            Point p = e.GetPosition(canvasArea);
            Cities.move_city(element.Name.Replace("_shape", ""), p.X, p.Y);
            TextBlock t = (TextBlock)dataset_grid.FindName(element.Name.Replace("_shape", "_txt"));
            Canvas.SetLeft(t, p.X + 6);
            Canvas.SetTop(t, p.Y + 3);
            Canvas.SetLeft(element, p.X - 10);
            Canvas.SetTop(element, p.Y - 10);
            draw_connections(element.Name.Replace("_shape", ""));

        }

        private void canvasArea_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)//delete city
        {
            Point pt = e.GetPosition((Canvas)sender);
            HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);
            if (result != null)
            {
                TextBlock t = result.VisualHit as TextBlock;
                String nm;
                if (t != null)
                {
                    nm = t.Text;
                }
                else
                {
                    Shape s = result.VisualHit as Shape;
                    nm = s.Name.Replace("_shape", "");
                }
                canvasArea.Children.Remove(dataset_grid.FindName(nm + "_shape") as Shape);
                canvasArea.UnregisterName(nm + "_shape");
                canvasArea.Children.Remove(dataset_grid.FindName(nm + "_txt") as TextBlock);
                canvasArea.UnregisterName(nm + "_txt");
                // Remove connection
                remove_connections(nm);
                Cities.Remove(nm);
                //canvasArea.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);//refresh canvas
            }
        }

        private void draw_connections(string nm) //redraws all the lines in the UI for the specific city
        {
            City_data currcity = Cities.get_city(nm);
            City_data tocity;
            Line l = new Line();
            Label info;
            ObservableCollection<Cost_Time_Dependency_Data> data = Cities.getmetriclist_asobservable(nm);
            //remove all connection
            remove_connections(nm);
            List<Line> links = new List<Line>();
            List<Point> points = new List<Point>();
            List<Label> infos = new List<Label>();
            foreach (var item in data)
            {
                if ((item.CityCost > 0) || (item.CityTime > 0))
                {
                    l = new Line();
                    info = new Label();
                    info.IsEnabled = false;
                    l.X1 = currcity.X;
                    l.Y1 = currcity.Y;
                    tocity = Cities.get_city(item.CityName);
                    l.X2 = tocity.X;
                    l.Y2 = tocity.Y;
                    l.Name = "link_" + nm + item.CityName;
                    l.StrokeThickness = 0.5;
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
                    info.Name = "info_" + nm + item.CityName;
                    infos.Add(info);
                    links.Add(l);
                }
            }
            for (int i = 0; i < links.Count; i++)
            {
                Addline(links[i]);
                Addlableinfo(infos[i], points[i]);
            }
        }

        private Point get_midpoint(double x1, double y1, double x2, double y2)
        {
            Point mid = new Point();
            mid.X = ((x1 + x2) / 2) + 2;
            mid.Y = ((y1 + y2) / 2) + 2;
            return mid;
        }

        private void remove_connections(string nm)// remove connectiona and label
        {
            List<AI_Algorithm_Simulator.classes.Path> Presentpaths = Cities.getmetriclist_asPath(nm);
            if (Presentpaths != null)
            {
                foreach (var item in Presentpaths)
                {
                    Line l;
                    Label t;
                    if (dataset_grid.FindName("link_" + nm + item.city2) as Line != null)
                    {
                        l = dataset_grid.FindName("link_" + nm + item.city2) as Line;
                        t = dataset_grid.FindName("info_" + nm + item.city2) as Label;
                        canvasArea.Children.Remove(l);
                        canvasArea.UnregisterName("link_" + nm + item.city2);
                        canvasArea.Children.Remove(t);
                        canvasArea.UnregisterName("info_" + nm + item.city2);
                    }
                    else if (dataset_grid.FindName("link_" + item.city2 + nm) as Line != null)
                    {
                        l = dataset_grid.FindName("link_" + item.city2 + nm) as Line;
                        t = dataset_grid.FindName("info_" + item.city2 + nm) as Label;
                        canvasArea.Children.Remove(l);
                        canvasArea.UnregisterName("link_" + item.city2 + nm);
                        canvasArea.Children.Remove(t);
                        canvasArea.UnregisterName("info_" + item.city2 + nm);
                    }
                }
            }
            // canvasArea.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        private void Addline(Line newline)//Draw connecting lines
        {
            Canvas.SetZIndex(newline, -1);
            newline.Effect = create_shadow();
            canvasArea.Children.Add(newline);
            canvasArea.RegisterName(newline.Name, newline);
        }

        private void Addlableinfo(Label info, Point p)//Draw  label info
        {
            Canvas.SetZIndex(info, -1);
            Canvas.SetLeft(info, p.X);
            Canvas.SetTop(info, p.Y);
            canvasArea.Children.Add(info);
            canvasArea.RegisterName(info.Name, info);
        }

        public ObservableCollection<Cost_Time_Dependency_Data> City_Metric_Collection
        { get { return City_Metric_Data; } }

        private void set_city_prop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Cities.Editmetrics(this.city_name_UI.Text, City_Metric_Data.ToList<Cost_Time_Dependency_Data>());
            Storyboard k = (Storyboard)TryFindResource("hide_cityprop");
            k.Begin();
            draw_connections(this.city_name_UI.Text);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //check if in grid
            Point pt = e.GetPosition((MainWindow)sender);
            HitTestResult result = VisualTreeHelper.HitTest(root, pt);
            if (result != null)
            {
                TextBlock t = result.VisualHit as TextBlock;
                Rectangle g = result.VisualHit as Rectangle;
                if ((t != null) && (t.Name == "title_name"))
                {
                    this.DragMove();
                    e.Handled = true;
                }
                else if ((g != null) && (g.Name == "title_back"))
                {
                    this.DragMove();
                    e.Handled = true;
                }
            }


        }

        private DropShadowEffect create_shadow()
        {
            DropShadowEffect shadow = new DropShadowEffect();
            Color myShadowColor = new Color();
            myShadowColor.ScA = 1;
            myShadowColor.ScB = 0;
            myShadowColor.ScG = 0;
            myShadowColor.ScR = 0;
            shadow.Color = myShadowColor;
            shadow.Direction = 320;
            // Set the depth of the shadow being cast.
            shadow.ShadowDepth = 6;
            // Set the shadow opacity to half opaque or in other words - half transparent.The range is 0-1.
            shadow.Opacity = 0.5;
            return shadow;
        }

        private BlurEffect create_blur()
        {
            BlurEffect blur = new BlurEffect();
            blur.Radius = 0;
            blur.KernelType = KernelType.Gaussian;
            return blur;

        }
        private void nextbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//next page and validations
        {
            if(ApplicationStatus==3)
            {
                foreach (var item in algo_threads)
                {
                    if(item.IsBusy)
                    {
                        MessageBox.Show("Please wait till all the algortihms are simulated completely","Alert",MessageBoxButton.OK);
                        return;
                    }
                }
            }
            ChangeAppStatus(ApplicationStatus + 1);
        }

        private void backbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeAppStatus(ApplicationStatus - 1);
        }

        private void dragbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editmode = "drag";
            this.canvasArea.Cursor = Cursors.Hand;
        }

        private void editbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editmode = "edit";
            this.canvasArea.Cursor = Cursors.Cross;

        }

        private bool checkIfPointin(Point p)
        {
            if ((p.X > 6.646) && (p.Y > 8.823) && (p.X < 1100.646) && (p.Y < 340.823))
            {
                return true;
            }
            return false;
        }

        private void Change_mode_Executed(object sender, ExecutedRoutedEventArgs e)//ctrl+D event Handler
        {
            if (editmode == "edit")
            {
                editmode = "drag";
                this.canvasArea.Cursor = Cursors.Hand;
            }
            else
            {
                editmode = "edit";
                this.canvasArea.Cursor = Cursors.Cross;
            }
        }

        private void openbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "XML files (.xml)|*.xml";
            System.Windows.Forms.DialogResult dr = open.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                openFile(open.FileName);
                for (int i = 0; i < Cities.Cities_list.Count; i++)
                {
                    createCityUI(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12);
                }
                for (int i = 0; i < Cities.used_alphas.Count; i++)
                {
                    draw_connections(Cities.used_alphas[i]);//do for all used alphas
                }
            }
        }

        private void openFile(string loc)
        {
            clearAllcities();
            XmlDocument doc = new XmlDocument();
            doc.Load(loc);
            XmlNodeList city = doc.SelectNodes("Algorithm_input/Cities/City");
            for (int i = 0; i < city.Count; i++)
            {
                XmlNode name = city[i].SelectSingleNode("name");
                XmlNode X = city[i].SelectSingleNode("X");
                XmlNode Y = city[i].SelectSingleNode("Y");
                Cities.Add(name.InnerText, Convert.ToDouble(X.InnerText), Convert.ToDouble(Y.InnerText));
            }
            XmlNodeList paths = doc.SelectNodes("Algorithm_input/metrics_list/Path");
            for (int i = 0; i < paths.Count; i++)
            {
                XmlNode pathname = paths[i].SelectSingleNode("path_name");
                XmlNode cost = paths[i].SelectSingleNode("cost");
                XmlNode time = paths[i].SelectSingleNode("time");
                List<Cost_Time_Dependency_Data> f = new List<Cost_Time_Dependency_Data>();
                f.Add(new Cost_Time_Dependency_Data { CityCost = Convert.ToInt32(cost.InnerText), CityName = pathname.InnerText[1].ToString(), CityTime = Convert.ToInt32(time.InnerText) });
                Cities.Editmetrics(pathname.InnerText[0].ToString(), f);
                f.Clear();
            }
            XmlNode reccost = doc.SelectSingleNode("Algorithm_input/Recommended_Cost");
            XmlNode rectime = doc.SelectSingleNode("Algorithm_input/Recommended_Time");
            this.rec_cost.Text = reccost.InnerText;
            this.rec_time.Text = rectime.InnerText;

        }

        private void createAndSaveFile()
        {
            System.Windows.Forms.SaveFileDialog filelocdialogue = new System.Windows.Forms.SaveFileDialog();
            filelocdialogue.OverwritePrompt = true;
            filelocdialogue.Filter = "XML files (.xml)|*.xml";
            System.Windows.Forms.DialogResult dr = filelocdialogue.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string loc = filelocdialogue.FileName;
                XmlTextWriter writer = new XmlTextWriter(loc, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Algorithm_input");
                writer.WriteStartElement("Cities");
                for (int i = 0; i < Cities.Cities_list.Count; i++)
                {
                    writer.WriteStartElement("City");
                    writer.WriteStartElement("name");
                    writer.WriteValue(Cities.Cities_list[i].Name);
                    writer.WriteEndElement();
                    writer.WriteStartElement("X");
                    writer.WriteValue(Cities.Cities_list[i].X);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Y");
                    writer.WriteValue(Cities.Cities_list[i].Y);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("metrics_list");
                List<AI_Algorithm_Simulator.classes.Path> pathlist = Cities.Metric_list;
                for (int i = 0; i < pathlist.Count; i++)
                {
                    writer.WriteStartElement("Path");
                    writer.WriteStartElement("path_name");
                    writer.WriteValue(pathlist[i].city1 + pathlist[i].city2);
                    writer.WriteEndElement();
                    writer.WriteStartElement("cost");
                    writer.WriteValue(pathlist[i].cost);
                    writer.WriteEndElement();
                    writer.WriteStartElement("time");
                    writer.WriteValue(pathlist[i].time);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Recommended_Cost");
                writer.WriteValue(this.rec_cost.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Recommended_Time");
                writer.WriteValue(this.rec_time.Text);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }



        }

        private void savebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Cities.Cities_list.Count > 2)
            {
                createAndSaveFile();
            }
            else
            {
                MessageBox.Show("Please create a proper input set to save", "Message");
            }
        }

        private void clearAllcities()
        {
            int count = Cities.used_alphas.Count;
            for (int i = 0; i < count; i++)
            {
                string nm = Cities.used_alphas[0];
                canvasArea.Children.Remove(dataset_grid.FindName(nm + "_shape") as Shape);
                canvasArea.UnregisterName(nm + "_shape");
                canvasArea.Children.Remove(dataset_grid.FindName(nm + "_txt") as TextBlock);
                canvasArea.UnregisterName(nm + "_txt");
                // Remove connection
                remove_connections(nm);
                Cities.Remove(nm);
            }
        }

        private void wizard_button_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Storyboard d = (Storyboard)TryFindResource("wizard_moving");
            if (wizard_info.Visibility == Visibility.Visible)
            {
                wizard_info.Visibility = Visibility.Hidden;
            }
            else
            {
                wizard_info.Visibility = Visibility.Visible;
                d.Begin();
            }
        }

       
    }   
}
