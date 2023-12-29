public enum State
{
    Idle,
    WalkLeft,
    WalkRight,
    Jump,
    DoubleJump,
    Slide,
}

public enum SuperState
{
    IsOnGround,
    IsFalling,
    IsJumping,
    IsDoubleJumping,
    IsDead,
    IsSliding,
    IsAppearing,
}

public enum AnimationID
{
    Idle,
    Walk,
    Jump,
    DoubleJump,
    Slide,
    Fall,
    Death,
    Appear,
}


