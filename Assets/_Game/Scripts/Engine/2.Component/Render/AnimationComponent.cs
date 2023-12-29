using System;
using UnityEngine;
using System.Collections.Generic;

public class AnimationComponent : IComponentData
{
    public int ID { get; set; }

    private Dictionary<AnimationID, ActionAnimation> _animations;
    private AnimationID _currentAction;
    private AnimationID _defaultAction;

    public AnimationComponent(AnimationID defaultAction = AnimationID.Idle)
    {
        _animations = new Dictionary<AnimationID, ActionAnimation>();
        _defaultAction = defaultAction;
        _currentAction = defaultAction;
    }

    public void AddAnimation(Texture2D spriteID, AnimationID action, int rows, int columns, float fps)
    {
        _animations[action] = new ActionAnimation(spriteID, rows, columns, fps);
    }


    public ActionAnimation GetCurrentAnimation()
    {

        if (!_animations.ContainsKey(_currentAction))
        {
            //Console.WriteLine($"Animation for action '{_currentAction}' does not exist, playing default animation"); // Debug message
            _currentAction = _defaultAction;
        }
        return _animations[_currentAction];
    }

    public void UpdateAnim()
    {
        ActionAnimation currentAnimation = GetCurrentAnimation();
        currentAnimation.UpdateAnim();
    }

    public void Draw(SpriteRenderer spriteBatch, Vector2 position, int direction = 1)
    {
        ActionAnimation currentAnimation = GetCurrentAnimation();
        currentAnimation.Draw(spriteBatch, position, direction);
    }

    public void SetCurrentAction(AnimationID action)
    {
        if (_currentAction != action)
        {
            if (GameConstants.AnimationDebugMessages)
            {
                Console.WriteLine($"Animation {_currentAction} changes to Animation {action}");
            }

            ResetCurrentAnimation();
            _currentAction = action;
        }
    }

    private void ResetCurrentAnimation()
    {
        ActionAnimation currentAnimation = GetCurrentAnimation();
        currentAnimation.Reset();
    }

}

/// <summary>
/// A helper class that represents an individual animation composed of multiple frames, each of which is a rectangle in a sprite sheet.
/// </summary>
public class ActionAnimation
{
    private Texture2D texture;
    private int rows;
    private int columns;
    private int currentFrame;
    private int totalFrames;
    private float frameTime;
    private float elapsedFrameTime;
    private Rect[] frames;

    public bool IsFinished { get { return currentFrame >= totalFrames - 1; } }

    public ActionAnimation(Texture2D spriteSheet, int rows, int columns, float framesPerSecond)
    {
        texture = spriteSheet;
        this.rows = rows;
        this.columns = columns;
        currentFrame = 0;
        totalFrames = rows * columns;
        frameTime = 1f / framesPerSecond;
        elapsedFrameTime = 0;

        frames = new Rect[totalFrames];
        float frameWidth = texture.width / columns;
        float frameHeight = texture.height / rows;
        for (int i = 0; i < totalFrames; i++)
        {
            int x = (i % columns) * (int)frameWidth;
            int y = (i / columns) * (int)frameHeight;
            frames[i] = new Rect(x, y, frameWidth, frameHeight);
        }
    }

    public void UpdateAnim()
    {
        if (frames.Length == 1)
        {
            return;
        }
        elapsedFrameTime += Time.deltaTime;
        if (elapsedFrameTime >= frameTime)
        {
            currentFrame++;
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
            elapsedFrameTime = 0;
        }
    }

    public void Draw(SpriteRenderer sprite, Vector2 position, int direction = 1)
    {
        bool isFacingLeft = direction == -1;
        Rect currentFrameRect = frames[currentFrame];

        sprite.sprite = Sprite.Create(texture,
            currentFrameRect, new Vector2(0.5f, 0.5f), // Sprite's pivot is at center by default
            100f); // Adjust the pixels per unit value as needed for your project

        sprite.transform.position = new Vector3(position.x, position.y, 0);
        sprite.flipX = isFacingLeft;
    }

    public void Reset()
    {
        currentFrame = 0;
        elapsedFrameTime = 0;
    }
}

