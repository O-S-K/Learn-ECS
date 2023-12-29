using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DeathSystem : ECSSystem
{
    private List<Entity> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeathSystem"/> class.
    /// </summary>
    public DeathSystem()
    {
        _entities = new List<Entity>();
    }

    public override void AddEntity(Entity entity)
    {
        _entities.Add(entity);
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
    /// Subscribes to appropriate messages
    /// </summary>
    public override void Subscribe()
    {
        MessageBus.Subscribe<EntityDiedMessage>(EntityDied);
    }


    /// <summary>
    /// Unsubscribes from all of the messages
    /// </summary>
    public override void Unsubscribe()
    {
        MessageBus.Unsubscribe<EntityDiedMessage>(EntityDied);
    }

    /// <summary>
    /// Updates the system, checking for entities in a dead state and triggering the appropriate action.
    /// </summary>
    /// <param name="gameTime">The current game time.</param>

    public override void TICK(float time)
    {
        int n = _entities.Count - 1;
        for (int i = n; i >= 0; i--)
        {
            Entity entity = _entities[i];
            if (!entity.IsActive)
            {
                continue;
            }

            StateComponent stateComponent = entity.Get<StateComponent>(); //can't be null
            AnimationComponent animatedComponent = entity.Get<AnimationComponent>(); //can't be null
            RespawnComponent canBeRespawned = entity.Get<RespawnComponent>();
            EntityTypeComponent entityType = entity.Get<EntityTypeComponent>();

            if (stateComponent.CurrentSuperState != SuperState.IsDead)
            {
                continue;
            }

            ActionAnimation deathAnimation = animatedComponent.GetCurrentAnimation();
            if (!deathAnimation.IsFinished)
            {
                continue;
            }

            // Trigger death event
            if (entityType != null && entityType.Type == EntityType.Player)
            {
                MessageBus.Publish(new DestroyEntityMessage(entity));
                MessageBus.Publish(new ReloadLevelMessage());
            }
            else if (canBeRespawned != null)
            {
                entity.IsActive = false;
                deathAnimation.Reset();
                canBeRespawned.StartRespawn();
            }
            else
            {
                MessageBus.Publish(new DestroyEntityMessage(entity));
            }
        }
    }

    /// <summary>
    /// Adds entities to the system
    /// </summary>
    /// <param name="message">The message containing the entity to add.</param>
    private void EntityDied(EntityDiedMessage message)
    {
        _entities.Add(message.Entity);
    }

}

