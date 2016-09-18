using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace AI_Algorithm_Simulator.classes
{
    public class Cost_Time_Dependency_Data:DependencyObject
    {
        public string CityName{get;set;}
       
        public static readonly DependencyProperty CityCostProperty =DependencyProperty.Register("CityCost", typeof(int),typeof(Cost_Time_Dependency_Data), new UIPropertyMetadata(null));

        public int CityCost
        {
            get { return (int)GetValue(CityCostProperty); }
            set { SetValue(CityCostProperty, value); }
        }

        public static readonly DependencyProperty CityTimeProperty = DependencyProperty.Register("CityTime", typeof(int), typeof(Cost_Time_Dependency_Data), new UIPropertyMetadata(null));

        public int CityTime
        {
            get { return (int)GetValue(CityTimeProperty); }
            set { SetValue(CityTimeProperty, value); }
        }



    }
}
