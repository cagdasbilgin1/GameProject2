using UnityEngine;

public interface ICollectable
{
    CollectableType Type { get; }
    int Point { get; }
    AudioClip CollectSound { get; }
    void SetPassive();
}

public enum CollectableType
{
    None,
    Star,
    Coin,
    Gem,
}
