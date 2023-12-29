using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
static class EntityFactory
{
    /// <summary>
    /// Creates a parallax background entity.
    /// </summary>
    /// <param name="texture">The texture to use for the background.</param>
    /// <param name="velocity">The velocity of the background.</param>
    /// <returns>The parallax background entity.</returns>
    public static void CreateParallaxBackground(string texture, Vector2 velocity)
    {
        Entity background = new Entity();

        //Define the desired area of parallax
        int viewX = GameConstants.SCREEN_WIDTH;
        int viewY = GameConstants.SCREEN_HEIGHT;

        //Parallax
        Enum.TryParse(texture, out BackgroundTexture textureKey);
        background.Add(new ParallaxComponent(textureKey, velocity, Vector2.zero, viewX, viewY));
        MessageBus.Publish(new AddEntityMessage(background));
    }

    /// <summary>
    /// Creates a player entity.
    /// </summary>
    /// <param name="position">The initial position of the player.</param>
    /// <returns>The player entity.</returns>
    public static void CreatePlayer(Vector2 position)
    {
        //Empty Player
        Entity player = new Entity();
        player.Add(new EntityTypeComponent(EntityType.Player));

        // Animations
        AnimationComponent animation = new AnimationComponent();
        animation.AddAnimation(PlayerTexture.Idle, AnimationID.Idle, 1, 11, 20);
        animation.AddAnimation(PlayerTexture.Walking, AnimationID.Walk, 1, 12, 20);
        animation.AddAnimation(PlayerTexture.Jump, AnimationID.Jump, 1, 1, 20);
        animation.AddAnimation(PlayerTexture.DoubleJump, AnimationID.DoubleJump, 1, 6, 20);
        animation.AddAnimation(PlayerTexture.Fall, AnimationID.Fall, 1, 1, 20);
        animation.AddAnimation(PlayerTexture.Slide, AnimationID.Slide, 1, 5, 20);
        animation.AddAnimation(PlayerTexture.Hit, AnimationID.Death, 1, 7, 20);
        animation.AddAnimation(FruitTexture.Collected, AnimationID.Appear, 1, 6, 20);
        player.Add(animation);

        // States
        player.Add(new StateComponent());
        player.Add(new PlayerInputComponent());

        // Position and transforms
        player.Add(new MovementComponent(position));

        // Collisions
        player.Add(new CollisionBoxComponent(
                position: position,
                width: 32,
                height: 32,
                vertTopOffset: 8,
                vertBottomOffset: 0,
                horLeftOffset: 4,
                horRightOffset: 6));

        MessageBus.Publish(new AddEntityMessage(player));
    }

    public static void CreateFruit(Vector2 position, string texture, float respawnTime = 5)
    {
        // Create an empty coin entity
        Entity coin = new Entity();
        coin.Add(new EntityTypeComponent(EntityType.Coin));

        // Add animations for the fruit
        AnimationComponent animation = new AnimationComponent();
        Enum.TryParse(texture, out FruitTexture textureKey);
        animation.AddAnimation(textureKey, AnimationID.Idle, 1, 17, 20);
        animation.AddAnimation(FruitTexture.Collected, AnimationID.Death, 1, 6, 20);
        animation.AddAnimation(FruitTexture.Collected, AnimationID.Appear, 1, 6, 20);
        coin.Add(animation);

        // Add the current state and super state for the fruit
        coin.Add(new StateComponent(State.Idle, SuperState.IsOnGround));

        // Add movement component to set initial position
        coin.Add(new MovementComponent(position));

        // Add collision box component to handle collisions
        coin.Add(new CollisionBoxComponent(
                position: position,
                width: 32,
                height: 32,
                vertTopOffset: 7,
                vertBottomOffset: 11,
                horLeftOffset: 10,
                horRightOffset: 10));

        //Add respawn component
        coin.Add(new RespawnComponent(position, respawnTime));

        MessageBus.Publish(new AddEntityMessage(coin));
    }

    public static void CreateRegularEnemy(Vector2 position, float leftRange, float rightRange, State initialDirection, float respawnTime = 5)
    {
        // Create an empty enemy entity
        Entity enemy = new Entity();
        enemy.Add(new EntityTypeComponent(EntityType.RegularEnemy));

        // Add animations for the enemy
        AnimationComponent animation = new AnimationComponent(AnimationID.Walk);
        animation.AddAnimation(MaskedEnemyTexture.Walking, AnimationID.Walk, 1, 12, 20);
        animation.AddAnimation(MaskedEnemyTexture.Hit, AnimationID.Death, 1, 7, 20);
        animation.AddAnimation(FruitTexture.Collected, AnimationID.Appear, 1, 6, 20);
        enemy.Add(animation);

        // Add the current state and super state for the enemy
        enemy.Add(new StateComponent(initialDirection));
        enemy.Add(new RegularEnemyComponent(position.x, leftRange, rightRange));

        // Add movement component to set initial position
        enemy.Add(new MovementComponent(position));

        // Add collision box component to handle collisions
        enemy.Add(new CollisionBoxComponent(
                position: position,
                width: 32,
                height: 32,
                vertTopOffset: 8,
                vertBottomOffset: 0,
                horLeftOffset: 4,
                horRightOffset: 6));

        //Add respawn component
        enemy.Add(new RespawnComponent(position, respawnTime));
        MessageBus.Publish(new AddEntityMessage(enemy));
    }

    public static void CreateTimer(Vector2 position, int duration, bool isActive)
    {
        // Create an empty timer entity
        Entity timer = new Entity(isActive);
        MessageBus.Publish(new GameTimerMessage(timer, duration, position));
    }
}

