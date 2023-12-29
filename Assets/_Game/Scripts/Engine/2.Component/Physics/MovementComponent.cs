using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementComponent : IComponentData
{
    public int ID { get; set; }

    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 _acceleration;

    public Vector2 LastPosition
    {
        get;
        private set;
    }

    public Vector2 Position
    {
        get => _position;
        set
        {
            LastPosition = _position;
            _position = value;
        }
    }

    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    public Vector2 Acceleration { get => _acceleration; set => _acceleration = value; }

    public MovementComponent(Vector2 initialPosition)
    {
        _position = initialPosition;
        _velocity = Vector2.zero;
        _acceleration = Vector2.zero;
    }
}

