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
                Health = Health,
                Energy = Energy
            };
        }

        public static CharacterRuntimeData FromSaveData(CharacterSaveData save)
        {
            return new CharacterRuntimeData
            {
                Id = save.Id,
                Health = save.Health,
                Energy = save.Energy
            };
        }
    }
}
