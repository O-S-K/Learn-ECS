using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimerSystem : ECSSystem
{
    private Entity _entity;
    private float _timer;
    private Vector2 _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerSystem"/> class.
    /// </summary>
    public TimerSystem()
    {
        _entity = null;
        _timer = 10;
    }

    /// <summary>
    /// Removes an entity from the system.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public override void RemoveEntity(Entity entity)
    {
        if (_entity == entity)
        {
            _entity = null;
        }
    }

    /// <summary>
    /// Subscribes to the GameTimerMessage.
    /// </summary>
    public override void Subscribe()
    {
        MessageBus.Subscribe<GameTimerMessage>(TimerStarted);
    }

    /// <summary>
    /// Unsubscribes from the GameTimerMessage.
    /// </summary>
    public override void Unsubscribe()
    {
        MessageBus.Unsubscribe<GameTimerMessage>(TimerStarted);
    }

    /// <summary>
    /// Called when a timer starts, sets the entity and timer based on the message received.
    /// </summary>
    /// <param name="message">The GameTimerMessage that contains information about the timer.</param>
    public void TimerStarted(GameTimerMessage message)
    {
        _entity = message.Entity;
        _timer = message.Timer;
        _position = message.Position;
    }

    public override void TICK(float deltaTime)
    {
        if (_entity == null || !_entity.IsActive)
        {
            return;
        }

        _timer -= deltaTime;
        if (_timer <= 0)
        {
            // Perform action when timer reaches zero
            _timer = 0;

            MessageBus.Publish(new NextLevelMessage());
        }
    }

    /// <summary>
    /// Draw method to display the timer on screen.
    /// </summary>
    public override void Draw(SpriteRenderer spriteBatch)
    {
        if (_entity != null && _entity.IsActive)
        {
            var font = Loader.GetFont("GameFont");
            int minutes = (int)_timer / 60;
            int seconds = (int)_timer % 60;
            string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);

            spriteBatch.DrawString(font, timerText, _position, Color.black);
        }
    }
}

