using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected int mecanimStateHash;
    protected StateMachine<T> stateMachine;
    protected T context;

    public State() { }

    /// <summary>
    /// Constructor that takes the mecanim state name as a string
    /// </summary>
    public State(string mecanimStateName) : this(Animator.StringToHash(mecanimStateName)) { }

    /// <summary>
    /// Constructor that takes the mecanim state hash
    /// </summary>
    public State(int mecanimStateHash) => this.mecanimStateHash = mecanimStateHash;

    /// <summary>
    /// Constructor that takes the mecanim state hash
    /// </summary>
    internal void SetMachineAndContext(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized();
    }

    /// <summary>
    /// Called directly after the machine and context are set allowing the state to do any required setup
    /// </summary>
    public virtual void OnInitialized() { }
    public virtual void OnEnter() { }
    public virtual void PreUpdate() { }
    public abstract void Update(float deltaTime);
    public virtual void OnExit() { }
}

public sealed class StateMachine<T>
{
    private T _context;
    public event Action OnChangedState;

    private State<T> _currentState;
    public State<T> CurrentState => _currentState;

    private State<T> _previousState;
    public State<T> PreviousState => _previousState;

    private float _elapsedTimeInState = 0.0f;
    public float ElapsedTimeInState => _elapsedTimeInState;

    private Dictionary<System.Type, State<T>> _states = new Dictionary<Type, State<T>>();

    public StateMachine(T context, State<T> initialState)
    {
        this._context = context;

        // Setup our initial state
        AddState(initialState);
        _currentState = initialState;
        _currentState.OnEnter();
    }

    /// <summary>
    /// Adds the state to the machine
    /// </summary>
    public void AddState(State<T> state)
    {
        state.SetMachineAndContext(this, _context);
        _states[state.GetType()] = state;
    }

    /// <summary>
    /// Tick the state machine with the provided delta time
    /// </summary>
    public void Update(float deltaTime)
    {
        _elapsedTimeInState += deltaTime;

        _currentState.PreUpdate();
        _currentState.Update(deltaTime);
    }

    /// <summary>
    /// Changes the current state
    /// </summary>
    public R ChangeState<R>() where R : State<T>
    {
        // avoid changing to the same state
        var newType = typeof(R);
        if (_currentState.GetType() == newType)
            return _currentState as R;

        // only call end if we have a currentState
        if (_currentState != null)
            _currentState.OnExit();


#if UNITY_EDITOR
        if (!_states.ContainsKey(newType))
        {
            var error = GetType() + ": state " + newType +
                        " does not exist. Did you forget to add it by calling addState?";
            Debug.LogError("error");
            throw new Exception(error);
        }
#endif

        // swap states and call OnEnter
        _previousState = _currentState;
        _currentState = _states[newType];
        _currentState.OnEnter();
        _elapsedTimeInState = 0.0f;

        // Fire the changed event if we hav a listener
        if (OnChangedState != null)
            OnChangedState();

        return _currentState as R;
    }
}