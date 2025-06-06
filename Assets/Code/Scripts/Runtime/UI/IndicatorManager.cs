using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class IndicatorManager : MonoBehaviour
    {
        [BoxGroup("Indicators")]
        [Tooltip("The indicators to activate")]
        [SerializeField, ChildGameObjectsOnly]
        private List<GameObject> m_indicators = new();

        [BoxGroup("Indicators")]
        [Tooltip("If true, the indicators will be active in use")]
        [SerializeField]
        private bool m_mode = false;

        public void Use(int index)
        {
            if (index >= 0 && index < this.m_indicators.Count)
                this.m_indicators[index].SetActive(this.m_mode);
        }

        public void UseNext()
        {
            foreach (var indicator in this.m_indicators)
            {
                if (indicator.activeSelf == this.m_mode) continue;

                indicator.SetActive(this.m_mode);
                return;
            }
        }

        public void UseAll()
        {
            foreach (var indicator in this.m_indicators)
                indicator.SetActive(this.m_mode);
        }

        public void Restore()
        {
            this.m_mode = !this.m_mode;
            UseAll();
            this.m_mode = !this.m_mode;
        }

        public bool HasSpin
        {
            get
            {
                foreach (var indicator in this.m_indicators)
                    if (indicator.activeSelf == this.m_mode)
                        return true;
                return false;
            }
        }

        public List<GameObject> Indicators => this.m_indicators;
    }
}
