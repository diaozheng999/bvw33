using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace PGT.Core.RemoteDebug {

    public class RemoteDebug : MonoBehaviour {

        [SerializeField] string remoteUrl;

        public static RemoteDebug instance = null;

        #if NETFX_CORE
        WebSocket ws;
        #endif

        void Awake(){
            if(!instance) instance = this;
        }

        ConcurrentDictionary<string, Action<string>> listeners;

        void Start(){
#if NETFX_CORE
            ws = new WebSocket(remoteUrl, WSHandler);
#endif
            Debug.Log(new Uri(remoteUrl));
            RegisterUpdateListener("ping", Ping);
        }


        public void Log(string format, params object[] args){
            Log(string.Format(format, args));
        }

        public void Log(string message){
#if NETFX_CORE
            if(ws!=null && ws.connected){
                ws?.SendMessage(message);
            }
            Debug.Log(ws);
            Debug.Log(message);
#else
            //Debug.Log(message);
            #endif
        }

        void Ping(string response)
        {
            Log("response:" + response + "\n");
        }

        void WSHandler(string handler){
            var tokens = handler.Split(':');
            Action<string> listener = null;

            if (listeners?.TryGetValue(tokens[0], out listener) ?? false) {
                UnityExecutionThread.instance.ExecuteInMainThread(() => {
                    listener(tokens[1]);
                });
            }

        }


        #if NETFX_CORE
        void OnDestroy(){
            ws.Dispose();
        }
        #endif

        public void RegisterUpdateListener(string keyword, Action<string> function){
#if NETFX_CORE
            if (listeners == null)
                listeners = new ConcurrentDictionary<string, Action<string>>();
            listeners.AddOrUpdate(keyword, function, 
            (string k, Action<string> old_fn) => (string l) => {
                function(l);
                old_fn(l);
            });
#else
            //Debug.LogWarning("Keyword will be registered when remote debugging.");
#endif
        }

    }

}