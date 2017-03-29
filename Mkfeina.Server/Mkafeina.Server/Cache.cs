using System.Collections.Generic;
using Mkfeina.Server.Domain;

namespace Mkfeina.Server
{
    public class Cache
    {
        private static Cache __singleton;

        private Cache()
        {
        }

        public static Cache Singleton {
            get {
                if (__singleton == null)
                    __singleton = new Cache();
                return __singleton;
            }
        }

        public Dictionary<string, CoffeeMachineProxy> CoffeeMachines { get; } = new Dictionary<string, CoffeeMachineProxy>();
    }
}