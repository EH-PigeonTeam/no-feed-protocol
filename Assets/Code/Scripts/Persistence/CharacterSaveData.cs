using System;

namespace NoFeedProtocol.Persistence.Character
{
    [Serializable]
    public class CharacterSaveData
    {
        public string Id;
        public int Health;
        public int Energy;
    }
}
