using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AppearSystem : ECSSystem
{
    private List<Entity> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppearSystem"/> class.
    /// </summary>
    public AppearSystem()
    {
        _entities = new List<Entity>();
    }

    /// <summary>
    /// Subscribes to appropriate messages
    /// </summary>
    public override void Subscribe()
    {
        MessageBus.Subscribe<EntityReAppearsMessage>(EntityAppeared);
    }

    /// <summary>
    /// Unsubscribes from all of the messages
    /// </summary>
    public override void Unsubscribe()
    {
        MessageBus.Unsubscribe<EntityReAppearsMessage>(EntityAppeared);
    }

    /// <summary>
    /// Adds an entity to the system.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        if (state == null || entity.Get<AnimationComponent>() == null)
        {
            return;
        }
        if (state.CurrentSuperState == SuperState.IsAppearing)
        {
            _entities.Add(entity);
        }
    }

    /// <summary>
    /// Handles addition of re-appearing entity to the system.
    /// </summary>
    /// <param name="message">The message containing an entity to add.</param>
    public void EntityAppeared(EntityReAppearsMessage message)
    {
        AddEntity(message.Entity);
    }

    /// <summary>
    /// Removes an entity from the system.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public override void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }

    /// <summary>
    /// Updates the system, checking for entities in an IsAppearing state, and triggering the appropriate action.
    /// </summary>
    /// <param name="gameTime">The current game time.</param>

    public override void TICK(float time)
    {
        int n = _entities.Count - 1;
        if (n == -1)
        {
            //Console.WriteLine("IsEmpty!");  //Making sure the list is empty after entities appeared
            return;
        }
        for (int i = n; i >= 0; i--)
        {
            Entity entity = _entities[i];
            if (!entity.IsActive)
            {
                continue;
            }
            StateComponent state = entity.Get<StateComponent>();
            AnimationComponent animations = entity.Get<AnimationComponent>();

            // Check if the entity is in the IsAppearing state
            if (state.CurrentSuperState == SuperState.IsAppearing)
            {
                ActionAnimation appearAnimation = animations.GetCurrentAnimation();

                // Check if the animation has completed
                if (appearAnimation.IsFinished)
                {
                    state.CanMoveLeft = true;
                    state.CanMoveRight = true;
                    state.CurrentSuperState = state.DefaultSuperState;
                    state.CurrentState = state.DefaultState;

                    RemoveEntity(entity);
                }
            }
        }
    }
}

