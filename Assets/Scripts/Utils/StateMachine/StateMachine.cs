
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLOTC.Utils.StateMachine
{
    public class StateMachine : IState
    {
        private readonly string _uniqueID = Guid.NewGuid().ToString();

        private Dictionary<string, List<Transition>> _transitions = new Dictionary<string, List<Transition>>();
        private List<Transition> _currentTransitions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();

        public IState CurrentState { get; private set; }

        public bool CanExit { get; set; } = true;

        public event Action<IState /* nextState */> OnBeforeChangeState;

        public void OnEnter()
        {
            CanExit = true;
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

            CurrentState?.OnUpdate(deltaTime);
        }

        public void SetState(IState state)
        {
            if (CurrentState != null && !CurrentState.CanExit)
            {
                //Debug.LogWarning("State: " + CurrentState.GetID() + " is not ready to exit. Cause: " + state.GetID());
                return;
            }

            OnBeforeChangeState?.Invoke(state);

            CurrentState?.OnExit();
            CurrentState = state;
            if(!_transitions.TryGetValue(CurrentState.GetID(), out _currentTransitions))
                _currentTransitions = new List<Transition>();

            CurrentState?.OnEnter();
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
                if (!transition.CanTransitionToItself && object.ReferenceEquals(CurrentState, transition.To))
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