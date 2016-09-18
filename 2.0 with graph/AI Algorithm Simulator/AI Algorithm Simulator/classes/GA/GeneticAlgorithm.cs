using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

namespace AI_Algorithm_Simulator.classes
{
    
    public class GeneticAlgorithm: Algorithm<genome>
    {
        //data params
        //inherited from algorithm          
        //algorithm params
        private ISelectionAlgorithm selectionalgorithm;
        private IFitnessAlgorithm fitnessalgorithm;
        private ICrossoverAlgorithm crossoveralgorithm;
        private IMutationAlgorithm mutationalgorithm;
        private bool etilism;
        private int popsize, maxgen;
        private double crossoverrate, mutationrate;
        private Delegate selection_updater, crossover_updater, mutation_updater;//for primary exec
        private Delegate ga_sec_updater;//for secondary exec

        public Delegate Ga_sec_updater
        {
            get { return ga_sec_updater; }
            set { ga_sec_updater = value; }
        }

        public Delegate Mutation_updater
        {
            get { return mutation_updater; }
            set { mutation_updater = value; }
        }

        public Delegate Crossover_updater
        {
            get { return crossover_updater; }
            set { crossover_updater = value; }
        }

        public Delegate Selection_updater
        {
            get { return selection_updater; }
            set { selection_updater = value; }
        }
        
        //properties
        public IFitnessAlgorithm FitnessAlgorithm
        {
            get
            {
                return fitnessalgorithm;
            }
            set
            {
                fitnessalgorithm = value;
            }
        }

        public ICrossoverAlgorithm CrossoverAlgorithm
        {
            get
            {
                return crossoveralgorithm;
            }
            set
            {
                crossoveralgorithm = value;
            }
        }

        public IMutationAlgorithm MutationAlgorihtm
        {
            get
            {
                return mutationalgorithm;
            }
            set
            {
                mutationalgorithm = value;
            }
        }

        public ISelectionAlgorithm SelectionAlgorithm
        {
            get
            {
                return selectionalgorithm;
            }
            set
            {
                selectionalgorithm = value;
            }
        }

        public bool Etilism
        {
            get
            {
                return etilism;
            }
            set
            {
                etilism = value;
            }
        }

        public int PopSize
        {
            get
            {
                return popsize;
            }
            set
            {
                popsize = value;
            }
        }

        public int MaxGen
        {
            get
            {
                return maxgen;
            }
            set
            {
                maxgen = value;
            }
        }
         
        public double CrossoverRate
        {
            get 
            {
                return crossoverrate;
            }
            set
            {
                crossoverrate = value;
            }
        }

        public double MutationRate
        {
            get
            {
                return mutationrate;
            }
            set
            {
                mutationrate = value;
            }
        }
    
        //functions
        public GeneticAlgorithm(Dispatcher c)//constructor
        {
            type = AlgorithmType.GeneticAlgorithm;
            Workingset = new List<genome>();
            UIDispatcher = c;
            Localbest = new List<genome>();
            if(globalbest==null)
            {
                globalbest = new List<City_data>();
            }
            Localbest_time = new List<long>();
        }

        public override void set_data(City_list input)//get input andgenerate random population
        {
            List<genome> nw = new List<genome>();
            Random rfunc = new Random();
            int randomno;
            for (int i=0;i<popsize ;i++ )
            {
                nw.Add(new genome());
            }
            for (int i = 0; i < popsize; i++)
            {
                City_list clone = new City_list(input);
                for (; clone.Cities_list.Count > 0; )
                {
                    randomno = rfunc.Next(clone.Cities_list.Count);
                    nw[i].addcity(clone.Cities_list[randomno]);
                    clone.Cities_list.Remove(clone.Cities_list[randomno]);//remove from input
                }
                if (nw[i].validateGenome())
                {
                    this.Workingset.Add(new genome(nw[i]));
                }
                else
                {
                    i--;
                    nw.RemoveAt(i);
                }
            }

        }

