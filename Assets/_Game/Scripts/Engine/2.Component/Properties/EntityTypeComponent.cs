using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityTypeComponent : IComponentData
{
    public int ID { get; set; }

    public  EntityType Type { get; private set; }
 
    public EntityTypeComponent(EntityType type)
    {
        Type = type;
    }
}
