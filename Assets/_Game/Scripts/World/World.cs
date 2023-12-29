using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering.VirtualTexturing;

public class World
{
    public LevelID CurrentLevelID
    {
        get => _currentLevel;
    }
    private LevelID _currentLevel;

    private SystemManager _systemManager;
    private int _totalLevels = Enum.GetValues(typeof(LevelID)).Length;

    private Queue<Entity> _entitiesToAdd;
    private Queue<Entity> _entitiesToDestroy;

    private bool _levelNeedsChanging;

    public World()
    {
        _systemManager = new SystemManager(_currentLevel);
        _entitiesToAdd = new Queue<Entity>();
        _entitiesToDestroy = new Queue<Entity>();

        MessageBus.Subscribe<AddEntityMessage>();
        MessageBus.Subscribe<AddEntityMessage>();
        MessageBus.Subscribe<AddEntityMessage>();
        MessageBus.Subscribe<AddEntityMessage>();
        MessageBus.Subscribe<AddEntityMessage>();

        ChangeLevel(LevelID.Level1);
    }

    private void ChangeLevel(LevelID level)
    {
        _currentLevel = level;
        _levelNeedsChanging = true;
    }

    private void LoadLevel()
    {
        MediaPlayer.Stop();
        _systemManager.Unsubscribe();
        _systemManager.ResetSystems(CurrentLevelID);
        _systemManager.Subscribe();

        LevelLoader.GetObjects(CurrentLevelID);
        Loader.PlayMusic(BackgroundMusic.Default, true);

    }
    
    public void ResetCurrentLevel(ReloadLevelMessage levelMessage = null)
    {
        _levelNeedsChanging = true;
    }

    public void NextLevel(ReloadLevelMessage levelMessage = null)
    {
        _currentLevel = (LevelID)(((int)_currentLevel + 1 + _totalLevels) % _totalLevels);
        _levelNeedsChanging = true;
    }

    public void PreviouseLevel(ReloadLevelMessage levelMessage = null)
    {
        _currentLevel = (LevelID)(((int)_currentLevel - 1 + _totalLevels) % _totalLevels);
        _levelNeedsChanging = true;
    }

    private void OnCreateEntity(AddEntityMessage message) // Add this method
    {
        _entitiesToAdd.Enqueue(message.Entity);
    }

    private void OnDestroyEntity(DestroyEntityMessage message)
    {
        _entitiesToDestroy.Enqueue(message.Entity);
    }

    public void Tick(float deltaTime)
    {
        if (_levelNeedsChanging)
        {
            LoadLevel();
            _levelNeedsChanging = false;
        }

        while (_entitiesToAdd.Count > 0)
        {
            Entity entity = _entitiesToAdd.Dequeue();
            _systemManager.Add(entity);
        }

        _systemManager.Tick(deltaTime);

        while (_entitiesToDestroy.Count > 0)
        {
            Entity entity = _entitiesToDestroy.Dequeue();
            _systemManager.Remove(entity);
        }
    }

    public void Draw(SpriteRenderer spriteBatch)
    {
        _systemManager.Draw(spriteBatch);
    }

}

