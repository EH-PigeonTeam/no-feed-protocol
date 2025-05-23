using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NoFeedProtocol.Runtime.Map
{
    public class ButtonEncounter : ButtonAudio
    {
        #region Fields -----------------------------------------------------

        private Sprite m_icon;

        #endregion

        #region Events -----------------------------------------------------

        public event Action OnClicked;

        #endregion

        #region Initialization ---------------------------------------------

        public void Initialize(string sceneName, Sprite icon, Vector3 position)
        {
            this.m_icon = icon;

            this.GetComponent<Image>().sprite = this.m_icon;
        }

        #endregion

        #region Unity Events -----------------------------------------------

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnClicked?.Invoke();
        }

        #endregion

        #region Public API -------------------------------------------------

        public Sprite Icon => this.m_icon;

        #endregion
    }
}
