using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Logic.Battle
{
    [HideMonoScript]
    public class BannerController : MonoBehaviour
    {
        [BoxGroup("References")]
        [Tooltip("The game object to show when the player wins the battle.")]
        [SerializeField, ChildGameObjectsOnly, Required]
        private GameObject m_winScreen;

        [BoxGroup("References")]
        [Tooltip("The game object to show when the player loses the battle.")]
        [SerializeField, ChildGameObjectsOnly, Required]
        private GameObject m_loseScreen;

        public void ShowScreen(bool isPlayerWinning)
        {
            if (isPlayerWinning)
            {
                this.m_winScreen.SetActive(true);
            }
            else
            {
                this.m_loseScreen.SetActive(true);
            }
        }

        #region Debug

        [Button]
        private void ShowWinScreen()
        {
            this.m_loseScreen.SetActive(false);
            this.m_winScreen.SetActive(true);
        }

        [Button]
        private void ShowLoseScreen()
        {
            this.m_winScreen.SetActive(false);
            this.m_loseScreen.SetActive(true);
        }

        #endregion

    }
}
