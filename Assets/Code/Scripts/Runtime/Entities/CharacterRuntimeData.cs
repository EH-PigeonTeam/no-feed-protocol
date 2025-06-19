using System;
using NoFeedProtocol.Persistence.Character;

namespace NoFeedProtocol.Runtime.Entities
{
    [Serializable]
    public class CharacterRuntimeData
    {
        public string Id;
        public int Health;
        public int Energy;

        public CharacterSaveData ToSaveData()
        {
            return new CharacterSaveData
            {
                Id = Id,
                Health = Health
            };
        }

        public static CharacterRuntimeData FromSaveData(CharacterSaveData save)
        {
            return new CharacterRuntimeData
            {
                Id = save.Id,
                Health = save.Health
            };
        }

        public bool IsAlive => Health > 0;
        public bool HasReadyToAttack(int energyCost) => Energy >= energyCost;
        public CharacterRuntimeData Clone() => new CharacterRuntimeData { Id = Id, Health = Health, Energy = Energy };
    }
}
