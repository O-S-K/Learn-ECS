using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class RespawnSystem : ECSSystem
{
    private List<Entity> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="RespawnSystem"/> class.
    /// </summary>
    public RespawnSystem()
    {
        _entities = new List<Entity>();
    }

    /// <summary>
    /// Adds an entity to the system if it has a RespawnComponent.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public override void AddEntity(Entity entity)
    {
        if (entity.Get<RespawnComponent>() != null)
        {
            _entities.Add(entity);
        }
    }

    /// <summary>
    /// Removes an entity from the system.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public override void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }

    public override void Subscribe()
    {
    }

    public override void Unsubscribe()
    {
    }

    /// <summary>
    /// Updates the system, checking for entities that need to be respawned.
    /// </summary>
    /// <param name="gameTime">The current game time.</param>

    public override void TICK(float time)
    {
        foreach (Entity entity in _entities)
        {
            RespawnComponent respawn = entity.Get<RespawnComponent>();

            if (!respawn.IsRespawning || entity.IsActive)
            {
                continue;
            }

            respawn.UpdateRespawn();

            if (respawn.IsRespawning)
            {
                continue;
            }

            StateComponent state = entity.Get<StateComponent>();
            MovementComponent movement = entity.Get<MovementComponent>();
            CollisionBoxComponent collision = entity.Get<CollisionBoxComponent>();

            // for any entity, state component should always exist
            state.CurrentSuperState = SuperState.IsAppearing;
            state.HorizontalDirection = state.DefaultHorizontalDirection;

            if (movement != null)
            {
                movement.Position = respawn.Position;
                if (collision != null)
                {
                    collision.UpdateBoxPosition(respawn.Position.x, respawn.Position.y, state.HorizontalDirection);
                }
            }

            entity.IsActive = true;
            MessageBus.Publish(new EntityReAppearsMessage(entity));
        }
    }


}

