using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGT.Core.DataStructures.Markov
{
    /// <summary>
    /// Simple Markov State that does an immediate transition.
    /// </summary>
    public class SimpleMarkovState : IState
    {
        MarkovChain chain;
        

        public SimpleMarkovState(MarkovChain chain)
        {
            this.chain = chain;
        }

        /// <summary>
        /// Override to implement the transition behaviour.
        /// To implement non-immediate states, use CoroutineMarkovState instead.
        /// </summary>
        /// <returns>Whether a chain.Transition() call is issued.</returns>
        protected virtual bool Behaviour() { return true; }

        public void OnEnter(IState from = null)
        {
            if (Behaviour()) chain.Transition();
        }

        public void OnLeave(IState to = null)
        {
        }

        public void Resume()
        {

        }

        public void Suspend()
        {

        }
    }
}
