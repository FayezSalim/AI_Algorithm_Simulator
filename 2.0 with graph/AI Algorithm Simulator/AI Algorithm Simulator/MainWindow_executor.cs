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
    partial class MainWindow:Window
    {
        Storyboard progress;
        private int curr_exec_no;
        private void start_execution() //main exec start function
        {
            algo_threads = new BackgroundWorker[this.algorithm_list.Count];
            this.backbtn.IsEnabled = false;
            for (int i = 0; i < this.algorithm_list.Count; i++)
            {
                algo_threads[i] = new BackgroundWorker();
                algo_threads[i].DoWork += executor_DoWork;
                algo_threads[i].WorkerReportsProgress = true;
                algo_threads[i].ProgressChanged += MainWindow_ProgressChanged;
                algorithm_list[i].Uidispatcher = this.Dispatcher;
                algo_threads[i].RunWorkerAsync(new object[]{algorithm_list[i],algo_threads[i]});
            }
            curr_exec_no = this.algorithm_list.Count;
        }

        void MainWindow_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            curr_exec_no -= 1;
            if ((curr_exec_no == 0)&&(this.loading.Visibility==Visibility.Visible))
            {
                //this.progress_circle.Visibility = Visibility.Hidden;
                this.loading.Visibility = Visibility.Hidden;
                this.COMPLETED.Visibility = Visibility.Visible;
                progress.Stop();
            }
            else if((curr_exec_no == 0)&&(this.progress_circle.Visibility!=Visibility.Visible))
            {
                MessageBox.Show("Simulation of all algorithms completed", "Message", MessageBoxButton.OK);
            }
        }

        void executor_DoWork(object sender, DoWorkEventArgs e)
        {
            Algorithm_base b = (Algorithm_base)((Object[])e.Argument)[0];
            BackgroundWorker me = (BackgroundWorker)((Object[])e.Argument)[1];
            b.execute();
            //MessageBox.Show(b.Name + " over " +" time - "+get_time( b.Watch.ElapsedTicks));
            me.ReportProgress(1);
        }

        private void swap_algo_interface()//swap the 2 specific algorithms
        {
            ignoreswap = true;
            foreach (var item in algorithm_list)
            {
                item.Algo_anim_status = 0;
            }
            Object one = this.algo_list_1.SelectedItem;
            Object two = this.algo_list_2.SelectedItem;
            this.algo_list_1.Items.Add(two);
            this.algo_list_1.SelectedItem = two;
            this.algo_list_2.Items.Add(one);
            this.algo_list_2.SelectedItem = one;
            this.algo_list_1.Items.Remove(one);
            this.algo_list_2.Items.Remove(two);
            Algorithm_base firstone = algorithm_list.Find((o) => o.Name == this.algo_list_1.SelectedItem.ToString());
            Algorithm_base secondone = algorithm_list.Find((o) => o.Name == this.algo_list_2.SelectedItem.ToString());

            set_primary_animation(firstone);
            set_secondary_animation(secondone);
            ignoreswap = false;
            //stop  all exections of animations and algorithms
            //lock all ui locks and unlock at the end
            //set all other algo as 0 primary-1 sec-2
        }

        private void pre_exec_prep()//set up for execution
        {
            ComboBoxItem itm=(ComboBoxItem)this.algo_mode_combo.SelectedItem;
            Algorithm_base.Mode = itm.Content.ToString();
            if (Convert.ToInt32(this.initial_speed_txt.Text)>10)
            {
                this.speed_slider.Value = 10;
            }
            else if (Convert.ToInt32(this.initial_speed_txt.Text) <0)
            {
                 this.speed_slider.Value = 0;
            }
            else
            {
            this.speed_slider.Value = Convert.ToInt32(this.initial_speed_txt.Text);
            }
            if (!(bool)this.perf_run.IsChecked)
            {
                Algorithm_base.Performancerun = 0;
                this.progress_circle.Visibility = Visibility.Hidden;
                this.loading.Visibility = Visibility.Hidden;
                this.COMPLETED.Visibility = Visibility.Hidden;
                initial_populate_algo_list();
                //check and set first algo
                Algorithm_base firstone = algorithm_list.Find((o) => o.Name == this.algo_list_1.SelectedItem.ToString());
                Algorithm_base secondone = algorithm_list.Find((o) => o.Name == this.algo_list_2.SelectedItem.ToString());
                foreach (var item in algorithm_list)
                {
                    item.Algo_anim_status = 0;
                }
                //color animation of sphere

                //-------------------------------------------------------------------------------------
                set_primary_animation(firstone);
                set_secondary_animation(secondone);

                //set all other algo as 0
                //ant_walker.RunWorkerAsync();
            }
            else
            {
                Algorithm_base.Performancerun = 1;
                this.progress_circle.Visibility = Visibility.Visible;
                this.loading.Visibility = Visibility.Visible;
                 Algorithm_base.Performancerun = 1;
                foreach (var item in algorithm_list)
                {
                    item.Algo_anim_status = 0;
                }
                //run animation
                Storyboard bac = (Storyboard)TryFindResource("algo_running");
                bac.Begin();
                progress = (Storyboard)TryFindResource("rotate");
                progress.RepeatBehavior = RepeatBehavior.Forever;
                progress.Begin();
            }
        }

        private void initial_populate_algo_list()
        {
            ignoreswap = true;
            foreach (var item in algorithm_list)
            {
                this.algo_list_1.Items.Add(item.Name);
            }
            this.algo_list_1.SelectedIndex = 0;
            foreach (var item in algorithm_list)
            {
                if (this.algo_list_1.SelectedItem.ToString() != item.Name)
                {
                    this.algo_list_2.Items.Add(item.Name);
                }
            }
            this.algo_list_2.SelectedIndex = 0;
            this.algo_list_1.Items.Remove(this.algo_list_2.SelectedItem);
            ignoreswap = false;
        }

        private void set_primary_animation(Algorithm_base algo)
        {
            if (algo.getalgotype() == "AntColonyOptimization")//aco
            {
                //show ant grid and hide ga grid
                //this.canvasArea1.Children.Clear();
                //draw cities
                this.ant_exec.Visibility = Visibility.Visible;
                this.ga_exec.Visibility = Visibility.Hidden;
                this.ant_img.Visibility = Visibility.Hidden;
                if (FindName(Cities.Cities_list[0].Name + "_shape_ant") == null)
                {
                    for (int i = 0; i < Cities.Cities_list.Count; i++)
                    {
                        create_cityUI_ant(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12);
                    }
                    for (int i = 0; i < Cities.used_alphas.Count; i++)
                    {
                        draw_connections_ant(Cities.used_alphas[i]);//do for all used alphas
                    }
                }
                Update_pheromone_delegate updater1 = new Update_pheromone_delegate(Update_pheromone);
                Draw_ant_delegate updater = new Draw_ant_delegate(draw_ant);
                ACO curr = (ACO)algo.returnalgoobj();
                curr.Algo_anim_status = 1;
                curr.Ant_updater = updater;
                curr.Pheromone_updater = updater1;
                if(algo.Algo_exec_status==AI_Algorithm_Simulator.classes.exec_status.completed)
                {
                    this.ant_process_text.Text = "Completed";
                }
            }
            else if (algo.getalgotype() == "GeneticAlgorithm")//genetic
            {
                this.ga_exec.Visibility = Visibility.Visible;
                this.ant_exec.Visibility = Visibility.Hidden;
                GeneticAlgorithm curr = (GeneticAlgorithm)algo.returnalgoobj();
                curr.Selection_updater = new selection_updater_delegate(selection_updater);
                curr.Algo_anim_status = 1;
                curr.Crossover_updater = new crossover_updater_delegate(crossover_updater);
                curr.Mutation_updater = new mutation_updater_delegate(mutation_updater);
                if (algo.Algo_exec_status == AI_Algorithm_Simulator.classes.exec_status.completed)
                {
                    this.ga_algo_status_txt.Text = "Completed";
                }
                if (FindName(Cities.Cities_list[0].Name + "_shape_ga" + "p1") == null)
                {
                    for (int i = 0; i < Cities.Cities_list.Count; i++)//create parent 1 city
                    {
                        create_cityUI_ga(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12, ga_canvas_parent1, 0.34, 0.37, "p1", 10);
                    }
                    for (int i = 0; i < Cities.used_alphas.Count; i++)//draw connections parent 1
                    {
                        draw_connections_ga(Cities.used_alphas[i], ga_canvas_parent1, 0.34, 0.37, "p1");//do for all used alphas
                    }
                    for (int i = 0; i < Cities.Cities_list.Count; i++)//create parent 2 city
                    {
                        create_cityUI_ga(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12, ga_canvas_parent2, 0.33, 0.33, "p2", 10);
                    }
                    for (int i = 0; i < Cities.used_alphas.Count; i++)//draw connections parent 2
                    {
                        draw_connections_ga(Cities.used_alphas[i], ga_canvas_parent2, 0.33, 0.33, "p2");//do for all used alphas
                    }
                    for (int i = 0; i < Cities.Cities_list.Count; i++) //create child city
                    {
                        create_cityUI_ga(Cities.Cities_list[i].Name, Cities.Cities_list[i].X - 12, Cities.Cities_list[i].Y - 12, ga_canvas_child, 0.68, 0.55, "c", 20);
                    }
                    for (int i = 0; i < Cities.used_alphas.Count; i++)//draw connections child
                    {
                        draw_connections_ga(Cities.used_alphas[i], ga_canvas_child, 0.68, 0.55, "c");//do for all used alphas
                    }
                }
            }
        }

        private void set_secondary_animation(Algorithm_base algo)
        {
            if (algo.getalgotype() == "AntColonyOptimization")
            {
                ACO curr = (ACO)algo.returnalgoobj();
                this.ga_sec_exec.Visibility = Visibility.Hidden;
                this.ant_sec_exec.Visibility = Visibility.Visible;
                curr.Ant_sec_updater = new Ant_sec_updater(ant_sec_update);
                curr.Algo_anim_status = 2;
                if (algo.Algo_exec_status == AI_Algorithm_Simulator.classes.exec_status.completed)
                {
                    this.ant_sec_gen_txt.Text = "Completed";
                }
            }
            else if (algo.getalgotype() == "GeneticAlgorithm")
            {
                GeneticAlgorithm curr = (GeneticAlgorithm)algo.returnalgoobj();
                this.ga_sec_exec.Visibility = Visibility.Visible;
                this.ant_sec_exec.Visibility = Visibility.Hidden;
                curr.Ga_sec_updater = new Ga_sec_updater(ga_sec_updater);
                curr.Algo_anim_status = 2;
                if (algo.Algo_exec_status == AI_Algorithm_Simulator.classes.exec_status.completed)
                {
                    this.ga_sec_gen_txt.Text = "Completed";
                }
            }
        }

        private void speed_slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Algorithm_base.sleep_value = (int)this.speed_slider.Value * 100;
            speed_val = this.speed_slider.Value;
            if (animate_ant != null)
            {
                MessageBox.Show("skipp");
                animate_ant.SkipToFill();
            }
        }

        private void algo_list_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ignoreswap)
            {
                this.algo_list_2.Items.Remove(e.AddedItems[0]);//remove from algo2
                this.algo_list_2.Items.Add(e.RemovedItems[0]);//add unselected item to algo2
                Algorithm_base firstone = algorithm_list.Find((o) => o.Name == this.algo_list_1.SelectedItem.ToString());
                foreach (var item in algorithm_list)
                {
                    item.Algo_anim_status = 0;
                }
                set_primary_animation(firstone);

            }
        }

        private void algo_list_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ignoreswap)
            {
                this.algo_list_1.Items.Remove(e.AddedItems[0]);//remove from algo1
                this.algo_list_1.Items.Add(e.RemovedItems[0]);//add unselected item to algo1
                Algorithm_base secondone = algorithm_list.Find((o) => o.Name == this.algo_list_2.SelectedItem.ToString());
                foreach (var item in algorithm_list)
                {
                    item.Algo_anim_status = 0;
                }
                set_secondary_animation(secondone);
            }
        }

        private void swap_btn_Click(object sender, RoutedEventArgs e)
        {
            swap_algo_interface();

        }
    }
}
