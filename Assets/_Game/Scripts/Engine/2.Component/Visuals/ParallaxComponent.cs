using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class ParallaxComponent : IComponentData
{
    public int ID { get; set; }

    private Texture2D _texture;
    private Vector2 _velocity;
    private Vector2 _position;
    private Vector2 _position2;

    private int _tileX;
    private int _tileY;

    /// <summary>
    /// Initializes a new instance of the ParallaxComponent class.
    /// </summary>
    /// <param name="sprite">The filename of the sprite to use.</param>
    /// <param name="velocity">The velocity of the parallax effect.</param>
    /// <param name="position">The starting position of the sprite.</param>
    /// <param name="viewX">The width of the parallax window.</param>
    /// <param name="viewY">The height of the parallax window.</param>
    public ParallaxComponent(Texture2D sprite, Vector2 velocity, Vector2 position, int viewX, int viewY)
    {
        _texture = Loader.GetTexture(sprite);
        // Check if the texture is null.
        if (_texture == null)
        {
            return;
        }

        _velocity = velocity;
        _position = position;
        _position2 = position;

        _tileX = (int)Math.Ceiling((float)viewX / _texture.width) + 1;
        _tileY = (int)Math.Ceiling((float)viewY / _texture.height) + 1;

        _position2 = position;

        if (velocity != Vector2.zero)
        {
            _position2.x += Math.Sign(velocity.x) * _texture.width;
            _position2.y -= Math.Sign(velocity.y) * _texture.height;
        }
    }

    /// <summary>
    /// Updates the position of the sprite based on the elapsed time and velocity. Also loops the sprite horizontally and vertically if necessary.
    /// </summary>
    /// <param name="gameTime">A snapshot of the current game time.</param>
    public void TICK(float gameTime)
    {
        float elapsedSeconds = gameTime;
        Vector2 displacement = _velocity * elapsedSeconds;

        _position += displacement;
        _position2 += displacement;

        // Loop the parallax background horizontally
        if (_velocity.x != 0)
        {
            if (_position.x <= -_texture.width)
            {
                _position.x = _position2.x+ _texture.width;
            }
            if (_position2.x <= -_texture.width)
            {
                _position2.x = _position.x + _texture.width;
            }
            if (_position.x >= _texture.width)
            {
                _position.x = _position2.x - _texture.width;
            }
            if (_position2.x >= _texture.width)
            {
                _position2.x = _position.x - _texture.width;
            }
        }

        // Loop the parallax background vertically
        if (_velocity.y != 0)
        {
            if (_position.y >= _texture.height)
            {             
                _position.y = _position2.y - _texture.height;
            }            
            if (_position2.y>= _texture.height)
            {
                _position2.y = _position.y - _texture.height;
            }
            if (_position.y <= -_texture.height)
            {
                _position.y = _position2.y + _texture.height;
            }
            if (_position2.y <= -_texture.height)
            {
                _position2.y = _position.y + _texture.height;
            }
        }

    }

    /// <summary>
    /// Draws the sprite with the parallax effect.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch object to use for drawing.</param>
    public  void Draw(Texture2D spriteBatch)
    {
        for (int x = -1; x < _tileX; x++)
        {
            for (int y = -1; y < _tileY; y++)
            {
                Vector2 texturePosition = new Vector2(x * spriteBatch.width, y * spriteBatch.height);

                // Create a new GameObject to render the texture
                GameObject textureObject = new GameObject("TextureObject");
                SpriteRenderer spriteRenderer = textureObject.AddComponent<SpriteRenderer>();

                // Set the position and texture of the SpriteRenderer
                textureObject.transform.position = new Vector3(_position.x + texturePosition.x, _position.y + texturePosition.y, 0);
                spriteRenderer.sprite = Sprite.Create(spriteBatch, new Rect(0, 0, spriteBatch.width, spriteBatch.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}

