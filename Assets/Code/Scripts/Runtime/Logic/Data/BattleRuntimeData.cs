using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Copies current persistent data into a mutable battle-safe container.
        /// </summary>
        public void InitializeFromRuntime()
        {
            var store = ServiceLocator.Get<RuntimeDataStore>();
            var runtime = store.GameData?.Run;

            if (runtime == null)
            {
                UnityEngine.Debug.LogError("BattleRuntimeData: No run data found.");
                return;
            }

            PlayerTeam = ClonePlayerData(runtime.Player);
            EnemyTeam = CreateEnemyData(); // For now, a placeholder
        }

        /// <summary>
        /// Clones the player team data to make it writable during battle.
        /// </summary>
        private PlayerRuntimeData ClonePlayerData(PlayerRuntimeData original)
        {
            return new PlayerRuntimeData
            {
                CharacterTop = new CharacterRuntimeData
                {
                    Id = original.CharacterTop.Id,
                    Health = original.CharacterTop.Health,
                    Energy = original.CharacterTop.Energy
                },
                CharacterBottom = new CharacterRuntimeData
                {
                    Id = original.CharacterBottom.Id,
                    Health = original.CharacterBottom.Health,
                    Energy = original.CharacterBottom.Energy
                },
                CurrentShield = original.CurrentShield,
                Coins = original.Coins,
                Items = new List<string>(original.Items)
            };
        }

        /// <summary>
        /// Creates placeholder enemy team data.
        /// This will be replaced by actual enemy loading logic later.
        /// </summary>
        private PlayerRuntimeData CreateEnemyData()
        {
            return new PlayerRuntimeData
            {
                CharacterTop = new CharacterRuntimeData { Id = "enemy_top", Health = 20, Energy = 0 },
                CharacterBottom = new CharacterRuntimeData { Id = "enemy_bottom", Health = 20, Energy = 0 },
                CurrentShield = 10,
                Coins = 0,
                Items = new List<string>() // Filled later based on enemy type
            };
        }

        /// <summary>
        /// Merges modified data back into the persistent run data.
        /// </summary>
        public RunRuntimeData ToPersistentData()
        {
            MapRuntimeData MapData = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Map;

            return new RunRuntimeData
            {
                Player = PlayerTeam,
                Map = MapData
            };
        }
    }
}
