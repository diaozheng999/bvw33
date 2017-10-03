namespace PGT.Core
{
    using UnityEngine;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
    using Func;
    
    public enum ExecutionOrder
    {
        FixedUpdate,
        Update,
        Coroutine,
        LateUpdate,
        Any
    }

    public class UnityExecutionThread : SyncMonoBehaviour
    {
        public static UnityExecutionThread instance;

        public enum ReturnState
        {
            Done,
            Exception,
            Pending
        }

        protected Thread UnityThread;
        int UnityThreadId;
        bool active;

        Dictionary<Lambda, Func.Tuple<ReturnState, object>> executed;
        Dictionary<ExecutionOrder, Queue<Lambda>> executionChain;

        AutoResetEvent resultFlag;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            UnityThread = Thread.CurrentThread;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
            active = true;
            executed = new Dictionary<Lambda, Tuple<ReturnState, object>>();
            executionChain = new Dictionary<ExecutionOrder, Queue<Lambda>>();

            resultFlag = new AutoResetEvent(false);

            foreach (ExecutionOrder exec in System.Enum.GetValues(typeof(ExecutionOrder)))
            {
                executionChain[exec] = new Queue<Lambda>();
            }
            instance = this;
            active = true;

            DontDestroyOnLoad(gameObject);
#if _DEBUG
            Debug.Log(instance);
#endif
        }

        public bool InUnityThread()
        {
            return Thread.CurrentThread.ManagedThreadId == UnityThreadId;
        }

        public void SetEventHandleWhenDone(AsyncOperation op, EventWaitHandle handle, Lambda InUnityFunction)
        {
            StartCoroutine(_setEventHandleWhenDone(op, handle, InUnityFunction));
        }

        IEnumerator _setEventHandleWhenDone(AsyncOperation op, EventWaitHandle handle, Lambda InUnityFunction)
        {
            yield return new WaitUntil(() => op.isDone);
            InUnityFunction();
            handle.Set();
        }

        void Execute(ExecutionOrder chain)
        {
            if (executionChain[chain].Count > 0)
            {
                lock (executionChain[chain])
                {
                    //while(executionChain[chain].Count > 0)
                    //{
                        Lambda fn = executionChain[chain].Dequeue();
                        Tuple<ReturnState, object> result;
                        try
                        {
                            fn.Invoke();
                            result = Tuple<ReturnState, object>._(ReturnState.Done, null);
                        }
                        catch (System.Exception e)
                        {
                            result = Tuple<ReturnState, object>._(ReturnState.Exception, e);
                        }
                        lock (executed)
                        {
                            executed.Add(fn, result);
                            resultFlag.Set();
                        }
                    //}
                }
            }
        }

        public void ExecuteInMainThread(Lambda fn, ExecutionOrder order = ExecutionOrder.Any)
        {
            if (InUnityThread())
            {
                fn.Invoke();
                return;
            }
            lock (executionChain[order])
            {
                executionChain[order].Enqueue(fn);
            }

            while (true){
                resultFlag.WaitOne();
                Tuple<ReturnState, object> result = Tuple<ReturnState, object>._(ReturnState.Pending, null);
                lock (executed)
                {
                    if (executed.ContainsKey(fn))
                    {
                        result = executed[fn];
                        executed.Remove(fn);
                    }
                }
                if (result.car == ReturnState.Exception)
                    throw (System.Exception)result.cdr;
                if (result.car == ReturnState.Done)
                    return;
            }
        }

        public T ExecuteInMainThread<T>(Future<T> fn, ExecutionOrder order = ExecutionOrder.Any)
        {
            if (InUnityThread()) return fn.Invoke();
            T val = default(T);
            ExecuteInMainThread(() =>
            {
                val = fn.Invoke();
            }, order);
            return val;
        }

        public void ExecuteSequence(ICollection<Lambda> t, ExecutionOrder order = ExecutionOrder.Any)
        {
            if (InUnityThread()) throw new System.Exception("Cannot call UnityExecutionThread::ExecuteInSequence in Unity thread.");

            foreach(Lambda step in t)
            {
                ExecuteInMainThread(step, order);
            }
        }

        void FixedUpdate()
        {
            Execute(ExecutionOrder.Any);
            Execute(ExecutionOrder.FixedUpdate);
        }

        void Update()
        {
            Execute(ExecutionOrder.Any);
            Execute(ExecutionOrder.Update);
            EventRegistry.instance.Update(Time.deltaTime);
        }

        void LateUpdate()
        {
            Execute(ExecutionOrder.Any);
            Execute(ExecutionOrder.LateUpdate);
        }

        IEnumerator Start()
        {
            while (active)
            {
                Execute(ExecutionOrder.Any);
                Execute(ExecutionOrder.LateUpdate);
                yield return null;
            }
        }

        void OnDestroy()
        {
            active = false;
        }
    }
}
