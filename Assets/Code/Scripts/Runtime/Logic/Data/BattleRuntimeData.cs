using System;
using NoFeedProtocol.Runtime.Entities;
using Code.Systems.Locator;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Logic.Data
{
    /// <summary>
    /// Temporary in-memory data used during a battle session.
    /// </summary>
    [Serializable]
    public class BattleRuntimeData
    {
        [ShowInInspector, ReadOnly]
        public PlayerRuntimeData PlayerTeam { get; private set; }

        [ShowInInspector, ReadOnly]
        public PlayerRuntimeData EnemyTeam { get; private set; }

        public void Set(PlayerRuntimeData player, PlayerRuntimeData enemy)
        {
            PlayerTeam = player;
            EnemyTeam = enemy;
        }

        public RunRuntimeData ToPersistentData()
        {
            var mapData = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Map;

            return new RunRuntimeData
            {
                Player = PlayerTeam,
                Map = mapData
            };
        }
    }
}
