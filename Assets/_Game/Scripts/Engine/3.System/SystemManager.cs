using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemManager
{
    private List<ECSSystem> _systems = new List<ECSSystem>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemManager"/> class.
    /// </summary>
    /// <param name="LevelID">The ID of the level.</param>
    public SystemManager(LevelID levelID)
    {
        ResetSystems(levelID);
    }

    /// <summary>
    /// Resets the system manager, used whenever a level is changed/reloaded
    /// </summary>
    /// <param name="LevelID">The ID of the level.</param>
    public void ResetSystems(LevelID levelID)
    {
        _systems = new List<ECSSystem>
        {
            // Input Systems
            new PlayerInputSystem(),
            new RegularEnemyInputSystem(),

            // Game Logic Systems
            new MovementSystem(),
            new PlayerEntityCollisionsSystem(),
            new ObstacleCollisionSystem(levelID),
            new RespawnSystem(),
            new AppearSystem(),

            // Render Systems
            new ParallaxSystem(),
            new LevelRenderSystem(levelID),
            new TimerSystem(),
            new AnimationRenderSystem(),

            // Death
            new DeathSystem()
        };

    }


    /// <summary>
    /// Adds an entity to all the systems.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public void Add(Entity entity)
    {
        foreach (var system in _systems)
        {
            system.AddEntity(entity);
        }
    }

    /// <summary>
    /// Removes an entity from all systems.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public void Remove(Entity entity)
    {
        foreach (var system in _systems)
        {
            system.RemoveEntity(entity);
        }
    }

    /// <summary>
    /// Subscribes all systems to their MessageBus events.
    /// </summary>
    public void Subscribe()
    {
        foreach (var system in _systems)
        {
            system.Subscribe();
        }
    }

    /// <summary>
    /// Unsubscribes all systems from their MessageBus events.
    /// </summary>
    public void Unsubscribe()
    {
        foreach (var system in _systems)
        {
            system.Unsubscribe();
        }
    }

    /// <summary>
    /// Updates all the systems with the specified <see cref="GameTime"/>.
    /// </summary>
    /// <param name="gameTime">The time since the last update.</param>
    public void Tick(float deltaTime)
    {
        foreach (var system in _systems)
        {
            system.TICK(deltaTime);
        }
    }

    /// <summary>
    /// Draws all the entities managed by the systems with the specified <see cref="SpriteBatch"/>.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to draw the entities.</param>
    public void Draw(SpriteRenderer spriteBatch)
    {
        foreach (var system in _systems)
        {
            system.Draw(spriteBatch);
        }
    }
}
