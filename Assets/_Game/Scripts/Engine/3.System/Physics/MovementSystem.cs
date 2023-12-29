using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementSystem : ECSSystem
{
    private List<EntityData> entitiesData;

    public MovementSystem()
    {
        entitiesData = new List<EntityData>();
    }

    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        MovementComponent movement = entity.Get<MovementComponent>();

        if (state == null || movement == null)
            return;

        EntityData data = new EntityData()
        {
            entity = entity,
            state = state,
            movement = movement
        };
        entitiesData.Add(data);
    }

    public override void RemoveEntity(Entity entity)
    {
        var index = entitiesData.FindIndex(data => data.entity == entity);
        if(index != -1)
        {
            entitiesData.RemoveAt(index);
        }
    }

    public override void Subscribe()
    {
    }

    public override void TICK(float deltaTime)
    {
        foreach(EntityData data in entitiesData)
        {
            if(!data.entity.IsActive || data.state.CurrentSuperState == SuperState.IsAppearing)
            {
                continue;
            }

            UpdatePositionBaseOnState(deltaTime, data.state, data.movement);
            CollisionBoxComponent collisionBox = data.entity.Get<CollisionBoxComponent>();
            if(collisionBox != null)
            {
                collisionBox.UpdateBoxPosition(data.movement.Position.x, data.movement.Position.y, data.state.HorizontalDirection);
            }
        }
    }

    protected void UpdatePositionBaseOnState(float deltaTime, StateComponent state, MovementComponent movement)
    {
        Vertical(state, movement);
        Horizontal(state, movement);

        movement.Velocity += movement.Acceleration * deltaTime;

        if(state.CurrentSuperState != SuperState.IsOnGround)
        {
            movement.Velocity = new Vector2(Mathf.Clamp(movement.Velocity.x, -GameConstants.SpeedX, GameConstants.SpeedX), movement.Velocity.y);
        }
        movement.Position += movement.Velocity * deltaTime;
        if(movement.Position.y >= GameConstants.SCREEN_HEIGHT)
        {
            MessageBus.Publish(new ReloadLevelMessege());
        }
    }

    protected void Horizontal(StateComponent state, MovementComponent movement)
    {
        switch(state.CurrentState)
        {
            case State.WalkLeft:
                state.HorizontalDirection = -1;
                movement.Velocity += new Vector2(-GameConstants.SpeedX, 0);
                break;
            case State.WalkRight:
                state.HorizontalDirection = 1;
                movement.Velocity += new Vector2(GameConstants.SpeedX, 0);
                break;
        }
    }

    protected void Vertical(StateComponent state, MovementComponent movement)
    {
        // Vertical Movement
        switch (state.CurrentSuperState)
        {
            case SuperState.IsOnGround:
                movement.Acceleration = Vector2.zero;
                movement.Velocity = Vector2.zero;
                if (state.CurrentState == State.Jump)
                {
                    movement.Velocity = new Vector2(movement.Velocity.X, GameConstants.SpeedY);
                    state.CurrentSuperState = SuperState.IsJumping;
                }
                break;

            case SuperState.IsFalling:
                movement.Acceleration = new Vector2(0, GameConstants.GRAVITY);
                if (state.CurrentState == State.Slide)
                {
                    movement.Acceleration = new Vector2(0, GameConstants.GRAVITY / 10);
                }
                if (state.CurrentState == State.DoubleJump)
                {
                    movement.Velocity += new Vector2(0, GameConstants.SpeedY);
                    state.CurrentSuperState = SuperState.IsDoubleJumping;
                }
                break;

            case SuperState.IsDead:
                break;

            case SuperState.IsJumping:
            case SuperState.IsDoubleJumping:
                movement.Acceleration = new Vector2(0, GameConstants.GRAVITY);
                if (movement.Velocity.y > 0)
                {
                    state.CurrentSuperState = SuperState.IsFalling;
                }
                break;

            default:
                break;
        }

    }


    public override void Unsubscribe()
    {
    }
}