        public override void execute()
        {
            //put in for loop
            this.Algo_exec_status = AI_Algorithm_Simulator.classes.exec_status.running;
            try
            {
                Watch = new System.Diagnostics.Stopwatch();
                this.Watch.Start();
                if (Mode == "cost-time")
                {
                    this.costtimefitness();
                }
                Localbest.Add( new genome(workingset[0]));
                Localbest_time.Add(this.Watch.ElapsedTicks);
                foreach (var item in workingset)
                {
                    find_localbest(item);
                }
                if (Performancerun == 0)
                {
                    for (int i = 0; i < MaxGen; i++)
                    {
                        foreach (var item in Workingset)
                        {
                            item.recalc();//if necessary
                        }
                        if (Mode == "cost-time")
                        {
                            this.costtimefitness();
                        }
                        fitness();
                        Workingset.Sort((fit1, fit2) => fit1.Fitness.CompareTo(fit2.Fitness));//sort genomes according to fitness ascending order least->most
                        // Workingset.Reverse();//reverse it so highest fitness first
                        List<genome> newpop = new List<genome>();
                        for (int k = 0; k < PopSize; k++)
                        {
                            selection();

                            this.Watch.Stop();
                            while (UI_lock)
                            {

                            }
                            if (this.Algo_anim_status == 1)
                            {
                                UI_lock = true;
                                Uidispatcher.BeginInvoke(selection_updater, SelectionAlgorithm.getparents()[0], SelectionAlgorithm.getparents()[1], returnalgoobj(), i);
                            }
                            while (UI_lock)
                            {

                            }
                            this.Watch.Start();

                            //send selected parents to UI
                            genome g = new genome(crossover());
                            this.Watch.Stop();
                            while (UI_lock)
                            {

                            }
                            if (this.Algo_anim_status == 1)
                            {
                                UI_lock = true;
                                Uidispatcher.BeginInvoke(crossover_updater, g, returnalgoobj());
                            }
                            this.Watch.Start();
                            //send child

                            g = new genome(mutation(g));
                            //send mutated child
                            newpop.Add(new genome(g));
                            find_localbest(g);

                            this.Watch.Stop();
                            while (UI_lock)
                            {

                            }
                            if (this.Algo_anim_status == 1)
                            {
                                UI_lock = true;
                                Uidispatcher.BeginInvoke(mutation_updater, g, returnalgoobj());
                            }
                            else if (this.Algo_anim_status == 2)
                            {
                                UI_lock = true;
                                Uidispatcher.BeginInvoke(ga_sec_updater, newpop, returnalgoobj(),i);// secondary animation only done fr each child
                                while (sleep_value == 0)
                                { }
                                Thread.Sleep(1 / sleep_value);
                            }
                            else if (Algo_anim_status == 0)
                            {
                                while (sleep_value == 0)
                                { }
                                Thread.Sleep(1 / sleep_value);
                            }
                            while (UI_lock)
                            {

                            }
                            this.Watch.Start();
                        }
                        workingset.Clear();
                        foreach (var item in newpop)
                        {
                            workingset.Add(item);
                        }
                        newpop.Clear();
                    }
                }
                else//performance run
                {
                    for (int i = 0; i < MaxGen; i++)
                    {
                        foreach (var item in Workingset)
                        {
                            item.recalc();//if necessary
                        }
                        if (Mode == "cost-time")
                        {
                            this.costtimefitness();
                        }
                        fitness();
                        Workingset.Sort((fit1, fit2) => fit1.Fitness.CompareTo(fit2.Fitness));//sort genomes according to fitness ascending order least->most
                        // Workingset.Reverse();//reverse it so highest fitness first
                        List<genome> newpop = new List<genome>();
                        for (int k = 0; k < PopSize; k++)
                        {
                            selection();
                            genome g = new genome(crossover());
                            g = new genome(mutation(g));
                            newpop.Add(new genome(g));
                            find_localbest(g);
                        }
                        workingset.Clear();
                        foreach (var item in newpop)
                        {
                            workingset.Add(item);
                        }
                        newpop.Clear();
                    }
                }
                this.Watch.Stop();
            }
            catch (Exception t)
            {
                System.Windows.Forms.MessageBox.Show(t.ToString());
            }
            finally
            {
                this.Algo_exec_status = AI_Algorithm_Simulator.classes.exec_status.completed;
            }
        }

        private void fitness()
        {
            fitnessalgorithm.execute(Workingset,Mode);
        }

        private void selection()
        {
            selectionalgorithm.Initialize();//clear parent
            selectionalgorithm.execute(Workingset,2);
        }

        private genome crossover()
        {
            return crossoveralgorithm.execute(new genome(selectionalgorithm.getparents()[0]),new genome(selectionalgorithm.getparents()[1]));
        }

        private genome mutation(genome g)
        {
             return mutationalgorithm.execute(new genome(g));
            //return g;
        }

       public override  object returnalgoobj()
        {
            return this;
        }
    }

}
