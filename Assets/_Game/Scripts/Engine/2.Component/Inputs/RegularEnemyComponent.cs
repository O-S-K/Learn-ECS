using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegularEnemyComponent : IComponentData
{
    public int ID { get; set; }

    public bool IsLeft { get; private set; }
    public bool IsRight { get; private set; }

    private float _left;
    private float _right;

    private float leftLimit;
    private float rightLimit;

    public RegularEnemyComponent(float start, float leftRange, float rightRange)
    {
        _left = start - leftRange;
        _right = start + rightRange;
    }


    public void UpdateMovement(float positionX)
    {
        IsLeft = false;
        IsRight = false;

        if (positionX <= leftLimit)
        {
            IsRight = true;
        }
        else if (positionX >= rightLimit)
        {
            IsLeft = true;
        }
    }
}

