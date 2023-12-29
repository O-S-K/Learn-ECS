using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputSystem : ECSSystem
{
    private List<Entity> entities;
    private List<StateComponent> states;
    private List<PlayerInputComponent> inputs;


    public PlayerInputSystem()
    {
        entities = new List<Entity>();
        states = new List<StateComponent>();
        inputs = new List<PlayerInputComponent>();
    }

    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        PlayerInputComponent playerInput = entity.Get<PlayerInputComponent>();

        if (state == null || playerInput == null)
            return;

        entities.Add(entity);
        states.Add(state);
        inputs.Add(playerInput);
    }

    public override void RemoveEntity(Entity entity)
    {
        int index = entities.IndexOf(entity);
        if (index != -1)
        {
            entities.RemoveAt(index);
            states.RemoveAt(index);
            inputs.RemoveAt(index);
        }
    }

    public override void Subscribe()
    {
    }

    public override void Unsubscribe()
    {
    }

    public override void TICK(float time)
    {
        int n = inputs.Count;
        for (int i = 0; i < n; i++)
        {
            Entity entity = entities[i];
            StateComponent state = states[i];
            PlayerInputComponent input = inputs[i];

            if (!entity.IsActive || state.CurrentSuperState == SuperState.IsDead || state.CurrentSuperState != SuperState.IsAppearing)
            {
                continue;
            }

            input.UpdateInput();
            UpdateEntityState(input, state);
        }
    }

    private void UpdateEntityState(PlayerInputComponent input, StateComponent state)
    {
        bool bothKeysDown = input.IsLeftKeyDown && input.IsRightKeyDown;
        bool bothKeysUp = !input.IsLeftKeyDown && !input.IsRightKeyDown;

        switch (state.CurrentState)
        {
            case State.Slide:
                if (bothKeysDown || bothKeysUp)
                {
                    break;
                }
                else if (input.IsLeftKeyDown && state.HorizontalDirection == 1)
                {
                    state.CurrentState = State.WalkLeft;
                }
                else if (input.IsRightKeyDown && state.HorizontalDirection == -1)
                {
                    state.CurrentState = State.WalkRight;
                }
                break;

            default:
                if (bothKeysDown || bothKeysUp)
                {
                    state.CurrentState = State.Idle;
                }
                else if (input.IsLeftKeyDown)
                {
                    state.CurrentState = State.WalkLeft;
                }
                else if (input.IsRightKeyDown)
                {
                    state.CurrentState = State.WalkRight;
                }
                break;
        }

        switch (state.CurrentSuperState)
        {
            case SuperState.IsOnGround:
                state.JumpsPerformed = 0;
                if (input.IsJumpKeyDown && !bothKeysDown)
                {
                    state.JumpsPerformed = 1;
                    state.CurrentState = State.Jump;
                }
                break;

            case SuperState.IsFalling:
                if (state.CurrentState == State.Slide)
                {
                    state.JumpsPerformed = 1;
                }
                else if (input.IsJumpKeyDown && !bothKeysDown)
                {
                    if (state.JumpsPerformed == 0)
                    {
                        state.CurrentState = State.Jump;
                    }
                    else if (state.JumpsPerformed == 1)
                    {
                        state.CurrentState = State.DoubleJump;
                    }
                    state.JumpsPerformed += 1;
                }
                break;

            default:
                break;
        }
    }
}

