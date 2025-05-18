using UnityEngine;
using NoFeelProtocol.Runtime.Data.Characters;
using NoFeelProtocol.Runtime.Data.Save;
using NoFeelProtocol.Runtime.Logic.Conversion;
using Core.Selection_Characters;
using PsychoGarden.Systems.Save;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;

namespace NoFeelProtocol.Runtime.Logic.Player
{
    /// <summary>
    /// Creates a new player from selected characters and saves the result using SaveSystem.
    /// </summary>
    [HideMonoScript]
    public class PlayerCreator : MonoBehaviour
    {
        #region Exposed Members ---------------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private CharacterSelectionController m_selectionController;

        [BoxGroup("Settings")]
        [Tooltip("The name use for create file")]
        [SerializeField]
        private string m_saveFileName = "player_data";

        [BoxGroup("Settings")]
        [Tooltip("The name use for create file")]
        [SerializeField]
        private TriggerEvent m_onFinish;

        #endregion

        #region Public Methods ----------------------------------------------

        /// <summary>
        /// Creates the player from selected characters and saves it.
        /// </summary>
        public void CreateAndSave()
        {
            if (!m_selectionController.HasTwoCharacterSelected())
            {
                Debug.LogWarning("Cannot create player: two characters not selected.");
                return;
            }

            CharacterData top = m_selectionController.SelectedTopCharacter;
            CharacterData bottom = m_selectionController.SelectedBottomCharacter;

            var runtime = new RuntimePlayerData(
                top,
                bottom,
                top.Shield + bottom.Shield,
                0, // Coins
                0  // Column index
            );

            PlayerSaveData saveData = PlayerDataConverter.ToSaveData(runtime);

            SaveSystem.Save(saveData, m_saveFileName);

            Debug.Log($"<color=green>Player saved successfully →</color> {m_saveFileName}");

            m_onFinish?.Invoke(this.transform);
        }

        #endregion
    }
}
