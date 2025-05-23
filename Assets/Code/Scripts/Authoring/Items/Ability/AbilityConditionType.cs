#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    public enum AbilityConditionType
    {
        OwnsItem,
        TurnsPassed,
        OnDeathCharacter,
        OnDeathEnemy,
        OnHpBelowCharacter,
        OnHpBelowEnemy,
        OnShieldBelowCharacters,
        OnShieldBelowEnemies,
        WheelSpin,
        WheelResultsProvided,
        WheelLocked,
        WheelRotationsRemaining,
        WheelPointsProvided,
        OnActCharacter,
        OnActEnemy,
        ConsumableUsed,
        OnGain,
        OnLoss
    }
}
