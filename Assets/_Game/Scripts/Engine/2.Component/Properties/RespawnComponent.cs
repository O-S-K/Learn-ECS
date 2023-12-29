using UnityEngine;
using System;

public class RespawnComponent : IComponentData
{
    public int ID { get; set; }

    private readonly float _respawnTimer;
    private float _currentTimer;
    private bool _isRespawning;

    public bool IsRespawning => _isRespawning;
    public Vector2 Position { get; private set; }

    public RespawnComponent(Vector2 respawnPosition, float respawnTimer = 5)
    {
        _respawnTimer = respawnTimer;
        _currentTimer = 0f;
        _isRespawning = false;
        Position = respawnPosition;
    }

    public void StartRespawn()
    {
        _currentTimer = 0f;
        _isRespawning = true;
    }
 

    public void UpdateRespawn()
    {
        if (_isRespawning)
        {
            _currentTimer += Time.deltaTime;

            if (_currentTimer >= _respawnTimer)
            {
                // Respawned
                _isRespawning = false;
            }
        }
    }
}

