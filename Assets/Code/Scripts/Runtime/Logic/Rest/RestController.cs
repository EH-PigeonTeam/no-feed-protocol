using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Entities;
using PsychoGarden.TriggerEvents;
using NoFeedProtocol.Runtime.Services.Characters;
using PsychoGarden.Utils;

namespace NoFeedProtocol.Runtime.Logic.Rest
{
    [HideMonoScript]
    public class RestController : MonoBehaviour
    {
        #region Exposed Members

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, Required, SceneObjectsOnly]
        private Button m_healthRegenBtn = null;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, Required, SceneObjectsOnly]
        private Button m_shieldRegenBtn = null;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, Required, SceneObjectsOnly]
        private Button m_increaseHealthDamageBtn = null;

        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField, Required, SceneObjectsOnly]
        private Button m_increaseShieldDamageBtn = null;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_healthRegenRate = 0;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_shieldRegenRate = 0;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_increaseHealthDamageRate = 0;
        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_increaseShieldDamageRate = 0;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private TriggerEvent AfterAction = null;

        #endregion

        #region Private Members

        private PlayerRuntimeData Player => ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player;

        #endregion

        #region Init

        private void Start()
        {
            m_healthRegenBtn?.onClick.AddListener(() => RestHealth());
            m_shieldRegenBtn?.onClick.AddListener(() => RestShield());
            m_increaseHealthDamageBtn?.onClick.AddListener(() => IncreaseHealthDamage());
            m_increaseShieldDamageBtn?.onClick.AddListener(() => IncreaseShieldDamage());
        }

        private void OnDestroy()
        {
            m_healthRegenBtn?.onClick.RemoveAllListeners();
            m_shieldRegenBtn?.onClick.RemoveAllListeners();
            m_increaseHealthDamageBtn?.onClick.RemoveAllListeners();
            m_increaseShieldDamageBtn?.onClick.RemoveAllListeners();
        }

        #endregion

        #region Actions

        private void RestHealth()
        {
            if (Player.CharacterTop.IsAlive)
            {
                Player.CharacterTop.Health = Mathf.Clamp(
                    Player.CharacterTop.Health+m_healthRegenRate,
                    0,
                    ServiceLocator.Get<CharacterResolver>().GetById(Player.CharacterTop.Id).MaxHealth
                    );
            }

            if (Player.CharacterBottom.IsAlive)
            {
                Player.CharacterBottom.Health = Mathf.Clamp(
                    Player.CharacterTop.Health + m_healthRegenRate,
                    0,
                    ServiceLocator.Get<CharacterResolver>().GetById(Player.CharacterBottom.Id).MaxHealth
                    );
            }

            OnExecute();
        }

        private void RestShield()
        {
            Player.CurrentShield = Mathf.Clamp(
                Player.CurrentShield + m_shieldRegenRate, 
                0, 
                Player.MaxShield
                );

            OnExecute();
        }

        private void IncreaseHealthDamage()
        {
            Debug.Log("Not implemented");

            OnExecute();
        }

        private void IncreaseShieldDamage()
        {
            Debug.Log("Not implemented");

            OnExecute();
        }

        private void OnExecute()
        {
            AfterAction?.Invoke(this.transform);
        }

        #endregion
    }
}
