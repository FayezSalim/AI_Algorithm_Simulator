using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AI_Algorithm_Simulator.classes;
using System.Collections.Generic;
using System.Windows;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        City_list f = new City_list();
        List<Cost_Time_Dependency_Data> City_Metric_Data = new List<Cost_Time_Dependency_Data>();

        [TestMethod]
        public void TestMethod1()
        {
           /* f.Add("a", 1, 1, City_Metric_Data);
            City_Metric_Data.Add(new Cost_Time_Dependency_Data { CityName = "a", CityCost = 12, CityTime = 31 });
            f.Add("b", 1, 1, City_Metric_Data);
            City_Metric_Data.Clear();
            City_Metric_Data.Add(new Cost_Time_Dependency_Data { CityName = "a", CityCost = 4, CityTime = 3 });
            f.Edit("b",City_Metric_Data);
            //City_Metric_Data.Add(new Cost_Time_Dependency_Data { CityName = "b", CityCost = 2, CityTime = 3 });*/
        }
    }
}
