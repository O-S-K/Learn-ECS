using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class ObstacleCollisionSystem : ECSSystem
{
    private List<EntityData> entityDatas;
    private Dictionary<string, List<Rectangle>> obstacles;


    public ObstacleCollisionSystem(LevelID levelID)
    {
        entityDatas = new List<EntityData>();
        obstacles = new Dictionary<string, List<Rectangle>>();
        obstacles = Loader.tileHandler.objects[levelID.ToString()];
    }


    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        MovementComponent movement = entity.Get<MovementComponent>();
        CollisionBoxComponent collisionBox = entity.Get<CollisionBoxComponent>();

        if (state == null || movement == null || collisionBox == null)
        {
            return;
        }

        EntityData entityData = new EntityData()
        {
            entity = entity,
            movement = movement,
            collisionBox = collisionBox
        };
        entityDatas.Add(entityData);
    }

    public override void RemoveEntity(Entity entity)
    {
        entityDatas.RemoveAll(data => data.entity == entity);
    }

    public override void Subscribe()
    {
    }

    public override void TICK(float deltaTime)
    {
        foreach (EntityData data in entityDatas)
        {
            if (!data.entity.IsActive || data.state.CurrentSuperState == SuperState.IsAppearing || data.state.CurrentSuperState == SuperState.IsDead)
            {
                continue;
            }

            //Should not be needed if collision box was updated after any movements beforehand
            //data.CollisionBox.UpdateBoxPosition(data.Movement.Position.X, data.Movement.Position.Y, data.State.HorizontalDirection);

            Rectangle box = data.collisionBox.GetRectangle();
            float positionX = data.movement.Position.x;
            float positionY = data.movement.Position.y;
            data.state.CanMoveRight = true;
            data.state.CanMoveLeft = true;

            foreach (string key in obstacles.Keys)
            {
                foreach (Rectangle rect in obstacles[key])
                {
                    if (!box.Intersects(rect))
                    {
                        continue;
                    }

                    switch (data.state.CurrentSuperState)
                    {
                        case SuperState.IsFalling:
                            HandleFallCollision(data, box, rect, ref positionX, ref positionY, key);
                            break;

                        case SuperState.IsOnGround:
                            HandleGroundCollision(data, box, rect, ref positionX);
                            break;

                        case SuperState.IsJumping:
                        case SuperState.IsDoubleJumping:
                            if (key == "float")
                            {
                                break;
                            }

                            HandleJumpCollision(data, box, rect, ref positionX, ref positionY);
                            break;

                        default:
                            break;
                    }
                    //Console.WriteLine($"Obstacle: {rect.ToString()}, StateID: {data.State.stateID}"); //Debug Message
                }
            }

            //Update entity's position and collisionBox
            data.movement.Position = new Vector2(positionX, positionY);
            data.collisionBox.UpdateBoxPosition(data.movement.Position.x, data.movement.Position.y, data.state.HorizontalDirection);

            //Check if entity is not on platform anymore
            if (data.collisionBox.CheckIfInAir(data.movement.Position.x, data.state.HorizontalDirection))
            {
                if (data.state.CurrentSuperState == SuperState.IsOnGround)
                {
                    data.state.CurrentSuperState = SuperState.IsFalling;
                }
            }
            //Check if entity is not sliding anymore
            if (data.collisionBox.CheckIfBelow(data.movement.Position.y))
            {
                if (data.state.CurrentState == State.Slide)
                {
                    data.state.CurrentState = State.Idle;
                }
            }
        }
    }

    /// <summary>
    /// Handles collision when the entity is in a falling state.
    /// </summary>
    /// <param name="data">The EntityData containing the entity's components.</param>
    /// <param name="box">The entity's collision box.</param>
    /// <param name="rect">The obstacle's rectangle.</param>
    /// <param name="positionX">The entity's current X position.</param>
    /// <param name="positionY">The entity's current Y position.</param>
    private void HandleFallCollision(EntityData data, Rectangle box, Rectangle rect, ref float positionX, ref float positionY, string key)
    {
        data.state.CurrentState = State.Idle;
        bool wasAbove = data.movement.LastPosition.y + data.collisionBox.OriginalHeight - data.collisionBox.VertBottomOffset <= rect.Top + 1;

        if (wasAbove)
        {
            data.state.CurrentSuperState = SuperState.IsOnGround;
            positionY = rect.Top - data.collisionBox.OriginalHeight + data.collisionBox.VertBottomOffset - 0.1f;
            data.collisionBox.SetGroundLocation(rect.Left, rect.Right);
        }
        else if (key != "float")
        {
            HandleHorizontalInAirCollision(data, box, rect, ref positionX);
        }
    }

    /// <summary>
    /// Handles collision when the entity is in a jumping state.
    /// </summary>
    /// <param name="data">The EntityData containing the entity's components.</param>
    /// <param name="box">The entity's collision box.</param>
    /// <param name="rect">The obstacle's rectangle.</param>
    /// <param name="positionX">The entity's current X position.</param>
    /// <param name="positionY">The entity's current Y position.</param>
    /// <param name="key">The key indicating the current layer being checked.</param>
    private void HandleJumpCollision(EntityData data, Rectangle box, Rectangle rect, ref float positionX, ref float positionY)
    {
        bool wasBelow = data.movement.LastPosition.y + data.collisionBox.VertTopOffset >= rect.Bottom - 1;
        if (wasBelow)
        {
            positionY = rect.Bottom - data.collisionBox.VertTopOffset + 0.1f;
            data.state.CurrentSuperState = SuperState.IsFalling;
            data.movement.Velocity = Vector2.zero;
        }
        else
        {
            HandleHorizontalInAirCollision(data, box, rect, ref positionX);
        }
    }

    /// <summary>
    /// Handles collision when the entity is on the ground.
    /// </summary>
    /// <param name="data">The EntityData containing the entity's components.</param>
    /// <param name="box">The entity's collision box.</param>
    /// <param name="rect">The obstacle's rectangle.</param>
    /// <param name="positionX">The entity's current X position.</param>
    private void HandleGroundCollision(EntityData data, Rectangle box, Rectangle rect, ref float positionX)
    {
        if (data.state.CurrentState == State.Slide)
        {
            data.state.CurrentState = State.Idle;
        }

        if (data.movement.Velocity.x > 0 && box.Left <= rect.Left)
        {
            positionX = rect.Left - data.collisionBox.OriginalWidth + data.collisionBox.HorRightOffset - 0.1f;
            data.state.CanMoveRight = false;
        }
        if (data.movement.Velocity.x < 0 && box.Right >= rect.Right)
        {
            positionX = rect.Right - data.collisionBox.HorRightOffset + 0.1f;
            data.state.CanMoveLeft = false;
        }
    }

    /// <summary>
    /// Handles horizontal collision when the entity is in air (jumping or falling).
    /// </summary>
    /// <param name="data">The EntityData containing the entity's components.</param>
    /// <param name="box">The entity's collision box.</param>
    /// <param name="rect">The obstacle's rectangle.</param>
    /// <param name="positionX">The entity's current X position.</param>
    private void HandleHorizontalInAirCollision(EntityData data, Rectangle box, Rectangle rect, ref float positionX)
    {
        if (data.movement.Velocity.x > 0 && box.Left <= rect.Left)
        {
            positionX = rect.Left - data.collisionBox.OriginalWidth + data.collisionBox.HorRightOffset;
            data.state.CanMoveRight = false;
            data.collisionBox.SetSlidingLocation(rect.Bottom);
            data.state.CurrentSuperState = SuperState.IsFalling;
            data.movement.Velocity = Vector2.zero;
        }
        if (data.movement.Velocity.x < 0 && box.Right >= rect.Right)
        {
            positionX = rect.Right - data.collisionBox.HorRightOffset;
            data.state.CanMoveLeft = false;
            data.collisionBox.SetSlidingLocation(rect.Bottom);
            data.state.CurrentSuperState = SuperState.IsFalling;
            data.movement.Velocity = Vector2.zero;
        }

    }

    public override void Unsubscribe()
    {
    }
}

