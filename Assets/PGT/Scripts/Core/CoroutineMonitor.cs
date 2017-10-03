using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PGT.Core
{
    public static class CoroutineMonitor
    {
        static HashSet<object> inUse;

        public static bool TryEnter(object mutex)
        {
            if (UnityExecutionThread.instance.InUnityThread())
            {
                if (inUse==null) inUse = new HashSet<object>();

                if (inUse.Contains(mutex)) return false;
                inUse.Add(mutex);
                Monitor.Enter(mutex);
                return true;
            }
            return Monitor.TryEnter(mutex);
        }


        public static void Exit(object mutex)
        {
            Monitor.Exit(mutex);
            if (UnityExecutionThread.instance.InUnityThread())
            {
                inUse.Remove(mutex);
            }
        }
    }
}
