public interface ICollectable
{
    CollectableType Type { get; }
    int Point { get; }
    void SetPassive();
}

public enum CollectableType
{
    None,
    Star,
    Coin,
    Gem,
}
