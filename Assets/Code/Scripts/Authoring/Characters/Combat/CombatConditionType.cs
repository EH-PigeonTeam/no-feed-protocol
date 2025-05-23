using System;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
    [Flags]
    public enum CombatConditionType
    {
        Always = 0,
        WithShield = 1 << 0,
        WithOutShield = 1 << 1,
        HasEnemyAtLeast = 1 << 2,
        HasAllyAtLeast = 1 << 3,
        SelfHpBelow = 1 << 4
    }
}
