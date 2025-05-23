using UnityEngine;
using Core.Selection_Characters;
using PsychoGarden.TriggerEvents;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Entities;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;

namespace NoFeedProtocol.Runtime.Logic.Player
{
    [HideMonoScript]
    public class RunCreate : MonoBehaviour
    {
        #region Inspector References ----------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("Character selection controller reference")]
        [SerializeField, Required]
        private CharacterSelectionController m_selectionController;

        [BoxGroup("Settings")]
        [Tooltip("Event triggered after saving the run")]
        [SerializeField]
        private TriggerEvent m_onFinish;

        #endregion

        #region Public Methods ----------------------------------------------

        /// <summary>
        /// Creates player and run data and saves it to file.
        /// </summary>
        public void CreateAndSave()
        {
            if (!m_selectionController.HasTwoCharacterSelected())
            {
                Debug.LogWarning("[RunCreator] Cannot create run: two characters must be selected.");
                return;
            }

            CharacterData top = m_selectionController.SelectedTopCharacter;
            CharacterData bottom = m_selectionController.SelectedBottomCharacter;

            var playerData = new PlayerRuntimeData
            {
                CharacterTop = new CharacterRuntimeData { Id = top.Id, Health = top.MaxHealth, Energy = top.EnergyRequired },
                CharacterBottom = new CharacterRuntimeData { Id = bottom.Id, Health = bottom.MaxHealth, Energy = bottom.EnergyRequired },
                CurrentShield = top.Shield + bottom.Shield,
                Coins = 0,
                Items = new System.Collections.Generic.List<string>()
            };

            var run = new RunRuntimeData
            {
                Player = playerData,
                Map = new MapRuntimeData
                {
                    LastNode = new GridPosition(0, 0),
                    Nodes = new System.Collections.Generic.List<NodeRuntimeData>()
                }
            };

            var data = ServiceLocator.Get<RuntimeDataStore>();

            data.GameData.Run = run;
            data.Save();

            m_onFinish?.Invoke(transform);
        }


        #endregion
    }
}
