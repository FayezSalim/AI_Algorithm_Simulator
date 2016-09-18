using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AI_Algorithm_Simulator.classes
{
    public static class SelectionAlgorithmFactory
    {
        private static Dictionary<string, Func<ISelectionAlgorithm>> InstanceCache=new Dictionary<string, Func<ISelectionAlgorithm>>();

        public static ISelectionAlgorithm CreateCachableISelectionAlgorithm(String className)
        {
            if (!InstanceCache.ContainsKey(className))
            {
                Type type = TypeDelegator.GetType("AI_Algorithm_Simulator.classes." + className);
                MethodInfo creator = type.GetMethod("Create");// works with public instance/static methods
                var creatorDelegate = (Func<ISelectionAlgorithm>)Delegate.CreateDelegate(typeof(Func<ISelectionAlgorithm>),creator );// turn it into a delegate
                InstanceCache.Add(className, creatorDelegate);//store in cache
            }
            return InstanceCache[className].Invoke();
       }

        
    }
}
