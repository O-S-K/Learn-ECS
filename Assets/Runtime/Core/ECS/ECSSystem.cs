using UnityEngine;
using System;

public abstract class ECSSystem
{
    public abstract void AddEntity(Entity entity);
    public abstract void RemoveEntity(Entity entity);

    public abstract void Subscribe();
    public abstract void Unsubscribe();

    public virtual void Draw(SpriteRenderer sprite) { }

    public abstract void TICK(float deltaTime);
}

