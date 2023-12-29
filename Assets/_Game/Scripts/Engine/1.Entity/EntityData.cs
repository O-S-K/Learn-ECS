using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct EntityData 
{
    public Entity entity;
    public MovementComponent movement;
    public StateComponent state;
    public CollisionBoxComponent collisionBox;
    public AnimationComponent animation;
}

