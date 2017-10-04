using UnityEngine;

namespace PGT.Core {

    public class Singleton<T> : Disposable where T : Singleton<T>  {

        public static T instance { get; private set; }

        protected virtual void Awake() {
            if (instance != null) {
                Debug.Log("Duplicate instantiation of singleton object "+GetType());
            }
            instance = (T)this;

            AddDisposable(() => instance = null);
        }

    }

}