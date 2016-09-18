using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AI_Algorithm_Simulator.classes
{
    class FitnessAlgorithmFactory
    {
        private static Dictionary<string, Func<IFitnessAlgorithm>> InstanceCache = new Dictionary<string, Func<IFitnessAlgorithm>>();

        public static IFitnessAlgorithm CreateCachableISelectionAlgorithm(String className)
        {
            if (!InstanceCache.ContainsKey(className))
            {
                Type type = TypeDelegator.GetType("AI_Algorithm_Simulator.classes." + className);
                MethodInfo creator = type.GetMethod("Create");// works with public instance/static methods
                var creatorDelegate = (Func<IFitnessAlgorithm>)Delegate.CreateDelegate(typeof(Func<IFitnessAlgorithm>), creator);// turn it into a delegate
                InstanceCache.Add(className, creatorDelegate);//store in cache
            }
            return InstanceCache[className].Invoke();
        }
    }
}
