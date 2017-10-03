namespace PGT.Core
{
    using UnityEngine;
    using System.Collections.Generic;
    

    public class AnimationController : MonoBehaviour
    {
        // some default constants so that we can animate different things in different tracks.
        public const int POSITION = 0;
        public const int LOCAL_POSITION = 1;
        public const int ROTATION = 2;
        public const int LOCAL_ROTATION = 3;
        public const int SCALE = 4;
        public const int LOCAL_SCALE = 5;

        public enum Ease {Out, None};

        public struct LinearAnimationState<T>
        {
            public T origin;
            public float t;
        }

        public float epsilon = 0.1f;
        

        public Queue<Animation>[] animationQueue;

        public int numTracks = 8;

        void Awake()
        {
            //initialisation
            animationQueue = new Queue<Animation>[numTracks];

            for (int i = 0; i < numTracks; i++)
                animationQueue[i] = new Queue<Animation>();
        }


        public void Resize(int tracks)
        {
            numTracks = tracks;
            Awake();
        }

        public delegate void FieldSetFn<T>(T value);
        public delegate T FieldGetFn<T>();
        public delegate T LerpFn<T>(T a, T b, float t);
        public delegate float Comparator<T>(T a, T b);
        public delegate void Callback();

        public void RunAnimation(Animation anim, bool immediate = false, int track = 0)
        {
            if (immediate) animationQueue[track] = new Queue<Animation>();
            animationQueue[track].Enqueue(anim);
        }
        
        public Animation Linear<T>(
            FieldGetFn<T> getter,
            FieldSetFn<T> setter,
            LerpFn<T> lerp,
            T dest,
            float velocity,
            Animation.Callback callback = null)
        {
            T origin = getter.Invoke();
            float t = 0;
            return new Animation(
                () =>
                {
                    origin = getter.Invoke();
                    t = 0f;
                    return null;
                },
                (object state, float dt) =>
                {
                    t += dt * velocity;
                    setter.Invoke(lerp.Invoke(origin, dest, t));
                    return state;
                },
                (object state) => t>=1, 
                (object state) =>
                {
                    if(callback!=null)
                        callback.Invoke(null);
                });
        }

        public Animation Lerp<T>(
            FieldGetFn<T> getter,
            FieldSetFn<T> setter,
            LerpFn<T> lerp,
            Comparator<T> magnitude,
            T dest,
            float velocity,
            Animation.Callback callback = null)
        {
            return new Animation(
                () => null,
                (object state, float t) =>
                {
                    setter.Invoke(lerp.Invoke(getter.Invoke(), dest, velocity * t ));
                    return state;
                },
                (object state) => (magnitude.Invoke(getter.Invoke(), dest) < epsilon),
                (object state) =>
                {
                    setter.Invoke(dest);
                    if (callback != null) callback.Invoke(state);
                }
                );
        }

        public static LerpFn<float> Slerp(bool clockwise = true)
        {
            return (float i, float f, float t) =>
            {
                float x = i % 360;
                float y = f % 360;
                float distX = 360 - x;
                float distY = 360 - y;
                float threshold;

                if (x < 0) x += 360;
                if (y < 0) y += 360;


                if ((x<y && clockwise) || (x>=y && !clockwise))
                {
                    return Mathf.Lerp(i, f, t);
                }

                if(x>=y && clockwise)
                {
                    threshold = distX / (distX + y);
                    if (t < threshold)
                        return Mathf.Lerp(x, 360, t / threshold);
                    else
                        return Mathf.Lerp(0, y, t / threshold - 1);
                }

                threshold = x / (x + distY);
                if (t < threshold)
                    return Mathf.Lerp(x, 0, t / threshold);
                else
                    return Mathf.Lerp(360, y, t / threshold - 1);
            };
        }
        
        void LateUpdate()
        {
            for (int i = 0; i < numTracks; i++)
            {
                if (animationQueue[i].Count > 0 && animationQueue[i].Peek().autoUpdate && animationQueue[i].Peek().Step(Time.deltaTime))
                    animationQueue[i].Dequeue();
            }
        }

        /// <summary>
        /// Manually step animations. Note that this will only step any non-autoUpdated animations.
        /// Non-autoUpdated functions will not be stepped in the Update() function and would therefore stop an animation queue.
        /// </summary>
        public void StepAnimations()
        {
            for (int i = 0; i < numTracks; i++)
                if (animationQueue[i].Count > 0 && !animationQueue[i].Peek().autoUpdate && animationQueue[i].Peek().Step(Time.deltaTime))
                    animationQueue[i].Dequeue();
        }
        

        public void StopAnimation(int track = -1)
        {
            if (track >= 0) animationQueue[track] = new Queue<Animation>();
            else
            {
                for(int i = 0; i < animationQueue.Length; i++)
                {
                    animationQueue[i] = new Queue<Animation>();
                }
            }
        }
    }
}
