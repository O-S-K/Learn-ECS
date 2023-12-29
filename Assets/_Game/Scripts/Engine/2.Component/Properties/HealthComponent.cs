using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthComponent : IComponentData
{
    public int ID { get; set; }

    private int _currentHealth;
    private int _maxHealth;

    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }


    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = Mathf.Max(0, value);
    }

    public HealthComponent(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
}

