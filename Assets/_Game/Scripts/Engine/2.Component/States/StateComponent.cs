using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateComponent : IComponentData
{
    public int ID { get; set; }

    private State _currentState;
    public State previousState { get; private set; }

    private SuperState _currentSuperState;
    public SuperState previousSuperState { get; private set; }

    private bool _canMoveLeft, _canMoveRight;

    private int _horizontalDirection = 1;

    public AnimationID AnimationState { get; private set; }
    public SuperState DefaultSuperState { get; private set; }
    public State DefaultState { get; private set; }
    
    public int DefaultHorizontalDirection { get; private set; }
    public int JumpsPerformed = 0;

   
    public StateComponent(State defaultState = State.Idle, SuperState defaultSuperState = SuperState.IsFalling, State currentState = State.Idle, SuperState currentSuperState = SuperState.IsAppearing)
    {
        _currentState = currentState;
        _currentSuperState = currentSuperState;
        DefaultSuperState = defaultSuperState;
        DefaultState = defaultState;
        DefaultHorizontalDirection = 1;
        if (defaultState == State.WalkLeft)
        {
            DefaultHorizontalDirection = -1;
        }
        _canMoveRight = true;
        _canMoveLeft = true;

        UpdateStateID();
    }

    public void UpdateStateID()
    {
        switch (_currentSuperState)
        {
            case SuperState.IsOnGround:
                AnimationState = AnimationID.Idle;
                if (_currentState == State.WalkLeft || _currentState == State.WalkRight)
                {
                    AnimationState = AnimationID.Walk;
                }
                break;

            case SuperState.IsFalling:
                AnimationState = AnimationID.Fall;
                if (_currentState == State.Slide)
                {
                    AnimationState = AnimationID.Slide;
                }
                break;

            case SuperState.IsJumping:
                AnimationState = AnimationID.Jump;
                break;

            case SuperState.IsDoubleJumping:
                AnimationState = AnimationID.DoubleJump;
                break;

            case SuperState.IsDead:
                AnimationState = AnimationID.Death;
                break;

            case SuperState.IsAppearing:
                AnimationState = AnimationID.Appear;
                break;

            default:
                break;
        }
    }

    public bool CanMoveLeft
    {
        get { return _canMoveLeft; }
        set
        {
            _canMoveLeft = value;

            //if don't want a slide mechanic, delete this
            if (!_canMoveLeft)
            {
                CurrentState = State.Slide;
                HorizontalDirection = -1;
            }
        }
    }

    public bool CanMoveRight
    {
        get { return _canMoveRight; }
        set
        {
            _canMoveRight = value;

            //if don't want a slide mechanic, delete this
            if (!_canMoveRight)
            {
                CurrentState = State.Slide;
                HorizontalDirection = 1;
            }
        }
    }

    public int HorizontalDirection 
    { 
        get => _horizontalDirection; 
        set => _horizontalDirection = value; 
    }

    public State CurrentState
    {
        get => _currentState;
        set
        {
            previousState = _currentState;
            _currentState = value;
            UpdateStateID();
        }
    }

    public SuperState CurrentSuperState
    {
        get => _currentSuperState;
        set
        {
            previousSuperState = _currentSuperState;
            _currentSuperState = value;
            UpdateStateID();
        }
    }

}

