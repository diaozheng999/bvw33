using System.Threading;
using System.Collections;
using UnityEngine;
using PGT.Core.Func;
using System;
using System.Collections.Generic;

namespace PGT.Core
{
    public static class MonoBehaviourCoroutineExtensions {
        /// <summary>
        /// Starts a coroutine that allows for suspending, interrupting and resuming. The started 
        /// coroutine is not awaitable like a Unity Coroutine. Instead, use Coroutine_.GetAwaiter() 
        /// if you want to wait for the coroutine to complete similar to Unity's coroutines.
        /// 
        /// This function works on disposable coroutines. So the finally block in the coroutine is 
        /// guaranteed to execute.
        /// </summary>
        /// <param name="b">The MonoBehaviour that starts the coroutine.</param>
        /// <param name="coroutine">The Coroutine in question.</param>
        /// <param name="mutex">A mutex. If supplied, this mutex will be used and the coroutine is 
        /// started in chained mode. If null, the coroutine is started normally.</param>
        /// <returns>A interruptable coroutine.</returns>
        public static Coroutine_ StartCoroutine1(this MonoBehaviour b, IEnumerator<object> coroutine, object mutex = null)
        {
            return Coroutine_.StartCoroutine1(b, coroutine, mutex);
        }


        /// <summary>
        /// Starts a coroutine that allows for suspending, interrupting and resuming. The started 
        /// coroutine is not awaitable like a Unity Coroutine. Instead, use Coroutine_.GetAwaiter() 
        /// if you want to wait for the coroutine to complete similar to Unity's coroutines.
        /// 
        /// This function works on disposable coroutines. So the finally block in the coroutine is 
        /// guaranteed to execute.
        /// </summary>
        /// <param name="b">The MonoBehaviour that starts the coroutine.</param>
        /// <param name="coroutine">The Coroutine in question.</param>
        /// <param name="mutex">A mutex. If supplied, this mutex will be used and the coroutine is 
        /// started in chained mode. If null, the coroutine is started normally.</param>
        /// <returns>A interruptable coroutine.</returns>
        public static Coroutine_ StartCoroutine1(this MonoBehaviour b, IEnumerable<object> coroutine, object mutex = null)
        {
            return Coroutine_.StartCoroutine1(b, coroutine.GetEnumerator(), mutex);
        }

        /// <summary>
        /// Starts a coroutine that allows for suspending, interrupting and resuming. The started 
        /// coroutine is not awaitable like a Unity Coroutine. Instead, use Coroutine_.GetAwaiter() 
        /// if you want to wait for the coroutine to complete similar to Unity's coroutines.
        /// 
        /// This function works on non-disposable coroutines. So the finally block is not guaranteed
        /// to execute.
        /// </summary>
        /// <param name="b">The MonoBehaviour that starts the coroutine.</param>
        /// <param name="coroutine">The Coroutine in question.</param>
        /// <param name="mutex">A mutex. If supplied, this mutex will be used and the coroutine is 
        /// started in chained mode. If null, the coroutine is started normally.</param>
        /// <returns>A interruptable coroutine.</returns>
        public static Coroutine_ StartCoroutine1(this MonoBehaviour b, IEnumerator coroutine, object mutex = null)
        {
            return Coroutine_.StartCoroutine1(b, coroutine, mutex);
        }


        /// <summary>
        /// Attempts to start a coroutine in single mode. The difference in execution order between
        /// this and the StartCoroutine1 with a mutex is that the mutex lock is acquired before the
        /// coroutine is started. If a lock on the mutex cannot be acquired, the previous coroutine
        /// that acquired the lock will be returned. 
        /// 
        /// The started coroutine allows for suspending, interrupting and resuming. The started 
        /// coroutine is not awaitable like a Unity Coroutine. Instead, use Coroutine_.GetAwaiter() 
        /// if you want to wait for the coroutine to complete similar to Unity's coroutines.
        /// 
        /// This function works on disposable coroutines. So the finally block in the coroutine is 
        /// guaranteed to execute.
        /// </summary>
        /// <param name="b">The MonoBehaviour that starts the coroutine.</param>
        /// <param name="coroutine">The Coroutine in question.</param>
        /// <param name="mutex">A mutex that should not be null. A warning will be issued if it is 
        /// null and the coroutine will be started normally.</param>
        /// <returns></returns>
        public static Coroutine_ StartCoroutineSingle(this MonoBehaviour b, IEnumerator<object> coroutine, object mutex = null)
        {
            return Coroutine_.StartCoroutineSingle(b, coroutine, mutex);
        }


