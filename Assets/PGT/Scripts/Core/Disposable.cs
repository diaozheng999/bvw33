using UnityEngine;
using System;
using System.Collections.Generic;

namespace PGT.Core {
    public class Disposable : MonoBehaviour, IDisposable {
        LinkedList<IDisposable> objects;

        ///<summary>Adds an IDisposable object to be executed OnDestroy
        ///or disposed by calling Dispose() of this object</summary>
        protected void AddDisposable(IDisposable item){
            objects = objects ?? new LinkedList<IDisposable>();
            objects.AddLast(item);
        }

        ///<summary>Adds a function to be executed OnDestroy
        ///or disposed by calling Dispose() of this object</summary>
        public void AddDisposable(Action dispose_fn) => 
            AddDisposable(new LambdaDisposable(dispose_fn));

        public void Dispose(){
            foreach(var item in objects){
                item?.Dispose();
            }
            objects.Clear();
            objects = null;
        }

        protected virtual void OnDestroy() => Dispose();
    }

    class LambdaDisposable : IDisposable {
        Action _dispose;
        public LambdaDisposable(Action disposeFunction){
            _dispose = disposeFunction;
        }

        public void Dispose() => _dispose();
    }
}