using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Threading;
using PGT.Core.Func;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace PGT.Core
{
    public class JsonLoaderAwaiter<T> : CustomYieldInstruction
    {
        JsonLoader<T> loader;
        public JsonLoaderAwaiter(JsonLoader<T> loader)
        {
            this.loader = loader;
        }

        public override bool keepWaiting
        {
            get
            {
                return !loader.IsComplete;
            }
        }
    }


    public class JsonLoader<T> : Base
    {
        public const string LOADED = "PGT/Core/JsonLoader/loaded";

        string resource;
        volatile bool emitEvent;

        volatile string json;
        volatile byte[] bson;

        ManualResetEvent handle;
        ManualResetEvent on_complete_handle;
        volatile bool is_complete;

        T result;

        public T Result
        {
            get
            {
                return result;
            }
        }

        public bool IsComplete
        {
            get
            {
                return is_complete;
            }
        }

        volatile bool executeInUnityThread;

        ResourceRequest request;

        public JsonLoader(string resource, bool emitEvent = false)
        {
            this.resource = resource;
            this.emitEvent = emitEvent;
            handle = new ManualResetEvent(false);
            on_complete_handle = new ManualResetEvent(false);
            is_complete = false;
        }
        

        public void LoadJson(Action<T> onComplete, bool executeInUnityThread = false)
        {
            /*
             * this is a REALLY hacky way of doing multithreading.
             * 
             * Essentially, a Coroutine is spawned to check (frame-by-frame) that the resource request
             * is loaded. If it is, execute a bit of the code in the Unity scripting thread and set
             * an event handle. The spawned thread listens for that event handle.
             */
            request = Resources.LoadAsync<TextAsset>(resource);
            Debug.Log("[JsonLoader] loading resource " + resource);
            this.executeInUnityThread = executeInUnityThread;
            ThreadPool.QueueUserWorkItem(parseJson, onComplete);
            UnityExecutionThread.instance.SetEventHandleWhenDone(request, handle, fillJsonString);
        }

        void parseJson(object _onComplete)
        {
            handle.WaitOne();


            ITraceWriter traceWriter = new JsonDebugTrace(System.Diagnostics.TraceLevel.Error);

            result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TraceWriter = traceWriter });
            onComplete((Action<T>)_onComplete, result);
            on_complete_handle.Set();
            is_complete = true;
        }

        public void LoadBson(Action<T> onComplete, bool executeInUnityThread = false)
        {
            /*
             * this is a REALLY hacky way of doing multithreading.
             * 
             * Essentially, a Coroutine is spawned to check (frame-by-frame) that the resource request
             * is loaded. If it is, execute a bit of the code in the Unity scripting thread and set
             * an event handle. The spawned thread listens for that event handle.
             */
            request = Resources.LoadAsync<TextAsset>(resource);
            this.executeInUnityThread = executeInUnityThread;
            ThreadPool.QueueUserWorkItem(parseBson, onComplete);
            UnityExecutionThread.instance.SetEventHandleWhenDone(request, handle, fillBsonString);
        }

        void parseBson(object _onComplete)
        {
            handle.WaitOne();
            var ms = new MemoryStream(bson);
            using(var BsonReader = new BsonReader(ms))
            {
                var serialiser = new JsonSerializer();
                result = serialiser.Deserialize<T>(BsonReader);
            }
            onComplete((Action<T>)_onComplete, result);
            on_complete_handle.Set();
            is_complete = true;
        }

        void onComplete(Action<T> __onComplete, T result)
        {
            if (executeInUnityThread && !emitEvent)
            {
                UnityExecutionThread.instance.ExecuteInMainThread(() => __onComplete(result));
            }
            else if (executeInUnityThread && emitEvent)
            {
                UnityExecutionThread.instance.ExecuteInMainThread(() =>
                {
                    __onComplete(result);
                    EventRegistry.instance.InvokePrivate(this, LOADED, result);
                });
            }
            else if (emitEvent)
            {
                UnityExecutionThread.instance.ExecuteInMainThread(() =>
                    EventRegistry.instance.InvokePrivate(this, LOADED, result));
            }
            else
            {
                __onComplete(result);
            }
        }


        void fillJsonString()
        {
            json = ((TextAsset)request.asset).text;
        }

        void fillBsonString()
        {
            bson = ((TextAsset)request.asset).bytes;
        }

        public bool WaitForCompletion()
        {
            return on_complete_handle.WaitOne();
        }

        public CustomYieldInstruction GetAwaiter()
        {
            return new JsonLoaderAwaiter<T>(this);
        }
    }
}