        /// <summary>
        /// Attempts to start a coroutine in single mode. The difference in execution order between
        /// this and the StartCoroutine1 with a mutex is that the mutex lock is acquired before the
        /// coroutine is started. If a lock on the mutex cannot be acquired, the previous coroutine
        /// that acquired the lock will be returned. 
        /// 
        /// The started coroutine allows for suspending, interrupting and resuming. The started 
        /// coroutine is not awaitable like a Unity Coroutine. Instead, use Coroutine_.GetAwaiter() 
        /// if you want to wait for the coroutine to complete similar to Unity's coroutines.
        /// 
        /// This function works on non-disposable coroutines. So the finally block is not guaranteed
        /// to execute.
        /// </summary>
        /// <param name="b">The MonoBehaviour that starts the coroutine.</param>
        /// <param name="coroutine">The Coroutine in question.</param>
        /// <param name="mutex">A mutex that should not be null. A warning will be issued if it is 
        /// null and the coroutine will be started normally.</param>
        /// <returns></returns>
        public static Coroutine_ StartCoroutineSingle(this MonoBehaviour b, IEnumerator coroutine, object mutex = null)
        {
            return Coroutine_.StartCoroutineSingle(b, coroutine, mutex);
        }
    }

    /// <summary>
    /// A wrapper around any IEnumerator generator function that represents a Unity Coroutine. This 
    /// allows for the executing coroutine to be interrupted, suspended, resumed, started singly,
    /// or started in sequence. Unfortunately named to avoid name-mangling with 
    /// UnityEngine.Coroutine.
    /// </summary>
    public class Coroutine_ : Base
    {
        // How many mutexes do we allow in the dictionary before we start to clean the dictionary
        // for finished coroutines.
        const int MAX_IN_USE_MUTEX = 5000;

        // interrupt flag
        bool interrupt = false;
        // suspend flag
        bool suspend = false;
        // started status flag
        bool started = false;
        // completed status flag
        bool completed = false;
        
