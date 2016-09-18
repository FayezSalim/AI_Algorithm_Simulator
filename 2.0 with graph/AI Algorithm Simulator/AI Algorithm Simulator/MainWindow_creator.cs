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

namespace AI_Algorithm_Simulator
{
    public partial class MainWindow : Window //creation of algorithms
    {
        List<Algorithm_base> algorithm_list = new List<Algorithm_base>();//algorithm list
        //List<Algorithm<T>> nw = new List<Algorithm<T>>();

        //-----------------------------ask for execution mode
        public enum AlgorithmType
        {
            GeneticAlgorithm,AntColonyOptimization
        }

        private void algo_selector_combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
         try
            {
                ComboBoxItem f = (ComboBoxItem)e.AddedItems[0];
                if (f.Content.ToString() == "Genetic Algorithm")
                {
                    this.GA_grid.Visibility = Visibility.Visible;
                    this.ant_grid.Visibility = Visibility.Collapsed;
                }
                else if (f.Content.ToString() == "Ant Colony Optimization")
                {
                    this.ant_grid.Visibility = Visibility.Visible;
                    this.GA_grid.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception d)
            {
                //to catch first run exception
            }
        }

        private void name_txt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.algorithm_name_lbl.Content = this.name_txt.Text;
        }

        private void addbtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (checkalgoname())
            {
                if (validate_algorithm())
                {
                    create_algorithm();
                    this.algo_list.Items.Add(this.algorithm_name_lbl.Content);//call to nother func
                    this.algo_list.SelectedIndex = this.algo_list.Items.Count - 1;
                }
            }
            else
            {
                MessageBox.Show("Please give a valid name for your algorithm", "Message");
            }
        }

        private bool checkalgoname()//check if already selected name
        {
            if (this.algorithm_name_lbl.Content.ToString() == "")
            {
                return false;
            }
            foreach (var item in this.algo_list.Items)
            {
                if (item == this.algorithm_name_lbl.Content)
                {
                    return false;
                }
            }
            return true;
        }

        private bool validate_algorithm()
        {
            if (this.GA_grid.Visibility == Visibility.Visible)
            {
                try
                {
                    double u = Convert.ToDouble(this.crossover_rate_txt.Text);
                    if ((u < 0) && (u <= 1))
                    {
                        throw new Exception();
                    }
                    u = Convert.ToDouble(this.mutation_rate_txt.Text);
                    if ((u < 0) && (u <= 1))
                    {
                        throw new Exception();
                    }
                    int g = Convert.ToInt32(this.gen_no_txt.Text);
                    if (g < 0)
                    {
                        throw new Exception();
                    }
                    g = Convert.ToInt32(this.pop_size_txt.Text);
                    if (g < 0)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show("Please provide appropriate values for all the the parameters", "Message");
                    return false;
                }
            }
            else if(this.ant_grid.Visibility==Visibility.Visible)
            {
               try
               {
                   if(Convert.ToInt32(this.ip_txt.Text)<1)
                   {
                       MessageBox.Show("Please enter an initial pheomone value greater than 1");
                       return false;
                   }
                   if(Convert.ToInt32(this.alpha_txt.Text)<0)
                   {
                       MessageBox.Show("Please enter an alpha value greater than 0");
                       return false;
                   }
                   if(Convert.ToInt32(this.beta_txt.Text)<1)
                   {
                       MessageBox.Show("Please enter an beta value greater than 1");
                       return false;
                   }
                   if(Convert.ToDouble(this.row_txt.Text)<0&&Convert.ToDouble(this.row_txt.Text)>1)
                   {
                       MessageBox.Show("Please enter a row value between 0 - 1");
                       return false;
                   }

               }
                catch(Exception t)
               {
                   MessageBox.Show("Please provide appropriate values for all the the parameters", "Message");
                   return false;
               }
            }
            return true;

        }

        private void create_algorithm()
        {
            ComboBoxItem algoselector = (ComboBoxItem)this.algo_selector_combo.SelectedItem;
             if (algoselector.Content.ToString()== "Genetic Algorithm")
            {
                GeneticAlgorithm ga = new GeneticAlgorithm(this.Dispatcher);
                //set params
                Algorithm_base.Recommended_Cost = Convert.ToInt32(this.rec_cost.Text);
                Algorithm_base.Recommended_Time = Convert.ToInt32(this.rec_time.Text);
                ga.Name = this.name_txt.Text;
                ga.PopSize = Convert.ToInt32(this.pop_size_txt.Text);
                ga.MaxGen = Convert.ToInt32(this.gen_no_txt.Text);
                ga.CrossoverRate = Convert.ToDouble(this.mutation_rate_txt.Text);
                ga.MutationRate = Convert.ToDouble(this.crossover_rate_txt.Text);
                ga.Etilism = (bool)this.etilism_chk.IsChecked;
                ga.set_data(Cities);//do just before exection remove from here
                //set algorithms
                ComboBoxItem selector=(ComboBoxItem)this.selectionalgo_selector.SelectedItem;
                ComboBoxItem crossoverselected=(ComboBoxItem)this.crossoveralgo_selector.SelectedItem;
                ComboBoxItem mutaionselected=(ComboBoxItem)this.mutationalgo_selector.SelectedItem;
                ga.FitnessAlgorithm = FitnessAlgorithmFactory.CreateCachableISelectionAlgorithm((fitnessalgo_selector.SelectedItem as ComboBoxItem).Content.ToString().Replace(" ", ""));
                ga.SelectionAlgorithm = SelectionAlgorithmFactory.CreateCachableISelectionAlgorithm(selector.Content.ToString().Replace(" ",""));
                ga.CrossoverAlgorithm = CrossoverAlgorithmFactory.CreateCachableISelectionAlgorithm(crossoverselected.Content.ToString().Replace(" ", ""));
                ga.MutationAlgorihtm = MutationAlgorithmFactory.CreateCachableISelectionAlgorithm(mutaionselected.Content.ToString().Replace(" ", ""));
                algorithm_list.Add(ga);
                Algorithm_base.Mode = "cost";//--------------------------------remove from here
                //ga.execute();
            }
             else if (algoselector.Content.ToString() == "Ant Colony Optimization")
            {
                Draw_ant_delegate updater = new Draw_ant_delegate(draw_ant);
                Algorithm_base.Recommended_Cost = Convert.ToInt32(this.rec_cost.Text);
                Algorithm_base.Recommended_Time = Convert.ToInt32(this.rec_time.Text);
                Algorithm_base.Mode = "cost";//--------------------------------remove from here
                ComboBoxItem selected=(ComboBoxItem)this.ant_algo_type_selector.SelectedItem;
                if (selected.Content.ToString() == "Ant System")
                {
                    ACO ac = new ACO(Convert.ToInt16(this.ip_txt.Text), Convert.ToInt16(this.alpha_txt.Text), Convert.ToInt16(this.beta_txt.Text), Convert.ToDouble(this.row_txt.Text), Convert.ToInt16(this.q_txt.Text), Convert.ToInt16(this.noofants_txt.Text), Convert.ToInt16(this.iterations_txt.Text));
                    ac.Name = this.name_txt.Text;
                    ac.set_data(Cities);//do just before execution
                    algorithm_list.Add(ac);
                }
                else if (selected.Content.ToString() == "Best Worst Ant")
                {
                    BestWorstAnt ac = new BestWorstAnt(Convert.ToInt16(this.ip_txt.Text), Convert.ToInt16(this.alpha_txt.Text), Convert.ToInt16(this.beta_txt.Text), Convert.ToDouble(this.row_txt.Text), Convert.ToInt16(this.q_txt.Text), Convert.ToInt16(this.noofants_txt.Text), Convert.ToInt16(this.iterations_txt.Text));
                    ac.Name = this.name_txt.Text;
                    ac.set_data(Cities);//do just before execution
                    algorithm_list.Add(ac);
                }
                else if (selected.Content.ToString() == "Min Max Ant")
                {
                    MinMaxAnt ac = new MinMaxAnt(Convert.ToDouble(this.min_txt.Text), Convert.ToDouble(this.max_txt.Text), Convert.ToInt16(this.ip_txt.Text), Convert.ToInt16(this.alpha_txt.Text), Convert.ToInt16(this.beta_txt.Text), Convert.ToDouble(this.row_txt.Text), Convert.ToInt16(this.q_txt.Text), Convert.ToInt16(this.noofants_txt.Text), Convert.ToInt16(this.iterations_txt.Text));
                    ac.Name = this.name_txt.Text;
                    ac.set_data(Cities);//do just before execution
                    algorithm_list.Add(ac);
                }
                else if (selected.Content.ToString() == "Rank Based Ant")
                {
                    RankBasedAnt ac = new RankBasedAnt(Convert.ToInt32(this.econst_txt.Text), Convert.ToInt16(this.ip_txt.Text), Convert.ToInt16(this.alpha_txt.Text), Convert.ToInt16(this.beta_txt.Text), Convert.ToDouble(this.row_txt.Text), Convert.ToInt16(this.q_txt.Text), Convert.ToInt16(this.noofants_txt.Text), Convert.ToInt16(this.iterations_txt.Text));
                    ac.Name = this.name_txt.Text;
                    ac.set_data(Cities);//do just before execution
                    algorithm_list.Add(ac);
                }
                else if (selected.Content.ToString() == "Etilist Ant")
                {
                    EtilistAnt ac = new EtilistAnt(Convert.ToInt32(this.econst_txt.Text), Convert.ToInt16(this.ip_txt.Text), Convert.ToInt16(this.alpha_txt.Text), Convert.ToInt16(this.beta_txt.Text), Convert.ToDouble(this.row_txt.Text), Convert.ToInt16(this.q_txt.Text), Convert.ToInt16(this.noofants_txt.Text), Convert.ToInt16(this.iterations_txt.Text));
                    ac.Name = this.name_txt.Text;
                    ac.set_data(Cities);//do just before execution
                    algorithm_list.Add(ac);
                }
                //ac.execute();
            }
        }
        
        private void delbtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var item in this.algo_list.Items)
            {
                if (item == this.algorithm_name_lbl.Content)
                {
                    this.algorithm_list.Remove(this.algorithm_list.Find((o) => o.Name == this.algorithm_name_lbl.Content.ToString()));
                    this.algo_list.Items.Remove(item);
                    if(this.algo_list.Items.Count!=0)
                    {
                        this.algo_list.SelectedIndex = 0;
                    }
                    break;
                }
            }
            //delete algo
        }

        private void ant_algo_type_selector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem c = (ComboBoxItem)e.AddedItems[0];
            try
            {
                if (c.Content.ToString() == "Ant System")
                {
                    this.e_grid.Visibility = Visibility.Collapsed;
                    this.min_max_grig.Visibility = Visibility.Collapsed;
                }
                else if (c.Content.ToString() == "Best Worst Ant")
                {
                    this.e_grid.Visibility = Visibility.Collapsed;
                    this.min_max_grig.Visibility = Visibility.Collapsed;
                }
                else if (c.Content.ToString() == "Etilist Ant")
                {
                    this.e_grid.Visibility = Visibility.Visible;
                    this.min_max_grig.Visibility = Visibility.Collapsed;
                }
                else if (c.Content.ToString() == "Min Max Ant")
                {
                    this.e_grid.Visibility = Visibility.Collapsed;
                    this.min_max_grig.Visibility = Visibility.Visible;
                }
                else if (c.Content.ToString() == "Rank Based Ant")
                {
                    this.e_grid.Visibility = Visibility.Visible;
                    this.min_max_grig.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception t)
            {
                //first time run
            }

        }

        private void algo_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           count_algo(); 
           if(this.algo_list.Items.Count==0)
           {
               this.algo_selector_combo.SelectedIndex = -1;
               return;
           }
          Algorithm_base selected = this.algorithm_list.Find((o) => o.Name == this.algo_list.SelectedItem.ToString());
          if (selected != null)
          {
              if (selected.getalgotype() == "GeneticAlgorithm")
              {
                  this.algo_selector_combo.SelectedIndex = 0;
                  GeneticAlgorithm g = (GeneticAlgorithm)selected.returnalgoobj();
                  this.name_txt.Text = g.Name;
                  this.pop_size_txt.Text = g.PopSize.ToString();
                  this.gen_no_txt.Text = g.MaxGen.ToString();
                  this.mutation_rate_txt.Text = g.CrossoverRate.ToString();
                  this.crossover_rate_txt.Text = g.MutationRate.ToString();
                  this.etilism_chk.IsChecked = g.Etilism;
                  //set algorithms
                  pick_selectionalgo(g.SelectionAlgorithm.GetType().ToString().Replace("AI_Algorithm_Simulator.classes.", ""));
                  pick_fitnessalgo(g.FitnessAlgorithm.GetType().ToString().Replace("AI_Algorithm_Simulator.classes.", ""));
                  pick_crossover(g.CrossoverAlgorithm.GetType().ToString().Replace("AI_Algorithm_Simulator.classes.", ""));
                  pick_mutation(g.MutationAlgorihtm.GetType().ToString().Replace("AI_Algorithm_Simulator.classes.", ""));
              }
              else if (selected.getalgotype() == "AntColonyOptimization")
              {
                  this.algo_selector_combo.SelectedIndex = 1;
                  ACO a = (ACO)selected.returnalgoobj();
                  this.noofants_txt.Text = a.Noofants.ToString();
                  this.iterations_txt.Text = a.Iterations.ToString();
                  this.ip_txt.Text = a.Initpheromone.ToString();
                  this.alpha_txt.Text = a.Alpha.ToString();
                  this.beta_txt.Text = a.Beta.ToString();
                  this.row_txt.Text = a.Row.ToString();
                  this.q_txt.Text = a.QConst.ToString();
                  string name = a.GetType().ToString().Replace("AI_Algorithm_Simulator.classes.", "");
                  foreach (var item in this.ant_algo_type_selector.Items)
                  {
                      ComboBoxItem t = (ComboBoxItem)item;
                      if (t.Content.ToString().Replace(" ", "") == name)
                      {
                          this.ant_algo_type_selector.SelectedItem = item;
                          break;
                      }
                  }
                  if ((name == "RankBasedAnt"))
                  {
                      RankBasedAnt r = (RankBasedAnt)selected.returnalgoobj();
                      this.econst_txt.Text = r.Econst.ToString();
                  }
                  else if (name == "MinMaxAnt")
                  {
                      MinMaxAnt m = (MinMaxAnt)selected.returnalgoobj();
                      this.min_txt.Text = m.Min.ToString();
                      this.max_txt.Text = m.Max.ToString();
                  }
                  else if (name == "EtilistAnt")
                  {
                      EtilistAnt ec = (EtilistAnt)selected.returnalgoobj();
                      this.econst_txt.Text = ec.Econst.ToString();
                  }
              }
          }
        }

        private void pick_selectionalgo(string name)
        {
            foreach (var item in this.selectionalgo_selector.Items )
            {
                ComboBoxItem t = (ComboBoxItem)item;
                if(t.Content.ToString().Replace(" ","")==name)
                {
                    this.selectionalgo_selector.SelectedItem = item;
                    break;
                }
            }
        }

        private void pick_fitnessalgo(string name)
        {
            foreach (var item in this.fitnessalgo_selector.Items)
            {
                ComboBoxItem t = (ComboBoxItem)item;
                if (t.Content.ToString().Replace(" ", "") == name)
                {
                    this.fitnessalgo_selector.SelectedItem = item;
                    break;
                }
            }
        }

        private void pick_crossover(string name)
        {
            foreach (var item in this.crossoveralgo_selector.Items)
            {
                ComboBoxItem t = (ComboBoxItem)item;
                if (t.Content.ToString().Replace(" ", "") == name)
                {
                    this.crossoveralgo_selector.SelectedItem = item;
                    break;
                }
            }
        }

        private void pick_mutation(string name)
        {
            foreach (var item in this.mutationalgo_selector.Items)
            {
                ComboBoxItem t = (ComboBoxItem)item;
                if (t.Content.ToString().Replace(" ", "") == name)
                {
                    this.mutationalgo_selector.SelectedItem = item;
                    break;
                }
            }
        }

        private void count_algo()
        {
            int g = 0, ac = 0;
            foreach (var item in this.algo_list.Items)
            {
                Algorithm_base selected = this.algorithm_list.Find((o) => o.Name == item.ToString());
                if(selected.getalgotype()=="GeneticAlgorithm")
                {
                    g += 1;
                }
                else if (selected.getalgotype() == "AntColonyOptimization")
                {
                    ac += 1;
                }
            }
            this.no_gen_algo.Text = g.ToString();
            this.no_aco_algo.Text = ac.ToString();
        }
    }
}
