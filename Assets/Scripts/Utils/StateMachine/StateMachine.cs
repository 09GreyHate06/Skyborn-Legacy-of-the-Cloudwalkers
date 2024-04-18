
using System;
using System.Collections.Generic;

namespace SLOTC.Utils.StateMachine
{
    public class StateMachine : IState
    {
        private readonly string _uniqueID = Guid.NewGuid().ToString();

        private Dictionary<string, List<Transition>> _transitions = new Dictionary<string, List<Transition>>();
        private List<Transition> _currentTransitions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();

        private IState _currentState;

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public string GetID()
        {
            return _uniqueID;
        }

        public void OnUpdate(float deltaTime)
        {
            Transition transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            _currentState?.OnUpdate(deltaTime);
        }

        public void SetState(IState state)
        {
            if (_currentState == state)
                return;

            _currentState?.OnExit();
            _currentState = state;
            if(!_transitions.TryGetValue(_currentState.GetID(), out _currentTransitions))
                _currentTransitions = new List<Transition>();

            _currentState?.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (!_transitions.TryGetValue(from.GetID(), out List<Transition> transitions))
            {
                transitions = new List<Transition>();
                _transitions[from.GetID()] = transitions;
            }

            transitions.Add(new Transition(to, false, condition));
        }

        public void AddAnyTransition(IState state, bool canTransitionToItself, Func<bool> condition)
        {
            _anyTransitions.Add(new Transition(state, canTransitionToItself, condition));
        }

        private Transition GetTransition()
        {
            foreach (Transition transition in _anyTransitions)
            {
                if (!transition.CanTransitionToItself && object.ReferenceEquals(_currentState, transition.To))
                    continue;

                if (transition.Condition())
                    return transition;
            }

            foreach (Transition transition in _currentTransitions)
            {
                if (transition.Condition())
                    return transition;
            }

            return null;
        }

        private class Transition
        {
            public IState To { get; private set; }
            public bool CanTransitionToItself { get; private set; }
            public Func<bool> Condition { get; private set; }

            public Transition(IState to, bool canTransitionToItself, Func<bool> condition)
            {
                To = to;
                CanTransitionToItself = canTransitionToItself;
                Condition = condition;
            }
        }
    }
}