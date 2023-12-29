using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerInputComponent : IComponentData
{
    public int ID { get; set; }

    public bool IsLeftKeyDown { get; private set; }
    public bool IsRightKeyDown { get; private set; }
    public bool IsJumpKeyDown { get; private set; }

    public void UpdateInput()
    {
        IsLeftKeyDown = Input.GetKey(KeyCode.A);
        IsRightKeyDown = Input.GetKey(KeyCode.D);
        IsJumpKeyDown = Input.GetKey(KeyCode.Space);
    }
}

