using System;
using System.Collections.Generic;

public interface IComponentData
{
    public int ID { get; set; }
}

public readonly struct ComponentData
{
    private static Dictionary<Type, ComponentData> ComponentsIds = new();

    private readonly byte ID;

    private ComponentData(byte id)
    {
        ID = id;
    }

    public static ComponentData Of(Type type)
    {
        if (ComponentsIds.TryGetValue(type, out ComponentData componentId))
        {
            return componentId;
        }

        componentId = new ComponentData((byte)ComponentsIds.Values.Count);
        ComponentsIds.Add(type, componentId);
        return componentId;
    }

    public static ComponentData Of<T>() where T : IComponentData => Of(typeof(T));

    public static implicit operator byte(ComponentData id)
    {
        return id.ID;
    }

    public static implicit operator ComponentData(byte id)
    {
        return new ComponentData(id);
    }

    internal string Name
    {
        get
        {
            foreach ((Type key, ComponentData value) in ComponentsIds)
            {
                if (value == ID) return key.Name;
            }

            return "None";
        }
    }
}

