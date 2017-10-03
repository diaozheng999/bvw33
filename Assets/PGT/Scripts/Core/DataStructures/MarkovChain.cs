namespace PGT.Core.DataStructures
{
    using System.Collections.Generic;
    using Markov;
    using System;

    namespace Markov
    {
        public interface IState
        {
            /// <summary>
            /// OnEnter is called by the MarkovChain class when the state is entered
            /// </summary>
            /// <param name="from"></param>
            void OnEnter(IState from=null);
            /// <summary>
            /// OnLeave is called by the MarkovChain class when the state is exitted. Note that OnLeave is always called before OnEnter.
            /// </summary>
            /// <param name="to"></param>
            void OnLeave(IState to=null);
            /// <summary>
            /// Halts internal state update function
            /// </summary>
            void Suspend();
            /// <summary>
            /// Resumes internal state update function
            /// </summary>
            void Resume();
        } 

        public class DimensionMismatchException : Exception { }
    }

    public class MarkovChain
    {
        IState[] states;
        float[,] delta;
        int size;
        int currState;

        public MarkovChain(int size, int currState)
        {
            states = new IState[size];
            delta = new float[size,size];
            this.size = size;
            this.currState = currState;
        }

        public void SetState(int i, IState state)
        {
            states[i] = state;
        }

        public void SetTransitionProb(int i, int j, float prob)
        {
            delta[i, j] = prob;
        }

        public void Begin()
        {
            states[currState].OnEnter(null);
        }

        public void Transition()
        {
            float trans = UnityEngine.Random.value;
            for(int i=0;i< size; i++)
            {
                if(delta[currState, i] > trans)
                {
                    HardTransition(i);
                    break;
                }
                trans -= delta[currState, i];
            }
        }

        public T GetCurrentStateAs<T>()
        {
            return (T)states[currState];
        }

        public void Suspend()
        {
            states[currState].Suspend();
        }

        public void Resume()
        {
            states[currState].Resume();
        }

        public void ResetState()
        {
            HardTransition(currState);
        }

        public void HardTransition(int toState)
        {
            int _currState = currState;
            currState = toState;
            states[_currState].OnLeave(states[toState]);
            states[toState].OnEnter(states[_currState]);
        }

        public int CurrentState()
        {
            return currState;
        }
    }
}
