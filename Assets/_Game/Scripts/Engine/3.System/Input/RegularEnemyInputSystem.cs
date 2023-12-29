using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RegularEnemyInputSystem : ECSSystem
{
    private List<Entity> entities;
    private List<StateComponent> states;
    private List<RegularEnemyComponent> inputs;
    private List<MovementComponent> movements;


    /// <summary>
    /// Initializes a new instance of RegularEnemyInputSystem class.
    /// </summary>
    public RegularEnemyInputSystem()
    {
        entities = new List<Entity>();
        states = new List<StateComponent>();
        inputs = new List<RegularEnemyComponent>();
        movements = new List<MovementComponent>();
    }

    /// <summary>
    /// Adds an entity to the system.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        RegularEnemyComponent input = entity.Get<RegularEnemyComponent>();
        MovementComponent movement = entity.Get<MovementComponent>();

        if (state == null || input == null || movement == null)
        {
            return;
        }

        entities.Add(entity);
        states.Add(state);
        inputs.Add(input);
        movements.Add(movement);
    }

    /// <summary>
    /// Removes an entity from the system.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public override void RemoveEntity(Entity entity)
    {
        int index = entities.IndexOf(entity);
        if (index != -1)
        {
            entities.RemoveAt(index);
            states.RemoveAt(index);
            inputs.RemoveAt(index);
            movements.RemoveAt(index);
        }
    }
 
    private void UpdateEntityState(float gameTime, RegularEnemyComponent input, StateComponent state)
    {
        //Debug.Log($"Entity state before update: {state.CurrentState}");

        switch (state.CurrentSuperState)
        {
            case SuperState.IsOnGround:
                break;
            default:
                return;
        }

        switch (state.CurrentState)
        {
            case State.WalkLeft:
                if (input.IsRight || !state.CanMoveLeft)
                {
                    state.CurrentState = State.WalkRight;
                }
                break;

            case State.WalkRight:
                if (input.IsLeft || !state.CanMoveRight)
                {
                    state.CurrentState = State.WalkLeft;

                }
                break;

            default:
                state.CurrentState = state.DefaultState;

                if (!state.CanMoveLeft)
                {
                    state.CurrentState = State.WalkRight;
                }
                else if (!state.CanMoveRight)
                {
                    state.CurrentState = State.WalkLeft;
                }
                break;
        }
    }
    public override void Subscribe()
    {
    }

    public override void TICK(float deltaTime)
    {
        int n = inputs.Count;
        for (int i = 0; i < n; i++)
        {
            StateComponent state = states[i];
            if (!entities[i].IsActive || state.CurrentSuperState == SuperState.IsAppearing || state.CurrentSuperState == SuperState.IsDead)
            {
                continue;
            }

            inputs[i].UpdateMovement(movements[i].Position.x);
            UpdateEntityState(deltaTime, inputs[i], state);
        }
    }

    public override void Unsubscribe()
    {
    }
}

