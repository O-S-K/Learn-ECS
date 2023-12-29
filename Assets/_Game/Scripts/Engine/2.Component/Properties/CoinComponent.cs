using UnityEngine;
using System.Collections;

public class CoinComponent : IComponentData
{
    public int ID { get; set; }

    public int Coins
    {
        get => _coins;
        set => _coins = value;
    }

    private int _coins;

    public CoinComponent(int initialCoins = 0)
    {
        _coins = initialCoins;
    }

    public void AddCoins(int value)
    {
        _coins += value;
    }
}
