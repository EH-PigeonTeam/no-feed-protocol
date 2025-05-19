using UnityEngine;
using NoFeedProtocol.Runtime.Data.Characters;
using NoFeedProtocol.Runtime.Data.Run;
using NoFeedProtocol.Runtime.Data.Save;
using NoFeedProtocol.Runtime.Logic.Conversion;
using Core.Selection_Characters;
using PsychoGarden.Systems.Save;
using PsychoGarden.TriggerEvents;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Logic.Player
{
    [HideMonoScript]
    public class RunCreator : MonoBehaviour
    {
        #region Inspector References ----------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("Character selection controller reference")]
        [SerializeField, Required]
        private CharacterSelectionController m_selectionController;

        [BoxGroup("Settings")]
        [Tooltip("File name used to save the run")]
        [SerializeField]
        private string m_saveFileName = "run_data";

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

            var playerData = new RuntimePlayerData(
                top,
                bottom,
                top.Shield + bottom.Shield,
                coins: 0,
                columnIndex: 0
            );

            var runData = new RunData(playerData, 0, null);
            var saveData = RunSaveData.FromRuntime(runData);

            SaveSystem.Save(saveData, m_saveFileName);

            Debug.Log($"<color=green>[RunCreator] Run saved →</color> {m_saveFileName}");

            m_onFinish?.Invoke(transform);
        }

        #endregion
    }
}
