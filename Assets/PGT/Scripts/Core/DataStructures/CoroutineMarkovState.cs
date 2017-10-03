using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGT.Core.DataStructures.Markov
{
    public abstract class CoroutineMarkovState : IState
    {
        MonoBehaviour _parentBehaviour;

        Coroutine_ coroutine;
        Coroutine_ coroutine_trans;
        MarkovChain _mc;

        protected MarkovChain markovChain { get { return _mc; } }
        protected GameObject gameObject { get { return _parentBehaviour.gameObject; } }

        public CoroutineMarkovState(MonoBehaviour parentBehaviour, MarkovChain mc)
        {
            _parentBehaviour = parentBehaviour;
            _mc = mc;
            coroutine = null;
        }


        public void Suspend()
        {
            coroutine.Suspend();
        }

        public void Resume()
        {
            coroutine.Resume();
        }
        
        protected abstract IEnumerator Behaviour();

        private IEnumerator _behaviour()
        {
            coroutine = _parentBehaviour.StartCoroutine1(Behaviour(), this);
            yield return coroutine.GetAwaiter();
            _mc.Transition();
        }

        protected Coroutine_ StartCoroutine(IEnumerator coroutine, object mutex = null)
        {
            return _parentBehaviour.StartCoroutine1(coroutine, mutex);
        }


        public void OnEnter(IState from = null)
        {
            if (coroutine != null && coroutine.IsSuspended()) OnLeave();
            coroutine_trans = _parentBehaviour.StartCoroutine1(_behaviour());
        }

        public void OnLeave(IState to = null)
        {
            if (coroutine_trans != null) coroutine_trans.Interrupt();
            if (coroutine != null) coroutine.Interrupt();
        }
    }
}