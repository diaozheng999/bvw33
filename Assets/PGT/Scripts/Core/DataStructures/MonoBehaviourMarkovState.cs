using System;
using System.Collections;
using UnityEngine;

namespace PGT.Core.DataStructures.Markov
{
    public abstract class MonoBehaviourMarkovState : MonoBehaviour, IState
    {
        protected MarkovChain parentChain;

        //transition time
        public void SetParentChain(MarkovChain _chain)
        {
            parentChain = _chain;
        }

        public abstract void OnEnterState();

        public void OnEnter(IState from = null)
        {
            enabled = true;
            OnEnterState();
        }
        

        public void OnLeave(IState to = null)
        {
            enabled = false;
        }

        public void Resume()
        {
            enabled = true;
        }

        public void Suspend()
        {
            enabled = false;
        }
    }
}
