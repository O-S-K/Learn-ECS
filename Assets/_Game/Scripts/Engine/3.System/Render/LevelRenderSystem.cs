using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelRenderSystem : ECSSystem
{
    // A list of EntityData instances, which store references to the associated Entity, StateComponent, AnimatedComponent, and MovementComponent.
    private string _levelID;

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelRenderSystem"/> class.
    /// </summary>
    public LevelRenderSystem(LevelID levelID)
    {
        _levelID = levelID.ToString();
    }

    /// <summary>
    /// Draws the current level.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public override void Draw(SpriteRenderer spriteBatch)
    {
        Loader.tiledHandler.Draw(_levelID, spriteBatch);
    }

    public override void AddEntity(Entity entity)
    {
       
    }

    public override void RemoveEntity(Entity entity)
    { 
    }

    public override void Subscribe()
    { 
    }

    public override void TICK(float deltaTime)
    { 
    }

    public override void Unsubscribe()
    { 
    }
}

