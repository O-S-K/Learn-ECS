using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class PlayerEntityCollisionsSystem : ECSSystem
{
    private List<EntityData> entityDatas;
    private EntityData playerData;

    public PlayerEntityCollisionsSystem()
    {
        entityDatas = new List<EntityData>();
        playerData = new EntityData();
    }

    public override void AddEntity(Entity entity)
    {
        StateComponent state = entity.Get<StateComponent>();
        CollisionBoxComponent collisionBox = entity.Get<CollisionBoxComponent>();
        MovementComponent movement = entity.Get<MovementComponent>();

        if (state == null || collisionBox == null || movement == null)
        {
            return;
        }

        var data = new EntityData
        {
            entity = entity,
            state = state,
            collisionBox = collisionBox,
            movement = movement,
        };

        if (entity.Get<EntityTypeComponent>().Type == EntityType.Player)
        {
            playerData = data;
        }
        else
        {
            entityDatas.Add(data);
        }
    }

    public override void RemoveEntity(Entity entity)
    {
        if (entity.Get<EntityTypeComponent>().Type == EntityType.Player)
        {
            playerData = new EntityData();
        }
        else
        {
            entityDatas.RemoveAll(data => data.entity == entity);
        }
    }

    public override void Subscribe()
    {
    }

    public override void TICK(float deltaTime)
    {
        foreach (EntityData data in entityDatas)
        {
            if (playerData.state.CurrentSuperState == SuperState.IsDead || playerData.state.CurrentSuperState == SuperState.IsAppearing || !playerData.Entity.IsActive)
            {
                return;
            }
            if (!data.entity.IsActive || data.state.CurrentSuperState == SuperState.IsAppearing || data.state.CurrentSuperState == SuperState.IsDead)
            {
                continue;
            }

            //Update Collision Boxes, should have done at any system manipulating movements before
            /*
            _playerData.CollisionBox.UpdateBoxPosition(_playerData.Movement.Position.X, _playerData.Movement.Position.Y, _playerData.State.HorizontalDirection);
            data.CollisionBox.UpdateBoxPosition(data.Movement.Position.X, data.Movement.Position.Y, data.State.HorizontalDirection);
            */

            // Check if the two entities are colliding
            if (playerData.collisionBox.GetRectangle().Intersects(data.collisionBox.GetRectangle()))
            {
                // Check the entity types to determine the appropriate collision resolution
                EntityTypeComponent entityType = data.entity.Get<EntityTypeComponent>();
                switch (entityType.Type)
                {
                    case EntityType.Coin:
                        ResolveCoinCollision(playerData, data);
                        break;
                    case EntityType.RegularEnemy:
                        ResolveRegularEnemyCollision(playerData, data);
                        break;
                    case EntityType.PortalToNextLevel:
                        ResolveNextLevelCollision(playerData, data);
                        break;
                    //Add more cases for new entity types as needed
                    default:
                        break;
                }

                //Update Collision Boxes
                playerData.collisionBox.UpdateBoxPosition(playerData.movement.Position.x, playerData.movement.Position.y, playerData.state.HorizontalDirection);
                data.collisionBox.UpdateBoxPosition(data.movement.Position.x, data.movement.Position.y, data.state.HorizontalDirection);
            }
        }
    }

    /// <summary>
    /// Resolves collisions between the player and a coin object.
    /// </summary>
    /// <param name="playerData">The data for the player entity.</param>
    /// <param name="coinData">The data for the coin entity.</param>
    private void ResolveCoinCollision(EntityData playerData, EntityData coinData)
    {
        // Mark the coin as dead
        coinData.state.CurrentSuperState = SuperState.IsDead;
        MessageBus.Publish(new EntityDiedMessage(coinData.entity));
    }

    /// <summary>
    /// Resolves collisions between the player and a portal object that leads to the next level.
    /// </summary>
    /// <param name="playerData">The data for the player entity.</param>
    /// <param name="portalData">The data for the portal entity.</param>
    private void ResolveNextLevelCollision(EntityData playerData, EntityData portalData)
    {
        // Publish a message indicating that the player has reached the next level
        MessageBus.Publish(new NextLevelMessage());
    }

    /// <summary>
    /// Resolves collisions between the player and a regular enemy that is walking on the ground.
    /// </summary>
    /// <param name="player">The data for the player entity.</param>
    /// <param name="enemy">The data for the enemy entity.</param>
    private void ResolveRegularEnemyCollision(EntityData player, EntityData enemy)
    {

        // If the player is falling, kill the enemy and set its state to idle
        switch (player.state.CurrentSuperState)
        {
            case SuperState.IsFalling:
                int direction = player.state.HorizontalDirection;
                float positionX = player.movement.Position.x;
                float positionY = player.movement.Position.y;

                bool properHit = HandleFallCollision(player, player.collisionBox.GetRectangle(), enemy.collisionBox.GetRectangle(), ref positionX, ref positionY);
                if (!properHit)
                {
                    break;
                }

                enemy.movement.Velocity = new Vector2(0, -20);
                enemy.movement.Acceleration = new Vector2(0, 100);
                enemy.state.CurrentSuperState = SuperState.IsDead;
                enemy.state.CurrentState = State.Idle;

                player.movement.Position = new Vector2(positionX, positionY);
                player.movement.Velocity = new Vector2(GameConstants.SpeedXonCollision * direction, player.movement.Velocity.y - GameConstants.SpeedYonCollision);
                player.state.CurrentSuperState = SuperState.IsJumping;
                player.state.CurrentState = State.Idle;
                player.state.JumpsPerformed = 1;

                MessageBus.Publish(new EntityDiedMessage(enemy.entity));
                break;

            default:
                player.state.CurrentSuperState = SuperState.IsDead;
                player.state.CurrentState = State.Idle;
                player.movement.Velocity = new Vector2(0, GameConstants.SpeedY / 2);
                player.movement.Acceleration = new Vector2(0, GameConstants.GRAVITY / 2);

                MessageBus.Publish(new EntityDiedMessage(player.entity));
                break;
        }
    }

    //Helper Methods

    /// <summary>
    /// Handles collision when the entity is in a falling state.
    /// </summary>
    /// <param name="data">The EntityData containing the entity's components.</param>
    /// <param name="box">The entity's collision box.</param>
    /// <param name="rect">The obstacle's rectangle.</param>
    /// <param name="positionX">The entity's current X position.</param>
    /// <param name="positionY">The entity's current Y position.</param>
    /// <returns>Returns true if the entity collided with the top side of the obstacle.</returns>
    private bool HandleFallCollision(EntityData data, Rectangle box, Rectangle rect, ref float positionX, ref float positionY)
    {
        bool wasAbove = data.movement.LastPosition.y + data.collisionBox.OriginalHeight - data.collisionBox.VertBottomOffset <= rect.Top + 1;
        if (!wasAbove)
        {
            return false;
        }

        positionY = rect.Top - data.collisionBox.OriginalHeight + data.collisionBox.VertBottomOffset - 0.1f;
        data.movement.Velocity = Vector2.zero;
        data.movement.Acceleration = Vector2.zero;
        return true;
    }

    public override void Unsubscribe()
    {
    }
}

