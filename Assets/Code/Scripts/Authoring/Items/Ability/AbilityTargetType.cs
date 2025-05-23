#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    public enum AbilityTargetType
    {
        Self,
        Ally,
        Enemy,
        AllAllies,
        AllEnemies,
        AllCharacters,
        SpecificCharacter,
        SpecificEnemy,
        SlotMachine
    }
}