        /// <summary>
        /// Convert a unity coroutine into a disposable coroutine
        /// </summary>
        /// <param name="routine">unity coroutine</param>
        /// <returns></returns>
        static IEnumerator<object> wrap(IEnumerator routine)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }
        }


        /// <summary>
        /// Whether the coroutine has completed executing.
        /// </summary>
        public bool Completed
        {
            get { return completed; }
        }

        /// <summary>
        /// Whether the coroutine has started executing. If a coroutine is started in chained 
        /// execution mode with a mutex, this value will be true only after the coroutine has 
        /// acquired lock on the mutex.
        /// </summary>
        public bool Started
        {
            get { return started; }
        }
        // the actual coroutine enumerator
        // this is done such that IDisposable can be called.
        IEnumerator<object> routine;
        // the mutex. This will be null if single execution is not required.
        object mutex;
        // the parent coroutine on Unity's end, returned by Unity's StartCoroutine function
        YieldInstruction unity_handle;
        // whether this is a single execution. 
        bool _single;

        // A dictionary to keep which coroutine has acquired a lock on a mutex
        static Dictionary<object, Coroutine_> inUseBy;

        // constructor for a Disposable coroutine
        private Coroutine_(IEnumerator<object> routine, object mutex, bool single = false)
        {
            this.mutex = mutex;
            this.routine = routine;
            _single = single;
        }

        // Function as delegate for WaitWhile
        private bool isSuspended()
        {
            return suspend;
        }

        // The coroutine that is started by Unity. This wraps the existing coroutine with
        // additional functions.
        IEnumerator _coroutine()
        {
            // Cache objects to avoid generating unnecessary rubbish.
            var waitToResume = new WaitWhile(isSuspended);

            if (mutex != null)
            {
                // wait to acquire the lock.
                if (!_single) 
                {
                    yield return new WaitUntil(Function.delay(CoroutineMonitor.TryEnter, mutex));
                }
                
                SetInUseBy(mutex, this);

                // it is possible for the suspend flag to be set before the coroutine even starts
                // executing, so we will have to pause the execution in such a situation.
                if (suspend)
                {
                    yield return waitToResume;
                }
            }
            // set the started status flag
            started = true;

            // this if statement is here because it is also possible for the coroutine to be 
            // interrupted before execution, specifically while waiting for a lock on the mutex
            // in chained mode. If such a case happens, we do not want the user code to run at all.
            if (!interrupt)
            {
                while (routine.MoveNext())
                {
                    // Due to the syntax of iterators, assume that the user code is executed here.

                    // The yield instruction for user code
                    yield return routine.Current;

                    // Check if the suspend flag is set before running the next chunk of user code.               
                    if (suspend) yield return waitToResume;
                    // Check if the interrupt flag is set before running the next chunk of user 
                    // code.
                    if (interrupt) break;
                }

                // guarantee execution of dispose block
                routine.Dispose();
            }

            // Release the lock on the mutex if possible. Note that if the interrupt flag is set,
            // Then the Interrupt() function would have already released the lock, so we don't do
            // it twice.
            if (mutex != null && !interrupt)
            {
                CoroutineMonitor.Exit(mutex);
            }

            // Set the complete status flag.
            completed = true;
        }
        
        /// <summary>
        /// Start the execution of a coroutine under a specified MonoBehaviour. Note that 
        /// GetAwaiter() returns null before StartWith() is called.
        /// </summary>
        /// <param name="parentBehaviour">The MonoBehaviour to run coroutine under.</param>
        public void StartWith(MonoBehaviour parentBehaviour)
        {
            unity_handle = parentBehaviour.StartCoroutine(_coroutine());
        }

        /// <summary>
        /// Returns a YieldInstruction that signals Unity's Coroutine engine to wait until this
        /// coroutine is done. Note that this returns null before the coroutine is started.
        /// Yielding GetAwaiter() ensures that Completed is set to true.
        /// </summary>
        /// <returns>A YieldInstruction that signals Unity's Coroutine engine to wait until this
        /// coroutine is done.</returns>
        public YieldInstruction GetAwaiter()
        {
            return unity_handle;
        }
        
        /// <summary>
        /// Set the interrupt flag and release locks on resources if applicable. The coroutine
        /// is not terminated immediately but rather the next context-switch. Because of this,
        /// Executing Interrupt() does not mean that Completed is set to true immediately, and
        /// that the lock on the resource will be released. Note that it does not immediately
        /// release manually locked objects such as calling CoroutineMonitor.TryEnter within
        /// a coroutine. These objects will never be released.
        /// [TODO] Fixed this.
        /// </summary>
        public void Interrupt()
        {
            suspend = false;
            interrupt = true;
            // this should remove the resource lock
            if (mutex != null)
            {
                CoroutineMonitor.Exit(mutex);
            }
        }

        /// <summary>
        /// Set the suspend flag. The coroutine will be suspended on the next context-switch.
        /// </summary>
        public void Suspend()
        {
            suspend = true;
        }

        /// <summary>
        /// Unset the suspend flag. The coroutine will resume execution at the next context-switch.
        /// Note that when yielding WaitForSeconds, the counter still ticks when the coroutine is
        /// suspended, and will resume in the next frame iff the WaitForSeconds is up after resuming.
        /// </summary>
        public void Resume()
        {
            suspend = false;
        }

        /// <summary>
        /// Whether the coroutine is suspended.
        /// </summary>
        /// <returns>Whether the coroutine is suspended.</returns>
        public bool IsSuspended()
        {
            return suspend;
        }

        public static Coroutine_ StartCoroutine1(MonoBehaviour b, IEnumerator<object> coroutine, object mutex = null)
        {
            var _coroutine = new Coroutine_(coroutine, mutex);
            _coroutine.StartWith(b);
            return _coroutine;
        }

        public static Coroutine_ StartCoroutine1(MonoBehaviour b, IEnumerator coroutine, object mutex = null)
        {
            return StartCoroutine1(b, wrap(coroutine), mutex);
        }

        public static Coroutine_ StartCoroutineSingle(MonoBehaviour b, IEnumerator<object> coroutine, object mutex)
        {
            if (mutex == null)
            {
                Debug.LogWarning("Mutex is null. Consider using StartCoroutine1 instead.");
                return StartCoroutine1(b, coroutine);
            }

            if (!CoroutineMonitor.TryEnter(mutex))
            {
                return GetInUseBy(mutex);
            }

            var _coroutine = new Coroutine_(coroutine, mutex, true);
            _coroutine.StartWith(b);
            return _coroutine;
        }

        public static Coroutine_ StartCoroutineSingle(MonoBehaviour b, IEnumerator coroutine, object mutex)
        {
            return StartCoroutineSingle(b, wrap(coroutine), mutex);
        }

        static void SetInUseBy(object mutex, Coroutine_ cr)
        {
            if (inUseBy == null) inUseBy = new Dictionary<object, Coroutine_>();
            if (!inUseBy.ContainsKey(mutex)) inUseBy.Add(mutex, cr);
            else inUseBy[mutex] = cr;
        }

        static Coroutine_ GetInUseBy(object mutex)
        {
            if (inUseBy != null && inUseBy.ContainsKey(mutex)) return inUseBy[mutex];
            return null;
        }
    }
}
