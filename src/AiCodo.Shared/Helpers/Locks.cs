using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo
{
    public static class Locks
    {
        private static Dictionary<object, object> _Locks =
            new Dictionary<object, object>();

        public static object GetLock(object key)
        {
            lock (_Locks)
            {
                if (_Locks.TryGetValue(key, out object obj))
                {
                    return obj;
                }
                obj = new object();
                _Locks.Add(key, obj);
                return obj;
            }
        }
    }
}
