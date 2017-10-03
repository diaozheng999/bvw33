using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PGT.Core
{
    public class Animation : CustomYieldInstruction
    {
        public delegate void Callback(object state);
        public delegate object UpdateFunction(object state, float t);
        public delegate object InitFunction();
        public delegate bool CompletionPredicate(object state);


        Callback callback;
        UpdateFunction updateFn;
        InitFunction initFn;
        CompletionPredicate completionPredicate;

        bool initialised;
        object state;
        bool completed;
        public bool autoUpdate;

        public Animation(InitFunction init, UpdateFunction update, CompletionPredicate pred, Callback callback = null, bool autoUpdate = true)
        {
            initFn = init;
            updateFn = update;
            completionPredicate = pred;
            this.callback = callback;
            initialised = false;
            completed = false;
            this.autoUpdate = autoUpdate;
        }

        /// <summary>
        /// Steps the animation by t, and stores the new state. If the animation sequence isn't initalised, initFn is called.
        /// CompletionPredicate is checked at every step, if true and a callback function exists, callback is called first,
        /// then the function returns true. Returns false if completionPredicate evaluates to false.
        /// </summary>
        /// <param name="t">A parameter (usually deltaTime) that indicates how far along is the animation</param>
        /// <returns></returns>
        public bool Step(float t)
        {
            if (!initialised)
            {
                state = initFn.Invoke();
                initialised = true;
            }
            state = updateFn.Invoke(state, t);

            completed = completionPredicate.Invoke(state);

            if (completed && callback != null)
                callback.Invoke(state);

            return completed;
        }


        public void InvokeCallback()
        {
            if (callback != null)
            {
                callback.Invoke(state);
            }
        }


        public override bool keepWaiting
        {
            get
            {
                return !completed;
            }
        }
    }
}
