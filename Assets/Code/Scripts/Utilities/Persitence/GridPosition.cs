using System;

[Serializable]
public struct GridPosition
{
    public int X;
    public int Y;

    public GridPosition(int x, int y)
    {
        X = x;
        Y = y;
    }
}

[Serializable]
public struct OptionalGridPosition
{
    public bool HasValue;
    public GridPosition Value;

    public OptionalGridPosition(GridPosition? position)
    {
        HasValue = position.HasValue;
        Value = position.GetValueOrDefault(); // Safe even if null
    }

    public GridPosition? ToNullable => HasValue ? Value : null;

    public void Set(GridPosition? position)
    {
        HasValue = position.HasValue;
        Value = position.GetValueOrDefault();
    }
}
