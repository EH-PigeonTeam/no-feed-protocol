using UnityEngine;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    public class EncounterCompleted : MonoBehaviour
    {
        public void LevelCompleted()
        {
            ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Map.LastNodeCompleted = true;
        }
    }
}
