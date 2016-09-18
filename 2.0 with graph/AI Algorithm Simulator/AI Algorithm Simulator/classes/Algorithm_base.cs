using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Diagnostics;

namespace AI_Algorithm_Simulator.classes
{
    public enum exec_status
    { running, completed }
    public abstract class Algorithm_base
    {
        private string name;
        private Stopwatch watch;
        private static string mode;//cost,time,cost-time
        protected static Dispatcher UIDispatcher;//the only dipatcher
        private int algo_anim_status = 0;//0- no anim,1-primary anim,2-secondary anim
        private List<long> localbest_time;
        private int rank = 0;
        private static int performancerun = 0;
        private exec_status algo_exec_status;

        public exec_status Algo_exec_status
        {
            get { return algo_exec_status; }
            set { algo_exec_status = value; }
        }

        
	         
        public static int Performancerun
        {
            get { return performancerun; }
            set { performancerun = value; }
        }

        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }
            
       //data params
        private static int recommended_cost;
        private static int recommended_time;
        public bool UI_lock;//the control that locks thread for animation
        public static int sleep_value;//sleep value should be static

        public List<long> Localbest_time
        {
            get { return localbest_time; }
            set { localbest_time = value; }
        }

        public int Algo_anim_status
        {
            get { return algo_anim_status; }
            set { algo_anim_status = value; }
        }

        public Stopwatch Watch
        {
            get { return watch; }
            set { watch = value; }
        }
        public Dispatcher Uidispatcher
        {
            get { return UIDispatcher; }
            set{UIDispatcher=value;}
        }
        public enum AlgorithmType
        {
            GeneticAlgorithm, AntColonyOptimization
        }

        protected AlgorithmType type;

        protected static List<City_data> globalbest;//global best soln

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static int Recommended_Cost
        {
            get
            {
                return recommended_cost;
            }
            set
            {
                recommended_cost = value;
            }
        }

        public static int Recommended_Time
        {
            get
            {
                return recommended_time;
            }
            set
            {
                recommended_time = value;
            }
        }

        public static string Mode
        {
            get { return Algorithm_base.mode; }
            set { Algorithm_base.mode = value; }
        }

        public abstract void set_data(City_list input);  //to set working data

        public abstract void execute();// execute algorithm

        public string getalgotype()
        {
            return type.ToString();
        }//return algo type

        public abstract object returnalgoobj();
       

        // Delegate UpdateProgressDelegate; in each algorithm
        //lock for accessing dispatcher
        //func to get global best at every instant
        //fitness function
        
        //timers




       // public abstract object RawValue { get; }
    }
}
