using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;

namespace AI_Algorithm_Simulator
{
	/// <summary>
	/// Interaction logic for WLoading.xaml
	/// </summary>
	public partial class WLoading : Window
	{
		public WLoading()
		{
			this.InitializeComponent();
	     	// Insert code required on object creation below this point.
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard evol = (Storyboard)TryFindResource("evolution_storyboard");
            evol.Completed += evol_Completed;
            //evol.Begin();
        }

        void evol_Completed(object sender, EventArgs e)
        {
            Storyboard bruteforce = (Storyboard)TryFindResource("bruteforce_storyboard");
            bruteforce.Completed += bruteforce_Completed;
            bruteforce.Begin();
        }

        void bruteforce_Completed(object sender, EventArgs e)
        {
            Storyboard bfs = (Storyboard)TryFindResource("search_storyboard");
            bfs.Completed += bfs_Completed;
            bfs.Begin();
        }

        void bfs_Completed(object sender, EventArgs e)
        {
            Storyboard GA = (Storyboard)TryFindResource("ga_storyboard");
            GA.Completed += GA_Completed;
            GA.Begin();
        }

        void GA_Completed(object sender, EventArgs e)
        {
            Storyboard ant = (Storyboard)TryFindResource("ant_storyboard");
            ant.Completed += ant_Completed;
            ant.Begin();
        }

        void ant_Completed(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }
	}
}