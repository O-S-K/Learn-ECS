using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Entity
{
    /// <summary>
    /// Stores a dictionary where the key is the Type of the Component and the value is the Component instance.
    /// </summary>
    private Dictionary<Type, IComponentData> _components;

    /// <summary>
    /// Tells if the Entity is active.
    /// </summary>
    public bool IsActive;

    /// <summary>
    /// Initializes a new instance of the Entity class.
    /// </summary>
    /// <param name="isActive">Determines if the entity is active or not. Default is true.</param>
    public Entity(bool isActive = true)
    {
        _components = new Dictionary<Type, IComponentData>();
        IsActive = isActive;
    }

    /// <summary>
    /// Adds a component to the entity.
    /// </summary>
    /// <param name="component">The component to add.</param>
    public void Add(IComponentData component)
    {
        Type type = component.GetType();
        if (!_components.ContainsKey(type))
        {
            _components.Add(type, component);
        }
        else 
        {
            Debug.Log($"Component of type {type} already exists!");
        }
    }

    /// <summary>
    /// Removes a component from the entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    public void Remove<T>() where T : IComponentData
    {
        Type type = typeof(T);
        if (_components.ContainsKey(type))
        {
            _components.Remove(type);
        }
        else  
        {
            Debug.Log("Tried to remove a component that doesn't exist!");
        }
    }

    /// <summary>
    /// Gets a component of a specified type from the entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to get.</typeparam>
    /// <returns>The component of the specified type, or null if it doesn't exist.</returns>
    public T Get<T>() where T : IComponentData
    {
        Type type = typeof(T);
        if (_components.TryGetValue(type, out IComponentData component) && component is T tComponent)
        {
            return tComponent;
        }
        else 
        {
            Debug.Log("Tried to get a component that doesn't exist!");
            return default(T);
        }
    }

    /// <summary>
    /// Gets all the components of the entity.
    /// </summary>
    /// <returns>A list of all the components of the entity.</returns>
    public List<IComponentData> GetAll()
    {
        List<IComponentData> componentList = new List<IComponentData>(_components.Values);
        return componentList;
    }

}

