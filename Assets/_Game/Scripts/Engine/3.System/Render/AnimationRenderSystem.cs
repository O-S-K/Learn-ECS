using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimationRenderSystem : ECSSystem
{
    private List<EntityData> _entitiesData;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimationRenderSystem"/> class.
    /// </summary>
    public AnimationRenderSystem()
    {
        _entitiesData = new List<EntityData>();
    }

    /// <summary>
    /// Adds an entity to the render system.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        AnimationComponent animations = entity.Get<AnimationComponent>();
        MovementComponent movement = entity.Get<MovementComponent>();

        if (state == null || animations == null || movement == null)
        {
            return;
        }

        EntityData data = new EntityData
        {
            entity = entity,
            state = state,
            animation = animations,
            movement = movement,
        };

        _entitiesData.Add(data);
    }

    /// <summary>
    /// Removes an entity from the render system.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public override void RemoveEntity(Entity entity)
    {
        var index = _entitiesData.FindIndex(data => data.entity == entity);
        if (index != -1)
        {
            _entitiesData.RemoveAt(index);
        }
    }

  
    /// <summary>
    /// Draws the entities in the render system.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public override void Draw(SpriteRenderer spriteBatch)
    {
        foreach (var data in _entitiesData)
        {
            if (!data.entity.IsActive)
            {
                continue;
            }
            data.animation.Draw(spriteBatch, data.movement.Position, data.state.HorizontalDirection);
        }
    }

    /// <summary>
    /// Sets the current animation for an entity based on its current state.
    /// </summary>
    /// <param name="state">The state component of the entity.</param>
    /// <param name="animations">The animated component of the entity.</param>
    private void GetAnimationForState(StateComponent state, AnimationComponent animations)
    {
        animations.SetCurrentAction(state.AnimationState);
    }


    public override void Subscribe()
    {
    }

    public override void TICK(float deltaTime)
    {
        foreach (EntityData data in _entitiesData)
        {
            if (!data.entity.IsActive)
            {
                continue;
            }
            // Get the appropriate animation for the current state
            GetAnimationForState(data.state, data.animation);
            data.animation.UpdateAnim();
        }
    }

    public override void Unsubscribe()
    {
    }
}

