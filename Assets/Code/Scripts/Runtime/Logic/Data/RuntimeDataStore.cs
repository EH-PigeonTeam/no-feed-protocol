using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using PsychoGarden.Systems.Save;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Persistence.Game;

namespace NoFeedProtocol.Runtime.Logic.Data
{
    [HideMonoScript]
    public class RuntimeDataStore : MonoBehaviour
    {
        [SerializeField]
        private GameRuntimeData m_gameData;

        private const string SaveFileName = "game_data";

        public GameRuntimeData GameData
        {
            get => m_gameData;
            set => m_gameData = value;
        }

        #region Unity Lifecycle ----------------------------------------------

        private void OnEnable()
        {
            ServiceLocator.Register<RuntimeDataStore>(this);
            Load();
        }

        private void OnDisable()
        {
            Save();
            ServiceLocator.Unregister<RuntimeDataStore>();
        }

        #endregion

        #region Save / Load --------------------------------------------------

        public void Save()
        {
            if (m_gameData == null)
            {
                Debug.LogError("RuntimeDataStore: Cannot save, m_gameData is null.");
                return;
            }

            if (m_gameData.Run == null)
            {
                Debug.LogWarning("RuntimeDataStore: Run was null. Rebuilding with default data.");

                var characterTop = new CharacterRuntimeData { Id = "", Health = 0, Energy = 0 };
                var characterBottom = new CharacterRuntimeData { Id = "", Health = 0, Energy = 0 };

                var player = new PlayerRuntimeData
                {
                    CharacterTop = characterTop,
                    CharacterBottom = characterBottom,
                    CurrentShield = 0,
                    Coins = 0,
                    Items = new List<string>()
                };

                var map = new MapRuntimeData
                {
                    LastNode = null,
                    Nodes = new List<NodeRuntimeData>()
                };

                m_gameData.Run = new RunRuntimeData
                {
                    Player = player,
                    Map = map
                };
            }

            var saveData = m_gameData.ToSaveData();

            if (saveData == null)
            {
                Debug.LogError("RuntimeDataStore: Save data is null after conversion. Aborting.");
                return;
            }

            SaveSystem.Save(saveData, SaveFileName);
            Debug.Log("<color=green>[RuntimeDataStore]</color> Game saved successfully.");
        }

        private void Load()
        {
            if (!SaveSystem.Exists(SaveFileName))
            {
                Create();
                return;
            }

            GameSaveData saveData = SaveSystem.Load<GameSaveData>(SaveFileName);
            m_gameData = RebuildGameData(saveData);
        }

        #endregion

        #region Game Data Reconstruction -------------------------------------

        private void Create()
        {
            m_gameData = new GameRuntimeData
            {
                Run = null,
                ItemIDsUnlocked = new List<string>()
            };

            Save();
        }

        private GameRuntimeData RebuildGameData(GameSaveData save)
        {
            if (save?.Run?.Player == null)
            {
                Debug.LogError("RebuildGameData: Missing Run or Player data.");
                return new GameRuntimeData
                {
                    Run = null,
                    ItemIDsUnlocked = save?.ItemIDsUnlocked ?? new List<string>()
                };
            }

            var topCharData = save.Run.Player.CharacterTop;
            var bottomCharData = save.Run.Player.CharacterBottom;

            var characterTop = topCharData != null
                ? new CharacterRuntimeData { Id = topCharData.Id, Health = topCharData.Health, Energy = topCharData.Energy }
                : new CharacterRuntimeData { Id = "", Health = 0, Energy = 0 };

            var characterBottom = bottomCharData != null
                ? new CharacterRuntimeData { Id = bottomCharData.Id, Health = bottomCharData.Health, Energy = bottomCharData.Energy }
                : new CharacterRuntimeData { Id = "", Health = 0, Energy = 0 };

            var player = new PlayerRuntimeData
            {
                CharacterTop = characterTop,
                CharacterBottom = characterBottom,
                CurrentShield = save.Run.Player.Shield,
                Coins = save.Run.Player.Coins,
                Items = new List<string>(save.Run.Player.OwnedItemIDs ?? new())
            };

            var nodes = new List<NodeRuntimeData>();
            if (save.Run.Map?.Nodes != null)
            {
                foreach (var node in save.Run.Map.Nodes)
                {
                    if (node != null)
                    {
                        nodes.Add(new NodeRuntimeData
                        {
                            Id = node.Id,
                            Position = node.Position,
                            Connections = node.Connections
                        });
                    }
                }
            }

            var map = new MapRuntimeData
            {
                LastNode = save.Run.Map?.LastNode.ToNullable,
                Nodes = nodes
            };

            var run = new RunRuntimeData
            {
                Player = player,
                Map = map
            };

            return new GameRuntimeData
            {
                Run = run,
                ItemIDsUnlocked = save.ItemIDsUnlocked ?? new List<string>()
            };
        }

        #endregion
    }
}
