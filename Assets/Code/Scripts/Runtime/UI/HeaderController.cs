using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using TMPro;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Services.Characters;
using Code.Systems.LoadingScene;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class HeaderController : MonoBehaviour
    {
        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_characterTopName;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_characterTopValue;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_characterBottomName;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_characterBottomValue;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_coins;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_shields;

        private void Start()
        {
            UpdateInfo();
        }

        private void OnEnable()
        {
            LoadSceneManager.OnSceneLoadFinished += UpdateInfo;
        }

        private void OnDisable()
        {
            LoadSceneManager.OnSceneLoadFinished -= UpdateInfo;
        }

        private void UpdateInfo()
        {
            CharacterResolver resolver = ServiceLocator.Get<CharacterResolver>();
            PlayerRuntimeData player = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player;

            UpdateText(m_characterTopName, resolver.GetById(player.CharacterTop.Id).Name);
            UpdateText(m_characterTopValue, player.CharacterTop.Health.ToString());

            UpdateText(m_characterBottomName, resolver.GetById(player.CharacterBottom.Id).Name);
            UpdateText(m_characterBottomValue, player.CharacterBottom.Health.ToString());

            UpdateText(m_coins, player.Coins.ToString());
            UpdateText(m_shields, player.CurrentShield.ToString());
        }

        #region Utility Methods -----------------------------------------

        private void UpdateText(TMP_Text textField, string value)
        {
            if (textField != null)
            {
                textField.text = value;
            }
        }

        #endregion
    }
}
