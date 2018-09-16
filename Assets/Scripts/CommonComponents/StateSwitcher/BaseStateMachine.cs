using System.Collections.Generic;

namespace Assets.Scripts.CommonComponents.StateSwitcher
{
    public class BaseStateMachine <TKey,TState> where TState:IBaseState
    {
        private Dictionary<TKey,TState> _states;

        private TState _currentState;
        public TState CurrentState {
            get { return _currentState;}
        }

        public BaseStateMachine()
        {
            _states = new Dictionary<TKey, TState> ();
        }

        public void Add(TKey key_,TState state_)
        {
            if (!_states.ContainsKey (key_)) {
                _states.Add (key_, state_);
            }
        }

        public void Remove(TKey key_)
        {
            _states.Remove (key_);
        }

        public void SetState(TKey key_)
        {
            if (_currentState != null) {
                _currentState.OnExitState();
            }

            _currentState = _states [key_];
            _currentState.OnEnterState ();
        }
    }
}


